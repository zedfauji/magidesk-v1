using System.Threading.Tasks;
using Magidesk.Application.Services; // For OrderNotification

namespace Magidesk.Application.Interfaces;

public interface IKitchenNotificationPublisher
{
    Task PublishAsync(OrderNotification notification);
}
