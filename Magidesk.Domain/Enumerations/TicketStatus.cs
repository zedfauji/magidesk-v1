namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Represents the status of a ticket.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket is being created, no items yet.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Ticket has items and can be modified.
    /// </summary>
    Open = 1,

    /// <summary>
    /// All payments received (PaidAmount >= TotalAmount).
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Ticket is finalized and settled.
    /// </summary>
    Closed = 3,

    /// <summary>
    /// Ticket was cancelled before payment.
    /// </summary>
    Voided = 4,

    /// <summary>
    /// Closed ticket that was refunded.
    /// </summary>
    Refunded = 5,

    /// <summary>
    /// Ticket is scheduled for future delivery/pickup.
    /// </summary>
    Scheduled = 6
}

