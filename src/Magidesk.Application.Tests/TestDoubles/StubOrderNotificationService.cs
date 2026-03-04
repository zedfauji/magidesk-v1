using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Tests.TestDoubles;

/// <summary>
/// S000-04: No-op stub for IOrderNotificationService used in unit tests.
/// Notification failure must not affect business logic results.
/// </summary>
public class StubOrderNotificationService : IOrderNotificationService
{
    public Task NotifyOrderReadyAsync(Guid kitchenOrderId, string tableNumber, string serverName)
        => Task.CompletedTask;

    public Task NotifyOrderStatusChangeAsync(Guid kitchenOrderId, KitchenStatus newStatus, string tableNumber, string serverName)
        => Task.CompletedTask;

    public Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName)
        => Task.CompletedTask;

    public Task NotifyOrderDeliveredAsync(Guid kitchenOrderId, Guid ticketId, string tableNumber, TimeSpan preparationTime)
        => Task.CompletedTask;

    public Task SubscribeToNotificationsAsync(Guid terminalId, Guid userId, string[]? tableNumbers = null)
        => Task.CompletedTask;

    public Task UnsubscribeFromNotificationsAsync(Guid terminalId)
        => Task.CompletedTask;
}
