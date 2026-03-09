using MediatR;

namespace Magidesk.Application.Commands.Inventory;

/// <summary>
/// Command to create a new inventory category.
/// </summary>
/// <param name="Name">The name of the category (required, max 100 characters, must be unique among active categories).</param>
/// <param name="SortOrder">The sort order for display (must be >= 0).</param>
/// <param name="ParentCategoryId">Optional parent category ID (must reference a valid active category if provided).</param>
public record CreateCategoryCommand(
    string Name,
    int SortOrder,
    Guid? ParentCategoryId) : IRequest<Guid>;
