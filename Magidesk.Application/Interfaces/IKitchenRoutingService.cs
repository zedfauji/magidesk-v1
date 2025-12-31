using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

public interface IKitchenRoutingService
{
    /// <summary>
    /// Routes items from a ticket to appropriate kitchen stations/orders.
    /// Creates and saves KitchenOrder entities.
    /// </summary>
    /// <param name="ticket">The source ticket.</param>
    /// <param name="itemIds">Specific items to fire (if null/empty, routes all un-fired items).</param>
    /// <returns>List of created KitchenOrder IDs.</returns>
    Task<List<Guid>> RouteToKitchenAsync(TicketDto ticket, List<Guid>? itemIds = null);
}
