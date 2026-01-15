using Magidesk.Api.Dtos.Menu;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly IMenuCategoryRepository _categoryRepository;
    private readonly IMenuRepository _menuRepository;

    public MenuController(
        IMenuCategoryRepository categoryRepository,
        IMenuRepository menuRepository)
    {
        _categoryRepository = categoryRepository;
        _menuRepository = menuRepository;
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<MenuCategoryDto>>> GetCategories()
    {
        // Repo Wrapper
        var categories = await _categoryRepository.GetAllAsync();
        
        // Map to DTO
        return Ok(categories.Select(c => new MenuCategoryDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Subcategories = null // Recursive mapping needed if deep structure exists
        }).ToList());
    }

    [HttpGet("items")]
    public async Task<ActionResult<List<MenuItemDto>>> GetItems([FromQuery] string categoryId)
    {
        if (!Guid.TryParse(categoryId, out var id)) return BadRequest("Invalid Category ID");

        // Gap: Repository typically gets by GroupId, not CategoryId directly unless method exists.
        // Assuming GetItemsByGroupIdAsync is actually GetItemsByCategoryIdAsync or we have to scan.
        // Falling back to GetAll and filtering (inefficient but safe plumbing) or using identified handler method.
        // Map explicitly mentioned: IMenuRepository.GetItemsByGroupIdAsync. 
        // This implies categoryId in request might actually be a GroupId from the UI perspective, 
        // OR we need to lookup groups for the category.
        
        // Simulating the "Best Effort" plumbing:
        var allItems = await _menuRepository.GetActiveItemsAsync(); // Available method in repo
        var filtered = allItems.Where(i => i.CategoryId == id || i.GroupId == id).ToList();

        return Ok(filtered.Select(i => new MenuItemDto
        {
            Id = i.Id.ToString(),
            Name = i.Name,
            Price = i.Price.Amount,
            Description = i.Description,
            CategoryId = i.CategoryId?.ToString() ?? "",
            StockQuantity = i.TrackStock ? i.StockQuantity : null
        }).ToList());
    }

    [HttpGet("items/search")]
    public ActionResult<List<MenuItemDto>> SearchItems([FromQuery] string q)
    {
        // Gap: Logic is in ViewModel memory. No backend query exists.
        throw new NotImplementedException("Backend specific search query not implemented.");
    }

    [HttpGet("items/{menuItemId}/modifiers")]
    public async Task<ActionResult<List<ModifierGroupDto>>> GetItemModifiers(string menuItemId)
    {
        if (!Guid.TryParse(menuItemId, out var id)) return BadRequest("Invalid ID");

        var item = await _menuRepository.GetByIdAsync(id);
        if (item == null) return NotFound();

        // Map Domain ModifierGroups to DTO
        // Requires item.ModifierGroups to be eager loaded (Repo does this by default per audit)
        return Ok(item.ModifierGroups.Select(g => new ModifierGroupDto
        {
            Id = g.Id.ToString(),
            Name = g.Name,
            MinSelection = g.MinSelection,
            MaxSelection = g.MaxSelection,
            Options = g.Options.Select(o => new ModifierOptionDto
            {
                Id = o.Id.ToString(),
                Name = o.Name,
                PriceDelta = o.PriceDelta.Amount
            }).ToList()
        }).ToList());
    }
}
