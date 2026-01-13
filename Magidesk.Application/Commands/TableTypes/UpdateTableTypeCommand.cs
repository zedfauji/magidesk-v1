using System;

namespace Magidesk.Application.Commands.TableTypes;

/// <summary>
/// Command to update table type configuration.
/// </summary>
public record UpdateTableTypeCommand(
    Guid TableTypeId,
    string Name,
    string Description,
    decimal HourlyRate,
    decimal? FirstHourRate,
    decimal MinimumCharge,
    TimeRoundingRule TimeRoundingRule
);

/// <summary>
/// Result of updating table type.
/// </summary>
public record UpdateTableTypeResult(
    Guid TableTypeId,
    string Name,
    DateTime UpdatedAt
);

/// <summary>
/// Time rounding rules for pricing calculations.
/// </summary>
public enum TimeRoundingRule
{
    FifteenMinutes,
    ThirtyMinutes,
    SixtyMinutes
}