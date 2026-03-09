using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands;

public class BulkUpdateInventoryItemsCommandHandler : ICommandHandler<BulkUpdateInventoryItemsCommand>
{
    private readonly IInventoryItemRepository _itemRepository;
    private readonly IInventoryAdjustmentRepository _adjustmentRepository;
    private readonly IUserContextService _userContextService;

    public BulkUpdateInventoryItemsCommandHandler(
        IInventoryItemRepository itemRepository,
        IInventoryAdjustmentRepository adjustmentRepository,
        IUserContextService userContextService)
    {
        _itemRepository = itemRepository;
        _adjustmentRepository = adjustmentRepository;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(
        BulkUpdateInventoryItemsCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetCurrentUserId();

        foreach (var entry in command.Items)
        {
            var item = await _itemRepository.GetByIdAsync(entry.Id, cancellationToken);

            if (item == null)
            {
                throw new InvalidOperationException($"Inventory item with ID {entry.Id} not found.");
            }

            decimal delta = entry.NewStockQuantity - item.StockQuantity;

            // Adjust stock
            item.AdjustStock(delta);

            // Set reorder point
            item.SetReorderPoint(entry.NewReorderPoint);

            // Create adjustment record if delta is non-zero
            if (delta != 0)
            {
                var adjustment = InventoryAdjustment.Create(
                    item.Id,
                    delta,
                    command.AdjustmentReason,
                    userId);

                await _adjustmentRepository.AddAsync(adjustment);
            }

            // Persist item changes
            await _itemRepository.UpdateAsync(item, cancellationToken);
        }
    }
}
