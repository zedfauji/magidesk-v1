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

    /// <summary>
    /// Automatically routes items to kitchen when they are added to a ticket.
    /// Implements requirement 9.1 for automatic routing after submission.
    /// </summary>
    /// <param name="ticketId">The ticket ID containing the items to route</param>
    /// <param name="orderLineIds">Specific order line IDs to route automatically</param>
    /// <returns>True if routing was successful, false otherwise</returns>
    Task<bool> AutoRouteOrderLinesAsync(Guid ticketId, List<Guid> orderLineIds);

    /// <summary>
    /// Checks if an order line should be automatically routed to kitchen.
    /// </summary>
    /// <param name="orderLine">The order line to check</param>
    /// <returns>True if the order line should be routed to kitchen</returns>
    bool ShouldAutoRoute(OrderLineDto orderLine);
}
