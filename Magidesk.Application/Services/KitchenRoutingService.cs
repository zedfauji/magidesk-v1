using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Services;

public class KitchenRoutingService : IKitchenRoutingService
{
    private readonly IKitchenOrderRepository _kitchenOrderRepository;

    public KitchenRoutingService(IKitchenOrderRepository kitchenOrderRepository)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
    }

    public async Task<List<Guid>> RouteToKitchenAsync(TicketDto ticket, List<Guid>? itemIds = null)
    {
        if (ticket == null) throw new ArgumentNullException(nameof(ticket));

        // 1. Identify items to route
        var itemsToRoute = ticket.OrderLines
            .Where(ol => itemIds == null || itemIds.Contains(ol.Id))
            .Where(ol => ol.ShouldPrintToKitchen) 
            .ToList();

        if (!itemsToRoute.Any()) return new List<Guid>();

        // 2. Fetch Helper Data
        var serverName = !string.IsNullOrEmpty(ticket.OwnerName) ? ticket.OwnerName : "Unknown Server";
        var tableNumber = !string.IsNullOrEmpty(ticket.TableName) ? ticket.TableName : "No Table";

        var createdOrderIds = new List<Guid>();

        // Logic Simplification: Currently treating entire ticket route as one KitchenOrder
        // In the future, we might split by Station (PrinterGroupId) here.
        
        var kitchenOrder = new KitchenOrder(
            ticket.Id,
            serverName, 
            tableNumber 
        );

        foreach (var item in itemsToRoute)
        {
            var modifiers = item.Modifiers.Select(m => m.Name).ToList();
            
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
