using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IInventoryCategoryRepository
{
    Task<IReadOnlyList<InventoryCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<InventoryCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves an inventory category by its name.
    /// </summary>
    /// <param name="name">The name of the category to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The category with the specified name, or null if no category exists with that name.</returns>
    Task<InventoryCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the number of active child categories under a specific parent category.
    /// </summary>
    /// <param name="parentCategoryId">The ID of the parent category.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The count of active child categories (IsActive = true) under the specified parent.</returns>
    Task<int> CountActiveChildCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
    
    Task AddAsync(InventoryCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryCategory category, CancellationToken cancellationToken = default);
}
