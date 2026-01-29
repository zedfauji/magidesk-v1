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
    /// Notifies KDS when a new order is created and routed to kitchen.
    /// This triggers real-time updates on kitchen display screens.
    /// </summary>
    /// <param name="kitchenOrderId">The newly created kitchen order ID</param>
    /// <param name="tableNumber">The table number for the order</param>
    /// <param name="serverName">The server responsible for the table</param>
    /// <returns>Task representing the async operation</returns>
    Task NotifyOrderCreatedAsync(Guid kitchenOrderId, string tableNumber, string serverName);

    /// <summary>
    /// Notifies POS when an order is delivered from kitchen.
    /// This triggers real-time updates on order entry screens.
    /// </summary>
    /// <param name="kitchenOrderId">The kitchen order that was delivered</param>
    /// <param name="ticketId">The ticket ID associated with the order</param>
    /// <param name="tableNumber">The table number for the order</param>
    /// <param name="preparationTime">Time taken from sent to kitchen to delivered</param>
    /// <returns>Task representing the async operation</returns>
    Task NotifyOrderDeliveredAsync(Guid kitchenOrderId, Guid ticketId, string tableNumber, TimeSpan preparationTime);

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