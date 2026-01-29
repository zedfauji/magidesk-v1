using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Magidesk.Api.Services;

public class SignalRKitchenNotificationPublisher : IKitchenNotificationPublisher
{
    private readonly IHubContext<KitchenHub> _hubContext;

    public SignalRKitchenNotificationPublisher(IHubContext<KitchenHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishAsync(OrderNotification notification)
    {
        // Broadcast to ALL connected clients. 
        // Clients (KDS) will filter locally based on their configuration or perform a refresh.
        await _hubContext.Clients.All.SendAsync("OrderUpdated", notification);
    }
}
