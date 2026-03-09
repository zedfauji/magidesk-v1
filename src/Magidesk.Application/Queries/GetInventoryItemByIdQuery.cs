using MediatR;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to retrieve a single inventory item by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the inventory item to retrieve.</param>
/// <remarks>
/// Returns null if the item is not found.
/// This query is used for loading item data into edit dialogs.
/// </remarks>
public record GetInventoryItemByIdQuery(Guid Id) : IRequest<InventoryItemDto?>;
