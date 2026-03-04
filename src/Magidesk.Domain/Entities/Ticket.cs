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
public partial class Ticket
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
}
