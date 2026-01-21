using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a payment is processed.
    /// </summary>
    public sealed class PaymentProcessed : DomainEventBase
    {
        public Guid PaymentId { get; }
        public Guid TicketId { get; }
        public Money Amount { get; }
        public PaymentType PaymentType { get; }
        public UserId ProcessedBy { get; }

        public PaymentProcessed(Guid paymentId, Guid ticketId, Money amount, PaymentType paymentType, UserId processedBy, Guid? correlationId = null)
            : base(correlationId)
        {
            PaymentId = paymentId;
            TicketId = ticketId;
            Amount = amount;
            PaymentType = paymentType;
            ProcessedBy = processedBy;
        }
    }
}
