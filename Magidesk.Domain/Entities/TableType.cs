using System;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a table type category (e.g., Pool Table, Snooker Table, Billiards Table).
/// Defines pricing rules for time-based billing.
/// </summary>
public class TableType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal HourlyRate { get; private set; }
    public decimal? FirstHourRate { get; private set; }
    public int MinimumMinutes { get; private set; }
    public int RoundingMinutes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private TableType()
    {
    }

    /// <summary>
    /// Creates a new table type.
    /// </summary>
    /// <param name="name">Name of the table type (e.g., "Pool Table")</param>
    /// <param name="hourlyRate">Standard hourly rate</param>
    /// <param name="description">Optional description</param>
    /// <returns>New TableType instance</returns>
    /// <exception cref="ArgumentException">Thrown when name is empty or hourlyRate is invalid</exception>
    public static TableType Create(
        string name,
        decimal hourlyRate,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Table type name cannot be empty.", nameof(name));
        }

        if (hourlyRate <= 0)
        {
            throw new ArgumentException("Hourly rate must be greater than zero.", nameof(hourlyRate));
        }

        var now = DateTime.UtcNow;

        return new TableType
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            HourlyRate = hourlyRate,
            FirstHourRate = null,
            MinimumMinutes = 0,
            RoundingMinutes = 1, // Default: no rounding
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Updates the hourly rates for this table type.
    /// </summary>
    /// <param name="hourlyRate">Standard hourly rate</param>
    /// <param name="firstHourRate">Optional first-hour rate (null to use standard rate)</param>
    /// <exception cref="ArgumentException">Thrown when rates are invalid</exception>
    public void UpdateRates(decimal hourlyRate, decimal? firstHourRate = null)
    {
        if (hourlyRate <= 0)
        {
            throw new ArgumentException("Hourly rate must be greater than zero.", nameof(hourlyRate));
        }

        if (firstHourRate.HasValue && firstHourRate.Value <= 0)
        {
            throw new ArgumentException("First hour rate must be greater than zero if specified.", nameof(firstHourRate));
        }

        HourlyRate = hourlyRate;
        FirstHourRate = firstHourRate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the time rounding rules for billing calculations.
    /// </summary>
    /// <param name="minimumMinutes">Minimum billable time in minutes (0 = no minimum)</param>
    /// <param name="roundingMinutes">Round time to nearest X minutes (1 = no rounding)</param>
    /// <exception cref="ArgumentException">Thrown when rounding values are invalid</exception>
    public void SetRounding(int minimumMinutes, int roundingMinutes)
    {
        if (minimumMinutes < 0)
        {
            throw new ArgumentException("Minimum minutes cannot be negative.", nameof(minimumMinutes));
        }

        if (roundingMinutes < 1)
        {
            throw new ArgumentException("Rounding minutes must be at least 1.", nameof(roundingMinutes));
        }

        MinimumMinutes = minimumMinutes;
        RoundingMinutes = roundingMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the table type name and description.
    /// </summary>
    /// <param name="name">New name</param>
    /// <param name="description">New description</param>
    /// <exception cref="ArgumentException">Thrown when name is empty</exception>
    public void UpdateDetails(string name, string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Table type name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates this table type (prevents new sessions from using it).
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates this table type.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
