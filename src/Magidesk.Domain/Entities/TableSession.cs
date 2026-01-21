using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a table session for time-based billing.
/// Tracks table usage time with support for pausing and billing integration.
/// </summary>
public class TableSession
{
    public Guid Id { get; private set; }
    public Guid TableId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? TicketId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public TimeSpan TotalPausedDuration { get; private set; }
    public TableSessionStatus Status { get; private set; }
    public Guid TableTypeId { get; private set; }
    public decimal HourlyRate { get; private set; }
    public Money TotalCharge { get; private set; } = Money.Zero();
    public int GuestCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private TableSession()
    {
    }

    /// <summary>
    /// Starts a new table session.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="tableTypeId">ID of the table type</param>
    /// <param name="hourlyRate">Hourly rate for billing</param>
    /// <param name="guestCount">Number of guests</param>
    /// <param name="customerId">Optional customer ID</param>
    /// <param name="ticketId">Optional ticket ID to link session to ticket</param>
    /// <returns>New TableSession instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public static TableSession Start(
        Guid tableId,
        Guid tableTypeId,
        decimal hourlyRate,
        int guestCount,
        Guid? customerId = null,
        Guid? ticketId = null)
    {
        if (tableId == Guid.Empty)
        {
            throw new ArgumentException("Table ID cannot be empty.", nameof(tableId));
        }

        if (tableTypeId == Guid.Empty)
        {
            throw new ArgumentException("Table type ID cannot be empty.", nameof(tableTypeId));
        }

        if (hourlyRate <= 0)
        {
            throw new ArgumentException("Hourly rate must be greater than zero.", nameof(hourlyRate));
        }

        if (guestCount <= 0)
        {
            throw new ArgumentException("Guest count must be greater than zero.", nameof(guestCount));
        }

        var now = DateTime.UtcNow;

        return new TableSession
        {
            Id = Guid.NewGuid(),
            TableId = tableId,
            TableTypeId = tableTypeId,
            HourlyRate = hourlyRate,
            GuestCount = guestCount,
            CustomerId = customerId,
            TicketId = ticketId,
            StartTime = now,
            Status = TableSessionStatus.Active,
            TotalPausedDuration = TimeSpan.Zero,
            TotalCharge = Money.Zero(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Pauses the session (stops time tracking).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when session cannot be paused</exception>
    public void Pause()
    {
        if (Status == TableSessionStatus.Ended)
        {
            throw new InvalidOperationException("Cannot pause an ended session.");
        }

        if (Status == TableSessionStatus.Paused)
        {
            throw new InvalidOperationException("Session is already paused.");
        }

        PausedAt = DateTime.UtcNow;
        Status = TableSessionStatus.Paused;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Resumes the session (restarts time tracking).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when session cannot be resumed</exception>
    public void Resume()
    {
        if (Status != TableSessionStatus.Paused)
        {
            throw new InvalidOperationException("Can only resume a paused session.");
        }

        if (!PausedAt.HasValue)
        {
            throw new InvalidOperationException("Session has no pause time recorded.");
        }

        // Add the pause duration to total
        var pauseDuration = DateTime.UtcNow - PausedAt.Value;
        TotalPausedDuration += pauseDuration;

        PausedAt = null;
        Status = TableSessionStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ends the session and records the final charge.
    /// </summary>
    /// <param name="calculatedCharge">The calculated total charge</param>
    /// <exception cref="InvalidOperationException">Thrown when session cannot be ended</exception>
    /// <exception cref="ArgumentNullException">Thrown when charge is null</exception>
    public void End(Money calculatedCharge)
    {
        if (calculatedCharge == null)
        {
            throw new ArgumentNullException(nameof(calculatedCharge));
        }

        if (Status == TableSessionStatus.Ended)
        {
            throw new InvalidOperationException("Session is already ended.");
        }

        if (Status == TableSessionStatus.Paused)
        {
            throw new InvalidOperationException("Cannot end a paused session. Resume it first.");
        }

        EndTime = DateTime.UtcNow;
        TotalCharge = calculatedCharge;
        Status = TableSessionStatus.Ended;
        UpdatedAt = DateTime.UtcNow;
    }

    public TimeSpan ManualAdjustment { get; private set; }

    /// <summary>
    /// Adjusts the session time by a specific amount.
    /// </summary>
    /// <param name="adjustment">Positive to add time, negative to subtract time.</param>
    public void AdjustTime(TimeSpan adjustment)
    {
        if (Status == TableSessionStatus.Ended)
        {
            throw new InvalidOperationException("Cannot adjust time of an ended session.");
        }
        
        ManualAdjustment += adjustment;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the billable time (total time minus paused duration + manual adjustment).
    /// </summary>
    /// <returns>Billable time span</returns>
    public TimeSpan GetBillableTime()
    {
        var endTime = EndTime ?? DateTime.UtcNow;
        var totalTime = endTime - StartTime;
        
        // If currently paused, add current pause duration
        var currentPauseDuration = TimeSpan.Zero;
        if (Status == TableSessionStatus.Paused && PausedAt.HasValue)
        {
            currentPauseDuration = DateTime.UtcNow - PausedAt.Value;
        }

        var billableTime = totalTime - TotalPausedDuration - currentPauseDuration + ManualAdjustment;

        // Ensure non-negative
        return billableTime < TimeSpan.Zero ? TimeSpan.Zero : billableTime;
    }

    /// <summary>
    /// Links this session to a ticket.
    /// </summary>
    /// <param name="ticketId">ID of the ticket</param>
    public void LinkToTicket(Guid ticketId)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException("Ticket ID cannot be empty.", nameof(ticketId));
        }

        TicketId = ticketId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the guest count.
    /// </summary>
    /// <param name="guestCount">New guest count</param>
    public void UpdateGuestCount(int guestCount)
    {
        if (guestCount <= 0)
        {
            throw new ArgumentException("Guest count must be greater than zero.", nameof(guestCount));
        }

        if (Status == TableSessionStatus.Ended)
        {
            throw new InvalidOperationException("Cannot update guest count on an ended session.");
        }

        GuestCount = guestCount;
        UpdatedAt = DateTime.UtcNow;
    }
}
