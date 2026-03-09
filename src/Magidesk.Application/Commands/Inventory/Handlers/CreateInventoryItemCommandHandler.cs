using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles the creation of new inventory items.
/// Validates category existence, SKU uniqueness, and creates initial stock adjustment records.
/// </summary>
public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IInventoryCategoryRepository _categoryRepository;
    private readonly IRepository<InventoryAdjustment> _adjustmentRepository;
    private readonly IUserContextService _userContextService;

    public CreateInventoryItemCommandHandler(
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
    /// Handles the CreateInventoryItemCommand by validating inputs and creating the inventory item.
    /// </summary>
    /// <param name="request">The command containing item creation data.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The unique identifier of the created inventory item.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when category is not found/inactive or SKU code already exists.
    /// </exception>
    public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate category if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(
                request.CategoryId.Value, 
                cancellationToken);
            
            if (category == null || !category.IsActive)
            {
                throw new InvalidOperationException("Category not found or inactive");
            }
        }

        // Step 2: Validate SKU uniqueness if provided
        if (!string.IsNullOrWhiteSpace(request.SkuCode))
        {
            var existingItem = await _inventoryItemRepository.GetBySkuCodeAsync(
                request.SkuCode, 
                cancellationToken);
            
            if (existingItem != null)
            {
                throw new InvalidOperationException("SKU code already exists");
            }
        }

        // Step 3: Create domain entity
        var item = InventoryItem.Create(
            request.Name,
            request.Unit,
            request.StockQuantity,
            request.ReorderPoint,
            request.SkuCode,
            request.CategoryId);

        // Step 4: Persist to database
        await _inventoryItemRepository.AddAsync(item, cancellationToken);

        // Step 5: Create initial stock adjustment record if stock quantity > 0
        if (request.StockQuantity > 0)
        {
            var userId = _userContextService.GetCurrentUserId();
            var adjustment = InventoryAdjustment.Create(
                item.Id,
                request.StockQuantity,
                "Initial stock",
                userId != Guid.Empty ? userId : null);
            
            await _adjustmentRepository.AddAsync(adjustment, cancellationToken);
        }

        // Step 6: Return created item ID
        return item.Id;
    }
}
