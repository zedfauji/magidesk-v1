using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Security;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

// Migrations runner for Magidesk.
// Usage:
//   1) Set MAGIDESK_CONNECTION_STRING (recommended), or
//   2) Put ConnectionStrings:Magidesk in Magidesk.Migrations/appsettings.json
// Then run:
//   dotnet run --project Magidesk.Migrations

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    Environment.GetEnvironmentVariable("MAGIDESK_CONNECTION_STRING")
    ?? configuration.GetConnectionString("Magidesk")
    ?? "Host=localhost;Port=5432;Database=magidesk_new;Username=postgres;Password=postgres;";

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseNpgsql(connectionString);

await using var db = new ApplicationDbContext(optionsBuilder.Options);

Console.WriteLine("Applying EF Core migrations...");
await db.Database.MigrateAsync();
Console.WriteLine("Migrations applied.");

// Seeding
if (!await db.Users.AnyAsync())
{
    Console.WriteLine("Seeding default user...");
    
    // Manually instantiate AesEncryptionService
    var inMemConfig = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"Security:EncryptionKey", "MagideskDevKey_MustChangeInProd!"}
        })
        .Build();
        
    var encryptionService = new AesEncryptionService(inMemConfig);
    var encryptedPin = encryptionService.Encrypt("123");

    // Create Role first
    // Assuming 0 or MaxValue for permissions. Casting 0 for now as placeholder for "All" or "None" depending on enum defaults, 
    // but typically explicit flags are safer. Given I can't see the Enum, I'll use (UserPermission)0 or try to import it.
    // Actually, let's just cast 0.
    var role = Role.Create("Manager", (UserPermission)0); 
    await db.Roles.AddAsync(role);
    
    var user = User.Create(
        "manager",
        "System",
        "Manager",
        role.Id, // Link to created role
        encryptedPin, // PIN 123
        null // No Password
    );
     
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    
    Console.WriteLine($"Seeded user 'manager' with PIN '123'.");
}
else
{
    Console.WriteLine("Users already exist. Skipping seed.");
}

// Seed Order Types
if (!await db.OrderTypes.AnyAsync())
{
    Console.WriteLine("Seeding Default Order Types...");

    var dineIn = OrderType.Create("DINE IN", closeOnPaid: false, allowSeatBasedOrder: true, allowToAddTipsLater: true);
    dineIn.SetProperty("RequiresTable", "true"); // F-0020
    await db.OrderTypes.AddAsync(dineIn);

    var takeOut = OrderType.Create("TAKE OUT", closeOnPaid: true);
    takeOut.SetProperty("RequiresCustomer", "false"); 
    await db.OrderTypes.AddAsync(takeOut);
    
    var pickUp = OrderType.Create("PICK UP", closeOnPaid: true);
    pickUp.SetProperty("RequiresCustomer", "true"); // Often requires customer info
    await db.OrderTypes.AddAsync(pickUp);

    var delivery = OrderType.Create("DELIVERY", closeOnPaid: false);
    delivery.SetProperty("RequiresCustomer", "true");
    await db.OrderTypes.AddAsync(delivery);

    var barTab = OrderType.Create("BAR TAB", isBarTab: true, allowToAddTipsLater: true);
    await db.OrderTypes.AddAsync(barTab);

    await db.SaveChangesAsync();
    Console.WriteLine("Seeded 5 Order Types.");
}
else
{
    Console.WriteLine("OrderTypes already exist. Skipping seed.");
}

// Seed Shifts
if (!await db.Shifts.AnyAsync())
{
    Console.WriteLine("Seeding Default Shifts...");

    // Breakfast: 06:00 - 11:00
    await db.Shifts.AddAsync(Shift.Create("Breakfast", new TimeSpan(6, 0, 0), new TimeSpan(11, 0, 0)));

    // Lunch: 11:00 - 16:00
    await db.Shifts.AddAsync(Shift.Create("Lunch", new TimeSpan(11, 0, 0), new TimeSpan(16, 0, 0)));

    // Dinner: 16:00 - 22:00
    await db.Shifts.AddAsync(Shift.Create("Dinner", new TimeSpan(16, 0, 0), new TimeSpan(22, 0, 0)));

    // Night: 22:00 - 06:00 (Spans midnight)
    await db.Shifts.AddAsync(Shift.Create("Night", new TimeSpan(22, 0, 0), new TimeSpan(6, 0, 0)));

    await db.SaveChangesAsync();
    Console.WriteLine("Seeded 4 Shifts.");
}

else
{
    Console.WriteLine("Shifts already exist. Skipping seed.");
}

// Seed Menu Data
if (!await db.MenuCategories.AnyAsync())
{
    Console.WriteLine("Seeding Default Menu...");

    // 1. Categories
    var beverages = MenuCategory.Create("Beverages");
    var food = MenuCategory.Create("Food");
    await db.MenuCategories.AddRangeAsync(beverages, food);
    // Be sure to save to get Generate IDs if needed (though Create() usually generates GUIDs)
    await db.SaveChangesAsync();

    // 2. Groups
    var softDrinks = MenuGroup.Create("Soft Drinks", beverages.Id);
    var pizzas = MenuGroup.Create("Pizzas", food.Id);
    await db.MenuGroups.AddRangeAsync(softDrinks, pizzas);
    await db.SaveChangesAsync();

    // 3. Items
    // 3. Items
    var coke = MenuItem.Create("Coke", new Magidesk.Domain.ValueObjects.Money(2.50m));
    coke.SetGroup(softDrinks.Id);
    
    var dietCoke = MenuItem.Create("Diet Coke", new Magidesk.Domain.ValueObjects.Money(2.50m));
    dietCoke.SetGroup(softDrinks.Id);
    
    var sprite = MenuItem.Create("Sprite", new Magidesk.Domain.ValueObjects.Money(2.50m));
    sprite.SetGroup(softDrinks.Id);
    
    var cheesePizza = MenuItem.Create("Cheese Pizza", new Magidesk.Domain.ValueObjects.Money(12.00m));
    cheesePizza.SetGroup(pizzas.Id);
    
    var pepperoniPizza = MenuItem.Create("Pepperoni Pizza", new Magidesk.Domain.ValueObjects.Money(14.00m));
    pepperoniPizza.SetGroup(pizzas.Id);

    await db.MenuItems.AddRangeAsync(coke, dietCoke, sprite, cheesePizza, pepperoniPizza);
    await db.SaveChangesAsync();

    Console.WriteLine("Seeded Menu: Beverages (Coke, Diet Coke, Sprite), Food (Cheese Pizza, Pepperoni Pizza).");
}
else
{
    Console.WriteLine("Menu Data already exists. Skipping seed.");
}

Console.WriteLine("Done.");
