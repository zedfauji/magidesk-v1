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
/// Property-based tests for GetTimeRevenueReportQueryHandler.
/// Feature: reporting-export, Property 1: Revenue Calculation Integrity
/// Validates: Requirements 3.1, 3.4
/// </summary>
public class GetTimeRevenueReportQueryHandlerPropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<ILogger<GetTimeRevenueReportQueryHandler>> _mockLogger;
    private readonly GetTimeRevenueReportQueryHandler _handler;

    public GetTimeRevenueReportQueryHandlerPropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockLogger = new Mock<ILogger<GetTimeRevenueReportQueryHandler>>();
        _handler = new GetTimeRevenueReportQueryHandler(_mockRepository.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Unit test: Basic time revenue report generation with known values.
    /// </summary>
    [Fact]
    public async Task TimeRevenueReport_WithKnownValues_ReturnsCorrectCalculations()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(40m, "USD"), TimeSpan.FromHours(2), 20m),
            new(new DateTime(2024, 1, 2), Guid.NewGuid(), "Snooker", new Money(75m, "USD"), TimeSpan.FromHours(3), 25m),
            new(new DateTime(2024, 1, 3), Guid.NewGuid(), "Pool", new Money(30m, "USD"), TimeSpan.FromHours(1.5), 20m),
            new(new DateTime(2024, 1, 6), Guid.NewGuid(), "Carom", new Money(120m, "USD"), TimeSpan.FromHours(4), 30m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify revenue calculation integrity
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        
        // Total revenue should equal sum of all time charges
        var expectedTotalRevenue = timeRevenueData.Sum(t => t.TimeRevenue.Amount);
        Assert.Equal(expectedTotalRevenue, result.TotalTimeRevenue.Amount);
        
        // Total billed time should equal sum of all session durations
        var expectedTotalTime = TimeSpan.FromTicks(timeRevenueData.Sum(t => t.BilledTime.Ticks));
        Assert.Equal(expectedTotalTime, result.TotalBilledTime);
        
        // Average hourly rate should be calculated correctly
        var expectedAverageRate = timeRevenueData.Average(t => t.HourlyRate);
        Assert.Equal(Math.Round(expectedAverageRate, 2), result.AverageHourlyRate);
        
        // Revenue per hour should be calculated correctly
        var expectedRevenuePerHour = (decimal)(expectedTotalRevenue / (decimal)expectedTotalTime.TotalHours);
        Assert.Equal(Math.Round(expectedRevenuePerHour, 2), result.RevenuePerHour);
        
        // Verify table type breakdown sums to total
        var tableTypeTotal = result.ByTableType.Sum(t => t.Revenue.Amount);
        Assert.Equal(result.TotalTimeRevenue.Amount, tableTypeTotal);
        
        // Verify day of week breakdown sums to total
        var dayOfWeekTotal = result.ByDayOfWeek.Sum(d => d.Revenue.Amount);
        Assert.Equal(result.TotalTimeRevenue.Amount, dayOfWeekTotal);
        
        // Verify hourly breakdown sums to total
        var hourlyTotal = result.ByHourOfDay.Sum(h => h.Revenue.Amount);
        Assert.Equal(result.TotalTimeRevenue.Amount, hourlyTotal);
    }

    /// <summary>
    /// Unit test: Empty data should result in zero values but valid structure.
    /// </summary>
    [Fact]
    public async Task EmptyDataResultsInZeroValuesWithValidStructure()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeRevenueData>());

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Empty data should result in zero values
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal(0m, result.TotalTimeRevenue.Amount);
        Assert.Equal(TimeSpan.Zero, result.TotalBilledTime);
        Assert.Equal(0m, result.AverageHourlyRate);
        Assert.Equal(0m, result.RevenuePerHour);
        
        // Verify collections are empty but not null
        Assert.NotNull(result.ByTableType);
        Assert.NotNull(result.ByDayOfWeek);
        Assert.NotNull(result.ByHourOfDay);
        Assert.Empty(result.ByTableType);
        Assert.Empty(result.ByDayOfWeek);
        Assert.Empty(result.ByHourOfDay);
    }

    /// <summary>
    /// Unit test: Table type breakdown percentages should sum to 100% when data exists.
    /// </summary>
    [Fact]
    public async Task TableTypeBreakdownPercentagesSumTo100Percent()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(60m, "USD"), TimeSpan.FromHours(2), 30m),
            new(new DateTime(2024, 1, 2), Guid.NewGuid(), "Snooker", new Money(90m, "USD"), TimeSpan.FromHours(3), 30m),
            new(new DateTime(2024, 1, 3), Guid.NewGuid(), "Carom", new Money(50m, "USD"), TimeSpan.FromHours(1), 50m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify percentage calculations
        var tableTypePercentageSum = result.ByTableType.Sum(t => t.PercentOfTotal);
        Assert.Equal(100m, tableTypePercentageSum, 1); // Allow 1 decimal place tolerance for rounding
        
        // Verify table type amounts sum to total
        var tableTypeAmountSum = result.ByTableType.Sum(t => t.Revenue.Amount);
        Assert.Equal(result.TotalTimeRevenue.Amount, tableTypeAmountSum);
        
        // Verify each table type has correct calculations
        foreach (var tableType in result.ByTableType)
        {
            Assert.True(tableType.Revenue.Amount >= 0);
            Assert.True(tableType.BilledTime >= TimeSpan.Zero);
            Assert.True(tableType.AverageRate >= 0);
            Assert.True(tableType.SessionCount >= 0);
            Assert.True(tableType.PercentOfTotal >= 0 && tableType.PercentOfTotal <= 100);
        }
    }

    /// <summary>
    /// Unit test: Revenue per hour calculation handles division by zero.
    /// </summary>
    [Fact]
    public async Task RevenuePerHourHandlesDivisionByZero()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            // Zero duration sessions
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(0m, "USD"), TimeSpan.Zero, 20m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Revenue per hour should be 0 when total billed time is 0
        Assert.Equal(0m, result.RevenuePerHour);
        Assert.Equal(TimeSpan.Zero, result.TotalBilledTime);
    }

    /// <summary>
    /// Unit test: Weekend vs weekday classification is correct.
    /// </summary>
    [Fact]
    public async Task WeekendVsWeekdayClassificationIsCorrect()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1); // Monday
        var endDate = new DateTime(2024, 1, 7);   // Sunday
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(40m, "USD"), TimeSpan.FromHours(2), 20m), // Monday
            new(new DateTime(2024, 1, 6), Guid.NewGuid(), "Pool", new Money(60m, "USD"), TimeSpan.FromHours(3), 20m), // Saturday
            new(new DateTime(2024, 1, 7), Guid.NewGuid(), "Pool", new Money(20m, "USD"), TimeSpan.FromHours(1), 20m)  // Sunday
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Verify weekend classification
        var mondayData = result.ByDayOfWeek.FirstOrDefault(d => d.DayOfWeek == DayOfWeek.Monday);
        var saturdayData = result.ByDayOfWeek.FirstOrDefault(d => d.DayOfWeek == DayOfWeek.Saturday);
        var sundayData = result.ByDayOfWeek.FirstOrDefault(d => d.DayOfWeek == DayOfWeek.Sunday);

        Assert.NotNull(mondayData);
        Assert.NotNull(saturdayData);
        Assert.NotNull(sundayData);
        
        Assert.False(mondayData.IsWeekend);
        Assert.True(saturdayData.IsWeekend);
        Assert.True(sundayData.IsWeekend);
    }

    /// <summary>
    /// Unit test: All breakdown collections should be non-null even when empty.
    /// </summary>
    [Fact]
    public async Task BreakdownCollectionsAreNeverNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(25m, "USD"), TimeSpan.FromHours(1), 25m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - All breakdown collections should be non-null
        Assert.NotNull(result.ByTableType);
        Assert.NotNull(result.ByDayOfWeek);
        Assert.NotNull(result.ByHourOfDay);
        
        // Verify they can be enumerated
        Assert.True(result.ByTableType.Count() >= 0);
        Assert.True(result.ByDayOfWeek.Count() >= 0);
        Assert.True(result.ByHourOfDay.Count() >= 0);
    }

    /// <summary>
    /// Unit test: Revenue calculations should be non-negative.
    /// </summary>
    [Fact]
    public async Task RevenueCalculationsAreNonNegative()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1), Guid.NewGuid(), "Pool", new Money(50m, "USD"), TimeSpan.FromHours(2), 25m),
            new(new DateTime(2024, 1, 2), Guid.NewGuid(), "Snooker", new Money(90m, "USD"), TimeSpan.FromHours(3), 30m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - All revenue values should be non-negative
        Assert.True(result.TotalTimeRevenue.Amount >= 0);
        Assert.True(result.TotalBilledTime >= TimeSpan.Zero);
        Assert.True(result.AverageHourlyRate >= 0);
        Assert.True(result.RevenuePerHour >= 0);
        
        // Verify all breakdown values are non-negative
        foreach (var tableType in result.ByTableType)
        {
            Assert.True(tableType.Revenue.Amount >= 0);
            Assert.True(tableType.BilledTime >= TimeSpan.Zero);
            Assert.True(tableType.AverageRate >= 0);
            Assert.True(tableType.SessionCount >= 0);
            Assert.True(tableType.PercentOfTotal >= 0);
        }
        
        foreach (var dayOfWeek in result.ByDayOfWeek)
        {
            Assert.True(dayOfWeek.Revenue.Amount >= 0);
            Assert.True(dayOfWeek.BilledTime >= TimeSpan.Zero);
            Assert.True(dayOfWeek.AverageRate >= 0);
            Assert.True(dayOfWeek.SessionCount >= 0);
        }
        
        foreach (var hourly in result.ByHourOfDay)
        {
            Assert.True(hourly.Revenue.Amount >= 0);
            Assert.True(hourly.BilledTime >= TimeSpan.Zero);
            Assert.True(hourly.AverageRate >= 0);
            Assert.True(hourly.SessionCount >= 0);
        }
    }

    /// <summary>
    /// Unit test: Hourly breakdown should be ordered by hour.
    /// </summary>
    [Fact]
    public async Task HourlyBreakdownIsOrderedByHour()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 1, 7);
        var query = new GetTimeRevenueReportQuery(startDate, endDate);

        var timeRevenueData = new List<TimeRevenueData>
        {
            new(new DateTime(2024, 1, 1, 18, 0, 0), Guid.NewGuid(), "Pool", new Money(25m, "USD"), TimeSpan.FromHours(1), 25m),
            new(new DateTime(2024, 1, 1, 14, 0, 0), Guid.NewGuid(), "Pool", new Money(25m, "USD"), TimeSpan.FromHours(1), 25m),
            new(new DateTime(2024, 1, 1, 16, 0, 0), Guid.NewGuid(), "Pool", new Money(25m, "USD"), TimeSpan.FromHours(1), 25m)
        };

        _mockRepository.Setup(r => r.GetTimeRevenueDataAsync(startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeRevenueData);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert - Hourly breakdown should be ordered by hour
        var hourlyBreakdown = result.ByHourOfDay.ToList();
        for (int i = 1; i < hourlyBreakdown.Count; i++)
        {
            Assert.True(hourlyBreakdown[i - 1].Hour <= hourlyBreakdown[i].Hour);
        }
    }
}