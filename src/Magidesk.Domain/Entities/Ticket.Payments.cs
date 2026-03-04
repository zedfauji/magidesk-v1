using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing payment management methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Adds a payment to the ticket.
    /// </summary>
    public void AddPayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot add payment to ticket in {Status} status.");
        }

        if (payment.TicketId != Id)
        {
            throw new BusinessRuleViolationException("Payment does not belong to this ticket.");
        }

        _payments.Add(payment);
        ActiveDate = DateTime.UtcNow;

        // Auto-open if still in Draft
        if (Status == TicketStatus.Draft)
        {
            Open();
        }

        RecalculatePaidAmount();

        // Keep DueAmount consistent when payments are added.
        // Money subtraction cannot go negative, so clamp at zero when fully paid (or slightly overpaid).
        DueAmount = PaidAmount >= TotalAmount
            ? Money.Zero(TotalAmount.Currency)
            : TotalAmount - PaidAmount;

        // Auto-transition to Paid if fully paid
        if (PaidAmount >= TotalAmount && Status == TicketStatus.Open)
        {
            Status = TicketStatus.Paid;
        }
    }

    /// <summary>
    /// Validates if a payment can be added to the ticket.
    /// </summary>
    public bool CanAddPayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            return false;
        }

        if (payment.TicketId != Id)
        {
            return false;
        }

        return true;
    }
}
