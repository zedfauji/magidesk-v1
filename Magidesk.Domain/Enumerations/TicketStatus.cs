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
    /// Ticket is held for later payment (tab/deferred payment).
    /// Table is released but ticket remains unpaid.
    /// </summary>
    Held = 2,

    /// <summary>
    /// All payments received (PaidAmount >= TotalAmount).
    /// </summary>
    Paid = 3,

    /// <summary>
    /// Ticket is finalized and settled.
    /// </summary>
    Closed = 4,

    /// <summary>
    /// Ticket was cancelled before payment.
    /// </summary>
    Voided = 5,

    /// <summary>
    /// Closed ticket that was refunded.
    /// </summary>
    Refunded = 6,

    /// <summary>
    /// Ticket is scheduled for future delivery/pickup.
    /// </summary>
    Scheduled = 7
}

