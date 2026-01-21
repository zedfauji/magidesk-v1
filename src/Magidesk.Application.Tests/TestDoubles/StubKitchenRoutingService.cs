using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Tests.TestDoubles;

public class StubKitchenRoutingService : IKitchenRoutingService
{
    public Task<List<Guid>> RouteToKitchenAsync(TicketDto ticket, List<Guid>? itemIds = null)
    {
        return Task.FromResult(new List<Guid>());
    }

    public Task<bool> AutoRouteOrderLinesAsync(Guid ticketId, List<Guid> orderLineIds)
    {
        return Task.FromResult(true);
    }

    public bool ShouldAutoRoute(OrderLineDto orderLine)
    {
        return true;
    }
}