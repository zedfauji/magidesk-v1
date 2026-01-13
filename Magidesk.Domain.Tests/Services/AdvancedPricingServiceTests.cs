using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Unit tests for AdvancedPricingService functionality.
/// Tests specific examples and edge cases for advanced pricing features.
/// </summary>
public class AdvancedPricingServiceTests
{
    private readonly AdvancedPricingService _advancedPricingService;

    public AdvancedPricingServiceTests()
    {
        _advancedPricingService = new AdvancedPricingService();
    }

    #region First Hour Pricing Tests

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithPartialFirstHour_ShouldProrateCorrectly()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        tableType.UpdateRates(20.00m, 30.00m); // $30 for first hour, $20 for subsequent
        var duration = TimeSpan.FromMinutes(30); // Half hour

        // Act
        var result = await _advancedPricingService.CalculateFirstHourPricingAsync(duration, tableType);

        // Assert
        result.Amount.Should().Be(15.00m, "Half hour should be 50% of first-hour rate ($30 * 0.5 = $15)");
    }

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithExactFirstHour_ShouldChargeFullFirstHourRate()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        tableType.UpdateRates(20.00m, 30.00m);
        var duration = TimeSpan.FromHours(1);

        // Act
        var result = await _advancedPricingService.CalculateFirstHourPricingAsync(duration, tableType);

        // Assert
        result.Amount.Should().Be(30.00m, "Exactly one hour should charge the full first-hour rate");
    }

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithMoreThanOneHour_ShouldChargeFirstHourPlusStandardRate()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        tableType.UpdateRates(20.00m, 30.00m);
        var duration = TimeSpan.FromMinutes(90); // 1.5 hours

        // Act
        var result = await _advancedPricingService.CalculateFirstHourPricingAsync(duration, tableType);

        // Assert
        // First hour: $30, remaining 0.5 hours: $20 * 0.5 = $10, total: $40
        result.Amount.Should().Be(40.00m, "1.5 hours should be $30 (first hour) + $10 (0.5 * $20) = $40");
    }

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithNoFirstHourRate_ShouldUseStandardCalculation()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        // No first-hour rate set
        var duration = TimeSpan.FromMinutes(90);

        // Act
        var result = await _advancedPricingService.CalculateFirstHourPricingAsync(duration, tableType);

        // Assert
        // Should use standard calculation: 1.5 hours * $20 = $30
        result.Amount.Should().Be(30.00m, "Without first-hour rate, should use standard hourly rate");
    }

    #endregion

    #region Time Rounding Tests

    [Theory]
    [InlineData(14, TimeRoundingRule.FifteenMinutes, 15)]
    [InlineData(15, TimeRoundingRule.FifteenMinutes, 15)]
    [InlineData(16, TimeRoundingRule.FifteenMinutes, 30)]
    [InlineData(29, TimeRoundingRule.ThirtyMinutes, 30)]
    [InlineData(31, TimeRoundingRule.ThirtyMinutes, 60)]
    [InlineData(59, TimeRoundingRule.SixtyMinutes, 60)]
    [InlineData(61, TimeRoundingRule.SixtyMinutes, 120)]
    public async Task ApplyTimeRoundingAsync_WithVariousRules_ShouldRoundCorrectly(
        int inputMinutes, 
        TimeRoundingRule rule, 
        int expectedMinutes)
    {
        // Arrange
        var duration = TimeSpan.FromMinutes(inputMinutes);

        // Act
        var result = await _advancedPricingService.ApplyTimeRoundingAsync(duration, rule);

        // Assert
        result.TotalMinutes.Should().Be(expectedMinutes, 
            $"Rounding {inputMinutes} minutes with {rule} should result in {expectedMinutes} minutes");
    }

    [Fact]
    public async Task ApplyTimeRoundingAsync_WithNoneRule_ShouldNotRound()
    {
        // Arrange
        var duration = TimeSpan.FromMinutes(37);

        // Act
        var result = await _advancedPricingService.ApplyTimeRoundingAsync(duration, TimeRoundingRule.None);

        // Assert
        result.TotalMinutes.Should().Be(37, "None rule should not round the time");
    }

    [Fact]
    public async Task ApplyTimeRoundingAsync_WithZeroDuration_ShouldReturnZero()
    {
        // Arrange
        var duration = TimeSpan.Zero;

        // Act
        var result = await _advancedPricingService.ApplyTimeRoundingAsync(duration, TimeRoundingRule.FifteenMinutes);

        // Assert
        result.Should().Be(TimeSpan.Zero, "Zero duration should remain zero regardless of rounding rule");
    }

    #endregion

    #region Minimum Charge Tests

    [Fact]
    public async Task ApplyMinimumChargeAsync_WhenCalculatedChargeIsLess_ShouldReturnMinimum()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        tableType.SetMinimumCharge(new Money(15.00m));
        var calculatedCharge = new Money(10.00m);

        // Act
        var result = await _advancedPricingService.ApplyMinimumChargeAsync(calculatedCharge, tableType);

        // Assert
        result.Amount.Should().Be(15.00m, "Should return minimum charge when calculated charge is less");
    }

    [Fact]
    public async Task ApplyMinimumChargeAsync_WhenCalculatedChargeIsGreater_ShouldReturnCalculated()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        tableType.SetMinimumCharge(new Money(15.00m));
        var calculatedCharge = new Money(25.00m);

        // Act
        var result = await _advancedPricingService.ApplyMinimumChargeAsync(calculatedCharge, tableType);

        // Assert
        result.Amount.Should().Be(25.00m, "Should return calculated charge when it exceeds minimum");
    }

    [Fact]
    public async Task ApplyMinimumChargeAsync_WithNoMinimumCharge_ShouldReturnCalculated()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        // No minimum charge set
        var calculatedCharge = new Money(10.00m);

        // Act
        var result = await _advancedPricingService.ApplyMinimumChargeAsync(calculatedCharge, tableType);

        // Assert
        result.Amount.Should().Be(10.00m, "Should return calculated charge when no minimum is set");
    }

    #endregion

    #region Pricing Simulation Tests

    [Fact]
    public async Task SimulatePricingAsync_WithComplexScenario_ShouldProvideDetailedBreakdown()
    {
        // Arrange
        var tableType = TableType.Create("Premium Pool Table", 25.00m);
        tableType.UpdateRates(25.00m, 40.00m); // $40 first hour, $25 subsequent
        tableType.SetRoundingRule(TimeRoundingRule.FifteenMinutes);
        tableType.SetMinimumCharge(new Money(20.00m));
        
        var scenario = PricingScenario.CreateBasic(
            TimeSpan.FromMinutes(77), // 1 hour 17 minutes -> rounds to 1 hour 30 minutes
            tableType,
            4 // 4 guests
        );

        // Act
        var result = await _advancedPricingService.SimulatePricingAsync(scenario);

        // Assert
        result.Should().NotBeNull();
        result.FinalCharge.Amount.Should().BeGreaterThan(0);
        result.FirstHourCharge.Amount.Should().Be(40.00m, "Should charge $40 for first hour");
        result.RemainingHoursCharge.Amount.Should().Be(12.50m, "Should charge $25 * 0.5 = $12.50 for remaining 30 minutes");
        result.FinalCharge.Amount.Should().Be(52.50m, "Total should be $40 + $12.50 = $52.50");
        result.RoundedDuration.TotalMinutes.Should().Be(90, "77 minutes should round to 90 minutes");
        result.AppliedRules.Should().NotBeEmpty("Should have applied rules listed");
    }

    [Fact]
    public async Task SimulatePricingAsync_WithMinimumChargeScenario_ShouldApplyMinimum()
    {
        // Arrange
        var tableType = TableType.Create("Basic Table", 20.00m);
        tableType.SetMinimumCharge(new Money(15.00m));
        
        var scenario = PricingScenario.CreateBasic(
            TimeSpan.FromMinutes(30), // 0.5 hours * $20 = $10, but minimum is $15
            tableType,
            2
        );

        // Act
        var result = await _advancedPricingService.SimulatePricingAsync(scenario);

        // Assert
        result.FinalCharge.Amount.Should().Be(15.00m, "Should apply minimum charge of $15");
        result.MinimumChargeApplied.Amount.Should().Be(15.00m, "Should show minimum charge was applied");
        result.WasMinimumChargeApplied().Should().BeTrue("Should indicate minimum charge was applied");
    }

    #endregion

    #region Pricing Rules Validation Tests

    [Fact]
    public async Task ValidatePricingRulesAsync_WithValidConfiguration_ShouldReturnTrue()
    {
        // Arrange
        var tableType = TableType.Create("Valid Table", 20.00m);
        tableType.UpdateRates(20.00m, 25.00m); // Reasonable first-hour premium
        tableType.SetMinimumCharge(new Money(10.00m)); // Reasonable minimum
        tableType.SetRounding(15, 15); // Consistent rounding

        // Act
        var result = await _advancedPricingService.ValidatePricingRulesAsync(tableType);

        // Assert
        result.Should().BeTrue("Valid pricing configuration should pass validation");
    }

    [Fact]
    public async Task ValidatePricingRulesAsync_WithExcessiveFirstHourRate_ShouldReturnFalse()
    {
        // Arrange
        var tableType = TableType.Create("Invalid Table", 20.00m);
        tableType.UpdateRates(20.00m, 120.00m); // 6x the hourly rate - excessive
        
        // Act
        var result = await _advancedPricingService.ValidatePricingRulesAsync(tableType);

        // Assert
        result.Should().BeFalse("Excessive first-hour rate should fail validation");
    }

    [Fact]
    public async Task ValidatePricingRulesAsync_WithTooLowFirstHourRate_ShouldReturnFalse()
    {
        // Arrange
        var tableType = TableType.Create("Invalid Table", 20.00m);
        tableType.UpdateRates(20.00m, 8.00m); // Less than 50% of hourly rate
        
        // Act
        var result = await _advancedPricingService.ValidatePricingRulesAsync(tableType);

        // Assert
        result.Should().BeFalse("Too low first-hour rate should fail validation");
    }

    [Fact]
    public async Task ValidatePricingRulesAsync_WithExcessiveMinimumCharge_ShouldReturnFalse()
    {
        // Arrange
        var tableType = TableType.Create("Invalid Table", 20.00m);
        tableType.SetMinimumCharge(new Money(50.00m)); // More than 2 hours at $20/hour
        
        // Act
        var result = await _advancedPricingService.ValidatePricingRulesAsync(tableType);

        // Assert
        result.Should().BeFalse("Excessive minimum charge should fail validation");
    }

    [Fact]
    public async Task ValidatePricingRulesAsync_WithInconsistentRounding_ShouldReturnFalse()
    {
        // Arrange
        var tableType = TableType.Create("Invalid Table", 20.00m);
        tableType.SetRounding(22, 15); // 22 minutes minimum with 15-minute rounding - inconsistent
        
        // Act
        var result = await _advancedPricingService.ValidatePricingRulesAsync(tableType);

        // Assert
        result.Should().BeFalse("Inconsistent rounding configuration should fail validation");
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithNullTableType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var duration = TimeSpan.FromMinutes(30);

        // Act & Assert
        await _advancedPricingService.Invoking(s => s.CalculateFirstHourPricingAsync(duration, null!))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("tableType");
    }

    [Fact]
    public async Task CalculateFirstHourPricingAsync_WithNegativeDuration_ShouldThrowArgumentException()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);
        var negativeDuration = TimeSpan.FromMinutes(-30);

        // Act & Assert
        await _advancedPricingService.Invoking(s => s.CalculateFirstHourPricingAsync(negativeDuration, tableType))
            .Should().ThrowAsync<ArgumentException>()
            .WithParameterName("billableTime");
    }

    [Fact]
    public async Task ApplyTimeRoundingAsync_WithNegativeDuration_ShouldThrowArgumentException()
    {
        // Arrange
        var negativeDuration = TimeSpan.FromMinutes(-15);

        // Act & Assert
        await _advancedPricingService.Invoking(s => s.ApplyTimeRoundingAsync(negativeDuration, TimeRoundingRule.FifteenMinutes))
            .Should().ThrowAsync<ArgumentException>()
            .WithParameterName("duration");
    }

    [Fact]
    public async Task ApplyMinimumChargeAsync_WithNullCharge_ShouldThrowArgumentNullException()
    {
        // Arrange
        var tableType = TableType.Create("Pool Table", 20.00m);

        // Act & Assert
        await _advancedPricingService.Invoking(s => s.ApplyMinimumChargeAsync(null!, tableType))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("calculatedCharge");
    }

    [Fact]
    public async Task SimulatePricingAsync_WithNullScenario_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await _advancedPricingService.Invoking(s => s.SimulatePricingAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("scenario");
    }

    [Fact]
    public async Task ValidatePricingRulesAsync_WithNullTableType_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await _advancedPricingService.Invoking(s => s.ValidatePricingRulesAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("tableType");
    }

    #endregion
}