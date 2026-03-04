using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing core status transition methods (Close, Void, Reopen).
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Closes the ticket (finalizes it).
    /// </summary>
    public void Close(UserId closedBy)
    {
        if (!CanClose())
        {
            throw new DomainInvalidOperationException($"Cannot close ticket in {Status} status.");
        }

        Status = TicketStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = closedBy;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates if the ticket can be closed.
    /// </summary>
    public bool CanClose()
    {
        // Allow closing from Paid status (normal flow) or Open status (when balance is zero)
        // This supports Held → Open → Closed transition when settling
        if (Status != TicketStatus.Paid && Status != TicketStatus.Open)
        {
            return false;
        }

        if (DueAmount > Money.Zero())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Voids the ticket (cancels it before payment).
    /// REQ-5.1, REQ-5.2, REQ-5.3: Void requires authorization and reason, only for Open tickets.
    /// </summary>
    /// <param name="reason">Reason for voiding the ticket</param>
    /// <param name="voidedBy">User who is voiding the ticket (requires manager authorization)</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket cannot be voided</exception>
    /// <exception cref="ArgumentException">Thrown if reason is empty</exception>
    public void Void(string reason, UserId voidedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Void reason is required.", nameof(reason));
        }

        if (voidedBy == null)
        {
            throw new ArgumentNullException(nameof(voidedBy));
        }

        // REQ-5.1: Only Open tickets can be voided
        if (Status != TicketStatus.Open && Status != TicketStatus.Draft && Status != TicketStatus.Held)
        {
            throw new DomainInvalidOperationException($"Cannot void ticket in {Status} status.");
        }

        // REQ-5.3: Cannot void if ticket has been paid (suggest refund instead)
        if (Status == TicketStatus.Paid || PaidAmount > Money.Zero())
        {
            throw new DomainInvalidOperationException("Cannot void a paid ticket. Use refund instead.");
        }

        Status = TicketStatus.Voided;
        VoidedBy = voidedBy;
        _properties["VoidReason"] = reason;
        ActiveDate = DateTime.UtcNow;

        // NOTE: Domain events are handled at the application layer via audit events.
        // See VoidTicketCommandHandler for audit event creation.
        // Domain event: TicketVoided(Id, reason, voidedBy)
    }

    /// <summary>
    /// Validates if the ticket can be voided.
    /// </summary>
    public bool CanVoid()
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Refunded)
        {
            return false;
        }

        // Cannot void if there are payments (must refund instead)
        if (_payments.Any(p => !p.IsVoided))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates if the ticket can be split.
    /// </summary>
    public bool CanSplit()
    {
        // Can only split open tickets
        return Status == TicketStatus.Open;
    }

    /// <summary>
    /// Gets the remaining due amount on the ticket.
    /// </summary>
    public Money GetRemainingDue()
    {
        return TotalAmount - PaidAmount;
    }

    /// <summary>
    /// Reopens a closed ticket.
    /// </summary>
    public void Reopen()
    {
        if (Status != TicketStatus.Closed)
        {
            throw new DomainInvalidOperationException($"Cannot reopen ticket in {Status} status.");
        }

        Status = TicketStatus.Open;
        IsReOpened = true;
        ClosedAt = null;
        ClosedBy = null;
        ActiveDate = DateTime.UtcNow;
    }
}
