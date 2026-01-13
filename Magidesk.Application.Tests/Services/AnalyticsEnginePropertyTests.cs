using FsCheck.Xunit;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Services;

/// <summary>
/// Property-based tests for AnalyticsEngine.
/// Feature: reporting-export, Property 3: Table Utilization Calculation Accuracy
/// Validates: Requirements 2.1, 2.2, 2.4
/// </summary>
public class AnalyticsEnginePropertyTests
{
    private readonly Mock<IAnalyticsRepository> _mockRepository;
    private readonly Mock<ILogger<AnalyticsEngine>> _mockLogger;
    private readonly AnalyticsEngine _analyticsEngine;

    public AnalyticsEnginePropertyTests()
    {
        _mockRepository = new Mock<IAnalyticsRepository>();
        _mockLogger = new Mock<ILogger<AnalyticsEngine>>();
        _analyticsEngine = new AnalyticsEngine(_mockRepository.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Unit test: Basic table utilization calculation with known values.
    /// </summary>
    [Fact]
    public async Task TableUtilizationCalculation_WithKnownValues_ReturnsCorrectResult()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);
        var operatingHours = TimeSpan.FromHours(12); // 12 hours per day
        
        var sessionData = new List<TableSessionData>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(9), DateTime.Today.AddHours(11), 
                TimeSpan.FromHours(2), new Money(20m, "USD"), 2, null), // 2 hours
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(14), DateTime.Today.AddHours(16), 
                TimeSpan.FromHours(2), new Money(20m, "USD"), 4, null), // 2 hours
        };

        _mockRepository.Setup(r => r.GetTableSessionDataAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionData);

        _mockRepository.Setup(r => r.GetOperatingHoursAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _analyticsEngine.CalculateTableUtilizationAsync(startDate, endDate);

        // Assert
        // Total occupied: 4 hours, Total operating: 24 hours (12 * 2 days), Expected: 16.67%
        Assert.Equal(16.67m, result.OccupancyPercent, 2);
        Assert.Equal(2, result.TotalSessions);
        Assert.Equal(TimeSpan.FromHours(2), result.AverageSessionDuration);
        Assert.Equal(TimeSpan.FromHours(4), result.TotalOccupiedTime);
    }

    /// <summary>
    /// Unit test: Empty session data should result in zero metrics.
    /// </summary>
    [Fact]
    public async Task EmptySessionDataResultsInZeroMetrics()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);
        var operatingHours = TimeSpan.FromHours(12);
        var emptySessionData = new List<TableSessionData>();

        _mockRepository.Setup(r => r.GetTableSessionDataAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptySessionData);

        _mockRepository.Setup(r => r.GetOperatingHoursAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _analyticsEngine.CalculateTableUtilizationAsync(startDate, endDate);

        // Assert - Empty data should result in zero metrics
        Assert.Equal(0m, result.OccupancyPercent);
        Assert.Equal(0, result.TotalSessions);
        Assert.Equal(TimeSpan.Zero, result.AverageSessionDuration);
        Assert.Equal(TimeSpan.Zero, result.TotalOccupiedTime);
    }

    /// <summary>
    /// Unit test: Zero operating hours should result in zero occupancy.
    /// </summary>
    [Fact]
    public async Task ZeroOperatingHoursResultsInZeroOccupancy()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);
        var zeroOperatingHours = TimeSpan.Zero;
        var sessionData = new List<TableSessionData>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(9), DateTime.Today.AddHours(11), 
                TimeSpan.FromHours(2), new Money(20m, "USD"), 2, null)
        };

        _mockRepository.Setup(r => r.GetTableSessionDataAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionData);

        _mockRepository.Setup(r => r.GetOperatingHoursAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(zeroOperatingHours);

        // Act
        var result = await _analyticsEngine.CalculateTableUtilizationAsync(startDate, endDate);

        // Assert - When operating hours is zero, occupancy should be zero
        Assert.Equal(0m, result.OccupancyPercent);
    }

    /// <summary>
    /// Unit test: Occupancy percentage should never exceed 100%.
    /// </summary>
    [Fact]
    public async Task OccupancyPercentageNeverExceeds100()
    {
        // Arrange - Create scenario where occupied time exceeds operating time
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);
        var operatingHours = TimeSpan.FromHours(8); // Only 8 hours per day
        
        var sessionData = new List<TableSessionData>
        {
            // Multiple overlapping sessions that would exceed operating hours
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(9), DateTime.Today.AddHours(17), 
                TimeSpan.FromHours(8), new Money(80m, "USD"), 2, null), // 8 hours
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(10), DateTime.Today.AddHours(18), 
                TimeSpan.FromHours(8), new Money(80m, "USD"), 4, null), // 8 hours
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(11), DateTime.Today.AddHours(19), 
                TimeSpan.FromHours(8), new Money(80m, "USD"), 3, null), // 8 hours
        };

        _mockRepository.Setup(r => r.GetTableSessionDataAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionData);

        _mockRepository.Setup(r => r.GetOperatingHoursAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _analyticsEngine.CalculateTableUtilizationAsync(startDate, endDate);

        // Assert - Occupancy should be capped at 100%
        Assert.True(result.OccupancyPercent <= 100m);
        Assert.True(result.OccupancyPercent >= 0m);
    }

    /// <summary>
    /// Unit test: Average session duration calculation.
    /// </summary>
    [Fact]
    public async Task AverageSessionDurationCalculatedCorrectly()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);
        var operatingHours = TimeSpan.FromHours(12);
        
        var sessionData = new List<TableSessionData>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(9), DateTime.Today.AddHours(10), 
                TimeSpan.FromHours(1), new Money(10m, "USD"), 2, null), // 1 hour
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.AddHours(14), DateTime.Today.AddHours(17), 
                TimeSpan.FromHours(3), new Money(30m, "USD"), 4, null), // 3 hours
        };

        _mockRepository.Setup(r => r.GetTableSessionDataAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionData);

        _mockRepository.Setup(r => r.GetOperatingHoursAsync(
            It.IsAny<DateTime>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatingHours);

        // Act
        var result = await _analyticsEngine.CalculateTableUtilizationAsync(startDate, endDate);

        // Assert - Average should be (1 + 3) / 2 = 2 hours
        Assert.Equal(TimeSpan.FromHours(2), result.AverageSessionDuration);
    }
}