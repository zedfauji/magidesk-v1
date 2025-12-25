using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for complex ticket operations.
/// Stateless service that coordinates ticket business logic.
/// </summary>
public class TicketDomainService
{
    private readonly TaxDomainService _taxDomainService;

    public TicketDomainService(TaxDomainService taxDomainService)
    {
        _taxDomainService = taxDomainService ?? throw new ArgumentNullException(nameof(taxDomainService));
    }

    /// <summary>
    /// Recalculates all ticket totals using enhanced tax calculation.
    /// </summary>
    public void CalculateTotals(Ticket ticket, TaxGroup? taxGroup = null)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        // Calculate subtotal from order lines
        var subtotal = ticket.OrderLines.Aggregate(
            Money.Zero(),
            (sum, line) => sum + line.TotalAmount);

        // Calculate tax using TaxDomainService
        var taxAmount = _taxDomainService.CalculateTax(
            subtotal,
            taxGroup,
            ticket.IsTaxExempt);

        // If price includes tax, we need to adjust the calculation
        // For now, we'll use the standard calculation
        // Enhanced price-includes-tax logic will be added later

        ticket.CalculateTotalsWithTax(taxAmount);
    }

    /// <summary>
    /// Validates if a payment can be added to the ticket.
    /// </summary>
    public bool CanAddPayment(Ticket ticket, Payment payment)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (!ticket.CanAddPayment(payment))
        {
            return false;
        }

        // Validate that adding this payment won't exceed the ticket total (allowing for small rounding differences)
        var newPaidAmount = ticket.PaidAmount + payment.Amount;
        var tolerance = new Money(0.01m); // Allow 1 cent tolerance for rounding
        
        if (newPaidAmount > ticket.TotalAmount + tolerance)
        {
            throw new BusinessRuleViolationException(
                $"Payment amount ({payment.Amount}) would exceed ticket total. " +
                $"Current paid: {ticket.PaidAmount}, Total: {ticket.TotalAmount}, " +
                $"Remaining due: {ticket.DueAmount}");
        }

        return true;
    }

    /// <summary>
    /// Validates if a partial payment can be added to the ticket.
    /// </summary>
    public bool CanAddPartialPayment(Ticket ticket, Money paymentAmount)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (paymentAmount <= Money.Zero())
        {
            throw new BusinessRuleViolationException("Payment amount must be greater than zero.");
        }

        if (ticket.Status == Enumerations.TicketStatus.Closed || 
            ticket.Status == Enumerations.TicketStatus.Voided || 
            ticket.Status == Enumerations.TicketStatus.Refunded)
        {
            return false;
        }

        // Partial payments are allowed as long as they don't exceed the total
        var newPaidAmount = ticket.PaidAmount + paymentAmount;
        var tolerance = new Money(0.01m);
        
        return newPaidAmount <= ticket.TotalAmount + tolerance;
    }

    /// <summary>
    /// Validates if the ticket can be closed.
    /// </summary>
    public bool CanCloseTicket(Ticket ticket)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        return ticket.CanClose();
    }

    /// <summary>
    /// Validates if the ticket can be voided.
    /// </summary>
    public bool CanVoidTicket(Ticket ticket)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        return ticket.CanVoid();
    }

    /// <summary>
    /// Validates if the ticket can be refunded.
    /// </summary>
    public bool CanRefundTicket(Ticket ticket, Money refundAmount)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (refundAmount <= Money.Zero())
        {
            throw new BusinessRuleViolationException("Refund amount must be greater than zero.");
        }

        if (!ticket.CanRefund())
        {
            return false;
        }

        // Refund amount cannot exceed paid amount
        if (refundAmount > ticket.PaidAmount)
        {
            throw new BusinessRuleViolationException($"Refund amount ({refundAmount}) cannot exceed paid amount ({ticket.PaidAmount}).");
        }

        return true;
    }

    /// <summary>
    /// Validates if the ticket can be split.
    /// </summary>
    public bool CanSplitTicket(Ticket ticket)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        return ticket.CanSplit();
    }

    /// <summary>
    /// Gets the remaining due amount on the ticket.
    /// </summary>
    public Money GetRemainingDue(Ticket ticket)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        return ticket.GetRemainingDue();
    }

    /// <summary>
    /// Validates if the ticket can be reopened.
    /// </summary>
    public bool CanReopenTicket(Ticket ticket)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        // Only closed tickets can be reopened
        return ticket.Status == Enumerations.TicketStatus.Closed;
    }
}

