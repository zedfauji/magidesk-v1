using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Services;

/// <summary>
/// Advanced domain service for calculating time-based pricing charges with enhanced features.
/// Extends the basic PricingService with advanced pricing rules, simulation, and validation.
/// Stateless service that applies sophisticated table type pricing rules.
/// </summary>
public class AdvancedPricingService : PricingService, IAdvancedPricingService
{
    /// <summary>
    /// Calculates the charge for a given billable time with first-hour pricing rules.
    /// Handles prorated partial hours for the first hour and standard rates for subsequent time.
    /// </summary>
    /// <param name="billableTime">The billable time duration.</param>
    /// <param name="tableType">The table type with pricing configuration.</param>
    /// <returns>The calculated charge as Money.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tableType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when billableTime is negative.</exception>
    public async Task<Money> CalculateFirstHourPricingAsync(TimeSpan billableTime, TableType tableType)
    {
        // Validation
        if (tableType == null)
        {
            throw new ArgumentNullException(nameof(tableType));
        }

        if (billableTime < TimeSpan.Zero)
        {
            throw new ArgumentException("Billable time cannot be negative.", nameof(billableTime));
        }

        // Handle zero time
        if (billableTime == TimeSpan.Zero)
        {
            return Money.Zero();
        }

        // If no first-hour pricing configured, use standard calculation
        if (!tableType.FirstHourRate.HasValue)
        {
            return CalculateTimeCharge(billableTime, tableType);
        }

        var firstHourTime = TimeSpan.FromHours(1);
        Money totalCharge = Money.Zero();

        if (billableTime <= firstHourTime)
        {
            // Prorate first hour rate for partial hour
            var fraction = (decimal)billableTime.TotalHours;
            totalCharge = new Money(tableType.FirstHourRate.Value * fraction);
        }
        else
        {
            // Full first hour at premium rate + remaining time at standard rate
            totalCharge = new Money(tableType.FirstHourRate.Value);
            var remainingTime = billableTime - firstHourTime;
            var remainingCharge = CalculateTimeChargeForDuration(remainingTime, tableType.HourlyRate);
            totalCharge += remainingCharge;
        }

        // Apply minimum charge enforcement
        return await ApplyMinimumChargeAsync(totalCharge, tableType);
    }

