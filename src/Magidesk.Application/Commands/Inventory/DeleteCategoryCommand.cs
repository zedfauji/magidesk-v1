using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to delete (soft delete) an inventory category.
/// </summary>
/// <param name="Id">The ID of the category to delete (must reference an existing category).</param>
/// <remarks>
/// This command performs a soft delete by setting IsActive = false.
/// The category must not have any active inventory items assigned to it.
/// The category must not have any active child categories.
/// The category will remain in the database but will be marked as inactive.
/// </remarks>
public record DeleteCategoryCommand(Guid Id) : IRequest;
