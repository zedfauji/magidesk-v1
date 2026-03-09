using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to update an existing inventory item.
/// </summary>
/// <param name="Id">The ID of the inventory item to update (must reference an existing item).</param>
/// <param name="Name">The name of the inventory item (required, max 200 characters).</param>
/// <param name="Unit">The unit of measure (required, max 50 characters).</param>
/// <param name="StockQuantity">The stock quantity (must be >= 0).</param>
/// <param name="ReorderPoint">The reorder point threshold (must be >= 0).</param>
/// <param name="SkuCode">Optional SKU code (max 50 characters, must be unique excluding self if provided).</param>
/// <param name="CategoryId">Optional category ID (must reference a valid active category if provided).</param>
/// <param name="IsActive">Whether the item is active (false for soft delete).</param>
public record UpdateInventoryItemCommand(
    Guid Id,
    string Name,
    string Unit,
    decimal StockQuantity,
    decimal ReorderPoint,
    string? SkuCode,
    Guid? CategoryId,
    bool IsActive) : IRequest;
