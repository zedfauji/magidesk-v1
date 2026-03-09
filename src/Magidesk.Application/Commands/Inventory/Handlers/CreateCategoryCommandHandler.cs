using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles the creation of new inventory categories.
/// Validates name uniqueness and parent category existence.
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IInventoryCategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(IInventoryCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Handles the CreateCategoryCommand by validating inputs and creating the category.
    /// </summary>
    /// <param name="request">The command containing category creation data.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The unique identifier of the created category.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when category name already exists or parent category is not found/inactive.
    /// </exception>
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate name uniqueness
        var existingCategory = await _categoryRepository.GetByNameAsync(
            request.Name, 
            cancellationToken);
        
        if (existingCategory != null && existingCategory.IsActive)
        {
            throw new InvalidOperationException("Category name already exists");
        }

        // Step 2: Validate parent if provided
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(
                request.ParentCategoryId.Value, 
                cancellationToken);
            
            if (parent == null || !parent.IsActive)
            {
                throw new InvalidOperationException("Parent category not found or inactive");
            }
        }

        // Step 3: Create domain entity
        var category = InventoryCategory.Create(
            request.Name,
            request.SortOrder,
            request.ParentCategoryId);

        // Step 4: Persist to database
        await _categoryRepository.AddAsync(category, cancellationToken);

        // Step 5: Return created category ID
        return category.Id;
    }
}
