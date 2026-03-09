using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles deletion (soft delete) of inventory categories.
/// Validates that no active items or child categories are assigned before deletion.
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IInventoryCategoryRepository _categoryRepository;
    private readonly IInventoryItemRepository _itemRepository;

    public DeleteCategoryCommandHandler(
        IInventoryCategoryRepository categoryRepository,
        IInventoryItemRepository itemRepository)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Handles the DeleteCategoryCommand by validating constraints and soft-deleting the category.
    /// </summary>
    /// <param name="request">The command containing the category ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when category not found, has assigned items, or has child categories.
    /// </exception>
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Load existing category
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Step 2: Check for assigned items
        var itemCount = await _itemRepository.CountActiveItemsByCategoryAsync(
            request.Id, 
            cancellationToken);
        
        if (itemCount > 0)
        {
            throw new InvalidOperationException("Cannot delete category with assigned items");
        }

        // Step 3: Check for child categories
        var childCount = await _categoryRepository.CountActiveChildCategoriesAsync(
            request.Id, 
            cancellationToken);
        
        if (childCount > 0)
        {
            throw new InvalidOperationException("Cannot delete category with child categories");
        }

        // Step 4: Soft delete (deactivate)
        category.Deactivate();

        // Step 5: Persist changes
        await _categoryRepository.UpdateAsync(category, cancellationToken);
    }
}
