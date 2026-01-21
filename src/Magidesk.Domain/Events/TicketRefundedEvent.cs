using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a ticket is refunded (full or partial).
    /// REQ-5.8: Audit trail for refund operations.
    /// </summary>
    public sealed class TicketRefunded : DomainEventBase
    {
        /// <summary>
        /// Gets the ID of the refunded ticket.
        /// </summary>
        public Guid TicketId { get; }

        /// <summary>
        /// Gets the amount refunded.
        /// </summary>
        public Money Amount { get; }

        /// <summary>
        /// Gets the reason for the refund.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the user who processed the refund.
        /// </summary>
        public UserId RefundedBy { get; }

        /// <summary>
        /// Gets whether this is a partial refund (true) or full refund (false).
        /// </summary>
        public bool IsPartial { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketRefunded"/> event.
        /// </summary>
        /// <param name="ticketId">The ID of the refunded ticket</param>
        /// <param name="amount">The amount refunded</param>
        /// <param name="reason">The reason for the refund</param>
        /// <param name="refundedBy">The user who processed the refund</param>
        /// <param name="isPartial">Whether this is a partial refund</param>
        /// <param name="correlationId">Optional correlation ID for tracking related events</param>
        public TicketRefunded(
            Guid ticketId, 
            Money amount, 
            string reason, 
            UserId refundedBy, 
            bool isPartial,
            Guid? correlationId = null)
            : base(correlationId)
        {
            TicketId = ticketId;
            Amount = amount;
            Reason = reason;
            RefundedBy = refundedBy;
            IsPartial = isPartial;
        }
    }
}
