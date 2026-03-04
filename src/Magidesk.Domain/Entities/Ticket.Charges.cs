using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing gratuity and charge management methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Adds or updates gratuity on the ticket.
    /// </summary>
    public void AddGratuity(Gratuity gratuity)
    {
        if (gratuity == null)
        {
            throw new ArgumentNullException(nameof(gratuity));
        }

        if (gratuity.TicketId != Id)
        {
            throw new BusinessRuleViolationException("Gratuity does not belong to this ticket.");
        }

        Gratuity = gratuity;
        // NOTE: We do NOT call IncrementVersion() here!
        // EF Core automatically manages the Version concurrency token.
        // Manually incrementing it before SaveChanges breaks the WHERE clause
        // in the UPDATE statement, causing "0 rows affected" exception.
        // EF will auto-increment Version when SaveChanges succeeds.
        CalculateTotals();
    }

    /// <summary>
    /// Marks gratuity as paid.
    /// </summary>
    public void MarkGratuityAsPaid()
    {
        if (Gratuity == null)
        {
            throw new DomainInvalidOperationException("No gratuity to mark as paid.");
        }

        Gratuity.MarkAsPaid();
        CalculateTotals();
    }

    /// <summary>
    /// Marks gratuity as refunded.
    /// </summary>
    public void MarkGratuityAsRefunded()
    {
        if (Gratuity == null)
        {
            throw new DomainInvalidOperationException("No gratuity to mark as refunded.");
        }

        Gratuity.MarkAsRefunded();
        CalculateTotals();
    }

    /// <summary>
    /// Sets the service charge amount.
    /// Service charge is typically calculated as a percentage of subtotal (after discounts).
    /// </summary>
    public void SetServiceCharge(Money amount)
    {
        if (amount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Service charge cannot be negative.");
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot modify service charge on ticket in {Status} status.");
        }

        ServiceChargeAmount = amount;
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }

    /// <summary>
    /// Sets the delivery charge amount.
    /// </summary>
    public void SetDeliveryCharge(Money amount)
    {
        if (amount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Delivery charge cannot be negative.");
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot modify delivery charge on ticket in {Status} status.");
        }

        DeliveryChargeAmount = amount;
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }

    /// <summary>
    /// Sets the adjustment amount (positive only - for price increases).
    /// Used for manual price adjustments, rounding, or corrections.
    /// Note: For price reductions, use discounts instead.
    /// </summary>
    public void SetAdjustment(Money amount)
    {
        if (amount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Adjustment amount cannot be negative. Use discounts for price reductions.");
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot modify adjustment on ticket in {Status} status.");
        }

        AdjustmentAmount = amount;
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }

    /// <summary>
    /// Sets the tax exempt status of the ticket.
    /// </summary>
    public void SetTaxExempt(bool isTaxExempt)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot modify tax exempt status on ticket in {Status} status.");
        }

        IsTaxExempt = isTaxExempt;
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }

    /// <summary>
    /// Sets the advance payment amount (prepayment before order completion).
    /// </summary>
    public void SetAdvancePayment(Money amount)
    {
        if (amount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Advance payment cannot be negative.");
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot modify advance payment on ticket in {Status} status.");
        }

        AdvanceAmount = amount;
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }
}
