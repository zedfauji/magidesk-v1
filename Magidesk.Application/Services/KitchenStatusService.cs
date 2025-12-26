using System;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class KitchenStatusService : IKitchenStatusService
{
    private readonly IKitchenOrderRepository _kitchenOrderRepository;

    public KitchenStatusService(IKitchenOrderRepository kitchenOrderRepository)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
    }

    public async Task BumpOrderAsync(Guid kitchenOrderId)
    {
        var order = await _kitchenOrderRepository.GetByIdAsync(kitchenOrderId);
        if (order == null)
        {
            throw new Exception($"Kitchen Order not found: {kitchenOrderId}"); 
            // In a real app, use NotFoundException or similar
        }

        order.Bump();
        
        // Repository needs Update method.
        // For EF Core, since we loaded it from context (tracked), calling SaveChanges via Repository (if exposed) or if Repository saves on update.
        // For MVP, if Repository.GetById tracks explicitly, we need to save changes.
        // I need to add UpdateAsync to IKitchenOrderRepository or just a generic SaveChangesAsync.
        // Standard pattern: Repository.Update(entity) -> Context.Update/Attach + SaveChanges.
        
        await _kitchenOrderRepository.UpdateAsync(order);
    }

    public async Task VoidOrderAsync(Guid kitchenOrderId)
    {
        var order = await _kitchenOrderRepository.GetByIdAsync(kitchenOrderId);
        if (order == null)
        {
             throw new Exception($"Kitchen Order not found: {kitchenOrderId}");
        }

        order.Void();
        await _kitchenOrderRepository.UpdateAsync(order);
    }
}
