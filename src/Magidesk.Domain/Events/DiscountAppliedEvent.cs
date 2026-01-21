using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a discount is applied to a ticket.
    /// </summary>
    public sealed class DiscountApplied : DomainEventBase
    {
        public Guid TicketId { get; }
        public Guid DiscountId { get; }
        public Money Amount { get; }
        public UserId AppliedBy { get; }
        public UserId? AuthorizedBy { get; }
        public DateTime AppliedAt { get; }

        public DiscountApplied(
            Guid ticketId, 
            Guid discountId, 
            Money amount, 
            UserId appliedBy, 
            UserId? authorizedBy,
            DateTime appliedAt,
            Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
            DiscountId = discountId;
            Amount = amount;
            AppliedBy = appliedBy;
            AuthorizedBy = authorizedBy;
            AppliedAt = appliedAt;
        }
    }
}
