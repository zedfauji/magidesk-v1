using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Services;

/// <summary>
/// Advanced domain service for calculating time-based pricing charges with enhanced features.
/// Extends the basic IPricingService with advanced pricing rules, simulation, and validation.
/// </summary>
public interface IAdvancedPricingService : IPricingService
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
    Task<Money> CalculateFirstHourPricingAsync(TimeSpan billableTime, TableType tableType);

    /// <summary>
    /// Applies time rounding rules to a duration based on the specified rounding rule.
    /// Rounds up to the nearest increment (15, 30, or 60 minutes).
    /// </summary>
    /// <param name="duration">The original duration to round.</param>
    /// <param name="rule">The rounding rule to apply.</param>
    /// <returns>The rounded duration.</returns>
    /// <exception cref="ArgumentException">Thrown when duration is negative.</exception>
    Task<TimeSpan> ApplyTimeRoundingAsync(TimeSpan duration, TimeRoundingRule rule);

    /// <summary>
    /// Applies minimum charge enforcement to a calculated charge.
    /// Ensures the charge meets the minimum charge requirement for the table type.
    /// </summary>
    /// <param name="calculatedCharge">The calculated charge before minimum enforcement.</param>
    /// <param name="tableType">The table type with minimum charge configuration.</param>
    /// <returns>The charge after minimum charge enforcement.</returns>
    /// <exception cref="ArgumentNullException">Thrown when calculatedCharge or tableType is null.</exception>
    Task<Money> ApplyMinimumChargeAsync(Money calculatedCharge, TableType tableType);

    /// <summary>
    /// Simulates pricing calculations for a given scenario with detailed breakdown.
    /// Provides comprehensive information about how charges are calculated.
    /// </summary>
    /// <param name="scenario">The pricing scenario to simulate.</param>
    /// <returns>Detailed simulation result with charge breakdown and applied rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown when scenario is null.</exception>
    Task<PricingSimulationResult> SimulatePricingAsync(PricingScenario scenario);

    /// <summary>
    /// Validates that pricing rules for a table type are mathematically consistent.
    /// Checks for conflicts and logical inconsistencies in pricing configuration.
    /// </summary>
    /// <param name="tableType">The table type to validate.</param>
    /// <returns>True if pricing rules are valid and consistent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tableType is null.</exception>
    Task<bool> ValidatePricingRulesAsync(TableType tableType);
}