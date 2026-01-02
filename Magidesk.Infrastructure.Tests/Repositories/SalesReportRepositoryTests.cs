using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

[Collection("Database Tests")]
public class SalesReportRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SalesReportRepository _repository;

    public SalesReportRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new SalesReportRepository(_context);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetSalesBalanceAsync_ShouldAggregateClosedTickets()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);

        // Ticket 1: $100 + $10 tax = $110 Total
        var t1 = await CreateClosedTicketAsync(100m, 10m, start.AddHours(10));
        
        // Ticket 2: $50 + $5 tax = $55 Total
        var t2 = await CreateClosedTicketAsync(50m, 5m, start.AddHours(12));

        // Ticket 3: Outside date range (Yesterday)
        var t3 = await CreateClosedTicketAsync(200m, 20m, start.AddDays(-1));

        // Act
        var report = await _repository.GetSalesBalanceAsync(start, end);

        // Assert
        Assert.Equal(2, report.Sales.TicketCount);
        Assert.Equal(165m, report.Sales.TotalGrossSales); // 110 + 55
        Assert.Equal(150m, report.Sales.NetSales); // 100 + 50
        Assert.Equal(15m, report.Sales.TaxAmount); // 10 + 5
    }

    [Fact]
    public async Task GetSalesBalanceAsync_ShouldAggregatePayments()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);

        // Payment 1: Cash $110
        await CreatePaymentAsync(110m, TransactionType.Credit, PaymentType.Cash, start.AddHours(10));

        // Payment 2: Card $55
        await CreatePaymentAsync(55m, TransactionType.Credit, PaymentType.CreditCard, start.AddHours(12));

        // Payment 3: Refund $10 (Debit)
        await CreatePaymentAsync(10m, TransactionType.Debit, PaymentType.Cash, start.AddHours(13));
        
        // Payment 4: Outside range
        await CreatePaymentAsync(100m, TransactionType.Credit, PaymentType.Cash, start.AddDays(-1));

        // Act
        var report = await _repository.GetSalesBalanceAsync(start, end);

        // Assert
        Assert.Equal(165m, report.Payments.TotalCollected); // 110 + 55
        Assert.Equal(10m, report.Payments.TotalRefunded);
        Assert.Equal(155m, report.Payments.NetCollected); // 165 - 10
        
        // Types
        var cashSummary = report.Payments.ByType.FirstOrDefault(x => x.PaymentType == "Cash");
        Assert.NotNull(cashSummary);
        Assert.Equal(100m, cashSummary.Amount); // 110 credit - 10 debit
        
        var visaSummary = report.Payments.ByType.FirstOrDefault(x => x.PaymentType == "CreditCard");
        Assert.NotNull(visaSummary);
        Assert.Equal(55m, visaSummary.Amount);
    }

    [Fact]
    public async Task GetSalesSummaryAsync_ShouldAggregateByCategory()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);

        // Ticket 1: 2 Burgers ($10 each) + 1 Coke ($2)
        // Burgers: Cat=Food, Grp=Mains
        // Coke: Cat=Drinks, Grp=SoftDrinks
        var t1 = Ticket.Create(1001, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var burgerLine = OrderLine.Create(t1.Id, Guid.NewGuid(), "Burger", 2, new Money(10m), 0.1m, "Food", "Mains");
        var cokeLine = OrderLine.Create(t1.Id, Guid.NewGuid(), "Coke", 1, new Money(2m), 0.1m, "Drinks", "SoftDrinks");
        
        // Reflection for Closed Status
        SetProperty(t1, "Status", TicketStatus.Closed);
        SetProperty(t1, "ClosedAt", start.AddHours(12));
        SetProperty(t1, "SubtotalAmount", new Money(22m)); // 20 + 2
        SetProperty(t1, "TotalAmount", new Money(24.2m)); // 22 + 2.2 tax
        
        _context.Tickets.Add(t1);
        _context.Entry(t1).Collection(t => t.OrderLines).CurrentValue = new List<OrderLine> { burgerLine, cokeLine };
        // We need to add lines to context explicitly if not attached via navigation automatically in setup
        // But since we are setting them via collection accessor or just adding them to context?
        // Let's explicitly add them to context to be safe
        _context.Set<OrderLine>().AddRange(burgerLine, cokeLine);


        // Ticket 2: 1 Salad ($8)
        // Salad: Cat=Food, Grp=Starters
        var t2 = Ticket.Create(1002, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var saladLine = OrderLine.Create(t2.Id, Guid.NewGuid(), "Salad", 1, new Money(8m), 0.1m, "Food", "Starters");
        
        SetProperty(t2, "Status", TicketStatus.Closed);
        SetProperty(t2, "ClosedAt", start.AddHours(13));
        SetProperty(t2, "SubtotalAmount", new Money(8m));
        SetProperty(t2, "TotalAmount", new Money(8.8m));
        
        _context.Tickets.Add(t2);
        _context.Set<OrderLine>().Add(saladLine);

        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetSalesSummaryAsync(start, end, includeGroups: true);

        // Assert
        Assert.Equal(22m + 8m, report.Totals.TotalGrossSales); // 30
        
        // Food Category
        var foodCat = report.Categories.FirstOrDefault(c => c.Name == "Food");
        Assert.NotNull(foodCat);
        Assert.Equal(28m, foodCat.GrossSales); // 20 (Burgers) + 8 (Salad)
        Assert.Equal(2, foodCat.Groups.Count);
        
        // Food Groups
        var mainsGroup = foodCat.Groups.FirstOrDefault(g => g.Name == "Mains");
        Assert.Equal(20m, mainsGroup.GrossSales);
        
        var startersGroup = foodCat.Groups.FirstOrDefault(g => g.Name == "Starters");
        Assert.Equal(8m, startersGroup.GrossSales);

        // Drinks Category
        var drinksCat = report.Categories.FirstOrDefault(c => c.Name == "Drinks");
        Assert.NotNull(drinksCat);
        Assert.Equal(2m, drinksCat.GrossSales);
    }
    


    [Fact]
    public async Task GetExceptionsReportAsync_ShouldAggregateItems()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);

        // 1. Voided Ticket
        var tVoid = Ticket.Create(2001, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(tVoid, "SubtotalAmount", new Money(100m));
        SetProperty(tVoid, "TotalAmount", new Money(110m));
        tVoid.Void(new UserId(Guid.NewGuid()), "Test Reason", false); // Use public method if possible, or Reflection
        // The public Void method sets Status and ActiveDate.
        // Assuming Void method logic is sufficient, but let's ensure properties are set if we use helper
        SetProperty(tVoid, "ActiveDate", start.AddHours(10));
        
        _context.Tickets.Add(tVoid);

        // 2. Refund Payment
        var tRefund = Ticket.Create(2002, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _context.Tickets.Add(tRefund);
        await _context.SaveChangesAsync(); // Need ID

        var refundPayment = CashPayment.Create(tRefund.Id, new Money(50m), new UserId(Guid.NewGuid()), Guid.NewGuid());
        // Set as Debit/Refund
        SetProperty(refundPayment, "TransactionType", TransactionType.Debit);
        SetProperty(refundPayment, "PaymentType", PaymentType.Cash);
        SetProperty(refundPayment, "TransactionTime", start.AddHours(11));
        SetProperty(refundPayment, "Note", "Customer disliked food");
        
        _context.Payments.Add(refundPayment);

        // 3. Discount
        // TicketDiscounts are usually added via Ticket.ApplyDiscount which adds to private list _discounts.
        // But for repository test we query DbSet<TicketDiscount>.
        // Check if TicketDiscount is added to DbSet explicitly or via navigation.
        // ApplicationDbContext has DbSet<TicketDiscount>.
        // We can add directly to DbSet.
        var tDiscount = Ticket.Create(2003, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _context.Tickets.Add(tDiscount);
        await _context.SaveChangesAsync();

        var discount = TicketDiscount.Create(tDiscount.Id, Guid.NewGuid(), "Promo 10", DiscountType.Percentage, 10, new Money(10m));
        // Need to set AppliedAt manually as Create sets it to Now? 
        // Create sets AppliedAt = DateTime.UtcNow.
        // If we want it in range, Now is fine if Start/End covers it.
        // Wait, start/end is today.Date to today.Date+1.
        // Discount created now should be in range.
        
        _context.TicketDiscounts.Add(discount);
        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetExceptionsReportAsync(start, end);

        // Assert
        Assert.Single(report.Voids);
        Assert.Equal(110m, report.Voids[0].Amount);
        
        Assert.Single(report.Refunds);
        Assert.Equal(50m, report.Refunds[0].Amount);
        Assert.Equal("Customer disliked food", report.Refunds[0].Reason);

        Assert.Equal("Promo 10", report.Discounts[0].Name);
    }

    [Fact]
    public async Task GetJournalReportAsync_ShouldReturnAuditEvents()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);
        
        // 1. Create User
        // User.Create(username, firstName, lastName, roleId, encryptedPin, encryptedPassword)
        var user = User.Create("admin", "Admin", "User", Guid.NewGuid(), null, "password");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // 2. Create Audit Event
        var auditEvent = AuditEvent.Create(
            AuditEventType.Created,
            "Ticket",
            Guid.NewGuid(),
            user.Id,
            "{Status: Open}",
            "Created new ticket",
            null
        );
        // Can't set timestamp on AuditEvent (private set, set to UtcNow in Create).
        // Since we are creating it now, it will be in range of today.Date to today.Date+1.
        
        _context.AuditEvents.Add(auditEvent);
        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetJournalReportAsync(start, end, null, null);

        // Assert
        Assert.Single(report.Entries);
        var entry = report.Entries[0];
        Assert.Equal("Created", entry.EventType);
        
        Assert.Equal("Admin User", entry.User);
        Assert.Equal("Created new ticket", entry.Description);
    }
    
    [Fact]
    public async Task GetServerProductivityAsync_ShouldAggregateStats()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);
        
        // 1. Create User
        var user = User.Create("Server", "John", "Doe", Guid.NewGuid(), null, "password");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 2. Create Ticket (Sales)
        var ticket = Ticket.Create(3001, new UserId(user.Id), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(ticket, "SubtotalAmount", new Money(200m));
        SetProperty(ticket, "TotalAmount", new Money(220m));
        SetProperty(ticket, "Status", TicketStatus.Closed);
        SetProperty(ticket, "ClosedAt", start.AddHours(14));
        _context.Tickets.Add(ticket);

        // 3. Create Gratuity (Tips)
        // Ensure Gratuity has CreatedAt set in Create, or set it manually if needed.
        // Gratuity.Create sets CreatedAt = UtcNow.
        var gratuity = Gratuity.Create(ticket.Id, new Money(20m), Guid.NewGuid(), new UserId(user.Id));
        // Force timestamp to be in range (it is UtcNow, so yes)
        _context.Gratuities.Add(gratuity);

        // 4. Create Cash Session (Time) - 5 Hours
        var session = CashSession.Open(new UserId(user.Id), Guid.NewGuid(), Guid.NewGuid(), new Money(100m));
        SetProperty(session, "OpenedAt", start.AddHours(10));
        SetProperty(session, "ClosedAt", start.AddHours(15));
        SetProperty(session, "Status", CashSessionStatus.Closed);
        _context.CashSessions.Add(session);

        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetServerProductivityAsync(start, end, null);

        // Assert
        Assert.Single(report.ServerStats);
        var stats = report.ServerStats[0];
        
        Assert.Equal("John Doe", stats.UserName);
        Assert.Equal(200m, stats.TotalSales); // Net Sales
        Assert.Equal(20m, stats.TipsCollected);
        Assert.Equal(5.0, stats.TotalHours, 1); // 10 to 15 = 5 hours
        Assert.Equal(40m, stats.SalesPerHour); // 200 / 5 = 40
        Assert.Equal(1, stats.TicketCount);
    }

    [Fact]
    public async Task GetLaborReportAsync_ShouldCalculateLaborCost()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);
        
        // 1. Create User with Hourly Rate $15
        var user = User.Create("Chef", "Gordon", "Ramsay", Guid.NewGuid(), null, "password", hourlyRate: 15m);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // 2. Create Cash Session (Time) - 5 Hours (10:00 - 15:00)
        var session = CashSession.Open(new UserId(user.Id), Guid.NewGuid(), Guid.NewGuid(), new Money(100m));
        SetProperty(session, "OpenedAt", start.AddHours(10));
        SetProperty(session, "ClosedAt", start.AddHours(15));
        SetProperty(session, "Status", CashSessionStatus.Closed);
        _context.CashSessions.Add(session);
        
        // 3. Create Sales (Tickets) attributed to User - Net Sales $300
        var ticket = Ticket.Create(4001, new UserId(user.Id), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(ticket, "SubtotalAmount", new Money(300m));
        SetProperty(ticket, "TotalAmount", new Money(330m));
        SetProperty(ticket, "Status", TicketStatus.Closed);
        SetProperty(ticket, "ClosedAt", start.AddHours(12));
        _context.Tickets.Add(ticket);
        
        await _context.SaveChangesAsync();
        
        // Act
        var report = await _repository.GetLaborReportAsync(start, end);
        
        // Assert
        Assert.Single(report.StaffLabor);
        var item = report.StaffLabor[0];
        
        Assert.Equal("Chef", item.UserName);
        Assert.Equal(5.0, item.TotalHours, 1);
        Assert.Equal(15m, item.HourlyRate);
        Assert.Equal(75m, item.TotalCost); // 5 * 15
        Assert.Equal(300m, item.TotalSales);
        
        // Check Aggregates
        Assert.Equal(75m, report.TotalLaborCost);
        Assert.Equal(300m, report.TotalNetSales);
        Assert.Equal(25m, report.LaborCostPercentage); // 75 / 300 * 100 = 25%
    }

    [Fact]
    public async Task GetDeliveryReportAsync_ShouldAggregateDriverStats()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);
        
        // 1. Create Driver
        var driver = User.Create("Driver", "Baby", "Driver", Guid.NewGuid(), null, "password");
        _context.Users.Add(driver);
        await _context.SaveChangesAsync();
        
        // 2. Ticket 1: 30 mins delivery
        var t1 = Ticket.Create(5001, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(t1, "AssignedDriverId", driver.Id);
        SetProperty(t1, "TotalAmount", new Money(50m));
        SetProperty(t1, "Status", TicketStatus.Closed);
        // Valid Dispatched/Closed times
        var dispatch1 = start.AddHours(10);
        var close1 = start.AddHours(10).AddMinutes(30);
        SetProperty(t1, "DispatchedTime", dispatch1);
        SetProperty(t1, "ClosedAt", close1);
        _context.Tickets.Add(t1);

        // 3. Ticket 2: 20 mins delivery
        var t2 = Ticket.Create(5002, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(t2, "AssignedDriverId", driver.Id);
        SetProperty(t2, "TotalAmount", new Money(20m));
        SetProperty(t2, "Status", TicketStatus.Closed);
        var dispatch2 = start.AddHours(11);
        var close2 = start.AddHours(11).AddMinutes(20);
        SetProperty(t2, "DispatchedTime", dispatch2);
        SetProperty(t2, "ClosedAt", close2);
        _context.Tickets.Add(t2);

        // 4. Ticket 3: No Driver (Should be ignored)
        var t3 = Ticket.Create(5003, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetProperty(t3, "TotalAmount", new Money(100m));
        SetProperty(t3, "Status", TicketStatus.Closed);
        SetProperty(t3, "ClosedAt", start.AddHours(12));
        _context.Tickets.Add(t3);

        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetDeliveryReportAsync(start, end);

        // Assert
        Assert.Single(report.DriverStats);
        var stats = report.DriverStats[0];
        Assert.Equal("Driver", stats.DriverName); // Username
        Assert.Equal(2, stats.DeliveryCount);
        Assert.Equal(70m, stats.TotalSales); // 50 + 20
        Assert.Equal(25.0, stats.AverageTimeMinutes, 1); // (30 + 20) / 2 = 25
        
        Assert.Equal(2, report.TotalDeliveries);
        Assert.Equal(70m, report.TotalDeliverySales);
        Assert.Equal(25.0, report.AverageDeliveryTimeMinutes, 1);
    }

    private async Task<Ticket> CreateClosedTicketAsync(decimal subtotal, decimal tax, DateTime closedAt)
    {
        var ticket = Ticket.Create(
            new Random().Next(1000, 9999), 
            new UserId(Guid.NewGuid()), 
            Guid.NewGuid(), // terminal
            Guid.NewGuid(), // shift
            Guid.NewGuid()); // orderType

        // Hackily calculate total on order lines or just mock properties via reflection/setup if possible?
        // Domain entity logic computes total from lines. So we must add lines.
        // Or we use private setters if Entity Framework allows, but here we are using the helper method.
        var line = OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1, new Money(subtotal), taxRate: 0); // Base
        // Manually adjust tax? 
        // Or simpler: Add line with subtotal, tax is calculated by tax domain service usually.
        // But Ticket entity has TaxAmount property.
        // Let's just create line with correct price.
        // If I want tax specific, I should use `ticket.AddOrderLine` and relies on recalculation?
        // Ticket entity updates totals when Closed? Or when lines added?
        
        // Use reflection to set amounts directly to avoid domain logic complexity for this repository test
        // seeing as we just need to test aggregation of stored values.
        
        SetProperty(ticket, "SubtotalAmount", new Money(subtotal));
        SetProperty(ticket, "TaxAmount", new Money(tax));
        SetProperty(ticket, "TotalAmount", new Money(subtotal + tax));
        SetProperty(ticket, "Status", TicketStatus.Closed);
        SetProperty(ticket, "ClosedAt", closedAt);
        SetProperty(ticket, "ActiveDate", closedAt); // Important for some reports

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }
    
    private void SetProperty(object obj, string propName, object value)
    {
        var prop = obj.GetType().GetProperty(propName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
    }

    private async Task CreatePaymentAsync(decimal amount, TransactionType type, PaymentType paymentType, DateTime time)
    {
        // Must create a ticket first to satisfy FK
        var ticket = Ticket.Create(new Random().Next(100000, 999999), new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        var payment = CashPayment.Create(ticket.Id, new Money(amount), new UserId(Guid.NewGuid()), Guid.NewGuid());
        // Override properties
        SetProperty(payment, "TransactionType", type);
        SetProperty(payment, "PaymentType", paymentType);
        SetProperty(payment, "TransactionTime", time);
        
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAttendanceReportAsync_ShouldCalculateHoursCorrectly()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var start = today.Date;
        var end = today.Date.AddDays(1).AddTicks(-1);

        // 1. Create Shift
        var shift = Shift.Create("Morning Shift", TimeSpan.FromHours(8), TimeSpan.FromHours(16));
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        // 1.5 Create Role
        var role = Role.Create("Server", UserPermission.None);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // 2. Create User
        var user = User.Create("Staff", "Alice", "Wonder", role.Id, null, "password");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 3. Create Attendance Record (6 hours)
        var att1 = AttendanceHistory.Create(new UserId(user.Id), shift.Id);
        // Use reflection or helper to set ClockOutTime as it's private set usually?
        // AttendanceHistory.ClockOut sets it.
        // Or set property directly.
        SetProperty(att1, "ClockInTime", start.AddHours(9));
        SetProperty(att1, "ClockOutTime", start.AddHours(15));
        _context.AttendanceHistories.Add(att1);

        // 4. Create Partial Attendance with NULL ClockOut (Should be 0 hours)
        var att2 = AttendanceHistory.Create(new UserId(user.Id), shift.Id);
        SetProperty(att2, "ClockInTime", start.AddHours(16));
        _context.AttendanceHistories.Add(att2);
        
        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetAttendanceReportAsync(start, end, null);

        // Assert
        Assert.Equal(2, report.Items.Count); // 2 records
        
        var completedShift = report.Items.FirstOrDefault(x => x.ClockOutTime.HasValue);
        Assert.NotNull(completedShift);
        Assert.Equal("Alice Wonder", completedShift.UserName);
        Assert.Equal("Morning Shift", completedShift.ShiftName);
        Assert.Equal(6.0, completedShift.HoursWorked, 1);
        
        var ongoingShift = report.Items.FirstOrDefault(x => !x.ClockOutTime.HasValue);
        Assert.NotNull(ongoingShift);
        Assert.Equal(0, ongoingShift.HoursWorked);
        
        Assert.Equal(2, report.TotalShifts);
        Assert.Equal(6.0, report.TotalHours, 1);
    }

    [Fact]
    public async Task GetCashOutReportAsync_ShouldCalculateNetDueCorrectly()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;

        // Create Role
        var role = Role.Create("Server", UserPermission.None);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Create Users
        var userA = User.Create("S01", "Bob", "Builder", role.Id, null, "123");
        var userB = User.Create("S02", "Dora", "Explorer", role.Id, null, "123");
        _context.Users.AddRange(userA, userB);
        await _context.SaveChangesAsync();
        
        var userIdA = new UserId(userA.Id);
        var userIdB = new UserId(userB.Id);

        // Ticket 1: User A - $100 Cash (Net Due +100)
        var t1 = Ticket.Create(101, userIdA, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        // Use CashPayment factory
        var p1 = CashPayment.Create(t1.Id, new Money(100.00m), userIdA, Guid.NewGuid());
        t1.AddPayment(p1);
        SetProperty(t1, "Status", TicketStatus.Closed);
        SetProperty(t1, "ClosedAt", now);
        _context.Tickets.Add(t1);

        // Ticket 2: User A - $100 Credit + $20 Tip (Net Due -20)
        var t2 = Ticket.Create(102, userIdA, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        // Use CreditCardPayment factory
        var p2 = CreditCardPayment.Create(t2.Id, new Money(100.00m), userIdA, Guid.NewGuid());
        SetProperty(p2, "TipsAmount", new Money(20.00m));
        t2.AddPayment(p2);
        SetProperty(t2, "Status", TicketStatus.Closed);
        SetProperty(t2, "ClosedAt", now);
        _context.Tickets.Add(t2);

        // Ticket 3: User B - $10 Credit + $50 Tip (Net Due -50)
        var t3 = Ticket.Create(103, userIdB, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        // Use CreditCardPayment factory
        var p3 = CreditCardPayment.Create(t3.Id, new Money(10.00m), userIdB, Guid.NewGuid());
        SetProperty(p3, "TipsAmount", new Money(50.00m));
        t3.AddPayment(p3);
        SetProperty(t3, "Status", TicketStatus.Closed);
        SetProperty(t3, "ClosedAt", now);
        _context.Tickets.Add(t3);

        await _context.SaveChangesAsync();

        // Act
        var report = await _repository.GetCashOutReportAsync(today, today.AddDays(1));

        // Assert
        Assert.Equal(2, report.Items.Count);

        var reportA = report.Items.First(i => i.UserName == "Bob Builder");
        // Cash Sales: 100 (from T1). T2 was Credit.
        Assert.Equal(100.00m, reportA.CashSales);
        // Charged Tips: 20 (from T2). T1 was Cash.
        Assert.Equal(20.00m, reportA.ChargedTips);
        // Net Due: 100 - 20 = 80
        Assert.Equal(80.00m, reportA.NetDue);

        var reportB = report.Items.First(i => i.UserName == "Dora Explorer");
        // Cash Sales: 0
        Assert.Equal(0.00m, reportB.CashSales);
        // Charged Tips: 50
        Assert.Equal(50.00m, reportB.ChargedTips);
        // Net Due: 0 - 50 = -50
        Assert.Equal(-50.00m, reportB.NetDue);

        Assert.Equal(100.00m, report.TotalCashSales);
        Assert.Equal(70.00m, report.TotalChargedTips); // 20 + 50
        Assert.Equal(30.00m, report.TotalNetDue); // 80 - 50 = 30
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
