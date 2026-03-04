using System;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing state metadata, domain events, and utility methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Schedules the ticket for future fulfillment.
    /// </summary>
    public void Schedule(DateTime deliveryDate)
    {
        if (deliveryDate <= DateTime.UtcNow)
        {
            throw new BusinessRuleViolationException("Delivery date must be in the future to schedule.");
        }

        if (Status != TicketStatus.Draft && Status != TicketStatus.Open)
        {
            throw new DomainInvalidOperationException($"Cannot schedule ticket in {Status} status.");
        }

        DeliveryDate = deliveryDate;
        Status = TicketStatus.Scheduled;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Fires a scheduled ticket, moving it to Open status for processing.
    /// </summary>
    public void Fire()
    {
        if (Status != TicketStatus.Scheduled)
        {
            throw new DomainInvalidOperationException($"Cannot fire ticket. Expected Scheduled status, but was {Status}.");
        }

        Status = TicketStatus.Open;
        ActiveDate = DateTime.UtcNow;
        // Logic to update CreatedAt/OpenedAt? Keep original.
    }

    /// <summary>
    /// Changes the order type of the ticket with validation.
    /// </summary>
    public void ChangeOrderType(OrderType orderType)
    {
        if (orderType == null) throw new ArgumentNullException(nameof(orderType));
        if (orderType.Id == OrderTypeId) return;

        // F-0068: Validate switch requirements
        if (orderType.Name.Contains("Delivery", StringComparison.OrdinalIgnoreCase))
        {
            if (CustomerId == null)
                throw new BusinessRuleViolationException("Delivery orders require a customer.");
            if (string.IsNullOrWhiteSpace(DeliveryAddress))
                throw new BusinessRuleViolationException("Delivery orders require a delivery address.");
        }

        OrderTypeId = orderType.Id;
        IsBarTab = orderType.IsBarTab;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the customer and delivery info for the ticket.
    /// </summary>
    public void SetCustomer(Guid? customerId, string? address = null, string? extraInfo = null)
    {
        CustomerId = customerId;
        DeliveryAddress = address;
        ExtraDeliveryInfo = extraInfo;
        ActiveDate = DateTime.UtcNow;

        // Re-validate current type requirements if removing customer
        if (CustomerId == null && !string.IsNullOrWhiteSpace(address))
        {
            // If address is present but customer is null, that's weird but maybe allowed for Guest Delivery?
            // Usually Customer is required for tracking.
        }
    }

    /// <summary>
    /// Sets the note for the ticket.
    /// </summary>
    public void SetNote(string? note)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot update note on ticket in {Status} status.");
        }

        Note = note;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the number of guests for the ticket.
    /// </summary>
    public void SetNumberOfGuests(int numberOfGuests)
    {
        if (numberOfGuests < 0)
        {
            throw new BusinessRuleViolationException("Number of guests cannot be negative.");
        }

        // Audit F-0023: Allow 0 if business logic allows "Skip"?
        // Audit says "Skip guest count: Default to 1".
        // We will allow 0 if explicitly set, but UI defaults to 1.

        NumberOfGuests = numberOfGuests;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Recalculates all ticket totals.
    /// Note: This method uses a simplified tax calculation.
    /// For enhanced tax calculations (tax groups, price-includes-tax), use TicketDomainService.CalculateTotals().
    /// </summary>
    public void CalculateTotals()
    {
        // Calculate subtotal from order lines
        SubtotalAmount = _orderLines.Aggregate(
            Money.Zero(),
            (sum, line) => sum + line.TotalAmount);

        // Calculate tax (simplified - default 10% if not tax exempt)
        // For enhanced calculations, use TicketDomainService.CalculateTotals()
        TaxAmount = IsTaxExempt
            ? Money.Zero()
            : SubtotalAmount * 0.10m; // Default 10% tax (domain service will override)

        // Calculate discount amount
        DiscountAmount = _discounts.Aggregate(
            Money.Zero(),
            (sum, d) => sum + d.Amount);

        // Calculate total
        // Note: When PriceIncludesTax is true, the tax is already included in SubtotalAmount
        // So we don't add it again here
        if (PriceIncludesTax)
        {
            // Tax is already included in subtotal, so total = subtotal + charges - discounts
            TotalAmount = SubtotalAmount
                + ServiceChargeAmount
                + DeliveryChargeAmount
                + AdjustmentAmount
                - DiscountAmount;
        }
        else
        {
            // Standard calculation: add tax to subtotal
            TotalAmount = SubtotalAmount
                + TaxAmount
                + ServiceChargeAmount
                + DeliveryChargeAmount
                + AdjustmentAmount
                - DiscountAmount;
        }

        // Add gratuity if present
        if (Gratuity != null)
        {
            TotalAmount = TotalAmount + Gratuity.Amount;
        }

        // Recalculate due amount
        RecalculatePaidAmount();

        // Ensure we don't crash if paid > total (e.g. cash overpayment)
        if (PaidAmount >= TotalAmount)
        {
            DueAmount = Money.Zero(TotalAmount.Currency);
        }
        else
        {
            DueAmount = TotalAmount - PaidAmount;
        }
    }

    /// <summary>
    /// Internal method to recalculate totals with a pre-calculated tax amount.
    /// Used by TicketDomainService for enhanced tax calculations.
    /// </summary>
    internal void CalculateTotalsWithTax(Money taxAmount)
    {
        // Calculate subtotal from order lines
        SubtotalAmount = _orderLines.Aggregate(
            Money.Zero(),
            (sum, line) => sum + line.TotalAmount);

        // Set tax amount (calculated by domain service)
        TaxAmount = taxAmount;

        // Calculate discount amount
        DiscountAmount = _discounts.Aggregate(
            Money.Zero(),
            (sum, d) => sum + d.Amount);

        // Calculate total
        // When PriceIncludesTax is true, tax is already included in SubtotalAmount
        // So we don't add TaxAmount again
        if (PriceIncludesTax)
        {
            // Tax is already included in subtotal, so total = subtotal + charges - discounts
            TotalAmount = SubtotalAmount
                + ServiceChargeAmount
                + DeliveryChargeAmount
                + AdjustmentAmount
                - DiscountAmount;
        }
        else
        {
            // Standard calculation: add tax to subtotal
            TotalAmount = SubtotalAmount
                + TaxAmount
                + ServiceChargeAmount
                + DeliveryChargeAmount
                + AdjustmentAmount
                - DiscountAmount;
        }

        // Add gratuity if present
        if (Gratuity != null)
        {
            TotalAmount = TotalAmount + Gratuity.Amount;
        }

        // Recalculate due amount
        RecalculatePaidAmount();
        DueAmount = TotalAmount - PaidAmount;
    }

    /// <summary>
    /// Recalculates the paid amount from payments.
    /// Credits (payments) increase PaidAmount, Debits (refunds) decrease it.
    /// </summary>
    private void RecalculatePaidAmount()
    {
        var validPayments = _payments.Where(p => !p.IsVoided).ToList();

        var totalCredits = validPayments
            .Where(p => p.TransactionType == TransactionType.Credit)
            .Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);

        var totalDebits = validPayments
            .Where(p => p.TransactionType == TransactionType.Debit)
            .Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);

        // Ensure we don't crash if debits > credits (e.g. if original credit was voided but refund remains)
        if (totalDebits > totalCredits)
        {
            // In this case, we have more refunds than credits contextually active.
            // We should clamp to zero to avoid "Negative Money" exception.
            PaidAmount = Money.Zero(totalCredits.Currency);
        }
        else
        {
            PaidAmount = totalCredits - totalDebits;
        }
    }

    /// <summary>
    /// [OBSOLETE] Do NOT call this method.
    ///
    /// Version is a concurrency token managed exclusively by VersionIncrementInterceptor.
    /// Manual mutation of Version breaks EF Core's optimistic concurrency mechanism,
    /// causing deterministic DbUpdateConcurrencyException (0 rows affected).
    ///
    /// EF Core uses OriginalValues["Version"] in the WHERE clause of UPDATE statements.
    /// When you manually increment Version before SaveChanges, you create a mismatch:
    /// - OriginalValues["Version"] = N (from load)
    /// - CurrentValues["Version"] = N+1 (manual increment)
    /// - EF generates: UPDATE ... WHERE Version = N
    /// - But expects to set Version = N+2 (interceptor increments again)
    /// This causes the WHERE clause to match, but the concurrency check fails.
    ///
    /// CORRECT APPROACH:
    /// - Remove all calls to IncrementVersion()
    /// - Let VersionIncrementInterceptor handle Version during SaveChanges
    /// - Version will be automatically incremented when entity is Modified or has Added/Deleted children
    /// </summary>
    [Obsolete("Do NOT manually increment Version. Use VersionIncrementInterceptor instead. Manual mutation breaks optimistic concurrency.", error: true)]
    public void IncrementVersion()
    {
        Version++;
    }
}
