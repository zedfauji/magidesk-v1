using System;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Property-based tests for enhanced TableType pricing functionality.
/// **Feature: table-game-management, Property 1: First-Hour Pricing Calculation Accuracy**
/// **Validates: Requirements 1.1, 1.3, 1.4**
/// </summary>
public class TableTypeAdvancedPricingTests
{
    #region Test Data Generators

    /// <summary>
    /// Generator for valid hourly rates (between $1 and $100).
    /// </summary>
    public static Arbitrary<decimal> ValidHourlyRateGenerator() =>
        Arb.From(Gen.Choose(100, 10000).Select(x => x / 100m)); // $1.00 to $100.00

    /// <summary>
    /// Generator for valid first-hour rates (between $1 and $150).
    /// </summary>
    public static Arbitrary<decimal> ValidFirstHourRateGenerator() =>
        Arb.From(Gen.Choose(100, 15000).Select(x => x / 100m)); // $1.00 to $150.00

    /// <summary>
    /// Generator for valid minimum charges (between $0 and $50).
    /// </summary>
    public static Arbitrary<Money> ValidMinimumChargeGenerator() =>
        Arb.From(Gen.Choose(0, 5000).Select(x => new Money(x / 100m))); // $0.00 to $50.00

    /// <summary>
    /// Generator for valid time rounding rules.
    /// </summary>
    public static Arbitrary<TimeRoundingRule> ValidRoundingRuleGenerator() =>
        Arb.From(Gen.Elements(
            TimeRoundingRule.None,
            TimeRoundingRule.FifteenMinutes,
            TimeRoundingRule.ThirtyMinutes,
            TimeRoundingRule.SixtyMinutes
        ));

    /// <summary>
    /// Generator for valid table type names.
    /// </summary>
    public static Arbitrary<string> ValidTableTypeNameGenerator() =>
        Arb.From(Gen.Elements(
            "Pool Table",
            "Snooker Table",
            "Billiards Table",
            "Carom Table",
            "Bar Table"
        ));

    #endregion

    #region Property 1: First-Hour Pricing Calculation Accuracy

    [Property(MaxTest = 100)]
    public Property FirstHourPricing_WithFirstHourRate_ShouldUseCorrectRates()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidFirstHourRateGenerator(),
            (validHourlyRate, validFirstHourRate) =>
            {
                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);
                tableType.UpdateRates(validHourlyRate, validFirstHourRate);

                // Act & Assert - First hour rate should be properly set
                tableType.FirstHourRate.Should().Be(validFirstHourRate);
                tableType.HourlyRate.Should().Be(validHourlyRate);

                // Pricing configuration should be valid when first hour rate >= minimum charge
                if (tableType.MinimumCharge.Amount == 0 || validFirstHourRate >= tableType.MinimumCharge.Amount)
                {
                    tableType.ValidatePricingConfiguration().Should().BeTrue();
                }

