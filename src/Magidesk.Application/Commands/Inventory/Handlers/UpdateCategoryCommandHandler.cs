using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles updating existing inventory categories.
/// Validates name uniqueness, parent existence, and prevents circular references.
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IInventoryCategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(IInventoryCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Handles the UpdateCategoryCommand by validating inputs and updating the category.
    /// </summary>
    /// <param name="request">The command containing category update data.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when category not found, name already exists, parent not found/inactive, or circular reference detected.
    /// </exception>
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Load existing category
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        // Step 2: Validate name uniqueness if changed
        if (request.Name != category.Name)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(
                request.Name, 
                cancellationToken);
            
            if (existingCategory != null && 
                existingCategory.Id != request.Id && 
                existingCategory.IsActive)
            {
                throw new InvalidOperationException("Category name already exists");
            }
        }

        // Step 3: Validate parent if changed
        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == request.Id)
            {
                throw new InvalidOperationException("Category cannot be its own parent");
            }

            var parent = await _categoryRepository.GetByIdAsync(
                request.ParentCategoryId.Value, 
                cancellationToken);
            
            if (parent == null || !parent.IsActive)
            {
                throw new InvalidOperationException("Parent category not found or inactive");
            }

            // Check for circular reference (parent's parent chain)
            var hasCircularReference = await CheckCircularReferenceAsync(
                request.ParentCategoryId.Value, 
                request.Id, 
                cancellationToken);
            
            if (hasCircularReference)
            {
                throw new InvalidOperationException("Circular category reference detected");
            }
        }

        // Step 4: Update category properties
        category.UpdateName(request.Name);
        category.UpdateSortOrder(request.SortOrder);

        if (request.ParentCategoryId.HasValue)
        {
            category.SetParent(request.ParentCategoryId.Value);
        }
        else
        {
            category.ClearParent();
        }

        // Step 5: Persist changes
        await _categoryRepository.UpdateAsync(category, cancellationToken);
    }

    /// <summary>
    /// Checks if setting a parent would create a circular reference in the category hierarchy.
    /// </summary>
    /// <param name="parentId">The proposed parent category ID.</param>
    /// <param name="targetId">The category being updated.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a circular reference would be created, false otherwise.</returns>
    private async Task<bool> CheckCircularReferenceAsync(
        Guid parentId, 
        Guid targetId, 
        CancellationToken cancellationToken)
    {
        var currentId = parentId;
        var visited = new HashSet<Guid>();

        while (currentId != Guid.Empty)
        {
            // Check if we've reached the target (circular reference)
            if (currentId == targetId)
            {
                return true;
            }

            // Check if we've visited this node (infinite loop protection)
            if (visited.Contains(currentId))
            {
                return true;
            }

            visited.Add(currentId);

            // Move to parent
            var category = await _categoryRepository.GetByIdAsync(currentId, cancellationToken);
            if (category == null)
            {
                return false;
            }

            currentId = category.ParentCategoryId ?? Guid.Empty;
        }

        return false;
    }
}
