using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a payment is processed.
/// </summary>
public sealed class PaymentProcessed : DomainEventBase
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }
    public Money Amount { get; }

    public PaymentProcessed(Guid paymentId, Guid ticketId, Money amount, Guid? correlationId = null)
        : base(correlationId)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
        Amount = amount;
    }
}

/// <summary>
/// Domain event raised when a payment is authorized (for card transactions).
/// </summary>
public sealed class PaymentAuthorized : DomainEventBase
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }
    public Money Amount { get; }

    public PaymentAuthorized(Guid paymentId, Guid ticketId, Money amount, Guid? correlationId = null)
        : base(correlationId)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
        Amount = amount;
    }
}

/// <summary>
/// Domain event raised when a payment is captured (for card transactions).
/// </summary>
public sealed class PaymentCaptured : DomainEventBase
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }
    public Money Amount { get; }

    public PaymentCaptured(Guid paymentId, Guid ticketId, Money amount, Guid? correlationId = null)
        : base(correlationId)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
        Amount = amount;
    }
}

/// <summary>
/// Domain event raised when a payment is voided.
/// </summary>
public sealed class PaymentVoided : DomainEventBase
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }

    public PaymentVoided(Guid paymentId, Guid ticketId, Guid? correlationId = null)
        : base(correlationId)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
    }
}

/// <summary>
/// Domain event raised when a payment is refunded.
/// </summary>
public sealed class PaymentRefunded : DomainEventBase
{
    public Guid PaymentId { get; }
    public Guid TicketId { get; }
    public Money RefundAmount { get; }

    public PaymentRefunded(Guid paymentId, Guid ticketId, Money refundAmount, Guid? correlationId = null)
        : base(correlationId)
    {
        PaymentId = paymentId;
        TicketId = ticketId;
        RefundAmount = refundAmount;
    }
}

