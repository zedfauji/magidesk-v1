using System;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a work shift (e.g., Morning, Afternoon, Evening, Night).
/// Reference data entity for organizing tickets and cash sessions by time periods.
/// </summary>
public class Shift
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public bool IsActive { get; private set; }
    public int Version { get; private set; }

    // Private constructor for EF Core
    private Shift()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Creates a new shift.
    /// </summary>
    public static Shift Create(
        string name,
        TimeSpan startTime,
        TimeSpan endTime,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Shift name cannot be empty.");
        }

        if (startTime == endTime)
        {
            throw new Exceptions.BusinessRuleViolationException("Shift start time and end time cannot be the same.");
        }

        return new Shift
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = isActive,
            Version = 1
        };
    }

    /// <summary>
    /// Updates the shift name.
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exceptions.BusinessRuleViolationException("Shift name cannot be empty.");
        }

        Name = name;
    }

    /// <summary>
    /// Updates the shift times.
    /// </summary>
    public void UpdateTimes(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime == endTime)
        {
            throw new Exceptions.BusinessRuleViolationException("Shift start time and end time cannot be the same.");
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Activates the shift.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the shift.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Checks if a given time falls within this shift's time range.
    /// Handles shifts that span midnight (e.g., 22:00 - 06:00).
    /// </summary>
    public bool IsTimeInShift(TimeSpan time)
    {
        if (StartTime < EndTime)
        {
            // Normal shift (doesn't span midnight)
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            // Shift spans midnight (e.g., 22:00 - 06:00)
            return time >= StartTime || time <= EndTime;
        }
    }

    /// <summary>
    /// Gets the current shift based on current time.
    /// </summary>
    public static Shift? GetCurrentShift(IEnumerable<Shift> shifts)
    {
        var currentTime = DateTime.Now.TimeOfDay;
        return shifts.FirstOrDefault(s => s.IsActive && s.IsTimeInShift(currentTime));
    }
}

