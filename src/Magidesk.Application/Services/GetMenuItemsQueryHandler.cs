using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetMenuItemsQuery.
/// </summary>
public class GetMenuItemsQueryHandler : IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenuItemsQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<MenuItemDto>> HandleAsync(GetMenuItemsQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Entities.MenuItem> menuItems;

        // Get menu items based on query parameters
        if (query.CategoryId.HasValue)
        {
            menuItems = await _menuRepository.GetByGroupAsync(query.CategoryId.Value, cancellationToken);
        }
        else
        {
            menuItems = await _menuRepository.GetAllAsync(cancellationToken);
        }

        // Filter by active status if specified
        if (query.IsActive.HasValue)
        {
            menuItems = menuItems.Where(m => m.IsActive == query.IsActive.Value);
        }

        // Map to DTOs
        var result = menuItems.Select(m => new MenuItemDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price.Amount, // Extract decimal amount from Money value object
            TaxRate = m.TaxRate,
            CategoryId = m.GroupId,
            CategoryName = null, // Can be populated if needed by joining with category data
            IsActive = m.IsActive,
            IsBeverage = false // Default to false, can be enhanced later if needed
        }).ToList();

        return result;
    }
}
