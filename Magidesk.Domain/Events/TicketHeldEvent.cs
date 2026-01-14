using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a ticket is held for later payment.
    /// </summary>
    public sealed class TicketHeld : DomainEventBase
    {
        public Guid TicketId { get; }
        public string Reason { get; }
        public UserId HeldBy { get; }

        public TicketHeld(Guid ticketId, string reason, UserId heldBy, Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
            Reason = reason;
            HeldBy = heldBy;
        }
    }
}
