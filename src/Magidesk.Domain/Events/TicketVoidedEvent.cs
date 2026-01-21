using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a ticket is voided.
    /// REQ-5.8: Audit trail for void operations.
    /// </summary>
    public sealed class TicketVoided : DomainEventBase
    {
        /// <summary>
        /// Gets the ID of the voided ticket.
        /// </summary>
        public Guid TicketId { get; }

        /// <summary>
        /// Gets the reason for voiding the ticket.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the user who voided the ticket.
        /// </summary>
        public UserId VoidedBy { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketVoided"/> event.
        /// </summary>
        /// <param name="ticketId">The ID of the voided ticket</param>
        /// <param name="reason">The reason for voiding</param>
        /// <param name="voidedBy">The user who voided the ticket</param>
        /// <param name="correlationId">Optional correlation ID for tracking related events</param>
        public TicketVoided(Guid ticketId, string reason, UserId voidedBy, Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
            Reason = reason;
            VoidedBy = voidedBy;
        }
    }
}
