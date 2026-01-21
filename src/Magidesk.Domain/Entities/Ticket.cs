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
    public Guid? SessionId { get; private set; }
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
    public int Version { get; internal set; }
    
    // Properties (flexible metadata)
    // Properties (flexible metadata)
    public IReadOnlyDictionary<string, string> Properties => _properties.AsReadOnly();
    
    // F-0125: Ticket Note
    public string? Note { get; private set; }

    // Hold Ticket Support (C.2)
    /// <summary>
    /// Timestamp when ticket was held (if applicable).
    /// </summary>
    public DateTime? HeldAt { get; private set; }
    
    /// <summary>
    /// Reason for holding the ticket.
    /// </summary>
    public string? HoldReason { get; private set; }
    
    /// <summary>
    /// User who held the ticket.
    /// </summary>
    public UserId? HeldBy { get; private set; }

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
        CalculateTotals();
    }

    /// <summary>
    /// Applies a discount to the ticket with authorization support.
    /// </summary>
    /// <param name="discount">The discount definition to apply</param>
    /// <param name="appliedBy">User applying the discount</param>
    /// <param name="authorizedBy">Manager who authorized the discount (required for discounts > 50%)</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket status doesn't allow discounts</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown if discount would result in negative total or authorization is missing</exception>
    public void ApplyDiscount(Discount discount, UserId appliedBy, UserId? authorizedBy = null)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (appliedBy == null)
        {
            throw new ArgumentNullException(nameof(appliedBy));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        if (!discount.IsActive)
        {
            throw new BusinessRuleViolationException("Cannot apply inactive discount.");
        }

        // Calculate discount amount
        var discountAmount = discount.CalculateDiscount(SubtotalAmount);

        // Validate discount doesn't result in negative total
        var newTotal = TotalAmount - discountAmount;
        if (newTotal < Money.Zero())
        {
            throw new BusinessRuleViolationException("Discount would result in negative total.");
        }

        // Check if discount requires authorization (> 50% of subtotal)
        var discountPercentage = SubtotalAmount.Amount > 0 
            ? (discountAmount.Amount / SubtotalAmount.Amount) * 100m 
            : 0m;

        if (discountPercentage > 50m && authorizedBy == null)
        {
            throw new BusinessRuleViolationException("Discounts greater than 50% require manager authorization.");
        }

        // Create ticket discount snapshot
        var ticketDiscount = TicketDiscount.Create(
            ticketId: Id,
            discountId: discount.Id,
            name: discount.Name,
            type: discount.Type,
            value: discount.Value,
            amount: discountAmount,
            appliedBy: appliedBy,
            authorizedBy: authorizedBy,
            minimumAmount: discount.MinimumBuy
        );

        _discounts.Add(ticketDiscount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();

        // TODO: Raise DiscountAppliedEvent (Task 2.1.4)
    }

    /// <summary>
    /// Removes a discount from the ticket.
    /// </summary>
    /// <param name="discountId">The ID of the TicketDiscount to remove</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket status doesn't allow discount removal</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown if discount not found</exception>
    public void RemoveDiscount(Guid discountId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot remove discount from ticket in {Status} status.");
        }

        var discount = _discounts.FirstOrDefault(d => d.Id == discountId);
        if (discount == null)
        {
            throw new BusinessRuleViolationException($"Discount {discountId} not found on this ticket.");
        }

        _discounts.Remove(discount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();

        // TODO: Raise DiscountRemovedEvent (Task 2.1.4)
    }

    /// <summary>
    /// Applies a discount to a specific order line in the ticket.
    /// </summary>
    public void ApplyLineDiscount(Guid orderLineId, OrderLineDiscount discount)
    {
        if (discount == null) throw new ArgumentNullException(nameof(discount));
        
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        var line = _orderLines.FirstOrDefault(x => x.Id == orderLineId);
        if (line == null)
            throw new BusinessRuleViolationException($"OrderLine {orderLineId} not found in this ticket.");

        line.ApplyDiscount(discount);
        ActiveDate = DateTime.UtcNow;
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
        }
    }

    /// <summary>
    /// Removes a table number from the ticket.
    /// </summary>
    public void RemoveTableNumber(int tableNumber)
    {
        if (_tableNumbers.Remove(tableNumber))
        {
        }
    }

    /// <summary>
    /// Assigns the ticket to a specific table, removing any previous assignments.
    /// </summary>
    public void AssignTable(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new BusinessRuleViolationException("Table number must be greater than zero.");
        }

        if (_tableNumbers.Count == 1 && _tableNumbers[0] == tableNumber)
        {
            return;
        }

        _tableNumbers.Clear();
        _tableNumbers.Add(tableNumber);
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
    /// Links the ticket to a table session.
    /// </summary>
    public void SetSession(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
        {
             throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot link session to ticket in {Status} status.");
        }

        SessionId = sessionId;
        ActiveDate = DateTime.UtcNow;
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
    }

    /// <summary>
    /// Transfers the ticket to a new owner.
    /// </summary>
    public void Transfer(UserId newOwner)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot transfer ticket in {Status} status.");
        }

        if (newOwner == null)
        {
            throw new ArgumentNullException(nameof(newOwner));
        }

        CreatedBy = newOwner;
        ActiveDate = DateTime.UtcNow;
    }

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
