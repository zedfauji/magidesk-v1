using System;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Defines a time window during which a specific discount is automatically applicable (e.g., Happy Hour).
/// </summary>
public class PromotionSchedule
{
    public Guid Id { get; private set; }
    public Guid DiscountId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public virtual Discount Discount { get; private set; }

    // Private constructor for EF Core
    private PromotionSchedule() { }

    /// <summary>
    /// Creates a new promotion schedule.
    /// </summary>
    public static PromotionSchedule Create(
        Guid discountId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        if (discountId == Guid.Empty)
        {
            throw new ArgumentException("Discount ID cannot be empty.", nameof(discountId));
        }

        if (endTime <= startTime)
        {
            throw new BusinessRuleViolationException("End time must be after start time.");
        }

        return new PromotionSchedule
        {
            Id = Guid.NewGuid(),
            DiscountId = discountId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = true
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Checks if the provided time falls within this schedule's window.
    /// </summary>
    public bool IsApplicable(DateTime dateTime)
    {
        if (!IsActive) return false;
        
        if (dateTime.DayOfWeek != DayOfWeek) return false;

        var timeOfDay = dateTime.TimeOfDay;
        return timeOfDay >= StartTime && timeOfDay <= EndTime;
    }
}
