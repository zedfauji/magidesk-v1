using System;
using System.Collections.Generic;

namespace Magidesk.Domain.Entities;

public class GroupSettlement
{
    public Guid Id { get; private set; }
    public Guid MasterPaymentId { get; private set; }
    
    // In simpler designs, just storing the IDs is enough for audit.
    // If we want database integrity, we might want a join table or specific relationships.
    // Storing as a primitive collection for MVP parity.
    public List<Guid> ChildTicketIds { get; private set; } = new();

    public string Strategy { get; private set; } = "EqualSplit"; // Enum later if strictly needed

    protected GroupSettlement() { }

    public GroupSettlement(Guid masterPaymentId, List<Guid> childTicketIds, string strategy = "EqualSplit")
    {
        Id = Guid.NewGuid();
        MasterPaymentId = masterPaymentId;
        ChildTicketIds = childTicketIds ?? new List<Guid>();
        Strategy = strategy;
    }
}
