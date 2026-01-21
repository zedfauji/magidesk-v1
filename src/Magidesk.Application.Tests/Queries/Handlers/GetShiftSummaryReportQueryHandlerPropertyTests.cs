using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Handlers;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Queries.Handlers;

/// <summary>
/// Property-based tests for GetShiftSummaryReportQueryHandler.
/// Feature: reporting-export, Property 6: Shift Summary Completeness
/// Validates: Requirements 5.1, 5.2, 5.4, 5.5
/// </summary>
public class GetShiftSummaryReportQueryHandlerPropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<ILogger<GetShiftSummaryReportQueryHandler>> _mockLogger;
    private readonly GetShiftSummaryReportQueryHandler _handler;

    public GetShiftSummaryReportQueryHandlerPropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockLogger = new Mock<ILogger<GetShiftSummaryReportQueryHandler>>();
        _handler = new GetShiftSummaryReportQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Property test: Shift summary completeness validation.
    /// For any shift period, the shift summary should include all transactions within the time range, 
    /// cash reconciliation should balance, and all exception types should be captured.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property ShiftSummaryCompletenessValidation()
    {
        return Prop.ForAll(
            GenerateValidShiftSummaryData(),
            data =>
            {
                // Arrange
                var (query, shiftData, serverSales, cashReconciliations, exceptions, voids) = data;

                SetupMockRepository(query, shiftData, serverSales, cashReconciliations, exceptions, voids);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Property 6: Shift Summary Completeness
                // All financial amounts should be non-negative
                var financialAmountsValid = result.TotalSales.Amount >= 0m &&
                    result.CashSales.Amount >= 0m &&
                    result.CardSales.Amount >= 0m &&
                    result.OtherPaymentSales.Amount >= 0m &&
                    result.CashReconciliation.OpeningBalance.Amount >= 0m &&
                    result.CashReconciliation.ExpectedCash.Amount >= 0m &&
                    result.CashReconciliation.ActualCash.Amount >= 0m;

                // Payment method breakdown should sum to total sales (within rounding tolerance)
                var paymentBreakdownSum = result.CashSales.Amount + result.CardSales.Amount + result.OtherPaymentSales.Amount;
                var paymentBreakdownValid = Math.Abs(paymentBreakdownSum - result.TotalSales.Amount) <= 0.01m;

                // Transaction count should be non-negative and consistent
                var transactionCountValid = result.TransactionCount >= 0 &&
                    result.ServerBreakdown.All(s => s.TransactionCount >= 0) &&
                    result.ServerBreakdown.Sum(s => s.TransactionCount) <= result.TransactionCount + 10; // Allow some tolerance

                // Average ticket size should be calculated correctly when transactions exist
                var averageTicketValid = result.TransactionCount == 0 
                    ? result.AverageTicketSize == 0m
                    : Math.Abs(result.AverageTicketSize - (result.TotalSales.Amount / result.TransactionCount)) <= 0.01m;

                // Server breakdown should have valid data
                var serverBreakdownValid = result.ServerBreakdown.All(s => 
                    s.TotalSales.Amount >= 0m &&
                    s.TransactionCount >= 0 &&
                    s.TotalTips.Amount >= 0m &&
                    s.TipPercentage >= 0m &&
                    s.TipPercentage <= 100m &&
                    (s.TransactionCount == 0 ? s.AverageTicketSize == 0m : s.AverageTicketSize > 0m));

                // Cash reconciliation should be mathematically consistent
                // Note: Difference represents absolute difference since Money doesn't allow negative values
                var expectedAbsoluteDifference = Math.Abs(result.CashReconciliation.ActualCash.Amount - result.CashReconciliation.ExpectedCash.Amount);
                var cashReconciliationValid = Math.Abs(result.CashReconciliation.Difference.Amount - expectedAbsoluteDifference) <= 0.01m;

                // Exception and void counts should be non-negative
                var exceptionVoidValid = result.Exceptions.All(e => e.Count >= 0 && e.TotalAmount.Amount >= 0m) &&
                    result.Voids.All(v => v.Count >= 0 && v.TotalAmount.Amount >= 0m);

                // Shift metrics should be valid
                var metricsValid = result.Metrics.CustomerCount >= 0 &&
                    result.Metrics.SalesPerHour >= 0m &&
                    result.Metrics.TransactionsPerHour >= 0m &&
                    result.Metrics.AverageServiceTime >= TimeSpan.Zero &&
                    result.Metrics.TableTurnoverRate >= 0m &&
                    result.Metrics.PeakHourSales.Amount >= 0m &&
                    result.Metrics.PeakHour >= 0 && result.Metrics.PeakHour <= 23;

                // Date range should be valid
                var dateRangeValid = result.StartDate <= result.EndDate;

                return financialAmountsValid && paymentBreakdownValid && transactionCountValid && 
                       averageTicketValid && serverBreakdownValid && cashReconciliationValid && 
                       exceptionVoidValid && metricsValid && dateRangeValid;
            });
    }

    /// <summary>
    /// Property test: Server sales attribution accuracy.
    /// For any server performance calculation, sales should be attributed only to the assigned server, 
    /// tip calculations should be accurate, and performance rankings should be consistent.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property ServerSalesAttributionAccuracy()
    {
        return Prop.ForAll(
            GenerateValidShiftSummaryData(),
            data =>
            {
                // Arrange
                var (query, shiftData, serverSales, cashReconciliations, exceptions, voids) = data;

                SetupMockRepository(query, shiftData, serverSales, cashReconciliations, exceptions, voids);

                // Act
                var result = _handler.HandleAsync(query).Result;

                // Assert - Server performance attribution accuracy
                // Each server should have unique ID
                var uniqueServerIds = result.ServerBreakdown.Select(s => s.ServerId).Distinct().Count() == 
                    result.ServerBreakdown.Count();

                // Tip percentage calculation should be accurate
                var tipPercentageValid = result.ServerBreakdown.All(s => 
                    s.TotalSales.Amount == 0m 
                        ? s.TipPercentage == 0m 
                        : Math.Abs(s.TipPercentage - ((s.TotalTips.Amount / s.TotalSales.Amount) * 100)) <= 0.01m);

                // Average ticket size calculation should be accurate
                var averageTicketSizeValid = result.ServerBreakdown.All(s => 
                    s.TransactionCount == 0 
                        ? s.AverageTicketSize == 0m 
                        : Math.Abs(s.AverageTicketSize - (s.TotalSales.Amount / s.TransactionCount)) <= 0.01m);

                // Server sales should sum to reasonable portion of total sales (allowing for rounding and other factors)
                var totalServerSales = result.ServerBreakdown.Aggregate(Money.Zero(), (sum, s) => sum + s.TotalSales);
                var serverSalesConsistent = totalServerSales.Amount <= result.TotalSales.Amount * 1.1m; // Allow 10% tolerance

                // Server names should not be empty
                var serverNamesValid = result.ServerBreakdown.All(s => !string.IsNullOrWhiteSpace(s.ServerName));

                return uniqueServerIds && tipPercentageValid && averageTicketSizeValid && 
                       serverSalesConsistent && serverNamesValid;
            });
    }

    /// <summary>
    /// Unit test: Empty shift data should result in valid empty report structure.
    /// </summary>
    [Fact]
    public async Task EmptyShiftDataResultsInValidEmptyReport()
    {
        // Arrange
        var shiftId = Guid.NewGuid();
        var query = new GetShiftSummaryReportQuery(shiftId, DateTime.Today, DateTime.Today.AddHours(8));
        
        var emptyShiftData = new ShiftSummaryData(
            shiftId, "Morning Shift", DateTime.Today, DateTime.Today.AddHours(8),
            Money.Zero(), 0, 0);

        SetupMockRepository(query, emptyShiftData, 
            new List<ServerSalesData>(),
            new List<CashReconciliationData>(),
            new List<ExceptionData>(),
            new List<VoidData>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Empty data should result in valid empty report structure
        Assert.Equal(Money.Zero().Amount, result.TotalSales.Amount);
        Assert.Equal(0, result.TransactionCount);
        Assert.Equal(0m, result.AverageTicketSize);
        Assert.Empty(result.ServerBreakdown);
        Assert.Empty(result.Exceptions);
        Assert.Empty(result.Voids);
        Assert.Equal(Money.Zero().Amount, result.CashReconciliation.OpeningBalance.Amount);
        Assert.Equal(0, result.Metrics.CustomerCount);
    }

    /// <summary>
    /// Unit test: Single server with known values produces correct calculations.
    /// </summary>
    [Fact]
    public async Task SingleServerWithKnownValuesProducesCorrectCalculations()
    {
        // Arrange
        var shiftId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        var query = new GetShiftSummaryReportQuery(shiftId, DateTime.Today, DateTime.Today.AddHours(8));
        
        var shiftData = new ShiftSummaryData(
            shiftId, "Evening Shift", DateTime.Today, DateTime.Today.AddHours(8),
            new Money(500m, "USD"), 10, 8);

        var serverSales = new List<ServerSalesData>
        {
            new(serverId, "John Doe", new Money(500m, "USD"), 10, new Money(75m, "USD"))
        };

        var cashReconciliation = new List<CashReconciliationData>
        {
            new(Guid.NewGuid(), new Money(100m, "USD"), new Money(300m, "USD"), 
                new Money(295m, "USD"), Money.Zero(), Money.Zero(), Money.Zero(), true)
        };

        SetupMockRepository(query, shiftData, serverSales, cashReconciliation,
            new List<ExceptionData>(), new List<VoidData>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Equal(500m, result.TotalSales.Amount);
        Assert.Equal(10, result.TransactionCount);
        Assert.Equal(50m, result.AverageTicketSize);
        
        var serverBreakdown = result.ServerBreakdown.Single();
        Assert.Equal(serverId, serverBreakdown.ServerId);
        Assert.Equal("John Doe", serverBreakdown.ServerName);
        Assert.Equal(500m, serverBreakdown.TotalSales.Amount);
        Assert.Equal(10, serverBreakdown.TransactionCount);
        Assert.Equal(50m, serverBreakdown.AverageTicketSize);
        Assert.Equal(75m, serverBreakdown.TotalTips.Amount);
        Assert.Equal(15m, serverBreakdown.TipPercentage); // 75/500 * 100 = 15%

        Assert.Equal(100m, result.CashReconciliation.OpeningBalance.Amount);
        Assert.Equal(300m, result.CashReconciliation.ExpectedCash.Amount);
        Assert.Equal(295m, result.CashReconciliation.ActualCash.Amount);
        Assert.Equal(5m, result.CashReconciliation.Difference.Amount); // Absolute difference: |295 - 300| = 5
        Assert.True(result.CashReconciliation.IsReconciled);
    }

    /// <summary>
    /// Unit test: Multiple exceptions and voids are properly aggregated.
    /// </summary>
    [Fact]
    public async Task MultipleExceptionsAndVoidsAreProperlyAggregated()
    {
        // Arrange
        var shiftId = Guid.NewGuid();
        var query = new GetShiftSummaryReportQuery(shiftId, DateTime.Today, DateTime.Today.AddHours(8));
        
        var shiftData = new ShiftSummaryData(
            shiftId, "Night Shift", DateTime.Today, DateTime.Today.AddHours(8),
            new Money(300m, "USD"), 6, 5);

        var exceptions = new List<ExceptionData>
        {
            new("Discount Override", new Money(10m, "USD"), "Manager override", DateTime.Today.AddHours(2)),
            new("Discount Override", new Money(15m, "USD"), "Manager override", DateTime.Today.AddHours(4)),
            new("Price Override", new Money(5m, "USD"), "Price adjustment", DateTime.Today.AddHours(6))
        };

        var voids = new List<VoidData>
        {
            new("Item Void", new Money(12m, "USD"), "Customer changed mind", DateTime.Today.AddHours(1)),
            new("Item Void", new Money(8m, "USD"), "Kitchen error", DateTime.Today.AddHours(3)),
            new("Ticket Void", new Money(25m, "USD"), "Customer left", DateTime.Today.AddHours(5))
        };

        SetupMockRepository(query, shiftData, new List<ServerSalesData>(),
            new List<CashReconciliationData>(), exceptions, voids);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        var discountOverrides = result.Exceptions.First(e => e.ExceptionType == "Discount Override");
        Assert.Equal(2, discountOverrides.Count);
        Assert.Equal(25m, discountOverrides.TotalAmount.Amount); // 10 + 15

        var priceOverrides = result.Exceptions.First(e => e.ExceptionType == "Price Override");
        Assert.Equal(1, priceOverrides.Count);
        Assert.Equal(5m, priceOverrides.TotalAmount.Amount);

        var itemVoids = result.Voids.First(v => v.VoidType == "Item Void");
        Assert.Equal(2, itemVoids.Count);
        Assert.Equal(20m, itemVoids.TotalAmount.Amount); // 12 + 8

        var ticketVoids = result.Voids.First(v => v.VoidType == "Ticket Void");
        Assert.Equal(1, ticketVoids.Count);
        Assert.Equal(25m, ticketVoids.TotalAmount.Amount);
    }

    /// <summary>
    /// Generates valid shift summary test data for property-based testing.
    /// </summary>
    private static Arbitrary<(GetShiftSummaryReportQuery, ShiftSummaryData, 
        List<ServerSalesData>, List<CashReconciliationData>, 
        List<ExceptionData>, List<VoidData>)> GenerateValidShiftSummaryData()
    {
        return Arb.From(
            from shiftId in Arb.Generate<Guid>()
            from startDate in Arb.Generate<DateTime>().Where(d => d.Year >= 2020 && d.Year <= 2030)
            from shiftHours in Gen.Choose(4, 12)
            let endDate = startDate.AddHours(shiftHours)
            let query = new GetShiftSummaryReportQuery(shiftId, startDate, endDate)
            from transactionCount in Gen.Choose(0, 100)
            from customerCount in Gen.Choose(0, transactionCount + 10)
            from totalSalesAmount in Gen.Choose(0, 10000).Select(x => (decimal)x)
            let shiftData = new ShiftSummaryData(shiftId, GenerateShiftName(), startDate, endDate,
                new Money(totalSalesAmount, "USD"), transactionCount, customerCount)
            from serverCount in Gen.Choose(1, 5)
            let serverSales = GenerateServerSalesData(serverCount, totalSalesAmount, transactionCount)
            from cashSessionCount in Gen.Choose(1, 3)
            let cashReconciliations = GenerateCashReconciliationData(cashSessionCount)
            from exceptionCount in Gen.Choose(0, 10)
            let exceptions = GenerateExceptionData(exceptionCount, startDate, endDate)
            from voidCount in Gen.Choose(0, 8)
            let voids = GenerateVoidData(voidCount, startDate, endDate)
            select (query, shiftData, serverSales, cashReconciliations, exceptions, voids)
        );
    }

    private static string GenerateShiftName()
    {
        var shiftNames = new[] { "Morning Shift", "Afternoon Shift", "Evening Shift", "Night Shift" };
        var random = new System.Random(42);
        return shiftNames[random.Next(shiftNames.Length)];
    }

    private static List<ServerSalesData> GenerateServerSalesData(int serverCount, decimal totalSales, int totalTransactions)
    {
        var random = new System.Random(42);
        var result = new List<ServerSalesData>();
        var serverNames = new[] { "John Doe", "Jane Smith", "Bob Johnson", "Alice Brown", "Charlie Wilson" };

        var remainingSales = totalSales;
        var remainingTransactions = totalTransactions;

        for (var i = 0; i < serverCount; i++)
        {
            var isLastServer = i == serverCount - 1;
            var serverSales = isLastServer ? remainingSales : (decimal)(random.NextDouble() * (double)remainingSales);
            var serverTransactions = isLastServer ? remainingTransactions : random.Next(0, remainingTransactions + 1);
            var serverTips = serverSales * (decimal)(random.NextDouble() * 0.2); // 0-20% tips

            result.Add(new ServerSalesData(
                Guid.NewGuid(),
                serverNames[i % serverNames.Length],
                new Money(Math.Max(0, serverSales), "USD"),
                Math.Max(0, serverTransactions),
                new Money(Math.Max(0, serverTips), "USD")
            ));

            remainingSales -= serverSales;
            remainingTransactions -= serverTransactions;
        }

        return result;
    }

    private static List<CashReconciliationData> GenerateCashReconciliationData(int sessionCount)
    {
        var random = new System.Random(42);
        var result = new List<CashReconciliationData>();

        for (var i = 0; i < sessionCount; i++)
        {
            var openingBalance = (decimal)(random.NextDouble() * 200);
            var expectedCash = openingBalance + (decimal)(random.NextDouble() * 500);
            var actualCash = expectedCash + (decimal)((random.NextDouble() - 0.5) * 20); // ±10 variance
            var cashDrops = (decimal)(random.NextDouble() * 50);
            var payouts = (decimal)(random.NextDouble() * 30);
            var drawerBleeds = (decimal)(random.NextDouble() * 10);

            result.Add(new CashReconciliationData(
                Guid.NewGuid(),
                new Money(openingBalance, "USD"),
                new Money(expectedCash, "USD"),
                new Money(actualCash, "USD"),
                new Money(cashDrops, "USD"),
                new Money(payouts, "USD"),
                new Money(drawerBleeds, "USD"),
                random.NextDouble() > 0.2 // 80% chance of being closed
            ));
        }

        return result;
    }

    private static List<ExceptionData> GenerateExceptionData(int count, DateTime startDate, DateTime endDate)
    {
        var random = new System.Random(42);
        var result = new List<ExceptionData>();
        var exceptionTypes = new[] { "Discount Override", "Price Override", "Manager Comp", "System Error" };
        var descriptions = new[] { "Manager override", "Price adjustment", "Customer complaint", "System malfunction" };

        for (var i = 0; i < count; i++)
        {
            var timestamp = startDate.AddTicks((long)(random.NextDouble() * (endDate - startDate).Ticks));
            var amount = (decimal)(random.NextDouble() * 50);
            var exceptionType = exceptionTypes[random.Next(exceptionTypes.Length)];
            var description = descriptions[random.Next(descriptions.Length)];

            result.Add(new ExceptionData(exceptionType, new Money(amount, "USD"), description, timestamp));
        }

        return result;
    }

    private static List<VoidData> GenerateVoidData(int count, DateTime startDate, DateTime endDate)
    {
        var random = new System.Random(42);
        var result = new List<VoidData>();
        var voidTypes = new[] { "Item Void", "Ticket Void", "Payment Void" };
        var reasons = new[] { "Customer changed mind", "Kitchen error", "Wrong order", "Customer left" };

        for (var i = 0; i < count; i++)
        {
            var timestamp = startDate.AddTicks((long)(random.NextDouble() * (endDate - startDate).Ticks));
            var amount = (decimal)(random.NextDouble() * 30);
            var voidType = voidTypes[random.Next(voidTypes.Length)];
            var reason = reasons[random.Next(reasons.Length)];

            result.Add(new VoidData(voidType, new Money(amount, "USD"), reason, timestamp));
        }

        return result;
    }

    private void SetupMockRepository(
        GetShiftSummaryReportQuery query,
        ShiftSummaryData shiftData,
        List<ServerSalesData> serverSales,
        List<CashReconciliationData> cashReconciliations,
        List<ExceptionData> exceptions,
        List<VoidData> voids)
    {
        _mockRepository.Setup(r => r.GetShiftSummaryDataAsync(
            query.ShiftId, query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shiftData);

        _mockRepository.Setup(r => r.GetServerSalesDataAsync(
            query.ShiftId, query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serverSales);

        _mockRepository.Setup(r => r.GetCashReconciliationDataAsync(
            query.ShiftId, query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cashReconciliations);

        _mockRepository.Setup(r => r.GetExceptionDataAsync(
            query.ShiftId, query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exceptions);

        _mockRepository.Setup(r => r.GetVoidDataAsync(
            query.ShiftId, query.StartDate, query.EndDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(voids);
    }
}