using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Application.Interfaces;
using MediatR;

namespace Magidesk.Application.Queries;

public class GetCategoryHierarchyQueryHandler : IRequestHandler<GetCategoryHierarchyQuery, GetCategoryHierarchyResult>
{
    private readonly IRepository<MenuCategory> _categoryRepository;

    public GetCategoryHierarchyQueryHandler(IRepository<MenuCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<GetCategoryHierarchyResult> Handle(GetCategoryHierarchyQuery request, CancellationToken cancellationToken)
    {
        // Load all categories
        var allCategories = (await _categoryRepository.GetAllAsync(cancellationToken)).ToList();

        // Build hierarchy (root categories only)
        var rootCategories = allCategories
            .Where(c => c.IsRoot)
            .OrderBy(c => c.SortOrder)
            .Select(c => MapToDto(c, allCategories))
            .ToList();

        return new GetCategoryHierarchyResult(rootCategories);
    }

    private CategoryNodeDto MapToDto(MenuCategory category, List<MenuCategory> allCategories)
    {
        var dto = new CategoryNodeDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentCategoryId,
            SortOrder = category.SortOrder,
            IsVisible = category.IsVisible
        };

        // Recursively map children
        var children = allCategories
            .Where(c => c.ParentCategoryId == category.Id)
            .OrderBy(c => c.SortOrder)
            .Select(c => MapToDto(c, allCategories))
            .ToList();

        dto.Children = children;
        return dto;
    }
}
