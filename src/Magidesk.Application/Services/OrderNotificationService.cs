using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

/// <summary>
/// Service for handling order-related notifications to servers and staff.
/// Implements requirement 9.3 for order ready notifications.
/// </summary>
public class OrderNotificationService : IOrderNotificationService
{
    private readonly ILogger<OrderNotificationService> _logger;
    private readonly IKitchenNotificationPublisher _publisher;
    
    // In-memory storage for notification subscriptions
    // In a production system, this would be stored in Redis or database for multi-terminal support
    private readonly ConcurrentDictionary<Guid, NotificationSubscription> _subscriptions = new();

    public OrderNotificationService(
        ILogger<OrderNotificationService> logger,
        IKitchenNotificationPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task NotifyOrderReadyAsync(Guid kitchenOrderId, string tableNumber, string serverName)
    {
        _logger.LogInformation("Order ready notification: Kitchen Order {KitchenOrderId}, Table {TableNumber}, Server {ServerName}", 
            kitchenOrderId, tableNumber, serverName);

        var notification = new OrderNotification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.OrderReady,
            KitchenOrderId = kitchenOrderId,
            TableNumber = tableNumber,
            ServerName = serverName,
            Message = $"Order ready for pickup - Table {tableNumber}",
            Timestamp = DateTime.UtcNow
        };

        await BroadcastNotificationAsync(notification);
    }

    public async Task NotifyOrderStatusChangeAsync(Guid kitchenOrderId, KitchenStatus newStatus, string tableNumber, string serverName)
    {
        _logger.LogInformation("Order status change notification: Kitchen Order {KitchenOrderId}, Status {Status}, Table {TableNumber}", 
            kitchenOrderId, newStatus, tableNumber);

        var notification = new OrderNotification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.StatusChange,
            KitchenOrderId = kitchenOrderId,
            TableNumber = tableNumber,
            ServerName = serverName,
            Status = newStatus,
            Message = $"Order status changed to {newStatus} - Table {tableNumber}",
            Timestamp = DateTime.UtcNow
        };

        await BroadcastNotificationAsync(notification);
    }

    public async Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName)
    {
        _logger.LogInformation("New order notification: Kitchen Order {KitchenOrderId}, Table {TableNumber}, Server {ServerName}", 
            kitchenOrderId, tableNumber, serverName);

        var notification = new OrderNotification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.OrderCreated,
            KitchenOrderId = kitchenOrderId,
            TableNumber = tableNumber,
            ServerName = serverName,
            Message = $"New order for Table {tableNumber}",
            Timestamp = DateTime.UtcNow
        };

        await BroadcastNotificationAsync(notification);
    }

    public async Task NotifyOrderDeliveredAsync(Guid kitchenOrderId, Guid ticketId, string tableNumber, TimeSpan preparationTime)
    {
        _logger.LogInformation(
            "Order delivered notification: Kitchen Order {KitchenOrderId}, Ticket {TicketId}, Table {TableNumber}, Prep Time {PrepTime}s", 
            kitchenOrderId, ticketId, tableNumber, preparationTime.TotalSeconds);

        var notification = new OrderNotification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.OrderDelivered,
            KitchenOrderId = kitchenOrderId,
            TicketId = ticketId,
            TableNumber = tableNumber,
            Message = $"Order for Table {tableNumber} is ready (Prep time: {preparationTime.TotalMinutes:F1} min)",
            Timestamp = DateTime.UtcNow,
            PreparationTime = preparationTime
        };

        await BroadcastNotificationAsync(notification);
    }

    public async Task SubscribeToNotificationsAsync(Guid terminalId, Guid userId, string[]? tableNumbers = null)
    {
        var subscription = new NotificationSubscription
        {
            TerminalId = terminalId,
            UserId = userId,
            TableNumbers = tableNumbers,
            SubscribedAt = DateTime.UtcNow
        };

        _subscriptions.AddOrUpdate(terminalId, subscription, (key, existing) => subscription);
        
        _logger.LogInformation("Terminal {TerminalId} subscribed to notifications for user {UserId}", terminalId, userId);
        
        await Task.CompletedTask;
    }

    public async Task UnsubscribeFromNotificationsAsync(Guid terminalId)
    {
        _subscriptions.TryRemove(terminalId, out _);
        
        _logger.LogInformation("Terminal {TerminalId} unsubscribed from notifications", terminalId);
        
        await Task.CompletedTask;
    }

    private async Task BroadcastNotificationAsync(OrderNotification notification)
    {
        // 1. Publish to external systems (SignalR)
        try 
        {
            await _publisher.PublishAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish notification {NotificationId} to SignalR", notification.Id);
        }

        // 2. Process legacy in-memory subscriptions (if any)
        foreach (var subscription in _subscriptions.Values)
        {
            // Check if subscription should receive this notification
            if (ShouldReceiveNotification(subscription, notification))
            {
                _logger.LogInformation("Broadcasting notification {NotificationId} to terminal {TerminalId}: {Message}", 
                    notification.Id, subscription.TerminalId, notification.Message);
            }
        }
    }

    private static bool ShouldReceiveNotification(NotificationSubscription subscription, OrderNotification notification)
    {
        // If no specific tables are subscribed, receive all notifications
        if (subscription.TableNumbers == null || subscription.TableNumbers.Length == 0)
        {
            return true;
        }

        // Check if the notification's table is in the subscription list
        return Array.Exists(subscription.TableNumbers, table => 
            string.Equals(table, notification.TableNumber, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Represents a notification subscription for a terminal/user.
/// </summary>
public class NotificationSubscription
{
    public Guid TerminalId { get; set; }
    public Guid UserId { get; set; }
    public string[]? TableNumbers { get; set; }
    public DateTime SubscribedAt { get; set; }
}

/// <summary>
/// Represents an order notification.
/// </summary>
public class OrderNotification
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public Guid KitchenOrderId { get; set; }
    public Guid TicketId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
    public KitchenStatus? Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan? PreparationTime { get; set; }
}

/// <summary>
/// Types of order notifications.
/// </summary>
public enum NotificationType
{
    OrderReady,
    StatusChange,
    OrderCreated,
    OrderDelivered
}