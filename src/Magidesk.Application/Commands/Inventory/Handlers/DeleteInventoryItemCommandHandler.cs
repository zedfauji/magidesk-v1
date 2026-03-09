using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Commands.Inventory.Handlers;

/// <summary>
/// Handles the deletion (soft delete) of inventory items.
/// Validates that the item exists and is not referenced in active orders before deactivating.
/// </summary>
public class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly ITicketRepository _ticketRepository;

    public DeleteInventoryItemCommandHandler(
        IInventoryItemRepository inventoryItemRepository,
        ITicketRepository ticketRepository)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _ticketRepository = ticketRepository;
    }

    /// <summary>
    /// Handles the DeleteInventoryItemCommand by performing soft delete (deactivation).
    /// </summary>
    /// <param name="request">The command containing the item ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A completed task.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when item is not found or has active order references.
    /// </exception>
    public async Task Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Load existing item
        var item = await _inventoryItemRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (item == null)
        {
            throw new InvalidOperationException("Item not found");
        }

        // Step 2: Check for active order references (business rule)
        var hasActiveReferences = await _ticketRepository.HasActiveOrdersWithItemAsync(
            request.Id, 
            cancellationToken);
        
        if (hasActiveReferences)
        {
            throw new InvalidOperationException("Cannot delete item with active order references");
        }

        // Step 3: Soft delete (deactivate)
        item.Deactivate();

        // Step 4: Persist changes
        await _inventoryItemRepository.UpdateAsync(item, cancellationToken);
    }
}
