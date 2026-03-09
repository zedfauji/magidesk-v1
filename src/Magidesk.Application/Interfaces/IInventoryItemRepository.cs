using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Interfaces;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<InventoryItem> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        InventoryFilterType filter,
        Guid? categoryId,
        int skip,
        int take,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves an inventory item by its SKU code.
    /// </summary>
    /// <param name="skuCode">The SKU code to search for.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The inventory item with the specified SKU code, or null if not found.</returns>
    Task<InventoryItem?> GetBySkuCodeAsync(string skuCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the number of active inventory items assigned to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The count of active items (IsActive = true) assigned to the specified category.</returns>
    Task<int> CountActiveItemsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
