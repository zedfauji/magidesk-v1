using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;

namespace Magidesk.Presentation.Services;

/// <summary>
/// A client-side no-op publisher.
/// Since the client writes directly to the DB but cannot push to the SignalR Hub (server-side concern),
/// we simply absorb the publish request. 
/// In a full architecture, this would call a Web API endpoint which then broadcasts via SignalR.
/// </summary>
public class NoOpKitchenNotificationPublisher : IKitchenNotificationPublisher
{
    public Task PublishAsync(OrderNotification notification)
    {
        // Client-side execution: Do not crash, but do not broadcast (server capability).
        // A future improvement would be to call an API endpoint here.
        System.Diagnostics.Debug.WriteLine($"[Client-Side] Notification Generated but not broadcast: {notification.Message}");
        return Task.CompletedTask;
    }
}