                return true;
            });
    }

    [Property(MaxTest = 100)]
    public Property FirstHourPricing_WithoutFirstHourRate_ShouldUseStandardRate()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            (validHourlyRate) =>
            {
                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);

                // Act & Assert - No first hour rate should mean null
                tableType.FirstHourRate.Should().BeNull();
                tableType.HourlyRate.Should().Be(validHourlyRate);

                // Configuration should always be valid with standard pricing
                tableType.ValidatePricingConfiguration().Should().BeTrue();

                return true;
            });
    }

    [Property(MaxTest = 100)]
    public Property MinimumCharge_WhenSet_ShouldBeEnforced()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidMinimumChargeGenerator(),
            (validHourlyRate, validMinimumCharge) =>
            {
                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);

                // Act
                tableType.SetMinimumCharge(validMinimumCharge);

                // Assert
                tableType.MinimumCharge.Should().Be(validMinimumCharge);

                // If first hour rate is set, it should be >= minimum charge for valid configuration
                if (tableType.FirstHourRate.HasValue)
                {
                    var firstHourMoney = new Money(tableType.FirstHourRate.Value);
                    var isValid = tableType.ValidatePricingConfiguration();
                    
                    if (validMinimumCharge.Amount > 0 && firstHourMoney < validMinimumCharge)
                    {
                        isValid.Should().BeFalse();
                    }
                    else
                    {
                        isValid.Should().BeTrue();
                    }
                }

                return true;
            });
    }

    #endregion

    #region Time Rounding Rule Properties

    [Property(MaxTest = 100)]
    public Property TimeRoundingRule_WhenSet_ShouldUpdateLegacyRoundingMinutes()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidRoundingRuleGenerator(),
            (validHourlyRate, validRoundingRule) =>
            {
                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);

                // Act
                tableType.SetRoundingRule(validRoundingRule);

                // Assert
                tableType.RoundingRule.Should().Be(validRoundingRule);

                // Legacy RoundingMinutes should be updated correctly
                var expectedMinutes = validRoundingRule switch
                {
                    TimeRoundingRule.None => 1,
                    TimeRoundingRule.FifteenMinutes => 15,
                    TimeRoundingRule.ThirtyMinutes => 30,
                    TimeRoundingRule.SixtyMinutes => 60,
                    _ => 1
                };

                tableType.RoundingMinutes.Should().Be(expectedMinutes);

                return true;
            });
    }

    #endregion

    #region Pricing Configuration Validation Properties

    [Property(MaxTest = 100)]
    public Property PricingConfiguration_WithValidRates_ShouldBeValid()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            (validHourlyRate) =>
            {
                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);

                // Act & Assert
                tableType.ValidatePricingConfiguration().Should().BeTrue();

                // Hourly rate should always be positive
                tableType.HourlyRate.Should().BePositive();

                return true;
            });
    }

    [Property(MaxTest = 100)]
    public Property PricingConfiguration_WithFirstHourRateLessThanMinimum_ShouldBeInvalid()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidFirstHourRateGenerator(),
            ValidMinimumChargeGenerator(),
            (validHourlyRate, validFirstHourRate, validMinimumCharge) =>
            {
                // Only test when minimum charge is greater than first hour rate
                if (validMinimumCharge.Amount <= validFirstHourRate)
                {
                    return true; // Skip this test case
                }

                // Arrange
                var tableType = TableType.Create("Test Table", validHourlyRate);
                tableType.UpdateRates(validHourlyRate, validFirstHourRate);
                tableType.SetMinimumCharge(validMinimumCharge);

                // Act & Assert
                tableType.ValidatePricingConfiguration().Should().BeFalse();

                return true;
            });
    }

    #endregion

    #region Integration Properties

    [Property(MaxTest = 100)]
    public Property TableType_WithAllAdvancedFeatures_ShouldMaintainConsistency()
    {
        return Prop.ForAll(
            ValidTableTypeNameGenerator(),
            ValidHourlyRateGenerator(),
            (validName, validHourlyRate) =>
            {
                // Arrange & Act
                var tableType = TableType.Create(validName, validHourlyRate);
                var firstHourRate = validHourlyRate * 1.2m; // 20% higher than hourly rate
                var minimumCharge = new Money(validHourlyRate * 0.5m); // 50% of hourly rate
                var roundingRule = TimeRoundingRule.FifteenMinutes;

                tableType.UpdateRates(validHourlyRate, firstHourRate);
                tableType.SetMinimumCharge(minimumCharge);
                tableType.SetRoundingRule(roundingRule);

                // Assert - All properties should be set correctly
                tableType.Name.Should().Be(validName);
                tableType.HourlyRate.Should().Be(validHourlyRate);
                tableType.FirstHourRate.Should().Be(firstHourRate);
                tableType.MinimumCharge.Should().Be(minimumCharge);
                tableType.RoundingRule.Should().Be(roundingRule);

                // Should be active by default
                tableType.IsActive.Should().BeTrue();

                // UpdatedAt should be recent
                tableType.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

                return true;
            });
    }

    #endregion

    #region Unit Tests for Edge Cases

    [Fact]
    public void SetMinimumCharge_WithNullCharge_ThrowsArgumentNullException()
    {
        // Arrange
        var tableType = TableType.Create("Test Table", 15.00m);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => tableType.SetMinimumCharge(null!));
    }

    [Fact]
    public void ValidatePricingConfiguration_WithZeroHourlyRate_ReturnsFalse()
    {
        // This tests the edge case where hourly rate becomes zero somehow
        // We can't create a TableType with zero rate, but we can test the validation logic
        var tableType = TableType.Create("Test Table", 15.00m);
        
        // Use reflection to set hourly rate to zero for testing
        var hourlyRateProperty = typeof(TableType).GetProperty("HourlyRate");
        hourlyRateProperty?.SetValue(tableType, 0m);

        // Act & Assert
        tableType.ValidatePricingConfiguration().Should().BeFalse();
    }

    [Theory]
    [InlineData(TimeRoundingRule.None, 1)]
    [InlineData(TimeRoundingRule.FifteenMinutes, 15)]
    [InlineData(TimeRoundingRule.ThirtyMinutes, 30)]
    [InlineData(TimeRoundingRule.SixtyMinutes, 60)]
    public void SetRoundingRule_WithSpecificRule_SetsCorrectLegacyMinutes(
        TimeRoundingRule rule, 
        int expectedMinutes)
    {
        // Arrange
        var tableType = TableType.Create("Test Table", 15.00m);

        // Act
        tableType.SetRoundingRule(rule);

        // Assert
        tableType.RoundingRule.Should().Be(rule);
        tableType.RoundingMinutes.Should().Be(expectedMinutes);
    }

    #endregion
}