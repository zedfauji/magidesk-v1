using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

public class KitchenStatusService : IKitchenStatusService
{
    private readonly IKitchenOrderRepository _kitchenOrderRepository;
    private readonly IOrderNotificationService _notificationService;
    private readonly ILogger<KitchenStatusService> _logger;

    public KitchenStatusService(
        IKitchenOrderRepository kitchenOrderRepository,
        IOrderNotificationService notificationService,
        ILogger<KitchenStatusService> logger)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task BumpOrderAsync(Guid kitchenOrderId)
    {
        var order = await _kitchenOrderRepository.GetByIdAsync(kitchenOrderId);
        if (order == null)
        {
            throw new Exception($"Kitchen Order not found: {kitchenOrderId}"); 
            // In a real app, use NotFoundException or similar
        }

        var previousStatus = order.Status;
        order.Bump();
        
        // Repository needs Update method.
        // For EF Core, since we loaded it from context (tracked), calling SaveChanges via Repository (if exposed) or if Repository saves on update.
        // For MVP, if Repository.GetById tracks explicitly, we need to save changes.
        // I need to add UpdateAsync to IKitchenOrderRepository or just a generic SaveChangesAsync.
        // Standard pattern: Repository.Update(entity) -> Context.Update/Attach + SaveChanges.
        
        await _kitchenOrderRepository.UpdateAsync(order);

        // Send notification for status change
        await _notificationService.NotifyOrderStatusChangeAsync(
            kitchenOrderId, 
            order.Status, 
            order.TableNumber, 
            order.ServerName);

        // Send specific notification when order becomes ready (Done status)
        if (order.Status == KitchenStatus.Done)
        {
            await _notificationService.NotifyOrderReadyAsync(
                kitchenOrderId,
                order.TableNumber,
                order.ServerName);
        }

        _logger.LogInformation("Kitchen order {KitchenOrderId} status changed from {PreviousStatus} to {NewStatus}", 
            kitchenOrderId, previousStatus, order.Status);
    }

    public async Task VoidOrderAsync(Guid kitchenOrderId)
    {
        var order = await _kitchenOrderRepository.GetByIdAsync(kitchenOrderId);
        if (order == null)
        {
             throw new Exception($"Kitchen Order not found: {kitchenOrderId}");
        }

        var previousStatus = order.Status;
        order.Void();
        await _kitchenOrderRepository.UpdateAsync(order);

        // Send notification for void status change
        await _notificationService.NotifyOrderStatusChangeAsync(
            kitchenOrderId, 
            order.Status, 
            order.TableNumber, 
            order.ServerName);

        _logger.LogInformation("Kitchen order {KitchenOrderId} voided (was {PreviousStatus})", 
            kitchenOrderId, previousStatus);
    }

    public async Task MarkAsDeliveredAsync(Guid kitchenOrderId)
    {
        var order = await _kitchenOrderRepository.GetByIdAsync(kitchenOrderId);
        if (order == null)
        {
            throw new Exception($"Kitchen Order not found: {kitchenOrderId}");
        }

        // Mark as delivered
        order.MarkAsDelivered();
        await _kitchenOrderRepository.UpdateAsync(order);

        // Calculate preparation time
        var prepTime = order.PreparationTime ?? TimeSpan.Zero;

        // Send notification to POS
        await _notificationService.NotifyOrderDeliveredAsync(
            kitchenOrderId,
            order.TicketId,
            order.TableNumber,
            prepTime);

        _logger.LogInformation(
            "Kitchen order {KitchenOrderId} marked as delivered. Prep time: {PrepTime}s",
            kitchenOrderId, prepTime.TotalSeconds);
    }
}
