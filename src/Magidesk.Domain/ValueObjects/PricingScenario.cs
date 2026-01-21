using System;
using Magidesk.Domain.Entities;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a pricing scenario for simulation and testing.
/// Immutable value object containing all parameters needed for pricing calculation.
/// </summary>
public sealed record PricingScenario(
    TimeSpan Duration,
    TableType TableType,
    int GuestCount,
    DateTime StartTime,
    bool HasMemberDiscount = false
)
{
    /// <summary>
    /// Creates a basic pricing scenario with minimal parameters.
    /// </summary>
    /// <param name="duration">Session duration</param>
    /// <param name="tableType">Table type with pricing rules</param>
    /// <param name="guestCount">Number of guests</param>
    /// <returns>New PricingScenario instance</returns>
    public static PricingScenario CreateBasic(TimeSpan duration, TableType tableType, int guestCount)
    {
        return new PricingScenario(
            Duration: duration,
            TableType: tableType,
            GuestCount: guestCount,
            StartTime: DateTime.UtcNow
        );
    }

    /// <summary>
    /// Creates a pricing scenario with member discount.
    /// </summary>
    /// <param name="duration">Session duration</param>
    /// <param name="tableType">Table type with pricing rules</param>
    /// <param name="guestCount">Number of guests</param>
    /// <param name="startTime">Session start time</param>
    /// <returns>New PricingScenario instance with member discount</returns>
    public static PricingScenario CreateWithMemberDiscount(
        TimeSpan duration, 
        TableType tableType, 
        int guestCount, 
        DateTime startTime)
    {
        return new PricingScenario(
            Duration: duration,
            TableType: tableType,
            GuestCount: guestCount,
            StartTime: startTime,
            HasMemberDiscount: true
        );
    }
}