using System;
using System.Collections.Generic;
using MediatR;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to retrieve the category hierarchy for tree view display.
/// </summary>
public record GetCategoryHierarchyQuery : IRequest<GetCategoryHierarchyResult>;

public record GetCategoryHierarchyResult(
    List<CategoryNodeDto> RootCategories
);

/// <summary>
/// DTO representing a category node in the hierarchy tree.
/// </summary>
public class CategoryNodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public List<CategoryNodeDto> Children { get; set; } = new();
}
