using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a discount is removed from a ticket.
    /// </summary>
    public sealed class DiscountRemoved : DomainEventBase
    {
        public Guid TicketId { get; }
        public Guid DiscountId { get; }
        public UserId RemovedBy { get; }
        public DateTime RemovedAt { get; }

        public DiscountRemoved(
            Guid ticketId, 
            Guid discountId, 
            UserId removedBy, 
            DateTime removedAt,
            Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
            DiscountId = discountId;
            RemovedBy = removedBy;
            RemovedAt = removedAt;
        }
    }
}
