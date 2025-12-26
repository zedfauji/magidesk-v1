using System;
using System.Collections.Generic;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

public class KitchenOrder
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string ServerName { get; private set; } = string.Empty;
    public string TableNumber { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public KitchenStatus Status { get; private set; }
    
    private readonly List<KitchenOrderItem> _items = new();
    public IReadOnlyCollection<KitchenOrderItem> Items => _items.AsReadOnly();

    protected KitchenOrder() { } // For EF Core

    public KitchenOrder(Guid ticketId, string serverName, string tableNumber)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        ServerName = serverName;
        TableNumber = tableNumber;
        Timestamp = DateTime.UtcNow;
        Status = KitchenStatus.New;
    }

    public void AddItem(Guid ticketItemId, string itemName, int quantity, Guid destinationId, List<string> modifiers)
    {
        var item = new KitchenOrderItem(Id, ticketItemId, itemName, quantity, destinationId, modifiers);
        _items.Add(item);
    }

    public void Bump()
    {
        if (Status == KitchenStatus.New)
        {
            Status = KitchenStatus.Cooking;
        }
        else if (Status == KitchenStatus.Cooking)
        {
            Status = KitchenStatus.Done;
        }
    }

    public void Void()
    {
        Status = KitchenStatus.Void;
    }
}