    /// <summary>
    /// Applies time rounding rules to a duration based on the specified rounding rule.
    /// Rounds up to the nearest increment (15, 30, or 60 minutes).
    /// </summary>
    /// <param name="duration">The original duration to round.</param>
    /// <param name="rule">The rounding rule to apply.</param>
    /// <returns>The rounded duration.</returns>
    /// <exception cref="ArgumentException">Thrown when duration is negative.</exception>
    public async Task<TimeSpan> ApplyTimeRoundingAsync(TimeSpan duration, TimeRoundingRule rule)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentException("Duration cannot be negative.", nameof(duration));
        }

        if (duration == TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        var totalMinutes = (int)Math.Ceiling(duration.TotalMinutes);
        
        var roundedMinutes = rule switch
        {
            TimeRoundingRule.None => totalMinutes,
            TimeRoundingRule.FifteenMinutes => RoundUpToIncrement(totalMinutes, 15),
            TimeRoundingRule.ThirtyMinutes => RoundUpToIncrement(totalMinutes, 30),
            TimeRoundingRule.SixtyMinutes => RoundUpToIncrement(totalMinutes, 60),
            _ => totalMinutes
        };

        return TimeSpan.FromMinutes(roundedMinutes);
    }

    /// <summary>
    /// Applies minimum charge enforcement to a calculated charge.
    /// Ensures the charge meets the minimum charge requirement for the table type.
    /// </summary>
    /// <param name="calculatedCharge">The calculated charge before minimum enforcement.</param>
    /// <param name="tableType">The table type with minimum charge configuration.</param>
    /// <returns>The charge after minimum charge enforcement.</returns>
    /// <exception cref="ArgumentNullException">Thrown when calculatedCharge or tableType is null.</exception>
    public async Task<Money> ApplyMinimumChargeAsync(Money calculatedCharge, TableType tableType)
    {
        if (calculatedCharge == null)
        {
            throw new ArgumentNullException(nameof(calculatedCharge));
        }

        if (tableType == null)
        {
            throw new ArgumentNullException(nameof(tableType));
        }

        // If no minimum charge configured or calculated charge already meets minimum
        if (tableType.MinimumCharge == null || 
            tableType.MinimumCharge.Amount <= 0 || 
            calculatedCharge >= tableType.MinimumCharge)
        {
            return calculatedCharge;
        }

        // Return the minimum charge
        return tableType.MinimumCharge;
    }

    /// <summary>
    /// Simulates pricing calculations for a given scenario with detailed breakdown.
    /// Provides comprehensive information about how charges are calculated.
    /// </summary>
    /// <param name="scenario">The pricing scenario to simulate.</param>
    /// <returns>Detailed simulation result with charge breakdown and applied rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown when scenario is null.</exception>
    public async Task<PricingSimulationResult> SimulatePricingAsync(PricingScenario scenario)
    {
        if (scenario == null)
        {
            throw new ArgumentNullException(nameof(scenario));
        }

        var appliedRules = new List<string>();
        var originalDuration = scenario.Duration;

        // Step 1: Apply time rounding
        var roundedDuration = await ApplyTimeRoundingAsync(scenario.Duration, scenario.TableType.RoundingRule);
        if (roundedDuration != originalDuration)
        {
            appliedRules.Add($"Time rounded from {originalDuration.TotalMinutes:F1} to {roundedDuration.TotalMinutes:F0} minutes ({scenario.TableType.RoundingRule})");
        }

        // Step 2: Calculate base charge (without first-hour pricing)
        var baseCharge = CalculateTimeChargeForDuration(roundedDuration, scenario.TableType.HourlyRate);

        // Step 3: Calculate first-hour charge if applicable
        Money firstHourCharge = Money.Zero();
        Money remainingHoursCharge = Money.Zero();

        if (scenario.TableType.FirstHourRate.HasValue && roundedDuration.TotalHours >= 1)
        {
            var firstHourTime = TimeSpan.FromHours(1);
            if (roundedDuration <= firstHourTime)
            {
                // Prorated first hour
                var fraction = (decimal)roundedDuration.TotalHours;
                firstHourCharge = new Money(scenario.TableType.FirstHourRate.Value * fraction);
                appliedRules.Add($"First-hour rate applied (prorated): {fraction:P0} of ${scenario.TableType.FirstHourRate.Value:F2}");
            }
            else
            {
                // Full first hour + remaining time
                firstHourCharge = new Money(scenario.TableType.FirstHourRate.Value);
                var remainingTime = roundedDuration - firstHourTime;
                remainingHoursCharge = CalculateTimeChargeForDuration(remainingTime, scenario.TableType.HourlyRate);
                appliedRules.Add($"First-hour rate applied: ${scenario.TableType.FirstHourRate.Value:F2}");
                appliedRules.Add($"Remaining {remainingTime.TotalHours:F2} hours at standard rate: ${scenario.TableType.HourlyRate:F2}/hour");
            }
        }

        // Step 4: Calculate total before minimum charge
        var calculatedCharge = scenario.TableType.FirstHourRate.HasValue && roundedDuration.TotalHours >= 1
            ? firstHourCharge + remainingHoursCharge
            : baseCharge;

        // Step 5: Apply minimum charge
        var finalCharge = await ApplyMinimumChargeAsync(calculatedCharge, scenario.TableType);
        if (finalCharge > calculatedCharge)
        {
            appliedRules.Add($"Minimum charge applied: ${scenario.TableType.MinimumCharge.Amount:F2}");
        }

        // Step 6: Apply member discount if applicable
        if (scenario.HasMemberDiscount)
        {
            appliedRules.Add("Member discount would be applied (not yet implemented)");
        }

        return PricingSimulationResult.CreateDetailed(
            baseCharge: baseCharge,
            firstHourCharge: firstHourCharge,
            remainingHoursCharge: remainingHoursCharge,
            minimumChargeApplied: finalCharge > calculatedCharge ? scenario.TableType.MinimumCharge : Money.Zero(),
            finalCharge: finalCharge,
            roundedDuration: roundedDuration,
            appliedRules: appliedRules.AsReadOnly()
        );
    }

    /// <summary>
    /// Validates that pricing rules for a table type are mathematically consistent.
    /// Checks for conflicts and logical inconsistencies in pricing configuration.
    /// </summary>
    /// <param name="tableType">The table type to validate.</param>
    /// <returns>True if pricing rules are valid and consistent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tableType is null.</exception>
    public async Task<bool> ValidatePricingRulesAsync(TableType tableType)
    {
        if (tableType == null)
        {
            throw new ArgumentNullException(nameof(tableType));
        }

        // Use the existing validation method from TableType
        if (!tableType.ValidatePricingConfiguration())
        {
            return false;
        }

        // Additional advanced validation rules

        // 1. First hour rate should be reasonable compared to hourly rate
        if (tableType.FirstHourRate.HasValue)
        {
            // First hour rate shouldn't be more than 5x the hourly rate (reasonable business rule)
            if (tableType.FirstHourRate.Value > tableType.HourlyRate * 5)
            {
                return false;
            }

            // First hour rate shouldn't be less than 50% of hourly rate (business logic)
            if (tableType.FirstHourRate.Value < tableType.HourlyRate * 0.5m)
            {
                return false;
            }
        }

        // 2. Minimum charge should be reasonable
        if (tableType.MinimumCharge != null && tableType.MinimumCharge.Amount > 0)
        {
            // Minimum charge shouldn't exceed 2 hours at standard rate
            var twoHoursCharge = tableType.HourlyRate * 2;
            if (tableType.MinimumCharge.Amount > twoHoursCharge)
            {
                return false;
            }
        }

        // 3. Rounding rules should be consistent with minimum minutes
        if (tableType.MinimumMinutes > 0 && tableType.RoundingMinutes > 1)
        {
            // Minimum minutes should be a multiple of rounding minutes for consistency
            if (tableType.MinimumMinutes % tableType.RoundingMinutes != 0)
            {
                return false;
            }
        }

        return true;
    }

    #region Private Helper Methods

    /// <summary>
    /// Calculates charge for a duration at a specific hourly rate.
    /// </summary>
    /// <param name="duration">The duration to charge for.</param>
    /// <param name="hourlyRate">The hourly rate to apply.</param>
    /// <returns>The calculated charge.</returns>
    private Money CalculateTimeChargeForDuration(TimeSpan duration, decimal hourlyRate)
    {
        if (duration <= TimeSpan.Zero)
        {
            return Money.Zero();
        }

        var hours = (decimal)duration.TotalHours;
        return new Money(hours * hourlyRate);
    }

    /// <summary>
    /// Rounds up to the nearest increment.
    /// </summary>
    /// <param name="minutes">The minutes to round.</param>
    /// <param name="increment">The increment to round to.</param>
    /// <returns>The rounded minutes.</returns>
    private int RoundUpToIncrement(int minutes, int increment)
    {
        if (increment <= 1)
            return minutes;

        var intervals = (int)Math.Ceiling((double)minutes / increment);
        return intervals * increment;
    }

    #endregion
}