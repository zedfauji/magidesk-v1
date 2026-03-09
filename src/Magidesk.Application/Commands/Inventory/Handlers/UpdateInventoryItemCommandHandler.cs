using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles the update of existing inventory items.
/// Validates category existence, SKU uniqueness, and creates stock adjustment records when quantity changes.
/// </summary>
public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IInventoryCategoryRepository _categoryRepository;
    private readonly IRepository<InventoryAdjustment> _adjustmentRepository;
    private readonly IUserContextService _userContextService;

    public UpdateInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        IInventoryCategoryRepository categoryRepository,
        IRepository<InventoryAdjustment> adjustmentRepository,
        IUserContextService userContextService)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _categoryRepository = categoryRepository;
        _adjustmentRepository = adjustmentRepository;
        _userContextService = userContextService;
    }

    /// <summary>
    /// Handles the UpdateInventoryItemCommand by validating inputs and updating the inventory item.
    /// </summary>
    /// <param name="request">The command containing item update data.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when item is not found, category is not found/inactive, or SKU code already exists.
    /// </exception>
    public async Task Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Load existing item
        var item = await _inventoryItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            throw new InvalidOperationException("Item not found");
        }

        // Step 2: Validate category if changed
        if (request.CategoryId.HasValue && request.CategoryId != item.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(
                request.CategoryId.Value, 
                cancellationToken);
            
            if (category == null || !category.IsActive)
            {
                throw new InvalidOperationException("Category not found or inactive");
            }
        }

        // Step 3: Validate SKU uniqueness if changed
        if (!string.IsNullOrWhiteSpace(request.SkuCode) && request.SkuCode != item.SkuCode)
        {
            var existingItem = await _inventoryItemRepository.GetBySkuCodeAsync(
                request.SkuCode, 
                cancellationToken);
            
            if (existingItem != null && existingItem.Id != request.Id)
            {
                throw new InvalidOperationException("SKU code already exists");
            }
        }

        // Step 4: Calculate stock delta
        var stockDelta = request.StockQuantity - item.StockQuantity;

        // Step 5: Update item properties
        item.UpdateName(request.Name);
        item.UpdateUnit(request.Unit);
        item.SetReorderPoint(request.ReorderPoint);

        if (!string.IsNullOrWhiteSpace(request.SkuCode))
        {
            item.UpdateSkuCode(request.SkuCode);
        }

        if (request.CategoryId.HasValue)
        {
            item.AssignCategory(request.CategoryId.Value);
        }
        else
        {
            item.ClearCategory();
        }

        if (request.IsActive)
        {
            item.Activate();
        }
        else
        {
            item.Deactivate();
        }

        // Step 6: Adjust stock if changed
        if (stockDelta != 0)
        {
            item.AdjustStock(stockDelta);

            var userId = _userContextService.GetCurrentUserId();
            var adjustment = InventoryAdjustment.Create(
                item.Id,
                stockDelta,
                "Stock adjustment via update",
                userId != Guid.Empty ? userId : null);
            
            await _adjustmentRepository.AddAsync(adjustment, cancellationToken);
        }

        // Step 7: Persist changes
        await _inventoryItemRepository.UpdateAsync(item, cancellationToken);
    }
}
