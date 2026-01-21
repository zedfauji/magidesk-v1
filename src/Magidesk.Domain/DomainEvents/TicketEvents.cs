using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a ticket is created.
/// </summary>
public sealed class TicketCreated : DomainEventBase
{
    public Guid TicketId { get; }
    public int TicketNumber { get; }
    public UserId CreatedBy { get; }

    public TicketCreated(Guid ticketId, int ticketNumber, UserId createdBy, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        CreatedBy = createdBy;
    }
}

/// <summary>
/// Domain event raised when a ticket is opened.
/// </summary>
public sealed class TicketOpened : DomainEventBase
{
    public Guid TicketId { get; }

    public TicketOpened(Guid ticketId, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
    }
}

/// <summary>
/// Domain event raised when an order line is added to a ticket.
/// </summary>
public sealed class OrderLineAdded : DomainEventBase
{
    public Guid TicketId { get; }
    public Guid OrderLineId { get; }

    public OrderLineAdded(Guid ticketId, Guid orderLineId, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        OrderLineId = orderLineId;
    }
}

/// <summary>
/// Domain event raised when an order line is removed from a ticket.
/// </summary>
public sealed class OrderLineRemoved : DomainEventBase
{
    public Guid TicketId { get; }
    public Guid OrderLineId { get; }

    public OrderLineRemoved(Guid ticketId, Guid orderLineId, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        OrderLineId = orderLineId;
    }
}

/// <summary>
/// Domain event raised when a payment is added to a ticket.
/// </summary>
public sealed class PaymentAdded : DomainEventBase
{
    public Guid TicketId { get; }
    public Guid PaymentId { get; }
    public Money Amount { get; }

    public PaymentAdded(Guid ticketId, Guid paymentId, Money amount, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        PaymentId = paymentId;
        Amount = amount;
    }
}

/// <summary>
/// Domain event raised when a ticket is paid (all payments received).
/// </summary>
public sealed class TicketPaid : DomainEventBase
{
    public Guid TicketId { get; }
    public Money TotalAmount { get; }
    public Money PaidAmount { get; }

    public TicketPaid(Guid ticketId, Money totalAmount, Money paidAmount, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        TotalAmount = totalAmount;
        PaidAmount = paidAmount;
    }
}

/// <summary>
/// Domain event raised when a ticket is closed.
/// </summary>
public sealed class TicketClosed : DomainEventBase
{
    public Guid TicketId { get; }
    public UserId ClosedBy { get; }

    public TicketClosed(Guid ticketId, UserId closedBy, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        ClosedBy = closedBy;
    }
}

/// <summary>
/// Domain event raised when a ticket is voided.
/// </summary>
public sealed class TicketVoided : DomainEventBase
{
    public Guid TicketId { get; }
    public UserId VoidedBy { get; }

    public TicketVoided(Guid ticketId, UserId voidedBy, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        VoidedBy = voidedBy;
    }
}

/// <summary>
/// Domain event raised when a ticket is refunded.
/// </summary>
public sealed class TicketRefunded : DomainEventBase
{
    public Guid TicketId { get; }
    public Money RefundAmount { get; }

    public TicketRefunded(Guid ticketId, Money refundAmount, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
        RefundAmount = refundAmount;
    }
}

/// <summary>
/// Domain event raised when a ticket is reopened.
/// </summary>
public sealed class TicketReopened : DomainEventBase
{
    public Guid TicketId { get; }

    public TicketReopened(Guid ticketId, Guid? correlationId = null)
        : base(correlationId)
    {
        TicketId = ticketId;
    }
}

