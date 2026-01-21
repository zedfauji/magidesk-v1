using System;
using Xunit;
using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.DomainServices;

/// <summary>
/// Unit tests for PricingService.
/// Tests all pricing scenarios including time rounding, minimum charges, first-hour pricing, and edge cases.
/// Target: ≥90% coverage
/// </summary>
public class PricingServiceTests
{
    private readonly PricingService _pricingService;

    public PricingServiceTests()
    {
        _pricingService = new PricingService();
    }

    #region Validation Tests

    [Fact]
    public void CalculateTimeCharge_WithNullTableType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var billableTime = TimeSpan.FromMinutes(60);

        // Act
        var act = () => _pricingService.CalculateTimeCharge(billableTime, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("tableType");
    }

    [Fact]
    public void CalculateTimeCharge_WithNegativeTime_ShouldThrowArgumentException()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromMinutes(-10);

        // Act
        var act = () => _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*")
            .WithParameterName("billableTime");
    }

    [Fact]
    public void CalculateTimeCharge_WithZeroTime_ShouldReturnZeroCharge()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.Zero;

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(0m);
    }

    #endregion

    #region Basic Hourly Rate Tests

    [Fact]
    public void CalculateTimeCharge_WithExactlyOneHour_ShouldChargeHourlyRate()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromHours(1);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(15.00m);
    }

    [Fact]
    public void CalculateTimeCharge_WithTwoHours_ShouldChargeTwiceHourlyRate()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromHours(2);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(30.00m);
    }

    [Fact]
    public void CalculateTimeCharge_WithHalfHour_ShouldChargeHalfHourlyRate()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromMinutes(30);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(7.50m); // 0.5 hours * $15/hour
    }

    #endregion

    #region Time Rounding Tests

    [Fact]
    public void CalculateTimeCharge_WithRounding15Minutes_ShouldRoundUpCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        tableType.SetRounding(0, 15); // Round to 15 minutes
        var billableTime = TimeSpan.FromMinutes(62); // Should round to 75 minutes

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // 75 minutes = 1.25 hours * $15/hour = $18.75
        result.Amount.Should().Be(18.75m);
    }

    [Fact]
    public void CalculateTimeCharge_WithRounding15Minutes_ExactInterval_ShouldNotRoundUp()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        tableType.SetRounding(0, 15);
        var billableTime = TimeSpan.FromMinutes(60); // Exactly 4 intervals of 15

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(15.00m); // 1 hour * $15/hour
    }

    [Fact]
    public void CalculateTimeCharge_WithRounding1Minute_ShouldNotRound()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        tableType.SetRounding(0, 1); // No rounding
        var billableTime = TimeSpan.FromMinutes(62);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // 62 minutes = 1.0333... hours * $15/hour ≈ $15.50
        result.Amount.Should().BeApproximately(15.50m, 0.01m);
    }

    #endregion

    #region Minimum Charge Tests

    [Fact]
    public void CalculateTimeCharge_BelowMinimum_ShouldChargeMinimum()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        tableType.SetRounding(30, 15); // 30-minute minimum, 15-minute rounding
        var billableTime = TimeSpan.FromMinutes(10); // Below minimum

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // Should charge for 30 minutes (minimum)
        // 30 minutes = 0.5 hours * $15/hour = $7.50
        result.Amount.Should().Be(7.50m);
    }

    [Fact]
    public void CalculateTimeCharge_AboveMinimum_ShouldChargeActualTime()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        tableType.SetRounding(30, 15); // 30-minute minimum
        var billableTime = TimeSpan.FromMinutes(45); // Above minimum

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // 45 minutes = 0.75 hours * $15/hour = $11.25
        result.Amount.Should().Be(11.25m);
    }

    #endregion

    #region First-Hour Pricing Tests

    [Fact]
    public void CalculateTimeCharge_WithFirstHourRate_ExactlyOneHour_ShouldChargeFirstHourRate()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m); // First hour: $30, subsequent: $25
        var billableTime = TimeSpan.FromHours(1);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(30.00m); // First hour premium rate
    }

    [Fact]
    public void CalculateTimeCharge_WithFirstHourRate_TwoHours_ShouldChargeFirstHourPlusStandard()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m); // First hour: $30, subsequent: $25
        var billableTime = TimeSpan.FromHours(2);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // First hour: $30, second hour: $25 = $55
        result.Amount.Should().Be(55.00m);
    }

    [Fact]
    public void CalculateTimeCharge_WithFirstHourRate_OneAndHalfHours_ShouldChargeCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m);
        var billableTime = TimeSpan.FromMinutes(90); // 1.5 hours

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // First hour: $30, remaining 0.5 hours: $12.50 = $42.50
        result.Amount.Should().Be(42.50m);
    }

    [Fact]
    public void CalculateTimeCharge_WithFirstHourRate_LessThanOneHour_ShouldNotApplyFirstHourRate()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m);
        var billableTime = TimeSpan.FromMinutes(30); // Less than 1 hour

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // Should use standard rate: 0.5 hours * $25/hour = $12.50
        result.Amount.Should().Be(12.50m);
    }

    #endregion

    #region Combined Scenarios Tests

    [Fact]
    public void CalculateTimeCharge_WithAllFeatures_ShouldCalculateCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("Premium", 20.00m);
        tableType.UpdateRates(20.00m, 25.00m); // First hour: $25
        tableType.SetRounding(30, 15); // 30-minute minimum, 15-minute rounding
        var billableTime = TimeSpan.FromMinutes(77); // Should round to 90 minutes

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // Rounded: 90 minutes (6 intervals of 15)
        // Above minimum: yes
        // First hour: $25, remaining 30 minutes: $10 = $35
        result.Amount.Should().Be(35.00m);
    }

    [Fact]
    public void CalculateTimeCharge_BelowMinimumWithFirstHourRate_ShouldUseMinimumButNotFirstHourRate()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m); // First hour: $30
        tableType.SetRounding(30, 15); // 30-minute minimum
        var billableTime = TimeSpan.FromMinutes(10); // Below minimum

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // Minimum: 30 minutes (less than 1 hour, so no first-hour rate)
        // 0.5 hours * $25/hour = $12.50
        result.Amount.Should().Be(12.50m);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void CalculateTimeCharge_VeryLongSession_24Hours_ShouldCalculateCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromHours(24);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        result.Amount.Should().Be(360.00m); // 24 hours * $15/hour
    }

    [Fact]
    public void CalculateTimeCharge_VeryLongSession_WithFirstHourRate_ShouldCalculateCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("VIP", 25.00m);
        tableType.UpdateRates(25.00m, 30.00m);
        var billableTime = TimeSpan.FromHours(24);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // First hour: $30, remaining 23 hours: $575 = $605
        result.Amount.Should().Be(605.00m);
    }

    [Fact]
    public void CalculateTimeCharge_OneMinute_ShouldCalculateCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromMinutes(1);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // 1 minute = 1/60 hours * $15/hour = $0.25
        result.Amount.Should().Be(0.25m);
    }

    [Fact]
    public void CalculateTimeCharge_OneSecond_ShouldRoundUpToOneMinute()
    {
        // Arrange
        var tableType = TableType.Create("Standard", 15.00m);
        var billableTime = TimeSpan.FromSeconds(1);

        // Act
        var result = _pricingService.CalculateTimeCharge(billableTime, tableType);

        // Assert
        // Should round up to 1 minute = $0.25
        result.Amount.Should().Be(0.25m);
    }

    #endregion
}
