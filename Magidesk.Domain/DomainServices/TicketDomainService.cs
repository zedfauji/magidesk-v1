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

        Money taxAmount;

        if (ticket.PriceIncludesTax)
        {
            // When price includes tax, we need to extract the tax from the subtotal
            // Formula: baseAmount = totalAmount / (1 + taxRate)
            // Then: taxAmount = totalAmount - baseAmount
            var baseAmount = _taxDomainService.CalculateBaseAmountFromInclusivePrice(subtotal, taxGroup);
            taxAmount = subtotal - baseAmount;
        }
        else
        {
            // Standard calculation: tax is added to subtotal
            taxAmount = _taxDomainService.CalculateTax(
                subtotal,
                taxGroup,
                ticket.IsTaxExempt);
        }

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
        // Exception: Cash payments can exceed total (change due)
        // F-0007: Enforce non-cash overpayment rules
        if (payment.PaymentType != Enumerations.PaymentType.Cash)
        {
            var newPaidAmount = ticket.PaidAmount + payment.Amount;
            var tolerance = new Money(0.01m); // Allow 1 cent tolerance for rounding
            
            if (newPaidAmount > ticket.TotalAmount + tolerance)
            {
                throw new BusinessRuleViolationException(
                    $"Payment amount ({payment.Amount}) would exceed ticket total. " +
                    $"Current paid: {ticket.PaidAmount}, Total: {ticket.TotalAmount}, " +
                    $"Remaining due: {ticket.DueAmount}");
            }
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
    /// <summary>
    /// Validates if a coupon can be applied to the ticket.
    /// </summary>
    public void ValidateCouponApplication(Ticket ticket, Discount coupon)
    {
        if (ticket == null) throw new ArgumentNullException(nameof(ticket));
        if (coupon == null) throw new ArgumentNullException(nameof(coupon));

        if (!coupon.IsActive)
        {
             throw new BusinessRuleViolationException($"Coupon '{coupon.Name}' is not active.");
        }

        if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value < DateTime.UtcNow)
        {
             throw new BusinessRuleViolationException($"Coupon '{coupon.Name}' has expired.");
        }

        // Check if already applied
        if (ticket.Discounts.Any(d => d.DiscountId == coupon.Id))
        {
             throw new BusinessRuleViolationException($"Coupon '{coupon.Name}' is already applied to this ticket.");
        }

        // Minimum Buy Validation
        // Note: Using SubtotalAmount which excludes tax
        if (coupon.MinimumBuy != null && ticket.SubtotalAmount < coupon.MinimumBuy)
        {
             throw new BusinessRuleViolationException($"Ticket subtotal ({ticket.SubtotalAmount}) is less than minimum buy requirement ({coupon.MinimumBuy}) for coupon '{coupon.Name}'.");
        }


        // Minimum Quantity Validation (Simple count of items)
        if (coupon.MinimumQuantity.HasValue)
        {
            var totalItems = ticket.OrderLines.Sum(l => l.ItemCount);
            if (totalItems < coupon.MinimumQuantity.Value)
            {
                 throw new BusinessRuleViolationException($"Ticket item count ({totalItems}) is less than minimum quantity ({coupon.MinimumQuantity}) for coupon '{coupon.Name}'.");
            }
        }
    }

    /// <summary>
    /// Calculates the discount amount based on the discount type and ticket subtotal.
    /// </summary>
    public Money CalculateDiscountAmount(Ticket ticket, Discount discount)
    {
        if (ticket == null) throw new ArgumentNullException(nameof(ticket));
        if (discount == null) throw new ArgumentNullException(nameof(discount));

        if (discount.Type == Enumerations.DiscountType.Amount)
        {
            return new Money(discount.Value, ticket.SubtotalAmount.Currency);
        }
        else if (discount.Type == Enumerations.DiscountType.Percentage)
        {
            // Percentage (e.g., 10 means 10%)
            var percentage = discount.Value / 100m;
            return ticket.SubtotalAmount * percentage;
        }
        else
        {
            // Reprice/AltPrice usually apply to items, not whole tickets broadly yet.
            // For now, treat others as zero or throw not supported for ticket-level coupons.
            // Assuming simplified Coupon logic: Amount or Percentage off total.
            return Money.Zero(ticket.SubtotalAmount.Currency);
        }
    }
}

