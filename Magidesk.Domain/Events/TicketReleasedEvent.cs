using Magidesk.Domain.DomainEvents;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a held ticket is released for payment.
    /// </summary>
    public sealed class TicketReleased : DomainEventBase
    {
        public Guid TicketId { get; }

        public TicketReleased(Guid ticketId, Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
        }
    }
}
