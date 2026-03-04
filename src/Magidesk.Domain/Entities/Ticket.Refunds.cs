using System;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing refund management methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Refunds the ticket (full or partial refund).
    /// REQ-5.4, REQ-5.5, REQ-5.6, REQ-5.9: Refund requires authorization, validates amount, updates payments.
    /// </summary>
    /// <param name="amount">Amount to refund (must be <= PaidAmount)</param>
    /// <param name="reason">Reason for the refund</param>
    /// <param name="refundedBy">User processing the refund (requires manager authorization)</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket cannot be refunded</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown if refund amount exceeds paid amount</exception>
    /// <exception cref="ArgumentException">Thrown if reason is empty</exception>
    public void Refund(Money amount, string reason, UserId refundedBy)
    {
        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Refund reason is required.", nameof(reason));
        }

        if (refundedBy == null)
        {
            throw new ArgumentNullException(nameof(refundedBy));
        }

        // Only Paid or Closed tickets can be refunded
        if (Status != TicketStatus.Paid && Status != TicketStatus.Closed)
        {
            throw new DomainInvalidOperationException($"Cannot refund ticket in {Status} status. Only Paid or Closed tickets can be refunded.");
        }

        // REQ-5.9: Validate refund amount doesn't exceed paid amount
        if (amount > PaidAmount)
        {
            throw new BusinessRuleViolationException($"Refund amount {amount} exceeds paid amount {PaidAmount}.");
        }

        // REQ-5.5: Update RefundedAmount on payments
        // Distribute refund across payments proportionally
        var remainingRefund = amount;
        var paymentsToRefund = _payments
            .Where(p => p.TransactionType == TransactionType.Credit && !p.IsVoided)
            .OrderBy(p => p.TransactionTime)
            .ToList();

        foreach (var payment in paymentsToRefund)
        {
            if (remainingRefund <= Money.Zero())
                break;

            var availableToRefund = payment.Amount - payment.RefundedAmount;
            if (availableToRefund <= Money.Zero())
                continue;

            var refundForThisPayment = remainingRefund <= availableToRefund
                ? remainingRefund
                : availableToRefund;

            payment.AddRefund(refundForThisPayment);
            remainingRefund = remainingRefund - refundForThisPayment;
        }

        _properties["RefundReason"] = reason;
        _properties["RefundedBy"] = refundedBy.Value.ToString();
        _properties["RefundedAt"] = DateTime.UtcNow.ToString("O");
        ActiveDate = DateTime.UtcNow;

        // Recalculate paid amount after refunds
        RecalculatePaidAmount();

        // REQ-5.4: If full refund (PaidAmount <= 0), change status to Refunded
        if (PaidAmount <= Money.Zero())
        {
            Status = TicketStatus.Refunded;
        }

        // Recalculate due amount
        DueAmount = PaidAmount >= TotalAmount
            ? Money.Zero(TotalAmount.Currency)
            : TotalAmount - PaidAmount;

        // NOTE: Domain events are handled at the application layer via audit events.
        // See RefundTicketCommandHandler for audit event creation.
        // Domain event: TicketRefunded(Id, amount, reason, refundedBy, isPartial)
        // where isPartial = (PaidAmount > Money.Zero())
    }

    /// <summary>
    /// Validates if the ticket can be refunded.
    /// </summary>
    public bool CanRefund()
    {
        // Only Paid or Closed tickets can be refunded
        return Status == TicketStatus.Paid || Status == TicketStatus.Closed;
    }

    /// <summary>
    /// Processes a refund on the ticket (legacy method - use Refund instead).
    /// Adds a refund payment (TransactionType.Debit) and updates ticket status.
    /// </summary>
    [Obsolete("Use Refund(Money amount, string reason, UserId refundedBy) instead")]
    public void ProcessRefund(Payment refundPayment)
    {
        if (refundPayment == null)
        {
            throw new ArgumentNullException(nameof(refundPayment));
        }

        if (!CanRefund())
        {
            throw new DomainInvalidOperationException($"Cannot refund ticket in {Status} status.");
        }

        if (refundPayment.TicketId != Id)
        {
            throw new BusinessRuleViolationException("Refund payment does not belong to this ticket.");
        }

        if (refundPayment.TransactionType != TransactionType.Debit)
        {
            throw new BusinessRuleViolationException("Refund payment must have TransactionType.Debit.");
        }

        // Add refund payment (debit transaction)
        _payments.Add(refundPayment);
        ActiveDate = DateTime.UtcNow;
        RecalculatePaidAmount();

        // Recalculate due (refunds increase due again if ticket is not fully refunded)
        DueAmount = PaidAmount >= TotalAmount
            ? Money.Zero(TotalAmount.Currency)
            : TotalAmount - PaidAmount;

        // If fully refunded (PaidAmount <= 0), mark as refunded
        if (PaidAmount <= Money.Zero())
        {
            Status = TicketStatus.Refunded;
        }
    }
}
