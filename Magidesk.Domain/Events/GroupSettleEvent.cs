using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using System;
using System.Collections.Generic;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when multiple tickets are settled together in a group.
    /// </summary>
    public sealed class GroupSettled : DomainEventBase
    {
        public List<Guid> TicketIds { get; }
        public Money TotalAmount { get; }
        public PaymentType PaymentType { get; }
        public UserId ProcessedBy { get; }
        public List<Guid> PaymentIds { get; }

        public GroupSettled(List<Guid> ticketIds, Money totalAmount, PaymentType paymentType, UserId processedBy, List<Guid> paymentIds, Guid? correlationId = null)
            : base(correlationId)
        {
            TicketIds = ticketIds;
            TotalAmount = totalAmount;
            PaymentType = paymentType;
            ProcessedBy = processedBy;
            PaymentIds = paymentIds;
        }
    }
}
