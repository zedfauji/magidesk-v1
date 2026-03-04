using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing hold and release operations.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Holds the ticket for later payment.
    /// Transitions ticket to Held status and releases the table.
    /// </summary>
    /// <param name="reason">Reason for holding (e.g., "Customer tab", "Charge to room")</param>
    /// <param name="userId">User performing the hold operation</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket cannot be held</exception>
    /// <exception cref="ArgumentException">Thrown if reason is empty</exception>
    public void Hold(string reason, UserId userId)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new DomainInvalidOperationException("Cannot hold a closed ticket.");
        }

        if (Status == TicketStatus.Voided)
        {
            throw new DomainInvalidOperationException("Cannot hold a voided ticket.");
        }

        if (Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException("Cannot hold a refunded ticket.");
        }

        if (Status == TicketStatus.Held)
        {
            throw new DomainInvalidOperationException("Ticket is already held.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Hold reason is required.", nameof(reason));
        }

        if (userId == null)
        {
            throw new ArgumentNullException(nameof(userId));
        }

        Status = TicketStatus.Held;
        HeldAt = DateTime.UtcNow;
        HoldReason = reason;
        HeldBy = userId;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Releases a held ticket back to open status for payment processing.
    /// </summary>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket is not held</exception>
    public void Release()
    {
        if (Status != TicketStatus.Held)
        {
            throw new DomainInvalidOperationException("Only held tickets can be released.");
        }

        Status = TicketStatus.Open;
        ActiveDate = DateTime.UtcNow;
    }
}
