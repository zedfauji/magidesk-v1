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
    
    public Guid? PrinterGroupId { get; private set; }
    
    // Lifecycle timestamps
    public DateTime SentToKitchenAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    
    // Calculated property for preparation time
    public TimeSpan? PreparationTime => DeliveredAt.HasValue 
        ? DeliveredAt.Value - SentToKitchenAt 
        : null;
    
    private readonly List<KitchenOrderItem> _items = new();
    public IReadOnlyCollection<KitchenOrderItem> Items => _items.AsReadOnly();

    protected KitchenOrder() { } // For EF Core

    public KitchenOrder(Guid ticketId, string serverName, string tableNumber, Guid? printerGroupId)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        ServerName = serverName;
        TableNumber = tableNumber;
        PrinterGroupId = printerGroupId;
        Timestamp = DateTime.UtcNow;
        SentToKitchenAt = DateTime.UtcNow;
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

    public void MarkAsDelivered()
    {
        if (Status != KitchenStatus.Done)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Order must be Done before marking as Delivered");
        }
        
        Status = KitchenStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }
}
