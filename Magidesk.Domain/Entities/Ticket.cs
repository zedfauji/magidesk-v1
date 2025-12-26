using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;
using DomainInvalidOperationException = Magidesk.Domain.Exceptions.InvalidOperationException;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a customer order/transaction.
/// Aggregate root for order management.
/// </summary>
public class Ticket
{
    private readonly List<OrderLine> _orderLines = new();
    private readonly List<Payment> _payments = new();
    private readonly List<TicketDiscount> _discounts = new();
    private readonly List<int> _tableNumbers = new();
    private readonly Dictionary<string, string> _properties = new();

    // Core Properties
    public Guid Id { get; private set; }
    public int TicketNumber { get; private set; }
    public string? GlobalId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime ActiveDate { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public TicketStatus Status { get; private set; }
    
    // User References
    public UserId CreatedBy { get; private set; } = null!;
    public UserId? ClosedBy { get; private set; }
    public UserId? VoidedBy { get; private set; }
    
    // References
    public Guid TerminalId { get; private set; }
    public Guid ShiftId { get; private set; }
    public Guid OrderTypeId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? AssignedDriverId { get; private set; }
    
    // Table Management
    public IReadOnlyList<int> TableNumbers => _tableNumbers.AsReadOnly();
    public int NumberOfGuests { get; private set; }
    
    // Financial Amounts
    public Money SubtotalAmount { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money ServiceChargeAmount { get; private set; }
    public Money DeliveryChargeAmount { get; private set; }
    public Money AdjustmentAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money PaidAmount { get; private set; }
    public Money DueAmount { get; private set; }
    public Money AdvanceAmount { get; private set; }
    
    // Flags
    public bool IsTaxExempt { get; private set; }
    public bool PriceIncludesTax { get; private set; } // If true, prices already include tax
    public bool IsBarTab { get; private set; }
    public bool IsReOpened { get; private set; }
    
    // Delivery
    public string? DeliveryAddress { get; private set; }
    public string? ExtraDeliveryInfo { get; private set; }
    public bool CustomerWillPickup { get; private set; }
    public DateTime? DispatchedTime { get; private set; }
    public DateTime? ReadyTime { get; private set; }
    
    // Collections
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<TicketDiscount> Discounts => _discounts.AsReadOnly();
    public Gratuity? Gratuity { get; private set; }
    
    // Concurrency
    public int Version { get; private set; }
    
    // Properties (flexible metadata)
    public IReadOnlyDictionary<string, string> Properties => _properties.AsReadOnly();

    // Private constructor for EF Core
    private Ticket()
    {
        SubtotalAmount = Money.Zero();
        DiscountAmount = Money.Zero();
        TaxAmount = Money.Zero();
        ServiceChargeAmount = Money.Zero();
        DeliveryChargeAmount = Money.Zero();
        AdjustmentAmount = Money.Zero();
        TotalAmount = Money.Zero();
        PaidAmount = Money.Zero();
        DueAmount = Money.Zero();
        AdvanceAmount = Money.Zero();
        NumberOfGuests = 1;
    }

    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    public static Ticket Create(
        int ticketNumber,
        UserId createdBy,
        Guid terminalId,
        Guid shiftId,
        Guid orderTypeId,
        string? globalId = null)
    {
        return new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber,
            GlobalId = globalId,
            CreatedBy = createdBy,
            TerminalId = terminalId,
            ShiftId = shiftId,
            OrderTypeId = orderTypeId,
            Status = TicketStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            ActiveDate = DateTime.UtcNow,
            Version = 1
        };
    }

    /// <summary>
    /// Opens the ticket (transitions from Draft to Open when first item is added).
    /// </summary>
    public void Open()
    {
        if (Status != TicketStatus.Draft)
        {
            throw new DomainInvalidOperationException($"Cannot open ticket in {Status} status.");
        }

        Status = TicketStatus.Open;
        OpenedAt = DateTime.UtcNow;
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Adds an order line to the ticket.
    /// </summary>
    public void AddOrderLine(OrderLine orderLine)
    {
        if (orderLine == null)
        {
            throw new ArgumentNullException(nameof(orderLine));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot add items to ticket in {Status} status.");
        }

        if (orderLine.TicketId != Id)
        {
            throw new BusinessRuleViolationException("OrderLine does not belong to this ticket.");
        }

        _orderLines.Add(orderLine);
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();

        // Auto-open if still in Draft
        if (Status == TicketStatus.Draft)
        {
            Open();
        }

        CalculateTotals();
    }

    /// <summary>
    /// Removes an order line from the ticket.
    /// </summary>
    public void RemoveOrderLine(Guid orderLineId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot remove items from ticket in {Status} status.");
        }

        var orderLine = _orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
        {
            throw new BusinessRuleViolationException($"OrderLine {orderLineId} not found.");
        }

        _orderLines.Remove(orderLine);
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
        CalculateTotals();
    }

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
        IncrementVersion();
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
        IncrementVersion();
    }

