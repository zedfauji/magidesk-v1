using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to create a new inventory item.
/// </summary>
/// <param name="Name">The name of the inventory item (required, max 200 characters).</param>
/// <param name="Unit">The unit of measure (required, max 50 characters).</param>
/// <param name="StockQuantity">The initial stock quantity (must be >= 0).</param>
/// <param name="ReorderPoint">The reorder point threshold (must be >= 0).</param>
/// <param name="SkuCode">Optional SKU code (max 50 characters, must be unique if provided).</param>
/// <param name="CategoryId">Optional category ID (must reference a valid active category if provided).</param>
public record CreateInventoryItemCommand(
    string Name,
    string Unit,
    decimal StockQuantity,
    decimal ReorderPoint,
    string? SkuCode,
    Guid? CategoryId) : IRequest<Guid>;
