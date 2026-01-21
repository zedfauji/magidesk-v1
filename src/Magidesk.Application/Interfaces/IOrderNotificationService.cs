using System;
using System.Threading.Tasks;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for handling order-related notifications to servers and staff.
/// Implements requirement 9.3 for order ready notifications.
/// </summary>
public interface IOrderNotificationService
{
    /// <summary>
    /// Notifies servers when an order is ready for pickup.
    /// </summary>
    /// <param name="kitchenOrderId">The kitchen order that is ready</param>
    /// <param name="tableNumber">The table number for the order</param>
    /// <param name="serverName">The server responsible for the table</param>
    Task NotifyOrderReadyAsync(Guid kitchenOrderId, string tableNumber, string serverName);

    /// <summary>
    /// Notifies servers when an order status changes.
    /// </summary>
    /// <param name="kitchenOrderId">The kitchen order with status change</param>
    /// <param name="newStatus">The new status of the order</param>
    /// <param name="tableNumber">The table number for the order</param>
    /// <param name="serverName">The server responsible for the table</param>
    Task NotifyOrderStatusChangeAsync(Guid kitchenOrderId, KitchenStatus newStatus, string tableNumber, string serverName);

    /// <summary>
    /// Subscribes a terminal/user to receive notifications for specific tables or all tables.
    /// </summary>
    /// <param name="terminalId">The terminal ID to receive notifications</param>
    /// <param name="userId">The user ID to receive notifications</param>
    /// <param name="tableNumbers">Specific table numbers to monitor, or null for all tables</param>
    Task SubscribeToNotificationsAsync(Guid terminalId, Guid userId, string[]? tableNumbers = null);

    /// <summary>
    /// Unsubscribes a terminal/user from notifications.
    /// </summary>
    /// <param name="terminalId">The terminal ID to stop receiving notifications</param>
    Task UnsubscribeFromNotificationsAsync(Guid terminalId);
}