    /// <summary>
    /// Validates if the ticket can be closed.
    /// </summary>
    public bool CanClose()
    {
        if (Status != TicketStatus.Paid)
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
    /// </summary>
    public void Void(UserId voidedBy)
    {
        if (!CanVoid())
        {
            throw new DomainInvalidOperationException($"Cannot void ticket in {Status} status.");
        }

        Status = TicketStatus.Voided;
        VoidedBy = voidedBy;
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
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
    /// Validates if the ticket can be refunded.
    /// </summary>
    public bool CanRefund()
    {
        // Only closed tickets can be refunded
        return Status == TicketStatus.Closed;
    }

    /// <summary>
    /// Processes a refund on the ticket.
    /// Adds a refund payment (TransactionType.Debit) and updates ticket status.
    /// </summary>
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
        IncrementVersion();
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
        IncrementVersion();
    }

    /// <summary>
    /// Applies a discount to the ticket.
    /// </summary>
    public void ApplyDiscount(TicketDiscount discount)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        if (discount.TicketId != Id)
        {
            throw new BusinessRuleViolationException("Discount does not belong to this ticket.");
        }

        _discounts.Add(discount);
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
        CalculateTotals();
    }

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
        IncrementVersion();
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
        IncrementVersion();
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
        IncrementVersion();
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
        IncrementVersion();

        // Re-validate current type requirements if removing customer
        if (CustomerId == null && !string.IsNullOrWhiteSpace(address))
        {
             // If address is present but customer is null, that's weird but maybe allowed for Guest Delivery?
             // Usually Customer is required for tracking.
        }
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

        // Add gratuity if present and paid
        if (Gratuity != null && Gratuity.Paid)
        {
            TotalAmount = TotalAmount + Gratuity.Amount;
        }

        // Recalculate due amount
        RecalculatePaidAmount();
        DueAmount = TotalAmount - PaidAmount;
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

        // Add gratuity if present and paid
        if (Gratuity != null && Gratuity.Paid)
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
        PaidAmount = _payments
            .Where(p => !p.IsVoided)
            .Aggregate(Money.Zero(), (sum, p) => 
                p.TransactionType == TransactionType.Credit 
                    ? sum + p.Amount 
                    : sum - p.Amount); // Refunds (Debit) reduce paid amount
    }

    /// <summary>
    /// Adds a table number to the ticket.
    /// </summary>
    public void AddTableNumber(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new BusinessRuleViolationException("Table number must be greater than zero.");
        }

        if (!_tableNumbers.Contains(tableNumber))
        {
            _tableNumbers.Add(tableNumber);
            IncrementVersion();
        }
    }

    /// <summary>
    /// Removes a table number from the ticket.
    /// </summary>
    public void RemoveTableNumber(int tableNumber)
    {
        if (_tableNumbers.Remove(tableNumber))
        {
            IncrementVersion();
        }
    }

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

    /// <summary>
    /// Marks the ticket as ready for pickup or delivery.
    /// </summary>
    public void MarkAsReady()
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot mark ticket as ready in {Status} status.");
        }

        ReadyTime = DateTime.UtcNow;
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Marks the ticket as dispatched for delivery.
    /// </summary>
    public void MarkAsDispatched(Guid? driverId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot mark ticket as dispatched in {Status} status.");
        }

        if (CustomerWillPickup)
        {
             throw new DomainInvalidOperationException("Cannot dispatch a pickup ticket.");
        }

        DispatchedTime = DateTime.UtcNow;
        AssignedDriverId = driverId;
        ActiveDate = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Intecrements the version number.
    /// Should be called by any method that modifies the ticket state.
    /// Note: EF Core would also handle this if we configured RowVersion/Timestamp,
    /// but for integer usage manual increment is safer for domain events.
    /// </summary>
    private void IncrementVersion()
    {
        Version++;
    }
}

