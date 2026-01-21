using System;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for table type information.
/// </summary>
public class TableTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal? FirstHourRate { get; set; }
    public decimal MinimumCharge { get; set; }
    public TimeRoundingRule TimeRoundingRule { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string RoundingRuleDisplay => TimeRoundingRule switch
    {
        TimeRoundingRule.FifteenMinutes => "15 minutes",
        TimeRoundingRule.ThirtyMinutes => "30 minutes",
        TimeRoundingRule.SixtyMinutes => "60 minutes",
        _ => "Unknown"
    };

    public bool HasFirstHourPricing => FirstHourRate.HasValue && FirstHourRate.Value > 0;

    public string PricingDisplay
    {
        get
        {
            var display = $"${HourlyRate:F2}/hour";
            if (HasFirstHourPricing)
            {
                display += $" (First hour: ${FirstHourRate:F2})";
            }
            if (MinimumCharge > 0)
            {
                display += $" (Min: ${MinimumCharge:F2})";
            }
            return display;
        }
    }
}

/// <summary>
/// Time rounding rules for pricing calculations.
/// </summary>
public enum TimeRoundingRule
{
    FifteenMinutes,
    ThirtyMinutes,
    SixtyMinutes
}