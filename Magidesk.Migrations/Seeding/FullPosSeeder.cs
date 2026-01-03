using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Security;

namespace Magidesk.Migrations.Seeding;

public static class FullPosSeeder
{
    public static async Task<SeedResult> SeedAsync(
        IServiceProvider services,
        ApplicationDbContext db,
        AesEncryptionService encryptionService,
        SeedOptions options,
        CancellationToken cancellationToken = default)
    {
        var rng = new Random(options.RandomSeed);

        // =========================
        // STEP 2 — CORE MASTER DATA
        // =========================
        SeedRestaurantConfiguration(db);

        var terminals = await SeedTerminalsAsync(db, cancellationToken);
        var terminalFront = terminals["Front POS"];
        var terminalBar = terminals["Bar POS"];
        var terminalBackOffice = terminals["BackOffice"];

        var printerGroups = await SeedPrinterGroupsAsync(db, cancellationToken);
        await SeedPrinterMappingsAsync(db, terminalFront, terminalBar, terminalBackOffice, printerGroups, cancellationToken);

        await SeedMerchantGatewayConfigAsync(db, encryptionService, terminalFront, terminalBar, cancellationToken);

        var roles = await SeedRolesAsync(db, cancellationToken);
        var users = await SeedUsersAsync(db, encryptionService, roles, cancellationToken);

        var shifts = await SeedShiftsAsync(db, cancellationToken);
        var orderTypes = await SeedOrderTypesAsync(db, cancellationToken);

        var floors = new Dictionary<string, Guid>
        {
            ["Dining"] = Guid.NewGuid(),
            ["Patio"] = Guid.NewGuid(),
            ["Bar"] = Guid.NewGuid()
        };

        await SeedTablesAsync(db, floors, cancellationToken);

        // =========================
        // STEP 11 — INVENTORY
        // =========================
        // =========================
        // STEP 11 — INVENTORY
        // =========================
        // await EnsureInventoryTablesExistAsync(db, cancellationToken);
        var inventory = await SeedInventoryAsync(db, cancellationToken);

        // =========================
        // STEP 4 — MODIFIERS & OPTIONS
        // =========================
        var modifierGroups = await SeedModifierGroupsAsync(db, cancellationToken);
        var menuModifiers = await SeedMenuModifiersAsync(db, modifierGroups, cancellationToken);

        // =========================
        // STEP 5 — DISCOUNTS
        // (Tax rules/fees are limited by schema; documented in seed_profile)
        // =========================
        var discounts = await SeedDiscountsAsync(db, cancellationToken);

        // =========================
        // STEP 3 — MENU STRUCTURE (HEAVY)
        // =========================
        var menu = await SeedMenuAsync(
            db,
            inventory,
            printerGroups,
            modifierGroups,
            cancellationToken);

        await SeedMenuItemModifierLinksAsync(db, menu, modifierGroups, cancellationToken);

        // =========================
        // STEP 7 — SHIFTS & CASH MANAGEMENT
        // =========================
        var cashierUser = users["cashier"];
        var managerUser = users["manager"];
        var serverUser = users["server"];

        var currentShift = shifts.Values.First();
        var openCashSession = CashSession.Open(
            userId: cashierUser.Id,
            terminalId: terminalFront.Id,
            shiftId: currentShift.Id,
            openingBalance: new Money(200m));
        db.CashSessions.Add(openCashSession);
        await db.SaveChangesAsync(cancellationToken);

        // Seed a few historical cash sessions (closed, with variance)
        await SeedHistoricalCashSessionsAsync(
            db,
            cashierUser,
            terminalFront,
            currentShift,
            rng,
            cancellationToken);

        // =========================
        // STEP 8/9 — TICKETS & PAYMENTS (CRITICAL)
        // =========================
        var nextTicketNumber = 1000;
        nextTicketNumber = await SeedTodayOpenTicketsDirectAsync(
            db,
            rng,
            nextTicketNumber,
            menu,
            menuModifiers,
            orderTypes,
            shifts,
            terminals,
            users,
            discounts,
            cancellationToken);

        nextTicketNumber = await SeedScheduledTicketsDirectAsync(
            db,
            rng,
            nextTicketNumber,
            menu,
            orderTypes,
            shifts,
            terminalFront,
            serverUser,
            options.ScheduledTicketsToday,
            cancellationToken);

        // Historical sales data (last N days)
        nextTicketNumber = await SeedHistoricalTicketsDirectAsync(
            db,
            rng,
            nextTicketNumber,
            openCashSession,
            menu,
            menuModifiers,
            orderTypes,
            shifts,
            terminalFront,
            terminalBar,
            cashierUser,
            managerUser,
            serverUser,
            discounts,
            options,
            cancellationToken);

        // =========================
        // STEP 12 — REPORTING HISTORY (Labor)
        // =========================
        await SeedAttendanceHistoryAsync(db, users, shifts, options, cancellationToken);

        // Final counts
        var counts = new Dictionary<string, int>
        {
            ["Roles"] = await db.Roles.CountAsync(cancellationToken),
            ["Users"] = await db.Users.CountAsync(cancellationToken),
            ["Terminals"] = await db.Terminals.CountAsync(cancellationToken),
            ["PrinterGroups"] = await db.PrinterGroups.CountAsync(cancellationToken),
            ["PrinterMappings"] = await db.PrinterMappings.CountAsync(cancellationToken),
            ["MerchantGatewayConfigurations"] = await db.MerchantGatewayConfigurations.CountAsync(cancellationToken),
            ["Shifts"] = await db.Shifts.CountAsync(cancellationToken),
            ["OrderTypes"] = await db.OrderTypes.CountAsync(cancellationToken),
            ["Tables"] = await db.Tables.CountAsync(cancellationToken),
            ["InventoryItems"] = await db.InventoryItems.CountAsync(cancellationToken),
            ["ModifierGroups"] = await db.ModifierGroups.CountAsync(cancellationToken),
            ["MenuModifiers"] = await db.MenuModifiers.CountAsync(cancellationToken),
            ["MenuCategories"] = await db.MenuCategories.CountAsync(cancellationToken),
            ["MenuGroups"] = await db.MenuGroups.CountAsync(cancellationToken),
            ["MenuItems"] = await db.MenuItems.CountAsync(cancellationToken),
            ["MenuItemModifierGroups"] = await db.MenuItemModifierGroups.CountAsync(cancellationToken),
            ["Discounts"] = await db.Discounts.CountAsync(cancellationToken),
            ["Tickets"] = await db.Tickets.CountAsync(cancellationToken),
            ["OrderLines"] = await db.OrderLines.CountAsync(cancellationToken),
            ["OrderLineModifiers"] = await db.OrderLineModifiers.CountAsync(cancellationToken),
            ["Payments"] = await db.Payments.CountAsync(cancellationToken),
            ["CashSessions"] = await db.CashSessions.CountAsync(cancellationToken),
            ["Payouts"] = await db.Payouts.CountAsync(cancellationToken),
            ["CashDrops"] = await db.CashDrops.CountAsync(cancellationToken),
            ["DrawerBleeds"] = await db.DrawerBleeds.CountAsync(cancellationToken),
            ["KitchenOrders"] = await db.KitchenOrders.CountAsync(cancellationToken),
            ["KitchenOrderItems"] = await db.KitchenOrderItems.CountAsync(cancellationToken),
            ["AuditEvents"] = await db.AuditEvents.CountAsync(cancellationToken),
            ["AttendanceHistories"] = await db.AttendanceHistories.CountAsync(cancellationToken)
        };

        var keyFacts = new Dictionary<string, string>
        {
            ["Database"] = await DbFacts.GetCurrentDatabaseAsync(db, cancellationToken),
            ["Login: owner"] = "username: owner, pin: 9999",
            ["Login: manager"] = "username: manager, pin: 1234",
            ["Login: cashier"] = "username: cashier, pin: 1111",
            ["Login: server"] = "username: server, pin: 2222",
            ["Login: kitchen"] = "username: kitchen, pin: 3333",
            ["Terminals"] = "Front POS, Bar POS, BackOffice",
            ["Open cash session"] = openCashSession.Id.ToString()
        };

        return new SeedResult(counts, keyFacts);
    }

    private static void SeedRestaurantConfiguration(ApplicationDbContext db)
    {
        db.RestaurantConfigurations.Add(new RestaurantConfiguration
        {
            Id = 1,
            Name = "Magidesk Demo Restaurant",
            Address = "100 Main St, Springfield",
            Phone = "(555) 010-1234",
            Email = "hello@magidesk.local",
            Website = "magidesk.local",
            ReceiptFooterMessage = "Thank you! Please come again.",
            TaxId = "DEMO-TAX-123",
            ZipCode = "00000",
            Capacity = 120,
            CurrencySymbol = "$",
            ServiceChargePercentage = 0.18m,
            DefaultGratuityPercentage = 0.18m,
            IsKioskMode = false
        });
    }

    private static async Task<Dictionary<string, Terminal>> SeedTerminalsAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var front = Terminal.Create("Front POS", "TERM-FRONT-01");
        front.Location = "Main";
        front.ShowGuestSelection = true;
        front.ShowTableSelection = true;

        var bar = Terminal.Create("Bar POS", "TERM-BAR-01");
        bar.Location = "Bar";
        bar.ShowGuestSelection = false;
        bar.ShowTableSelection = false;

        var backOffice = Terminal.Create("BackOffice", "TERM-BO-01");
        backOffice.Location = "Office";
        backOffice.HasCashDrawer = false;

