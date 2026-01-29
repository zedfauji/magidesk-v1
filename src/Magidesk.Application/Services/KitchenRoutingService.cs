using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

public class KitchenRoutingService : IKitchenRoutingService
{
    private readonly IKitchenOrderRepository _kitchenOrderRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<KitchenRoutingService> _logger;

    public KitchenRoutingService(
        IKitchenOrderRepository kitchenOrderRepository,
        ITicketRepository ticketRepository,
        ILogger<KitchenRoutingService> logger)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
        _ticketRepository = ticketRepository;
        _logger = logger;
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

        // Idempotency Check: Filter out items that have already been routed
        var unroutedItems = new List<Magidesk.Application.DTOs.OrderLineDto>();
        foreach (var item in itemsToRoute)
        {
            if (!await _kitchenOrderRepository.IsTicketItemRoutedAsync(item.Id))
            {
                unroutedItems.Add(item);
            }
        }
        
        if (!unroutedItems.Any()) 
        {
             _logger.LogInformation("All valid items for ticket {TicketId} have already been routed. Skipping duplicate routing.", ticket.Id);
             return new List<Guid>();
        }

        itemsToRoute = unroutedItems;

        // 2. Fetch Helper Data
        var serverName = !string.IsNullOrEmpty(ticket.OwnerName) ? ticket.OwnerName : "Unknown Server";
        var tableNumber = !string.IsNullOrEmpty(ticket.TableName) ? ticket.TableName : "No Table";

        var createdOrderIds = new List<Guid>();

        // Group items by PrinterGroupId (Station)
        var itemsByStation = itemsToRoute.GroupBy(i => i.PrinterGroupId);

        foreach (var stationGroup in itemsByStation)
        {
            var printerGroupId = stationGroup.Key;
            
            var kitchenOrder = new KitchenOrder(
                ticket.Id,
                serverName, 
                tableNumber,
                printerGroupId
            );

            foreach (var item in stationGroup)
            {
                var modifiers = item.Modifiers.Select(m => m.Name).ToList();
                
                kitchenOrder.AddItem(
                    item.Id,
                    item.MenuItemName,
                    (int)item.Quantity,
                    printerGroupId ?? Guid.Empty, // Destination inside item matches group
                    modifiers
                );
            }

            await _kitchenOrderRepository.AddAsync(kitchenOrder);
            createdOrderIds.Add(kitchenOrder.Id);
        }
        
        _logger.LogInformation("Routed {ItemCount} items to kitchen for ticket {TicketId}, created {OrderCount} kitchen orders ({OrderIds})", 
            itemsToRoute.Count, ticket.Id, createdOrderIds.Count, string.Join(", ", createdOrderIds));

        return createdOrderIds;
    }

    public async Task<bool> AutoRouteOrderLinesAsync(Guid ticketId, List<Guid> orderLineIds)
    {
        try
        {
            _logger.LogInformation("Auto-routing order lines {OrderLineIds} for ticket {TicketId}", 
                string.Join(", ", orderLineIds), ticketId);

            // Get the ticket with order lines
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket {TicketId} not found for auto-routing", ticketId);
                return false;
            }

            // Convert to DTO for routing service
            var ticketDto = MapToDto(ticket);
            
            // Filter order lines that should be auto-routed
            var linesToRoute = ticketDto.OrderLines
                .Where(ol => orderLineIds.Contains(ol.Id) && ShouldAutoRoute(ol))
                .Select(ol => ol.Id)
                .ToList();

            if (!linesToRoute.Any())
            {
                _logger.LogInformation("No order lines require auto-routing for ticket {TicketId}", ticketId);
                return true; // Not an error, just nothing to route
            }

            // Route the filtered lines
            var kitchenOrderIds = await RouteToKitchenAsync(ticketDto, linesToRoute);
            
            _logger.LogInformation("Auto-routed {LineCount} order lines for ticket {TicketId}, created {OrderCount} kitchen orders", 
                linesToRoute.Count, ticketId, kitchenOrderIds.Count);

            return kitchenOrderIds.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to auto-route order lines {OrderLineIds} for ticket {TicketId}", 
                string.Join(", ", orderLineIds), ticketId);
            return false;
        }
    }

    public bool ShouldAutoRoute(OrderLineDto orderLine)
    {
        // Auto-route if the item should print to kitchen and hasn't been printed yet
        return orderLine.ShouldPrintToKitchen && !orderLine.PrintedToKitchen;
    }

    private TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            GlobalId = ticket.GlobalId,
            TableName = string.Join(", ", ticket.TableNumbers),
            OwnerName = "Unknown", // TODO: Get from user service or ticket
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ActiveDate = ticket.ActiveDate,
            OrderLines = ticket.OrderLines.Select(ol => new OrderLineDto
            {
                Id = ol.Id,
                TicketId = ol.TicketId,
                MenuItemId = ol.MenuItemId,
                MenuItemName = ol.MenuItemName,
                Quantity = ol.Quantity,
                ShouldPrintToKitchen = ol.ShouldPrintToKitchen,
                PrintedToKitchen = ol.PrintedToKitchen,
                PrinterGroupId = ol.PrinterGroupId,
                Instructions = ol.Instructions,
                SentToKitchenAt = ol.SentToKitchenAt,
                DeliveredAt = ol.DeliveredAt,
                Modifiers = ol.Modifiers.Select(m => new OrderLineModifierDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    ShouldPrintToKitchen = m.ShouldPrintToKitchen
                }).ToList()
            }).ToList()
        };
    }
}
