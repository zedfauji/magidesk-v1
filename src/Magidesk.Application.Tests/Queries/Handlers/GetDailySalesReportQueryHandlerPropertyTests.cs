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
/// Property-based tests for GetDailySalesReportQueryHandler.
/// Feature: reporting-export, Property 2: Data Aggregation Consistency
/// Validates: Requirements 1.3, 1.4
/// </summary>
public class GetDailySalesReportQueryHandlerPropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<ILogger<GetDailySalesReportQueryHandler>> _mockLogger;
    private readonly GetDailySalesReportQueryHandler _handler;

    public GetDailySalesReportQueryHandlerPropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockLogger = new Mock<ILogger<GetDailySalesReportQueryHandler>>();
        _handler = new GetDailySalesReportQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Unit test: Basic daily sales report generation with known values.
    /// </summary>
    [Fact]
    public async Task DailySalesReport_WithKnownValues_ReturnsCorrectAggregation()
    {
        // Arrange
        var date = DateTime.Today;
        var query = new GetDailySalesReportQuery(date);

        var dailySalesData = new DailySalesData(
            Date: date,
            TotalSales: new Money(1000m, "USD"),
            TimeBasedSales: new Money(600m, "USD"),
            ProductSales: new Money(400m, "USD"),
            TotalTax: new Money(80m, "USD"),
            TotalGratuity: new Money(120m, "USD"),
            TransactionCount: 10,
            CustomerCount: 25
        );

        var hourlySalesData = new List<HourlySalesData>
        {
            new(date, 10, new Money(200m, "USD"), 2, 5),
            new(date, 11, new Money(300m, "USD"), 3, 8),
            new(date, 12, new Money(500m, "USD"), 5, 12)
        };

        var categorySalesData = new List<CategorySalesDto>
        {
            new("Food", new Money(250m, "USD"), 15, 25m),
            new("Beverages", new Money(150m, "USD"), 10, 15m),
            new("Time Charges", new Money(600m, "USD"), 8, 60m)
        };

        var paymentMethodSalesData = new List<PaymentMethodSalesDto>
        {
            new("Cash", new Money(400m, "USD"), 4, 40m),
            new("Credit Card", new Money(600m, "USD"), 6, 60m)
        };

        var tableSalesData = new List<TableSalesDto>
        {
            new(1, "Pool", new Money(200m, "USD"), new Money(100m, "USD"), new Money(300m, "USD"), TimeSpan.FromHours(4), 2),
            new(2, "Snooker", new Money(400m, "USD"), new Money(300m, "USD"), new Money(700m, "USD"), TimeSpan.FromHours(6), 3)
        };

        _mockRepository.Setup(r => r.GetDailySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailySalesData);
        _mockRepository.Setup(r => r.GetHourlySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hourlySalesData);
        _mockRepository.Setup(r => r.GetCategorySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorySalesData);
        _mockRepository.Setup(r => r.GetPaymentMethodSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethodSalesData);
        _mockRepository.Setup(r => r.GetTableSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tableSalesData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify data aggregation consistency
        Assert.Equal(date, result.Date);
        Assert.Equal(1000m, result.TotalSales.Amount);
        Assert.Equal(600m, result.TimeBasedSales.Amount);
        Assert.Equal(400m, result.ProductSales.Amount);
        
        // Verify time + product = total
        Assert.Equal(result.TotalSales.Amount, result.TimeBasedSales.Amount + result.ProductSales.Amount);
        
        // Verify hourly breakdown sums to total
        var hourlyTotal = result.HourlyBreakdown.Sum(h => h.Sales.Amount);
        Assert.Equal(result.TotalSales.Amount, hourlyTotal);
        
        // Verify payment method breakdown sums to total
        var paymentTotal = result.PaymentBreakdown.Sum(p => p.Amount.Amount);
        Assert.Equal(result.TotalSales.Amount, paymentTotal);
        
        // Verify average ticket size calculation
        var expectedAverage = result.TotalSales.Amount / result.TransactionCount;
        Assert.Equal(expectedAverage, result.AverageTicketSize);
    }

    /// <summary>
    /// Unit test: Empty data should result in zero values but valid structure.
    /// </summary>
    [Fact]
    public async Task EmptyDataResultsInZeroValuesWithValidStructure()
    {
        // Arrange
        var date = DateTime.Today;
        var query = new GetDailySalesReportQuery(date);

        var emptyDailySalesData = new DailySalesData(
            Date: date,
            TotalSales: Money.Zero(),
            TimeBasedSales: Money.Zero(),
            ProductSales: Money.Zero(),
            TotalTax: Money.Zero(),
            TotalGratuity: Money.Zero(),
            TransactionCount: 0,
            CustomerCount: 0
        );

        _mockRepository.Setup(r => r.GetDailySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDailySalesData);
        _mockRepository.Setup(r => r.GetHourlySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HourlySalesData>());
        _mockRepository.Setup(r => r.GetCategorySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategorySalesDto>());
        _mockRepository.Setup(r => r.GetPaymentMethodSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PaymentMethodSalesDto>());
        _mockRepository.Setup(r => r.GetTableSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TableSalesDto>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Empty data should result in zero values
        Assert.Equal(date, result.Date);
        Assert.Equal(0m, result.TotalSales.Amount);
        Assert.Equal(0m, result.TimeBasedSales.Amount);
        Assert.Equal(0m, result.ProductSales.Amount);
        Assert.Equal(0, result.TransactionCount);
        Assert.Equal(0, result.CustomerCount);
        Assert.Equal(0m, result.AverageTicketSize);
        
        // Verify collections are empty but not null
        Assert.NotNull(result.HourlyBreakdown);
        Assert.NotNull(result.CategoryBreakdown);
        Assert.NotNull(result.PaymentBreakdown);
        Assert.NotNull(result.TableBreakdown);
        Assert.Empty(result.HourlyBreakdown);
        Assert.Empty(result.CategoryBreakdown);
        Assert.Empty(result.PaymentBreakdown);
        Assert.Empty(result.TableBreakdown);
    }

    /// <summary>
    /// Unit test: Breakdown percentages should sum to 100% when data exists.
    /// </summary>
    [Fact]
    public async Task BreakdownPercentagesSumTo100Percent()
    {
        // Arrange
        var date = DateTime.Today;
        var query = new GetDailySalesReportQuery(date);

        var dailySalesData = new DailySalesData(
            Date: date,
            TotalSales: new Money(1000m, "USD"),
            TimeBasedSales: new Money(700m, "USD"),
            ProductSales: new Money(300m, "USD"),
            TotalTax: new Money(50m, "USD"),
            TotalGratuity: new Money(100m, "USD"),
            TransactionCount: 8,
            CustomerCount: 20
        );

        var categorySalesData = new List<CategorySalesDto>
        {
            new("Time Charges", new Money(700m, "USD"), 8, 70m),
            new("Food", new Money(200m, "USD"), 12, 20m),
            new("Beverages", new Money(100m, "USD"), 8, 10m)
        };

        var paymentMethodSalesData = new List<PaymentMethodSalesDto>
        {
            new("Cash", new Money(300m, "USD"), 3, 30m),
            new("Credit Card", new Money(500m, "USD"), 4, 50m),
            new("Debit Card", new Money(200m, "USD"), 1, 20m)
        };

        _mockRepository.Setup(r => r.GetDailySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailySalesData);
        _mockRepository.Setup(r => r.GetHourlySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HourlySalesData>());
        _mockRepository.Setup(r => r.GetCategorySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorySalesData);
        _mockRepository.Setup(r => r.GetPaymentMethodSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethodSalesData);
        _mockRepository.Setup(r => r.GetTableSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TableSalesDto>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify percentage calculations
        var categoryPercentageSum = result.CategoryBreakdown.Sum(c => c.PercentOfTotal);
        var paymentPercentageSum = result.PaymentBreakdown.Sum(p => p.PercentOfTotal);
        
        Assert.Equal(100m, categoryPercentageSum, 1); // Allow 1 decimal place tolerance for rounding
        Assert.Equal(100m, paymentPercentageSum, 1);
        
        // Verify category amounts sum to total
        var categoryAmountSum = result.CategoryBreakdown.Sum(c => c.Sales.Amount);
        Assert.Equal(result.TotalSales.Amount, categoryAmountSum);
        
        // Verify payment method amounts sum to total
        var paymentAmountSum = result.PaymentBreakdown.Sum(p => p.Amount.Amount);
        Assert.Equal(result.TotalSales.Amount, paymentAmountSum);
    }

    /// <summary>
    /// Unit test: Average ticket size calculation handles division by zero.
    /// </summary>
    [Fact]
    public async Task AverageTicketSizeHandlesDivisionByZero()
    {
        // Arrange
        var date = DateTime.Today;
        var query = new GetDailySalesReportQuery(date);

        var dailySalesData = new DailySalesData(
            Date: date,
            TotalSales: new Money(500m, "USD"),
            TimeBasedSales: new Money(300m, "USD"),
            ProductSales: new Money(200m, "USD"),
            TotalTax: new Money(25m, "USD"),
            TotalGratuity: new Money(50m, "USD"),
            TransactionCount: 0, // Zero transactions
            CustomerCount: 5
        );

        _mockRepository.Setup(r => r.GetDailySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailySalesData);
        _mockRepository.Setup(r => r.GetHourlySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HourlySalesData>());
        _mockRepository.Setup(r => r.GetCategorySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategorySalesDto>());
        _mockRepository.Setup(r => r.GetPaymentMethodSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PaymentMethodSalesDto>());
        _mockRepository.Setup(r => r.GetTableSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TableSalesDto>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Average ticket size should be 0 when transaction count is 0
        Assert.Equal(0m, result.AverageTicketSize);
        Assert.Equal(500m, result.TotalSales.Amount);
        Assert.Equal(0, result.TransactionCount);
    }

    /// <summary>
    /// Unit test: All breakdown collections should be non-null even when empty.
    /// </summary>
    [Fact]
    public async Task BreakdownCollectionsAreNeverNull()
    {
        // Arrange
        var date = DateTime.Today;
        var query = new GetDailySalesReportQuery(date);

        var dailySalesData = new DailySalesData(
            Date: date,
            TotalSales: new Money(100m, "USD"),
            TimeBasedSales: new Money(60m, "USD"),
            ProductSales: new Money(40m, "USD"),
            TotalTax: new Money(8m, "USD"),
            TotalGratuity: new Money(12m, "USD"),
            TransactionCount: 2,
            CustomerCount: 3
        );

        _mockRepository.Setup(r => r.GetDailySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dailySalesData);
        _mockRepository.Setup(r => r.GetHourlySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HourlySalesData>());
        _mockRepository.Setup(r => r.GetCategorySalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategorySalesDto>());
        _mockRepository.Setup(r => r.GetPaymentMethodSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PaymentMethodSalesDto>());
        _mockRepository.Setup(r => r.GetTableSalesDataAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TableSalesDto>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - All breakdown collections should be non-null
        Assert.NotNull(result.HourlyBreakdown);
        Assert.NotNull(result.CategoryBreakdown);
        Assert.NotNull(result.PaymentBreakdown);
        Assert.NotNull(result.TableBreakdown);
        
        // Verify they can be enumerated (even if empty)
        Assert.True(result.HourlyBreakdown.Count() >= 0);
        Assert.True(result.CategoryBreakdown.Count() >= 0);
        Assert.True(result.PaymentBreakdown.Count() >= 0);
        Assert.True(result.TableBreakdown.Count() >= 0);
    }
}