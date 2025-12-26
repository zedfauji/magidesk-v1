using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

public class KitchenRoutingService : IKitchenRoutingService
{
    private readonly IKitchenOrderRepository _kitchenOrderRepository;

    public KitchenRoutingService(IKitchenOrderRepository kitchenOrderRepository)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
    }

    public async Task<List<Guid>> RouteToKitchenAsync(Ticket ticket, List<Guid>? itemIds = null)
    {
        if (ticket == null) throw new ArgumentNullException(nameof(ticket));

        // 1. Identify items to route
        var itemsToRoute = ticket.OrderLines
            .Where(ol => itemIds == null || itemIds.Contains(ol.Id))
            .Where(ol => ol.ShouldPrintToKitchen) // Basic filter
            // In a real system, we'd check if already fired, but for now we assume caller handles 'fire' action.
            .ToList();

        if (!itemsToRoute.Any()) return new List<Guid>();

        // 2. Group by "Destination" (Printer Group)
        // MVP: We assume creating one KitchenOrder per "Routing Destination" or just one Big Order?
        // TDD says: "Group by Destination".
        // For MVP Foundation, let's assume all items go to a default "Kitchen" if no specific group logic exists yet.
        // We will group by 'nothing' (Create 1 Order) for now, or simulate grouping.
        
        var createdOrderIds = new List<Guid>();

        // Create a single Kitchen Order for this batch (Simplest MVP implementation)
        // In future: Group by Station (Hot, Cold, Bar)
        
        var kitchenOrder = new KitchenOrder(
            ticket.Id,
            "Server Name", // Placeholder: Ticket doesn't have ServerName snapshot yet
            "Table 1"      // Placeholder
        );

        foreach (var item in itemsToRoute)
        {
            var modifiers = item.Modifiers.Select(m => m.Name).ToList();
            
            // Assuming KitchenOrderItem constructor: (kitchenOrderId, ticketItemId, itemName, quantity, modifiers, destinationId)
            // We need to fetch/determine DestinationId. defaulting to Guid.Empty or a meaningful value.
            
            kitchenOrder.AddItem(
                item.Id,
                item.MenuItemName,
                (int)item.Quantity,
                item.PrinterGroupId ?? Guid.Empty, 
                modifiers
            );
        }

        await _kitchenOrderRepository.AddAsync(kitchenOrder);
        
        createdOrderIds.Add(kitchenOrder.Id);

        return createdOrderIds;
    }
}
