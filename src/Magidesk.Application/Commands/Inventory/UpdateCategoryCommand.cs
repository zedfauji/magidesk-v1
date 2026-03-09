using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to update an existing inventory category.
/// </summary>
/// <param name="Id">The ID of the category to update (must reference an existing category).</param>
/// <param name="Name">The name of the category (required, max 100 characters, must be unique among active categories excluding self).</param>
/// <param name="SortOrder">The sort order for display (must be >= 0).</param>
/// <param name="ParentCategoryId">Optional parent category ID (must reference a valid active category if provided, cannot be self).</param>
/// <remarks>
/// This command validates against circular references in the category hierarchy.
/// A category cannot be set as its own parent, nor can it be set as a descendant of itself.
/// </remarks>
public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    int SortOrder,
    Guid? ParentCategoryId) : IRequest;
