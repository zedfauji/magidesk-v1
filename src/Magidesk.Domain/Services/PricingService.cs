using System;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service for calculating time-based pricing charges.
/// Stateless service that applies table type pricing rules.
/// </summary>
public class PricingService : IPricingService
{
    /// <summary>
    /// Calculates the charge for a given billable time based on table type pricing rules.
    /// </summary>
    /// <param name="billableTime">The billable time duration.</param>
    /// <param name="tableType">The table type with pricing configuration.</param>
    /// <returns>The calculated charge as Money.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tableType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when billableTime is negative.</exception>
    public Money CalculateTimeCharge(
        TimeSpan billableTime,
        TableType tableType)
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
            return new Money(0m);
        }

        // Step 1: Round time per tableType.RoundingMinutes
        var roundedMinutes = RoundTime(billableTime, tableType.RoundingMinutes);

        // Step 2: Apply minimum if < tableType.MinimumMinutes
        if (roundedMinutes < tableType.MinimumMinutes)
        {
            roundedMinutes = tableType.MinimumMinutes;
        }

        // Step 3 & 4: Calculate charge based on first-hour rate and standard hourly rate
        var totalCharge = CalculateChargeForTime(roundedMinutes, tableType);

        // Step 5: Return total charge
        // Note: Member discount support will be added when Member entity is available (BE-F.3-01)
        return new Money(totalCharge);
    }

    /// <summary>
    /// Rounds time up to the nearest rounding interval.
    /// </summary>
    /// <param name="time">The time to round.</param>
    /// <param name="roundingMinutes">The rounding interval in minutes.</param>
    /// <returns>Rounded time in minutes.</returns>
    private int RoundTime(TimeSpan time, int roundingMinutes)
    {
        var totalMinutes = (int)Math.Ceiling(time.TotalMinutes);
        
        // If rounding is 1 minute, no rounding needed
        if (roundingMinutes <= 1)
        {
            return totalMinutes;
        }

        // Round up to nearest interval
        // Example: 62 minutes with 15-minute rounding = 75 minutes (5 intervals)
        var intervals = (int)Math.Ceiling((double)totalMinutes / roundingMinutes);
        return intervals * roundingMinutes;
    }

    /// <summary>
    /// Calculates the charge for the given time based on first-hour and standard hourly rates.
    /// </summary>
    /// <param name="totalMinutes">Total billable minutes (after rounding and minimum).</param>
    /// <param name="tableType">The table type with pricing configuration.</param>
    /// <returns>Total charge amount.</returns>
    private decimal CalculateChargeForTime(int totalMinutes, TableType tableType)
    {
        decimal totalCharge = 0m;
        int remainingMinutes = totalMinutes;

        // Apply first-hour rate if configured and time >= 1 hour
        if (tableType.FirstHourRate.HasValue && totalMinutes >= 60)
        {
            // Charge first hour at premium rate
            totalCharge += tableType.FirstHourRate.Value;
            remainingMinutes -= 60;
        }

        // Calculate remaining time at standard hourly rate
        if (remainingMinutes > 0)
        {
            // Convert minutes to fractional hours
            var remainingHours = remainingMinutes / 60.0m;
            totalCharge += remainingHours * tableType.HourlyRate;
        }

        return totalCharge;
    }
}
