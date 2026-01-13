namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the type of manager override operation.
/// </summary>
public enum OverrideType
{
    /// <summary>
    /// Time adjustment override (adding or subtracting time).
    /// </summary>
    TimeAdjustment,

    /// <summary>
    /// Pricing override (changing the calculated charge).
    /// </summary>
    PricingOverride,

    /// <summary>
    /// Force end session override (ending session regardless of state).
    /// </summary>
    ForceEndSession,

    /// <summary>
    /// Guest count override (changing guest count during session).
    /// </summary>
    GuestCountOverride,

    /// <summary>
    /// Rate override (changing the hourly rate for the session).
    /// </summary>
    RateOverride
}