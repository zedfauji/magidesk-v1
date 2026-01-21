using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents the result of a pricing simulation.
/// Contains detailed breakdown of pricing calculations and applied rules.
/// </summary>
public sealed record PricingSimulationResult(
    Money BaseCharge,
    Money FirstHourCharge,
    Money RemainingHoursCharge,
    Money MinimumChargeApplied,
    Money FinalCharge,
    TimeSpan RoundedDuration,
    IReadOnlyList<string> AppliedRules
)
{
    /// <summary>
    /// Creates a simple pricing result with just the final charge.
    /// </summary>
    /// <param name="finalCharge">The final calculated charge</param>
    /// <param name="originalDuration">The original session duration</param>
    /// <returns>New PricingSimulationResult instance</returns>
    public static PricingSimulationResult CreateSimple(Money finalCharge, TimeSpan originalDuration)
    {
        return new PricingSimulationResult(
            BaseCharge: finalCharge,
            FirstHourCharge: Money.Zero(),
            RemainingHoursCharge: Money.Zero(),
            MinimumChargeApplied: Money.Zero(),
            FinalCharge: finalCharge,
            RoundedDuration: originalDuration,
            AppliedRules: new List<string> { "Standard hourly rate" }
        );
    }

    /// <summary>
    /// Creates a detailed pricing result with full breakdown.
    /// </summary>
    /// <param name="baseCharge">Base charge before adjustments</param>
    /// <param name="firstHourCharge">Charge for the first hour</param>
    /// <param name="remainingHoursCharge">Charge for remaining hours</param>
    /// <param name="minimumChargeApplied">Minimum charge if applied</param>
    /// <param name="finalCharge">Final calculated charge</param>
    /// <param name="roundedDuration">Duration after rounding rules</param>
    /// <param name="appliedRules">List of rules that were applied</param>
    /// <returns>New PricingSimulationResult instance</returns>
    public static PricingSimulationResult CreateDetailed(
        Money baseCharge,
        Money firstHourCharge,
        Money remainingHoursCharge,
        Money minimumChargeApplied,
        Money finalCharge,
        TimeSpan roundedDuration,
        IReadOnlyList<string> appliedRules)
    {
        return new PricingSimulationResult(
            BaseCharge: baseCharge,
            FirstHourCharge: firstHourCharge,
            RemainingHoursCharge: remainingHoursCharge,
            MinimumChargeApplied: minimumChargeApplied,
            FinalCharge: finalCharge,
            RoundedDuration: roundedDuration,
            AppliedRules: appliedRules ?? new List<string>()
        );
    }

    /// <summary>
    /// Checks if minimum charge was applied in this calculation.
    /// </summary>
    /// <returns>True if minimum charge was applied</returns>
    public bool WasMinimumChargeApplied()
    {
        return MinimumChargeApplied.Amount > 0;
    }

    /// <summary>
    /// Checks if first-hour pricing was used in this calculation.
    /// </summary>
    /// <returns>True if first-hour pricing was applied</returns>
    public bool WasFirstHourPricingApplied()
    {
        return FirstHourCharge.Amount > 0;
    }

    /// <summary>
    /// Gets the effective hourly rate for this session.
    /// </summary>
    /// <returns>Effective hourly rate</returns>
    public Money GetEffectiveHourlyRate()
    {
        if (RoundedDuration.TotalHours <= 0)
        {
            return Money.Zero();
        }

        var hoursDecimal = (decimal)RoundedDuration.TotalHours;
        return FinalCharge / hoursDecimal;
    }

    /// <summary>
    /// Gets the difference between FinalCharge and BaseCharge.
    /// Represents the total value added/removed due to rounding and minimums.
    /// </summary>
    public Money RoundingAdjustment => FinalCharge - BaseCharge;
}