        db.Terminals.AddRange(front, bar, backOffice);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, Terminal>
        {
            ["Front POS"] = front,
            ["Bar POS"] = bar,
            ["BackOffice"] = backOffice
        };
    }

    private static async Task<Dictionary<string, PrinterGroup>> SeedPrinterGroupsAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var kitchen = PrinterGroup.Create("Kitchen", PrinterType.Kitchen);
        var bar = PrinterGroup.Create("Bar", PrinterType.Kitchen);
        var receipt = PrinterGroup.Create("Receipt", PrinterType.Receipt);
        var reports = PrinterGroup.Create("Reports", PrinterType.Report);
        var kds = PrinterGroup.Create("KDS", PrinterType.Kds);

        db.PrinterGroups.AddRange(kitchen, bar, receipt, reports, kds);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, PrinterGroup>
        {
            ["Kitchen"] = kitchen,
            ["Bar"] = bar,
            ["Receipt"] = receipt,
            ["Reports"] = reports,
            ["KDS"] = kds
        };
    }

    private static async Task SeedPrinterMappingsAsync(
        ApplicationDbContext db,
        Terminal front,
        Terminal bar,
        Terminal backOffice,
        Dictionary<string, PrinterGroup> groups,
        CancellationToken cancellationToken)
    {
        var fallbackPrinter = "Microsoft Print to PDF";

        db.PrinterMappings.AddRange(
            PrinterMapping.Create(front.Id, groups["Kitchen"].Id, fallbackPrinter),
            PrinterMapping.Create(front.Id, groups["Receipt"].Id, fallbackPrinter),
            PrinterMapping.Create(front.Id, groups["Reports"].Id, fallbackPrinter),

            PrinterMapping.Create(bar.Id, groups["Bar"].Id, fallbackPrinter),
            PrinterMapping.Create(bar.Id, groups["Receipt"].Id, fallbackPrinter),

            PrinterMapping.Create(backOffice.Id, groups["Reports"].Id, fallbackPrinter)
        );

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedMerchantGatewayConfigAsync(
        ApplicationDbContext db,
        AesEncryptionService encryptionService,
        Terminal front,
        Terminal bar,
        CancellationToken cancellationToken)
    {
        var encryptedApiKey = encryptionService.Encrypt("demo_gateway_key");

        db.MerchantGatewayConfigurations.AddRange(
            MerchantGatewayConfiguration.Create(front.Id, "MockGateway", "MID-DEMO-FRONT", encryptedApiKey, "https://gateway.local"),
            MerchantGatewayConfiguration.Create(bar.Id, "MockGateway", "MID-DEMO-BAR", encryptedApiKey, "https://gateway.local")
        );

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, Role>> SeedRolesAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var allPerms = Enum.GetValues<UserPermission>()
            .Aggregate(UserPermission.None, (acc, p) => acc | p);

        var owner = Role.Create("Owner", allPerms);
        var manager = Role.Create("Manager", allPerms);
        var cashier = Role.Create("Cashier", UserPermission.CreateTicket | UserPermission.EditTicket | UserPermission.TakePayment | UserPermission.OpenDrawer);
        var server = Role.Create("Server", UserPermission.CreateTicket | UserPermission.EditTicket | UserPermission.TakePayment);
        var kitchen = Role.Create("Kitchen", UserPermission.None);

        db.Roles.AddRange(owner, manager, cashier, server, kitchen);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, Role>
        {
            ["Owner"] = owner,
            ["Manager"] = manager,
            ["Cashier"] = cashier,
            ["Server"] = server,
            ["Kitchen"] = kitchen
        };
    }

    private static async Task<Dictionary<string, User>> SeedUsersAsync(
        ApplicationDbContext db,
        AesEncryptionService encryptionService,
        Dictionary<string, Role> roles,
        CancellationToken cancellationToken)
    {
        var owner = User.Create("owner", "Olivia", "Owner", roles["Owner"].Id, encryptionService.Encrypt("1111"), hourlyRate: 45m);
        var manager = User.Create("manager", "Mason", "Manager", roles["Manager"].Id, encryptionService.Encrypt("1234"), hourlyRate: 30m);
        var cashier = User.Create("cashier", "Casey", "Cashier", roles["Cashier"].Id, encryptionService.Encrypt("4444"), hourlyRate: 18m);
        var server = User.Create("server", "Sam", "Server", roles["Server"].Id, encryptionService.Encrypt("2222"), hourlyRate: 12m);
        var kitchen = User.Create("kitchen", "Kai", "Kitchen", roles["Kitchen"].Id, encryptionService.Encrypt("3333"), hourlyRate: 20m);

        db.Users.AddRange(owner, manager, cashier, server, kitchen);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, User>
        {
            ["owner"] = owner,
            ["manager"] = manager,
            ["cashier"] = cashier,
            ["server"] = server,
            ["kitchen"] = kitchen
        };
    }

    private static async Task<Dictionary<string, Shift>> SeedShiftsAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var breakfast = Shift.Create("Breakfast", new TimeSpan(6, 0, 0), new TimeSpan(11, 0, 0));
        var lunch = Shift.Create("Lunch", new TimeSpan(11, 0, 0), new TimeSpan(16, 0, 0));
        var dinner = Shift.Create("Dinner", new TimeSpan(16, 0, 0), new TimeSpan(22, 0, 0));
        var night = Shift.Create("Night", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0));

        db.Shifts.AddRange(breakfast, lunch, dinner, night);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, Shift>
        {
            ["Breakfast"] = breakfast,
            ["Lunch"] = lunch,
            ["Dinner"] = dinner,
            ["Night"] = night
        };
    }

    private static async Task<Dictionary<string, OrderType>> SeedOrderTypesAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var dineIn = OrderType.Create("DINE IN", closeOnPaid: false, allowSeatBasedOrder: true, allowToAddTipsLater: true);
        dineIn.SetProperty("RequiresTable", "true");

        var takeOut = OrderType.Create("TAKE OUT", closeOnPaid: true);
        takeOut.SetProperty("RequiresCustomer", "false");

        var pickUp = OrderType.Create("PICK UP", closeOnPaid: true);
        pickUp.SetProperty("RequiresCustomer", "true");

        var delivery = OrderType.Create("DELIVERY", closeOnPaid: false);
        delivery.SetProperty("RequiresCustomer", "true");

        var barTab = OrderType.Create("BAR TAB", isBarTab: true, allowToAddTipsLater: true);

        db.OrderTypes.AddRange(dineIn, takeOut, pickUp, delivery, barTab);
        await db.SaveChangesAsync(cancellationToken);

        return new Dictionary<string, OrderType>
        {
            ["DINE IN"] = dineIn,
            ["TAKE OUT"] = takeOut,
            ["PICK UP"] = pickUp,
            ["DELIVERY"] = delivery,
            ["BAR TAB"] = barTab
        };
    }

    private static async Task SeedTablesAsync(
        ApplicationDbContext db,
        Dictionary<string, Guid> floors,
        CancellationToken cancellationToken)
    {
        var tables = new List<Table>();

        // Dining: 1-20
        for (var i = 1; i <= 20; i++)
        {
            tables.Add(Table.Create(
                tableNumber: i, 
                capacity: i <= 10 ? 4 : 2, 
                x: (i - 1) % 5 * 120, 
                y: (i - 1) / 5 * 90, 
                floorId: floors["Dining"],
                shape: TableShapeType.Rectangle,
                width: 80,
                height: 80));
        }

        // Patio: 21-28
        for (var i = 21; i <= 28; i++)
        {
            tables.Add(Table.Create(
                tableNumber: i, 
                capacity: 4, 
                x: (i - 21) % 4 * 140, 
                y: (i - 21) / 4 * 110, 
                floorId: floors["Patio"],
                shape: TableShapeType.Round,
                width: 90,
                height: 90));
        }

        // Bar: 29-34 (small tops)
        for (var i = 29; i <= 34; i++)
        {
            tables.Add(Table.Create(
                tableNumber: i, 
                capacity: 2, 
                x: (i - 29) * 100, 
                y: 0, 
                floorId: floors["Bar"],
                shape: TableShapeType.Square,
                width: 60,
                height: 60));
        }

        db.Tables.AddRange(tables);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, InventoryItem>> SeedInventoryAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var items = new[]
        {
            InventoryItem.Create("Pizza Dough Ball", "count", 120, 30),
            InventoryItem.Create("Mozzarella Cheese", "lbs", 40, 10),
            InventoryItem.Create("Pepperoni", "lbs", 25, 8),
            InventoryItem.Create("Tomato Sauce", "qt", 30, 10),
            InventoryItem.Create("Ground Beef", "lbs", 35, 10),
            InventoryItem.Create("Burger Buns", "count", 200, 60),
            InventoryItem.Create("Lettuce", "lbs", 15, 5),
            InventoryItem.Create("Tomatoes", "lbs", 20, 6),
            InventoryItem.Create("Onions", "lbs", 18, 6),
            InventoryItem.Create("French Fries", "lbs", 80, 25),
            InventoryItem.Create("Chicken Breast", "lbs", 50, 15),
            InventoryItem.Create("Coffee Beans", "lbs", 12, 4),
            InventoryItem.Create("Milk", "gal", 25, 8),
            InventoryItem.Create("Ice Cream", "qt", 15, 5),
            InventoryItem.Create("Cola Syrup", "gal", 10, 3),
            InventoryItem.Create("Beer Keg - Lager", "gal", 124, 31),
            InventoryItem.Create("Wine - House Red", "bottle", 48, 12),
            InventoryItem.Create("Limes", "count", 200, 60),
            InventoryItem.Create("Tequila", "bottle", 24, 6),
            InventoryItem.Create("Vodka", "bottle", 24, 6),
        };

        db.InventoryItems.AddRange(items);
        await db.SaveChangesAsync(cancellationToken);

        return items.ToDictionary(i => i.Name, i => i);
    }

    private static async Task<Dictionary<string, ModifierGroup>> SeedModifierGroupsAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var groups = new[]
        {
            ModifierGroup.Create("Pizza Size", isRequired: true, minSelections: 1, maxSelections: 1, allowMultipleSelections: false, description: "Choose a size", displayOrder: 1),
            ModifierGroup.Create("Pizza Toppings", isRequired: false, minSelections: 0, maxSelections: 6, allowMultipleSelections: true, description: "Choose toppings", displayOrder: 2),
            ModifierGroup.Create("Burger Temp", isRequired: true, minSelections: 1, maxSelections: 1, allowMultipleSelections: false, description: "Cooking temperature", displayOrder: 1),
            ModifierGroup.Create("Burger Add-ons", isRequired: false, minSelections: 0, maxSelections: 4, allowMultipleSelections: true, description: "Add-ons", displayOrder: 2),
            ModifierGroup.Create("Drink Size", isRequired: true, minSelections: 1, maxSelections: 1, allowMultipleSelections: false, description: "Size", displayOrder: 1),
            ModifierGroup.Create("Coffee Milk", isRequired: false, minSelections: 0, maxSelections: 1, allowMultipleSelections: false, description: "Milk choice", displayOrder: 2),
            ModifierGroup.Create("Coffee Syrup", isRequired: false, minSelections: 0, maxSelections: 2, allowMultipleSelections: true, description: "Syrups", displayOrder: 3),
        };

        db.ModifierGroups.AddRange(groups);
        await db.SaveChangesAsync(cancellationToken);

        return groups.ToDictionary(g => g.Name, g => g);
    }

    private static async Task<List<MenuModifier>> SeedMenuModifiersAsync(
        ApplicationDbContext db,
        Dictionary<string, ModifierGroup> groups,
        CancellationToken cancellationToken)
    {
        var mods = new List<MenuModifier>
        {
            // Pizza sizes (use as "size" upcharges)
            MenuModifier.Create("Small (10\")", ModifierType.Normal, new Money(0m), groups["Pizza Size"].Id, displayOrder: 1),
            MenuModifier.Create("Medium (12\")", ModifierType.Normal, new Money(2m), groups["Pizza Size"].Id, displayOrder: 2),
            MenuModifier.Create("Large (16\")", ModifierType.Normal, new Money(5m), groups["Pizza Size"].Id, displayOrder: 3),

            // Pizza toppings (some are section-wise in UI logic; we seed via ModifyOrderLine to create section modifiers)
            MenuModifier.Create("Pepperoni", ModifierType.Normal, new Money(1.50m), groups["Pizza Toppings"].Id, displayOrder: 1, isSectionWisePrice: true, sectionName: "Whole", multiplierName: "x1"),
            MenuModifier.Create("Mushrooms", ModifierType.Normal, new Money(1.00m), groups["Pizza Toppings"].Id, displayOrder: 2, isSectionWisePrice: true, sectionName: "Whole", multiplierName: "x1"),
            MenuModifier.Create("Onions", ModifierType.Normal, new Money(0.75m), groups["Pizza Toppings"].Id, displayOrder: 3, isSectionWisePrice: true, sectionName: "Whole", multiplierName: "x1"),
            MenuModifier.Create("Extra Cheese", ModifierType.Extra, new Money(1.75m), groups["Pizza Toppings"].Id, displayOrder: 4, isSectionWisePrice: true, sectionName: "Whole", multiplierName: "x1"),

            // Burger temps
            MenuModifier.Create("Rare", ModifierType.InfoOnly, new Money(0m), groups["Burger Temp"].Id, displayOrder: 1, shouldPrintToKitchen: true),
            MenuModifier.Create("Medium", ModifierType.InfoOnly, new Money(0m), groups["Burger Temp"].Id, displayOrder: 2, shouldPrintToKitchen: true),
            MenuModifier.Create("Well Done", ModifierType.InfoOnly, new Money(0m), groups["Burger Temp"].Id, displayOrder: 3, shouldPrintToKitchen: true),

            // Burger add-ons
            MenuModifier.Create("Bacon", ModifierType.Extra, new Money(2.00m), groups["Burger Add-ons"].Id, displayOrder: 1),
            MenuModifier.Create("Avocado", ModifierType.Extra, new Money(2.50m), groups["Burger Add-ons"].Id, displayOrder: 2),
            MenuModifier.Create("Extra Patty", ModifierType.Extra, new Money(4.00m), groups["Burger Add-ons"].Id, displayOrder: 3),

            // Drink sizes
            MenuModifier.Create("Small", ModifierType.Normal, new Money(0m), groups["Drink Size"].Id, displayOrder: 1),
            MenuModifier.Create("Medium", ModifierType.Normal, new Money(0.50m), groups["Drink Size"].Id, displayOrder: 2),
            MenuModifier.Create("Large", ModifierType.Normal, new Money(1.00m), groups["Drink Size"].Id, displayOrder: 3),

            // Coffee milk
            MenuModifier.Create("Whole Milk", ModifierType.InfoOnly, new Money(0m), groups["Coffee Milk"].Id, displayOrder: 1),
            MenuModifier.Create("Oat Milk", ModifierType.InfoOnly, new Money(0.75m), groups["Coffee Milk"].Id, displayOrder: 2),

            // Coffee syrup
            MenuModifier.Create("Vanilla Syrup", ModifierType.Extra, new Money(0.75m), groups["Coffee Syrup"].Id, displayOrder: 1),
            MenuModifier.Create("Caramel Syrup", ModifierType.Extra, new Money(0.75m), groups["Coffee Syrup"].Id, displayOrder: 2),
        };

        // Add a couple fractional modifiers for pizza halves (stored as inherited MenuModifier)
        var leftHalfPepperoni = new FractionalModifier(
            "Pepperoni (Left Half)",
            new Money(1.00m),
            sortOrder: 100,
            portion: ModifierPortion.LeftHalf,
            priceStrategy: PriceStrategy.SumOfHalves);
        ReflectionUtil.SetProperty(leftHalfPepperoni, "ModifierGroupId", groups["Pizza Toppings"].Id);
        ReflectionUtil.SetProperty(leftHalfPepperoni, "ShouldPrintToKitchen", true);
        ReflectionUtil.SetProperty(leftHalfPepperoni, "ModifierType", ModifierType.Normal);

        var rightHalfMushrooms = new FractionalModifier(
            "Mushrooms (Right Half)",
            new Money(0.75m),
            sortOrder: 101,
            portion: ModifierPortion.RightHalf,
            priceStrategy: PriceStrategy.SumOfHalves);
        ReflectionUtil.SetProperty(rightHalfMushrooms, "ModifierGroupId", groups["Pizza Toppings"].Id);
        ReflectionUtil.SetProperty(rightHalfMushrooms, "ShouldPrintToKitchen", true);
        ReflectionUtil.SetProperty(rightHalfMushrooms, "ModifierType", ModifierType.Normal);

        mods.Add(leftHalfPepperoni);
        mods.Add(rightHalfMushrooms);

        db.MenuModifiers.AddRange(mods);
        await db.SaveChangesAsync(cancellationToken);

        return mods;
    }

    private static async Task<List<Discount>> SeedDiscountsAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        var list = new List<Discount>
        {
            Discount.Create("Manager Comp $5", DiscountType.Amount, 5m, QualificationType.Order, ApplicationType.FixedPerOrder, autoApply: false),
            Discount.Create("10% Off (Promo)", DiscountType.Percentage, 0.10m, QualificationType.Order, ApplicationType.PercentagePerOrder, autoApply: false, couponCode: "PROMO10"),
            Discount.Create("BOGO 50% (Item)", DiscountType.Percentage, 0.50m, QualificationType.Item, ApplicationType.PercentagePerItem, autoApply: false),
            Discount.Create("Veteran $3 Off", DiscountType.Amount, 3m, QualificationType.Order, ApplicationType.FixedPerOrder, autoApply: false, couponCode: "VET3"),
        };

        db.Discounts.AddRange(list);
        await db.SaveChangesAsync(cancellationToken);

        return list;
    }

    private static async Task<MenuSeed> SeedMenuAsync(
        ApplicationDbContext db,
        Dictionary<string, InventoryItem> inventory,
        Dictionary<string, PrinterGroup> printerGroups,
        Dictionary<string, ModifierGroup> modifierGroups,
        CancellationToken cancellationToken)
    {
        // Categories
        var categories = new[]
        {
            MenuCategory.Create("Drinks", 1, isBeverage: true),
            MenuCategory.Create("Appetizers", 2),
            MenuCategory.Create("Salads", 3),
            MenuCategory.Create("Burgers", 4),
            MenuCategory.Create("Pizzas", 5),
            MenuCategory.Create("Desserts", 6),
            MenuCategory.Create("Combos", 7),
            MenuCategory.Create("Misc", 99)
        };
        db.MenuCategories.AddRange(categories);
        await db.SaveChangesAsync(cancellationToken);

        var cat = categories.ToDictionary(c => c.Name, c => c);

        // Groups
        var groups = new[]
        {
            MenuGroup.Create("Soft Drinks", cat["Drinks"].Id, 1),
            MenuGroup.Create("Beer", cat["Drinks"].Id, 2),
            MenuGroup.Create("Wine", cat["Drinks"].Id, 3),
            MenuGroup.Create("Coffee & Tea", cat["Drinks"].Id, 4),

            MenuGroup.Create("Starters", cat["Appetizers"].Id, 1),
            MenuGroup.Create("Wings", cat["Appetizers"].Id, 2),

            MenuGroup.Create("House Salads", cat["Salads"].Id, 1),

            MenuGroup.Create("Signature Burgers", cat["Burgers"].Id, 1),

            MenuGroup.Create("Classic Pizzas", cat["Pizzas"].Id, 1),
            MenuGroup.Create("Build Your Own", cat["Pizzas"].Id, 2),

            MenuGroup.Create("Dessert", cat["Desserts"].Id, 1),

            MenuGroup.Create("Lunch Combos", cat["Combos"].Id, 1),

            MenuGroup.Create("Open Item", cat["Misc"].Id, 1),
        };
        db.MenuGroups.AddRange(groups);
        await db.SaveChangesAsync(cancellationToken);

        var grp = groups.ToDictionary(g => g.Name, g => g);

        // Combo Definition + groups
        var lunchComboDef = new ComboDefinition("Lunch Combo", new Money(11.99m));
        db.ComboDefinitions.Add(lunchComboDef);
        await db.SaveChangesAsync(cancellationToken);

        var comboMain = new ComboGroup(lunchComboDef.Id, "Main", 1);
        var comboSide = new ComboGroup(lunchComboDef.Id, "Side", 2);
        var comboDrink = new ComboGroup(lunchComboDef.Id, "Drink", 3);
        lunchComboDef.AddGroup(comboMain);
        lunchComboDef.AddGroup(comboSide);
        lunchComboDef.AddGroup(comboDrink);
        db.ComboGroups.AddRange(comboMain, comboSide, comboDrink);
        await db.SaveChangesAsync(cancellationToken);

        // Menu Items (target ~110)
        var items = new List<MenuItem>();

        void AddItem(string name, decimal price, string groupName, string categoryName, Guid? printerGroupId = null, bool variablePrice = false)
        {
            var item = MenuItem.Create(name, new Money(price), taxRate: 0m);
            item.SetCategory(cat[categoryName].Id);
            item.SetGroup(grp[groupName].Id);
            if (printerGroupId.HasValue)
            {
                item.SetPrinterGroup(printerGroupId);
            }

            // Store a couple known UI keys via backing field (no public setter in entity)
            var props = ReflectionUtil.GetField<Dictionary<string, string>>(item, "_properties");
            if (variablePrice)
            {
                props["IsVariablePrice"] = "true";
            }

            props["ColorCode"] = categoryName == "Drinks" ? "#1E88E5" : "#43A047";

            items.Add(item);
        }

        // Drinks
        var barPrinter = printerGroups["Bar"].Id;
        AddItem("Coke", 2.50m, "Soft Drinks", "Drinks", barPrinter);
        AddItem("Diet Coke", 2.50m, "Soft Drinks", "Drinks", barPrinter);
        AddItem("Sprite", 2.50m, "Soft Drinks", "Drinks", barPrinter);
        AddItem("Iced Tea", 2.75m, "Soft Drinks", "Drinks", barPrinter);
        AddItem("Root Beer", 2.75m, "Soft Drinks", "Drinks", barPrinter);
        AddItem("Lemonade", 3.00m, "Soft Drinks", "Drinks", barPrinter);

        // Beer/Wine (a few)
        AddItem("House Lager (Pint)", 6.50m, "Beer", "Drinks", barPrinter);
        AddItem("IPA (Pint)", 7.50m, "Beer", "Drinks", barPrinter);
        AddItem("House Red (Glass)", 8.00m, "Wine", "Drinks", barPrinter);
        AddItem("House White (Glass)", 8.00m, "Wine", "Drinks", barPrinter);

        // Coffee/Tea
        AddItem("Brewed Coffee", 3.25m, "Coffee & Tea", "Drinks", barPrinter);
        AddItem("Latte", 4.75m, "Coffee & Tea", "Drinks", barPrinter);
        AddItem("Hot Tea", 3.00m, "Coffee & Tea", "Drinks", barPrinter);

        // Appetizers
        var kitchenPrinter = printerGroups["Kitchen"].Id;
        AddItem("Mozzarella Sticks", 9.50m, "Starters", "Appetizers", kitchenPrinter);
        AddItem("Loaded Fries", 11.00m, "Starters", "Appetizers", kitchenPrinter);
        AddItem("Chicken Wings (10pc)", 14.50m, "Wings", "Appetizers", kitchenPrinter);
        AddItem("Chicken Wings (20pc)", 26.00m, "Wings", "Appetizers", kitchenPrinter);

        // Salads
        AddItem("House Salad", 8.50m, "House Salads", "Salads", kitchenPrinter);
        AddItem("Caesar Salad", 9.50m, "House Salads", "Salads", kitchenPrinter);
        AddItem("Chicken Caesar Salad", 13.50m, "House Salads", "Salads", kitchenPrinter);

        // Burgers
        AddItem("Classic Cheeseburger", 13.99m, "Signature Burgers", "Burgers", kitchenPrinter);
        AddItem("BBQ Bacon Burger", 15.99m, "Signature Burgers", "Burgers", kitchenPrinter);
        AddItem("Veggie Burger", 14.50m, "Signature Burgers", "Burgers", kitchenPrinter);

        // Pizzas (classic)
        AddItem("Cheese Pizza", 12.00m, "Classic Pizzas", "Pizzas", kitchenPrinter);
        AddItem("Pepperoni Pizza", 14.00m, "Classic Pizzas", "Pizzas", kitchenPrinter);
        AddItem("Supreme Pizza", 16.00m, "Classic Pizzas", "Pizzas", kitchenPrinter);

        // Build your own (variable price)
        AddItem("Build Your Own Pizza", 11.00m, "Build Your Own", "Pizzas", kitchenPrinter, variablePrice: true);

        // Desserts
        AddItem("Chocolate Cake", 7.25m, "Dessert", "Desserts", kitchenPrinter);
        AddItem("Ice Cream Scoop", 3.50m, "Dessert", "Desserts", kitchenPrinter);

        // Combos (menu item that links to combo definition)
        var comboItem = MenuItem.Create("Lunch Combo", new Money(11.99m), taxRate: 0m);
        comboItem.SetCategory(cat["Combos"].Id);
        comboItem.SetGroup(grp["Lunch Combos"].Id);
        comboItem.SetComboDefinition(lunchComboDef.Id);
        comboItem.SetPrinterGroup(kitchenPrinter);
        items.Add(comboItem);

        // Misc open item
        AddItem("Misc Item", 0.00m, "Open Item", "Misc", kitchenPrinter, variablePrice: true);

        // Pad out to ~110 items with realistic variations (no new schema assumptions)
        var burgerNames = new[] { "Mushroom Swiss Burger", "Spicy Jalapeño Burger", "Double Stack Burger", "Blue Cheese Burger" };
        foreach (var n in burgerNames)
        {
            AddItem(n, 15.49m + (decimal)(items.Count % 3) * 1.00m, "Signature Burgers", "Burgers", kitchenPrinter);
        }

        for (var i = 1; i <= 60; i++)
        {
            AddItem($"Special #{i:000}", 9.99m + (i % 7), "Starters", "Appetizers", kitchenPrinter);
        }

        db.MenuItems.AddRange(items);
        await db.SaveChangesAsync(cancellationToken);

        // Link combo group items to real menu items (a few mains/sides/drinks)
        var mains = items.Where(i => i.GroupId == grp["Signature Burgers"].Id).Take(5).ToList();
        var sides = items.Where(i => i.GroupId == grp["Starters"].Id).Take(5).ToList();
        var drinks = items.Where(i => i.GroupId == grp["Soft Drinks"].Id).Take(6).ToList();

        var comboItems = mains.Select(m => new ComboGroupItem(comboMain.Id, m.Id, new Money(0m)))
            .Concat(sides.Select(s => new ComboGroupItem(comboSide.Id, s.Id, new Money(0m))))
            .Concat(drinks.Select(d => new ComboGroupItem(comboDrink.Id, d.Id, new Money(0m))))
            .ToList();

        db.ComboGroupItems.AddRange(comboItems);

        await db.SaveChangesAsync(cancellationToken);

        // Recipe lines (a few key items)
        var byName = items.ToDictionary(i => i.Name, i => i);
        if (byName.TryGetValue("Pepperoni Pizza", out var pep))
        {
            pep.AddRecipeLine(inventory["Pizza Dough Ball"].Id, 1m);
            pep.AddRecipeLine(inventory["Tomato Sauce"].Id, 0.25m);
            pep.AddRecipeLine(inventory["Mozzarella Cheese"].Id, 0.30m);
            pep.AddRecipeLine(inventory["Pepperoni"].Id, 0.20m);
        }

        if (byName.TryGetValue("Classic Cheeseburger", out var burger))
        {
            burger.AddRecipeLine(inventory["Burger Buns"].Id, 1m);
            burger.AddRecipeLine(inventory["Ground Beef"].Id, 0.33m);
            burger.AddRecipeLine(inventory["Lettuce"].Id, 0.05m);
            burger.AddRecipeLine(inventory["Tomatoes"].Id, 0.05m);
            burger.AddRecipeLine(inventory["Onions"].Id, 0.02m);
        }

        await db.SaveChangesAsync(cancellationToken);

        return new MenuSeed(categories, groups, items, lunchComboDef);
    }

    private static async Task SeedMenuItemModifierLinksAsync(
        ApplicationDbContext db,
        MenuSeed menu,
        Dictionary<string, ModifierGroup> modifierGroups,
        CancellationToken cancellationToken)
    {
        var links = new List<MenuItemModifierGroup>();

        var pizzaItems = menu.Items.Where(i => i.GroupId == menu.Groups.First(g => g.Name == "Build Your Own").Id
                                               || i.GroupId == menu.Groups.First(g => g.Name == "Classic Pizzas").Id);
        foreach (var item in pizzaItems)
        {
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Pizza Size"].Id, 1));
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Pizza Toppings"].Id, 2));
        }

        var burgerItems = menu.Items.Where(i => i.CategoryId == menu.Categories.First(c => c.Name == "Burgers").Id);
        foreach (var item in burgerItems)
        {
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Burger Temp"].Id, 1));
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Burger Add-ons"].Id, 2));
        }

        var drinks = menu.Items.Where(i => i.CategoryId == menu.Categories.First(c => c.Name == "Drinks").Id)
            .Where(i => i.Name.Contains("Coke", StringComparison.OrdinalIgnoreCase) || i.Name.Contains("Tea", StringComparison.OrdinalIgnoreCase) || i.Name.Contains("Lemonade", StringComparison.OrdinalIgnoreCase));
        foreach (var item in drinks)
        {
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Drink Size"].Id, 1));
        }

        var coffee = menu.Items.Where(i => i.Name.Contains("Coffee", StringComparison.OrdinalIgnoreCase) || i.Name.Contains("Latte", StringComparison.OrdinalIgnoreCase));
        foreach (var item in coffee)
        {
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Coffee Milk"].Id, 2));
            links.Add(MenuItemModifierGroup.Create(item.Id, modifierGroups["Coffee Syrup"].Id, 3));
        }

        db.MenuItemModifierGroups.AddRange(links);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedHistoricalCashSessionsAsync(
        ApplicationDbContext db,
        User cashier,
        Terminal front,
        Shift shift,
        Random rng,
        CancellationToken cancellationToken)
    {
        for (var daysAgo = 1; daysAgo <= 10; daysAgo++)
        {
            var session = CashSession.Open(cashier.Id, front.Id, shift.Id, new Money(150m));

            // Some cash management activity
            session.AddPayout(Payout.Create(session.Id, new Money(10m), cashier.Id, "Petty cash"));
            session.AddCashDrop(CashDrop.Create(session.Id, new Money(50m), cashier.Id, "Safe drop"));
            session.AddDrawerBleed(DrawerBleed.Create(session.Id, new Money(20m), cashier.Id, "Drawer bleed"));
            session.AddTransaction(new TerminalTransaction(session.Id, TerminalTransactionType.OpenFloat, new Money(150m), "Float"));

            // Close with random variance
            var actual = new Money(150m + rng.Next(0, 30));
            session.Close(cashier.Id, actual);

            // Set timestamps to past for reporting
            var openedAt = DateTime.UtcNow.Date.AddDays(-daysAgo).AddHours(9);
            ReflectionUtil.SetProperty(session, "OpenedAt", openedAt);
            ReflectionUtil.SetProperty(session, "ClosedAt", openedAt.AddHours(8));

            db.CashSessions.Add(session);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    // =========================
    // DIRECT TICKET SEEDING (no CQRS handlers)
    //
    // Rationale:
    // Application ticket command handlers currently hit optimistic concurrency failures on first
    // AddOrderLine update (Ticket.Version concurrency token). For seeding, we build complete aggregates
    // in-memory and insert them as new rows (no concurrent UPDATE needed).
    // =========================

    private static async Task<int> SeedTodayOpenTicketsDirectAsync(
        ApplicationDbContext db,
        Random rng,
        int nextTicketNumber,
        MenuSeed menu,
        List<MenuModifier> allModifiers,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Dictionary<string, Terminal> terminals,
        Dictionary<string, User> users,
        List<Discount> discounts,
        CancellationToken cancellationToken)
    {
        var server = users["server"];
        var terminal = terminals["Front POS"];
        var shift = shifts["Dinner"];
        var dineIn = orderTypes["DINE IN"];

        var tickets = new List<Ticket>();

        for (var i = 0; i < 10; i++)
        {
            var table = 1 + (i % 20);
            var ticket = Ticket.Create(
                ticketNumber: nextTicketNumber++,
                createdBy: server.Id,
                terminalId: terminal.Id,
                shiftId: shift.Id,
                orderTypeId: dineIn.Id);

            ticket.SetNumberOfGuests(rng.Next(1, 6));
            ticket.AssignTable(table);
            ticket.SetNote(i % 4 == 0 ? "Allergy: NO NUTS. Confirm with kitchen." : null);

            // Add 2-5 items
            var lineCount = rng.Next(2, 6);
            for (var l = 0; l < lineCount; l++)
            {
                var mi = menu.Items[rng.Next(menu.Items.Count)];
                var line = OrderLine.Create(
                    ticket.Id,
                    mi.Id,
                    mi.Name,
                    quantity: 1,
                    unitPrice: new Money(mi.Price.Amount, mi.Price.Currency),
                    taxRate: 0m,
                    categoryName: mi.CategoryId.HasValue ? menu.Categories.First(c => c.Id == mi.CategoryId).Name : null,
                    groupName: mi.GroupId.HasValue ? menu.Groups.First(g => g.Id == mi.GroupId).Name : null);
                line.SetPrinterGroup(mi.PrinterGroupId);
                ReflectionUtil.SetProperty(line, "ShouldPrintToKitchen", true);

                foreach (var mod in PickModifiersForMenuItem(allModifiers, mi.Name, rng))
                {
                    line.AddModifier(OrderLineModifier.Create(
                        orderLineId: line.Id,
                        modifierId: mod.Id,
                        name: mod.Name,
                        modifierType: mod.ModifierType,
                        itemCount: 1,
                        unitPrice: new Money(mod.BasePrice.Amount, mod.BasePrice.Currency),
                        basePrice: new Money(mod.BasePrice.Amount, mod.BasePrice.Currency),
                        taxRate: 0m,
                        modifierGroupId: mod.ModifierGroupId,
                        shouldPrintToKitchen: mod.ShouldPrintToKitchen));
                }

                ticket.AddOrderLine(line);
            }

            // Discount snapshot on some
            if (i % 5 == 0)
            {
                var d = discounts[0];
                var amount = new Money(Math.Min(5m, ticket.SubtotalAmount.Amount));
                ticket.ApplyDiscount(TicketDiscount.Create(ticket.Id, d.Id, d.Name, d.Type, d.Value, amount));
            }

            tickets.Add(ticket);
        }

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync(cancellationToken);
        return nextTicketNumber;
    }

    private static async Task<int> SeedScheduledTicketsDirectAsync(
        ApplicationDbContext db,
        Random rng,
        int nextTicketNumber,
        MenuSeed menu,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Terminal terminal,
        User createdBy,
        int count,
        CancellationToken cancellationToken)
    {
        var shift = shifts["Dinner"];
        var pickup = orderTypes["PICK UP"];

        var tickets = new List<Ticket>();
        for (var i = 0; i < count; i++)
        {
            var ticket = Ticket.Create(
                ticketNumber: nextTicketNumber++,
                createdBy: createdBy.Id,
                terminalId: terminal.Id,
                shiftId: shift.Id,
                orderTypeId: pickup.Id);

            var mi = menu.Items.Where(x => x.Name.Contains("Pizza", StringComparison.OrdinalIgnoreCase)).OrderBy(_ => rng.Next()).First();
            var scheduledLine = OrderLine.Create(ticket.Id, mi.Id, mi.Name, 1, new Money(mi.Price.Amount, mi.Price.Currency), taxRate: 0m, categoryName: "Pizzas", groupName: "Classic Pizzas");
            scheduledLine.SetPrinterGroup(mi.PrinterGroupId);
            ReflectionUtil.SetProperty(scheduledLine, "ShouldPrintToKitchen", true);
            ticket.AddOrderLine(scheduledLine);

            ticket.Schedule(DateTime.UtcNow.AddHours(1 + i));
            tickets.Add(ticket);
        }

        db.Tickets.AddRange(tickets);
        await db.SaveChangesAsync(cancellationToken);
        return nextTicketNumber;
    }

    private static async Task<int> SeedHistoricalTicketsDirectAsync(
        ApplicationDbContext db,
        Random rng,
        int nextTicketNumber,
        CashSession openCashSession,
        MenuSeed menu,
        List<MenuModifier> allModifiers,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Terminal front,
        Terminal bar,
        User cashier,
        User manager,
        User server,
        List<Discount> discounts,
        SeedOptions options,
        CancellationToken cancellationToken)
    {
        var dineIn = orderTypes["DINE IN"];
        var takeOut = orderTypes["TAKE OUT"];
        var barTab = orderTypes["BAR TAB"];

        for (var daysAgo = options.HistoryDays; daysAgo >= 1; daysAgo--)
        {
            var day = DateTime.UtcNow.Date.AddDays(-daysAgo);
            var ticketCount = rng.Next(options.TicketsPerDayMin, options.TicketsPerDayMax + 1);

            var batchTickets = new List<Ticket>();
            var batchKitchenOrders = new List<KitchenOrder>();
            var batchKitchenOrderItems = new List<KitchenOrderItem>();
            var batchAudits = new List<AuditEvent>();

            for (var i = 0; i < ticketCount; i++)
            {
                var shift = shifts.Values.ElementAt(rng.Next(shifts.Count));
                var orderType = i % 7 == 0 ? barTab : (i % 4 == 0 ? takeOut : dineIn);
                var terminal = orderType == barTab ? bar : front;
                var createdAt = day.AddHours(11 + rng.Next(0, 11)).AddMinutes(rng.Next(0, 55));

                // Build ticket aggregate in-memory
                var ticket = Ticket.Create(
                    ticketNumber: nextTicketNumber++,
                    createdBy: server.Id,
                    terminalId: terminal.Id,
                    shiftId: shift.Id,
                    orderTypeId: orderType.Id);

                if (orderType == dineIn)
                {
                    ticket.SetNumberOfGuests(rng.Next(1, 6));
                    ticket.AssignTable(rng.Next(1, 21));
                }

                // Add items
                var lineCount = rng.Next(1, 7);
                for (var l = 0; l < lineCount; l++)
                {
                    var mi = menu.Items[rng.Next(menu.Items.Count)];
                    var line = OrderLine.Create(
                        ticket.Id,
                        mi.Id,
                        mi.Name,
                        quantity: 1,
                        unitPrice: new Money(mi.Price.Amount, mi.Price.Currency),
                        taxRate: 0m,
                        categoryName: mi.CategoryId.HasValue ? menu.Categories.First(c => c.Id == mi.CategoryId).Name : null,
                        groupName: mi.GroupId.HasValue ? menu.Groups.First(g => g.Id == mi.GroupId).Name : null);
                    line.SetPrinterGroup(mi.PrinterGroupId);
                    ReflectionUtil.SetProperty(line, "ShouldPrintToKitchen", true);

                    foreach (var mod in PickModifiersForMenuItem(allModifiers, mi.Name, rng))
                    {
                        line.AddModifier(OrderLineModifier.Create(
                            orderLineId: line.Id,
                            modifierId: mod.Id,
                            name: mod.Name,
                            modifierType: mod.ModifierType,
                            itemCount: 1,
                            unitPrice: new Money(mod.BasePrice.Amount, mod.BasePrice.Currency),
                            basePrice: new Money(mod.BasePrice.Amount, mod.BasePrice.Currency),
                            taxRate: 0m,
                            modifierGroupId: mod.ModifierGroupId,
                            shouldPrintToKitchen: mod.ShouldPrintToKitchen));
                    }

                    ticket.AddOrderLine(line);
                }

                // Discount snapshot sometimes
                if (i % 9 == 0)
                {
                    var d = discounts[rng.Next(discounts.Count)];
                    var amount = new Money(Math.Min(3m + (i % 3), ticket.SubtotalAmount.Amount));
                    ticket.ApplyDiscount(TicketDiscount.Create(ticket.Id, d.Id, d.Name, d.Type, d.Value, amount));
                }

                // Tax exempt sometimes
                if (i % 17 == 0)
                {
                    ticket.SetTaxExempt(true);
                }

                // Void some tickets (no payment)
                if (i % 25 == 0)
                {
                    ticket.Void(manager.Id, "Customer changed mind", waste: false);
                    batchAudits.Add(AuditEvent.Create(AuditEventType.Voided, nameof(Ticket), ticket.Id, manager.Id, "{\"Action\":\"Void\"}", "Seed voided ticket"));
                    BackdateTicketInMemory(ticket, createdAt);
                    batchTickets.Add(ticket);
                    continue;
                }

                // Pay and close
                var due = ticket.TotalAmount.Amount == 0m
                    ? new Money(10m)
                    : new Money(ticket.TotalAmount.Amount, ticket.TotalAmount.Currency);

                if (i % 5 == 0)
                {
                    var tender = new Money(due.Amount + rng.Next(0, 5));
                    var pay = CashPayment.Create(ticket.Id, due, cashier.Id, terminal.Id);
                    ReflectionUtil.SetProperty(pay, "TenderAmount", tender);
                    ReflectionUtil.SetProperty(pay, "ChangeAmount", new Money(tender.Amount - due.Amount));
                    ReflectionUtil.SetProperty(pay, "CashSessionId", openCashSession.Id);
                    ticket.AddPayment(pay);
                }
                else if (i % 7 == 0)
                {
                    var first = new Money(Math.Round(due.Amount * 0.4m, 2));
                    var second = new Money(due.Amount - first.Amount);

                    var cash = CashPayment.Create(ticket.Id, first, cashier.Id, terminal.Id);
                    ReflectionUtil.SetProperty(cash, "TenderAmount", new Money(first.Amount, first.Currency));
                    ReflectionUtil.SetProperty(cash, "ChangeAmount", Money.Zero());
                    ReflectionUtil.SetProperty(cash, "CashSessionId", openCashSession.Id);
                    ticket.AddPayment(cash);

                    var cc = CreditCardPayment.Create(ticket.Id, second, cashier.Id, terminal.Id, cardNumber: "4111111111111111", cardHolderName: "Seed User", globalId: null);
                    cc.Authorize($"A{rng.Next(100000, 999999)}", referenceNumber: $"R{rng.Next(100000, 999999)}", cardType: "VISA");
                    cc.Capture();
                    if (i % 14 == 0) cc.AddTips(new Money(3m));
                    ticket.AddPayment(cc);
                }
                else
                {
                    var cc = CreditCardPayment.Create(ticket.Id, due, cashier.Id, terminal.Id, cardNumber: "5555555555554444", cardHolderName: "Seed User", globalId: null);
                    cc.Authorize($"A{rng.Next(100000, 999999)}", referenceNumber: $"R{rng.Next(100000, 999999)}", cardType: "MC");
                    cc.Capture();
                    if (i % 8 == 0) cc.AddTips(new Money(4m));
                    ticket.AddPayment(cc);
                }

                if (ticket.Status == TicketStatus.Paid)
                {
                    ticket.Close(cashier.Id);
                }

                // Kitchen order on some tickets
                if (ticket.OrderLines.Any() && i % 3 == 0)
                {
                    var ko = new KitchenOrder(ticket.Id, $"{server.FirstName} {server.LastName}", ticket.TableNumbers.Any() ? ticket.TableNumbers.First().ToString() : "ToGo");
                    foreach (var ol in ticket.OrderLines.Where(x => x.ShouldPrintToKitchen))
                    {
                        ko.AddItem(ol.Id, ol.MenuItemName, (int)Math.Ceiling(ol.Quantity), ol.PrinterGroupId ?? Guid.Empty, ol.Modifiers.Select(m => m.Name).ToList());
                    }
                    if (i % 9 == 0) ko.Bump();
                    if (i % 13 == 0) ko.Bump();
                    if (i % 29 == 0) ko.Void();
                    ReflectionUtil.SetProperty(ko, "Timestamp", createdAt);
                    batchKitchenOrders.Add(ko);
                    batchKitchenOrderItems.AddRange(ko.Items);
                }

                // Refund small subset (partial)
                if (ticket.Payments.Any(p => !p.IsVoided && p.TransactionType == TransactionType.Credit) && i % 40 == 0 && ticket.Status == TicketStatus.Closed)
                {
                    var original = ticket.Payments.First(p => !p.IsVoided && p.TransactionType == TransactionType.Credit);
                    var refund = Payment.CreateRefund(original, new Money(Math.Min(5m, original.Amount.Amount)), manager.Id, terminal.Id, reason: "Seed refund");
                    ticket.ProcessRefund(refund);
                    batchAudits.Add(AuditEvent.Create(AuditEventType.RefundProcessed, nameof(Payment), refund.Id, manager.Id, "{\"Action\":\"Refund\"}", "Seed refund payment"));
                }

                // Backdate core timestamps before insert
                BackdateTicketInMemory(ticket, createdAt);
                foreach (var p in ticket.Payments)
                {
                    ReflectionUtil.SetProperty(p, "TransactionTime", createdAt.AddMinutes(rng.Next(0, 20)));
                }

                batchTickets.Add(ticket);
            }

            db.Tickets.AddRange(batchTickets);
            db.KitchenOrders.AddRange(batchKitchenOrders);
            db.KitchenOrderItems.AddRange(batchKitchenOrderItems);
            db.AuditEvents.AddRange(batchAudits);
            await db.SaveChangesAsync(cancellationToken);
        }

        return nextTicketNumber;
    }

    private static void BackdateTicketInMemory(Ticket ticket, DateTime createdAt)
    {
        ReflectionUtil.SetProperty(ticket, "CreatedAt", createdAt);
        ReflectionUtil.SetProperty(ticket, "OpenedAt", ticket.Status == TicketStatus.Draft ? null : createdAt.AddMinutes(2));
        ReflectionUtil.SetProperty(ticket, "ClosedAt", ticket.Status == TicketStatus.Closed ? createdAt.AddMinutes(45) : null);
        ReflectionUtil.SetProperty(ticket, "ActiveDate", createdAt);
    }

    private static async Task SeedTodayOpenTicketsAsync(
        Random rng,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicket,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLine,
        ICommandHandler<ModifyOrderLineCommand> modifyOrderLine,
        ITicketRepository ticketRepo,
        ApplicationDbContext db,
        MenuSeed menu,
        List<MenuModifier> allModifiers,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Dictionary<string, Terminal> terminals,
        Dictionary<string, User> users,
        List<Discount> discounts,
        CancellationToken cancellationToken)
    {
        var server = users["server"];
        var shift = shifts["Dinner"];
        var terminal = terminals["Front POS"];
        var dineIn = orderTypes["DINE IN"];

        var pizzas = menu.Items.Where(i => i.Name.Contains("Pizza", StringComparison.OrdinalIgnoreCase)).ToList();
        var burgers = menu.Items.Where(i => i.CategoryId == menu.Categories.First(c => c.Name == "Burgers").Id).ToList();
        var drinks = menu.Items.Where(i => i.CategoryId == menu.Categories.First(c => c.Name == "Drinks").Id).ToList();

        for (var i = 0; i < 10; i++)
        {
            var table = 1 + (i % 20);
            var ticketId = (await createTicket.HandleAsync(new CreateTicketCommand
            {
                CreatedBy = server.Id,
                TerminalId = terminal.Id,
                ShiftId = shift.Id,
                OrderTypeId = dineIn.Id,
                NumberOfGuests = rng.Next(1, 6),
                TableNumbers = new List<int> { table }
            }, cancellationToken)).TicketId;

            // IMPORTANT:
            // Clear tracking so subsequent ticket loads have correct concurrency originals.
            // Otherwise, newly-created tickets can be served from the tracker with incorrect Version originals,
            // causing optimistic concurrency failures on the first AddOrderLine update.
            db.ChangeTracker.Clear();

            // Add 2-5 items
            var lineCount = rng.Next(2, 6);
            for (var l = 0; l < lineCount; l++)
            {
                var mi = (l % 3 == 0 ? pizzas : l % 3 == 1 ? burgers : drinks)[rng.Next(0, 5)];
                var mods = PickModifiersForMenuItem(allModifiers, mi.Name, rng);

                await addOrderLine.HandleAsync(new AddOrderLineCommand
                {
                    TicketId = ticketId,
                    MenuItemId = mi.Id,
                    MenuItemName = mi.Name,
                    Quantity = 1,
                    UnitPrice = mi.Price,
                    TaxRate = 0m, // avoid double-taxing at line + ticket; ticket uses simplified tax
                    CategoryName = menu.Categories.First(c => c.Id == mi.CategoryId).Name,
                    GroupName = menu.Groups.First(g => g.Id == mi.GroupId).Name,
                    AddedBy = server.Id,
                    Modifiers = mods
                }, cancellationToken);
            }

            // Make one pizza ticket with left/right half modifiers via ModifyOrderLine (fractional flow)
            if (i == 0)
            {
                var t = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
                var pizzaLine = t?.OrderLines.FirstOrDefault(ol => ol.MenuItemName.Contains("Pizza", StringComparison.OrdinalIgnoreCase));
                if (pizzaLine != null)
                {
                    await modifyOrderLine.HandleAsync(new ModifyOrderLineCommand
                    {
                        TicketId = ticketId,
                        OrderLineId = pizzaLine.Id,
                        Quantity = pizzaLine.Quantity,
                        Instructions = "HALF PEPPERONI / HALF MUSHROOMS",
                        Modifiers = new List<OrderLineModifierDto>
                        {
                            new()
                            {
                                OrderLineId = pizzaLine.Id,
                                ModifierId = null,
                                Name = "Pepperoni",
                                ModifierType = ModifierType.Normal,
                                ItemCount = 1,
                                UnitPrice = 1.00m,
                                TaxRate = 0m,
                                SectionName = "Left",
                                ShouldPrintToKitchen = true,
                                CreatedAt = DateTime.UtcNow
                            },
                            new()
                            {
                                OrderLineId = pizzaLine.Id,
                                ModifierId = null,
                                Name = "Mushrooms",
                                ModifierType = ModifierType.Normal,
                                ItemCount = 1,
                                UnitPrice = 0.75m,
                                TaxRate = 0m,
                                SectionName = "Right",
                                ShouldPrintToKitchen = true,
                                CreatedAt = DateTime.UtcNow
                            }
                        }
                    }, cancellationToken);
                }
            }

            // Apply a ticket note sometimes
            if (i % 4 == 0)
            {
                var t = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
                t?.SetNote("Allergy: NO NUTS. Server confirm with kitchen.");
                if (t != null) await ticketRepo.UpdateAsync(t, cancellationToken);
            }

            // Apply a simple manual discount snapshot sometimes (domain-level)
            if (i % 5 == 0)
            {
                var t = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
                if (t != null)
                {
                    var d = discounts[0];
                    var amount = new Money(Math.Min(5m, t.SubtotalAmount.Amount));
                    t.ApplyDiscount(TicketDiscount.Create(t.Id, d.Id, d.Name, d.Type, d.Value, amount));
                    await ticketRepo.UpdateAsync(t, cancellationToken);
                }
            }
        }
    }

    private static async Task SeedScheduledTicketsAsync(
        Random rng,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicket,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLine,
        ITicketRepository ticketRepo,
        ApplicationDbContext db,
        MenuSeed menu,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Terminal terminal,
        User createdBy,
        int count,
        CancellationToken cancellationToken)
    {
        var shift = shifts["Dinner"];
        var pickup = orderTypes["PICK UP"];
            var items = menu.Items.Where(i => i.CategoryId == menu.Categories.First(c => c.Name == "Pizzas").Id).Take(10).ToList();

        for (var i = 0; i < count; i++)
        {
            var res = await createTicket.HandleAsync(new CreateTicketCommand
            {
                CreatedBy = createdBy.Id,
                TerminalId = terminal.Id,
                ShiftId = shift.Id,
                OrderTypeId = pickup.Id,
                NumberOfGuests = 0
            }, cancellationToken);

            var ticketId = res.TicketId;
            db.ChangeTracker.Clear();
            var mi = items[rng.Next(items.Count)];
            await addOrderLine.HandleAsync(new AddOrderLineCommand
            {
                TicketId = ticketId,
                MenuItemId = mi.Id,
                MenuItemName = mi.Name,
                Quantity = 1,
                UnitPrice = mi.Price,
                TaxRate = 0m,
                CategoryName = "Pizzas",
                GroupName = "Classic Pizzas",
                AddedBy = createdBy.Id
            }, cancellationToken);

            var t = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
            if (t != null)
            {
                t.Schedule(DateTime.UtcNow.AddHours(1 + i));
                await ticketRepo.UpdateAsync(t, cancellationToken);
            }
        }
    }

    private static async Task SeedHistoricalTicketsAsync(
        Random rng,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicket,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLine,
        ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> processPayment,
        ICommandHandler<CloseTicketCommand> closeTicket,
        ICommandHandler<VoidTicketCommand> voidTicket,
        ICommandHandler<SplitTicketCommand, SplitTicketResult> splitTicket,
        ICommandHandler<RefundPaymentCommand, RefundPaymentResult> refundPayment,
        ITicketRepository ticketRepo,
        IKitchenOrderRepository kitchenOrderRepo,
        ApplicationDbContext db,
        CashSession openCashSession,
        MenuSeed menu,
        List<MenuModifier> allModifiers,
        Dictionary<string, OrderType> orderTypes,
        Dictionary<string, Shift> shifts,
        Terminal front,
        Terminal bar,
        User cashier,
        User manager,
        User server,
        List<Discount> discounts,
        SeedOptions options,
        CancellationToken cancellationToken)
    {
        var dineIn = orderTypes["DINE IN"];
        var takeOut = orderTypes["TAKE OUT"];
        var barTab = orderTypes["BAR TAB"];

        var menuItems = menu.Items.Where(i => i.IsActive).ToList();

        for (var daysAgo = options.HistoryDays; daysAgo >= 1; daysAgo--)
        {
            var day = DateTime.UtcNow.Date.AddDays(-daysAgo);
            var ticketCount = rng.Next(options.TicketsPerDayMin, options.TicketsPerDayMax + 1);

            for (var i = 0; i < ticketCount; i++)
            {
                var shift = shifts.Values.ElementAt(rng.Next(shifts.Count));
                var orderType = i % 7 == 0 ? barTab : (i % 4 == 0 ? takeOut : dineIn);
                var terminal = orderType == barTab ? bar : front;

                var tableNums = orderType == dineIn ? new List<int> { rng.Next(1, 21) } : null;

                var ticketRes = await createTicket.HandleAsync(new CreateTicketCommand
                {
                    CreatedBy = server.Id,
                    TerminalId = terminal.Id,
                    ShiftId = shift.Id,
                    OrderTypeId = orderType.Id,
                    NumberOfGuests = orderType == dineIn ? rng.Next(1, 6) : 0,
                    TableNumbers = tableNums
                }, cancellationToken);

                var ticketId = ticketRes.TicketId;
                db.ChangeTracker.Clear();

                // Add 1-6 items
                var lineCount = rng.Next(1, 7);
                for (var l = 0; l < lineCount; l++)
                {
                    var mi = menuItems[rng.Next(menuItems.Count)];
                    var mods = PickModifiersForMenuItem(allModifiers, mi.Name, rng);

                    await addOrderLine.HandleAsync(new AddOrderLineCommand
                    {
                        TicketId = ticketId,
                        MenuItemId = mi.Id,
                        MenuItemName = mi.Name,
                        Quantity = 1,
                        UnitPrice = mi.Price,
                        TaxRate = 0m,
                        CategoryName = mi.CategoryId.HasValue ? menu.Categories.First(c => c.Id == mi.CategoryId).Name : null,
                        GroupName = mi.GroupId.HasValue ? menu.Groups.First(g => g.Id == mi.GroupId).Name : null,
                        AddedBy = server.Id,
                        Modifiers = mods
                    }, cancellationToken);
                }

                var ticket = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
                if (ticket == null) continue;

                // Apply a discount snapshot sometimes
                if (i % 9 == 0)
                {
                    var d = discounts[rng.Next(discounts.Count)];
                    var amount = new Money(Math.Min(3m + (i % 3), ticket.SubtotalAmount.Amount));
                    ticket.ApplyDiscount(TicketDiscount.Create(ticket.Id, d.Id, d.Name, d.Type, d.Value, amount));
                }

                // Sometimes mark tax exempt
                if (i % 17 == 0)
                {
                    ticket.SetTaxExempt(true);
                }

                await ticketRepo.UpdateAsync(ticket, cancellationToken);

                // Void some tickets (no payment)
                if (i % 25 == 0)
                {
                await voidTicket.HandleAsync(new VoidTicketCommand
                    {
                        TicketId = ticket.Id,
                        VoidedBy = manager.Id,
                        Reason = "Customer changed mind",
                        IsWasted = false
                    }, cancellationToken);

                    // Backdate ticket times
                    BackdateTicket(ticket, day.AddHours(11 + rng.Next(0, 10)));
                    await ticketRepo.UpdateAsync(ticket, cancellationToken);
                    continue;
                }

                // Split a few tickets (moves order lines) to exercise workflow
                if (i % 33 == 0 && ticket.OrderLines.Count >= 2)
                {
                    var toSplit = ticket.OrderLines.Take(1).Select(ol => ol.Id).ToList();
                    await splitTicket.HandleAsync(new SplitTicketCommand
                    {
                        OriginalTicketId = ticket.Id,
                        OrderLineIdsToSplit = toSplit,
                        SplitBy = server.Id,
                        TerminalId = terminal.Id,
                        ShiftId = shift.Id,
                        OrderTypeId = orderType.Id
                    }, cancellationToken);
                }

                // Pay (cash vs card vs split tender)
                var due = ticket.TotalAmount;
                if (due.Amount <= 0m)
                {
                    due = new Money(10m);
                }

                if (i % 5 == 0)
                {
                    // Cash payment (with tender/change)
                    var tender = new Money(due.Amount + rng.Next(0, 5));
                    await processPayment.HandleAsync(new ProcessPaymentCommand
                    {
                        TicketId = ticket.Id,
                        PaymentType = PaymentType.Cash,
                        Amount = due,
                        TenderAmount = tender,
                        TipsAmount = i % 10 == 0 ? new Money(2m) : null,
                        ProcessedBy = cashier.Id,
                        TerminalId = terminal.Id,
                        CashSessionId = openCashSession.Id
                    }, cancellationToken);
                }
                else if (i % 7 == 0)
                {
                    // Split tender (cash + card)
                    var first = new Money(Math.Round(due.Amount * 0.4m, 2));
                    var second = new Money(due.Amount - first.Amount);

                    await processPayment.HandleAsync(new ProcessPaymentCommand
                    {
                        TicketId = ticket.Id,
                        PaymentType = PaymentType.Cash,
                        Amount = first,
                        TenderAmount = new Money(first.Amount),
                        ProcessedBy = cashier.Id,
                        TerminalId = terminal.Id,
                        CashSessionId = openCashSession.Id
                    }, cancellationToken);

                    await processPayment.HandleAsync(new ProcessPaymentCommand
                    {
                        TicketId = ticket.Id,
                        PaymentType = PaymentType.CreditCard,
                        Amount = second,
                        TipsAmount = i % 14 == 0 ? new Money(3m) : null,
                        ProcessedBy = cashier.Id,
                        TerminalId = terminal.Id,
                        CardType = "VISA",
                        Last4 = $"{rng.Next(1000, 9999)}",
                        AuthCode = $"A{rng.Next(100000, 999999)}"
                    }, cancellationToken);
                }
                else
                {
                    // Card payment
                    await processPayment.HandleAsync(new ProcessPaymentCommand
                    {
                        TicketId = ticket.Id,
                        PaymentType = PaymentType.CreditCard,
                        Amount = due,
                        TipsAmount = i % 8 == 0 ? new Money(4m) : null,
                        ProcessedBy = cashier.Id,
                        TerminalId = terminal.Id,
                        CardType = "MC",
                        Last4 = $"{rng.Next(1000, 9999)}",
                        AuthCode = $"A{rng.Next(100000, 999999)}"
                    }, cancellationToken);
                }

                // Close ticket if paid
                ticket = await ticketRepo.GetByIdAsync(ticket.Id, cancellationToken);
                if (ticket != null && ticket.Status == TicketStatus.Paid)
                {
                    await closeTicket.HandleAsync(new CloseTicketCommand
                    {
                        TicketId = ticket.Id,
                        ClosedBy = cashier.Id
                    }, cancellationToken);
                }

                ticket = await ticketRepo.GetByIdAsync(ticketId, cancellationToken);
                if (ticket == null) continue;

                // Kitchen/KDS: create kitchen orders for some tickets
                if (ticket.OrderLines.Any() && i % 3 == 0)
                {
                    var serverName = $"{server.FirstName} {server.LastName}";
                    var tableName = ticket.TableNumbers.Any() ? ticket.TableNumbers.First().ToString() : "ToGo";

                    var ko = new KitchenOrder(ticket.Id, serverName, tableName);
                    foreach (var ol in ticket.OrderLines.Where(x => x.ShouldPrintToKitchen))
                    {
                        ko.AddItem(
                            ol.Id,
                            ol.MenuItemName,
                            (int)Math.Ceiling(ol.Quantity),
                            ol.PrinterGroupId ?? Guid.Empty,
                            ol.Modifiers.Select(m => m.Name).ToList());
                    }

                    // Set a mix of statuses
                    if (i % 9 == 0) ko.Bump();
                    if (i % 13 == 0) ko.Bump();
                    if (i % 29 == 0) ko.Void();

                    // Backdate
                    ReflectionUtil.SetProperty(ko, "Timestamp", day.AddHours(11 + rng.Next(0, 11)));

                    await kitchenOrderRepo.AddAsync(ko);
                }

                // Refund a small subset
                if (ticket.Payments.Any(p => !p.IsVoided && p.TransactionType == TransactionType.Credit) && i % 40 == 0)
                {
                    var original = ticket.Payments.First(p => !p.IsVoided && p.TransactionType == TransactionType.Credit);
                    await refundPayment.HandleAsync(new RefundPaymentCommand
                    {
                        OriginalPaymentId = original.Id,
                        RefundAmount = new Money(Math.Min(5m, original.Amount.Amount)),
                        ProcessedBy = manager.Id,
                        TerminalId = terminal.Id,
                        Reason = "Test refund",
                    }, cancellationToken);
                }

                // Backdate ticket + payments
                BackdateTicket(ticket, day.AddHours(11 + rng.Next(0, 11)));
                foreach (var p in ticket.Payments)
                {
                    ReflectionUtil.SetProperty(p, "TransactionTime", day.AddHours(11 + rng.Next(0, 11)).AddMinutes(rng.Next(0, 55)));
                }

                await ticketRepo.UpdateAsync(ticket, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private static List<MenuModifier> PickModifiersForMenuItem(List<MenuModifier> all, string itemName, Random rng)
    {
        // Keep it simple and realistic without assuming complex menu rules.
        if (itemName.Contains("Pizza", StringComparison.OrdinalIgnoreCase))
        {
            var size = all.Where(m => m.Name.Contains("Small") || m.Name.Contains("Medium") || m.Name.Contains("Large")).ToList();
            var toppings = all.Where(m => m.ModifierGroupId != null && m.Name is "Pepperoni" or "Mushrooms" or "Onions" or "Extra Cheese").ToList();

            var selected = new List<MenuModifier> { size[rng.Next(size.Count)] };
            var tCount = rng.Next(0, 4);
            selected.AddRange(toppings.OrderBy(_ => rng.Next()).Take(tCount));
            return selected;
        }

        if (itemName.Contains("Burger", StringComparison.OrdinalIgnoreCase))
        {
            var temps = all.Where(m => m.Name is "Rare" or "Medium" or "Well Done").ToList();
            var addons = all.Where(m => m.Name is "Bacon" or "Avocado" or "Extra Patty").ToList();
            var selected = new List<MenuModifier> { temps[rng.Next(temps.Count)] };
            selected.AddRange(addons.OrderBy(_ => rng.Next()).Take(rng.Next(0, 3)));
            return selected;
        }

        if (itemName.Contains("Coke", StringComparison.OrdinalIgnoreCase)
            || itemName.Contains("Tea", StringComparison.OrdinalIgnoreCase)
            || itemName.Contains("Lemonade", StringComparison.OrdinalIgnoreCase))
        {
            var sizes = all.Where(m => m.ModifierGroupId != null && (m.Name == "Small" || m.Name == "Medium" || m.Name == "Large")).ToList();
            return new List<MenuModifier> { sizes[rng.Next(sizes.Count)] };
        }

        if (itemName.Contains("Coffee", StringComparison.OrdinalIgnoreCase) || itemName.Contains("Latte", StringComparison.OrdinalIgnoreCase))
        {
            var milk = all.Where(m => m.Name is "Whole Milk" or "Oat Milk").ToList();
            var syrup = all.Where(m => m.Name.Contains("Syrup")).ToList();
            var selected = new List<MenuModifier>();
            if (rng.NextDouble() < 0.6) selected.Add(milk[rng.Next(milk.Count)]);
            selected.AddRange(syrup.OrderBy(_ => rng.Next()).Take(rng.Next(0, 2)));
            return selected;
        }

        return new List<MenuModifier>();
    }

    private static void BackdateTicket(Ticket ticket, DateTime createdAt)
    {
        ReflectionUtil.SetProperty(ticket, "CreatedAt", createdAt);
        ReflectionUtil.SetProperty(ticket, "OpenedAt", createdAt.AddMinutes(2));
        ReflectionUtil.SetProperty(ticket, "ClosedAt", ticket.Status == TicketStatus.Closed ? createdAt.AddMinutes(45) : null);
        ReflectionUtil.SetProperty(ticket, "ActiveDate", createdAt);
    }

    private static async Task SeedAttendanceHistoryAsync(
        ApplicationDbContext db,
        Dictionary<string, User> users,
        Dictionary<string, Shift> shifts,
        SeedOptions options,
        CancellationToken cancellationToken)
    {
        // Simple labor history: server + cashier clocked in/out most days.
        var server = users["server"];
        var cashier = users["cashier"];
        var dinner = shifts["Dinner"];

        for (var d = 1; d <= options.HistoryDays; d++)
        {
            var day = DateTime.UtcNow.Date.AddDays(-d);

            var serverAttendance = AttendanceHistory.Create(server.Id, dinner.Id);
            ReflectionUtil.SetProperty(serverAttendance, "ClockInTime", day.AddHours(16));
            ReflectionUtil.SetProperty(serverAttendance, "ClockOutTime", day.AddHours(22));

            var cashierAttendance = AttendanceHistory.Create(cashier.Id, dinner.Id);
            ReflectionUtil.SetProperty(cashierAttendance, "ClockInTime", day.AddHours(15));
            ReflectionUtil.SetProperty(cashierAttendance, "ClockOutTime", day.AddHours(23));

            db.AttendanceHistories.AddRange(serverAttendance, cashierAttendance);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

internal static class DbFacts
{
    public static async Task<string> GetCurrentDatabaseAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        await db.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            await using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "select current_database();";
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return Convert.ToString(result) ?? string.Empty;
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }
}

internal sealed record MenuSeed(
    IReadOnlyList<MenuCategory> Categories,
    IReadOnlyList<MenuGroup> Groups,
    IReadOnlyList<MenuItem> Items,
    ComboDefinition LunchComboDefinition);


