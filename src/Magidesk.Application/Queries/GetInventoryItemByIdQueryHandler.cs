using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries;

/// <summary>
/// Handles retrieval of a single inventory item by its unique identifier.
/// Returns null if the item is not found.
/// </summary>
public class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemDto?>
{
    private readonly IInventoryItemRepository _repository;
    private readonly IInventoryCategoryRepository _categoryRepository;

    public GetInventoryItemByIdQueryHandler(
        IInventoryItemRepository repository,
        IInventoryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Handles the GetInventoryItemByIdQuery by loading the item and mapping to DTO.
    /// </summary>
    /// <param name="request">The query containing the item ID.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The inventory item DTO if found, null otherwise.</returns>
    public async Task<InventoryItemDto?> Handle(
        GetInventoryItemByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (item == null)
        {
            return null;
        }

        // Load category name if category is assigned
        string? categoryName = null;
        if (item.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(
                item.CategoryId.Value, 
                cancellationToken);
            categoryName = category?.Name;
        }

        // Map to DTO
        return new InventoryItemDto(
            item.Id,
            item.Name,
            item.Unit,
            item.SkuCode,
            item.StockQuantity,
            item.ReorderPoint,
            item.CategoryId,
            categoryName,
            item.CreatedAt,
            item.IsActive);
    }
}
