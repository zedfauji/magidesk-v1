using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to delete (soft delete) an inventory item.
/// </summary>
/// <param name="Id">The ID of the inventory item to delete (must reference an existing item).</param>
/// <remarks>
/// This command performs a soft delete by setting IsActive = false.
/// The item must not be referenced in any active orders.
/// The item will remain in the database but will be marked as inactive.
/// </remarks>
public record DeleteInventoryItemCommand(Guid Id) : IRequest;
