namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the time rounding rules for billing calculations.
/// </summary>
public enum TimeRoundingRule
{
    /// <summary>
    /// No rounding applied - bill exact time.
    /// </summary>
    None,

    /// <summary>
    /// Round up to the nearest 15-minute increment.
    /// </summary>
    FifteenMinutes,

    /// <summary>
    /// Round up to the nearest 30-minute increment.
    /// </summary>
    ThirtyMinutes,

    /// <summary>
    /// Round up to the nearest 60-minute increment.
    /// </summary>
    SixtyMinutes
}