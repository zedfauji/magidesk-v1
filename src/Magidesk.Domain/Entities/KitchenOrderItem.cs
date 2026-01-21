using System;
using System.Collections.Generic;

namespace Magidesk.Domain.Entities;

public class KitchenOrderItem
{
    public Guid Id { get; private set; }
    public Guid KitchenOrderId { get; private set; }
    public Guid TicketItemId { get; private set; } // Link back to OrderLine
    public string ItemName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Guid DestinationId { get; private set; } // PrinterGroup ID
    
    // Store simple list of strings for display
    public List<string> Modifiers { get; private set; } = new();

    protected KitchenOrderItem() { } // For EF Core

    public KitchenOrderItem(Guid kitchenOrderId, Guid ticketItemId, string itemName, int quantity, Guid destinationId, List<string> modifiers)
    {
        Id = Guid.NewGuid();
        KitchenOrderId = kitchenOrderId;
        TicketItemId = ticketItemId;
        ItemName = itemName;
        Quantity = quantity;
        DestinationId = destinationId;
        Modifiers = modifiers ?? new List<string>();
    }
}
