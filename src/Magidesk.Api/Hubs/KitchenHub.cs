using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Magidesk.Api.Hubs;

public class KitchenHub : Hub
{
    public async Task SubscribeToStation(string stationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, stationId);
    }

    public async Task UnsubscribeFromStation(string stationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, stationId);
    }
}
