using Magidesk.Domain.DomainEvents;
using Magidesk.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Magidesk.Domain.Events
{
    /// <summary>
    /// Domain event raised when a ticket is split by seat.
    /// </summary>
    public sealed class TicketSplitBySeatEvent : DomainEventBase
    {
        public Guid OriginalTicketId { get; }
        public List<Guid> NewTicketIds { get; }
        public int SeatsCount { get; }
        public Dictionary<int, int> ItemsPerSeat { get; }
        public UserId ProcessedBy { get; }

        public TicketSplitBySeatEvent(
            Guid originalTicketId,
            List<Guid> newTicketIds,
            int seatsCount,
            Dictionary<int, int> itemsPerSeat,
            UserId processedBy,
            Guid? correlationId = null)
            : base(correlationId)
        {
            OriginalTicketId = originalTicketId;
            NewTicketIds = newTicketIds;
            SeatsCount = seatsCount;
            ItemsPerSeat = itemsPerSeat;
            ProcessedBy = processedBy;
        }
    }
}
