using Microsoft.AspNetCore.Mvc;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using System.Linq;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuCategoriesController : ControllerBase
{
    private readonly IMenuCategoryRepository _categoryRepository;
    private readonly ILogger<MenuCategoriesController> _logger;

    public MenuCategoriesController(
        IMenuCategoryRepository categoryRepository,
        ILogger<MenuCategoriesController> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all menu categories, ordered by sort order.
    /// </summary>
    /// <param name="includeHidden">Include hidden categories in the response</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuCategoryDto>>> GetCategories(
        [FromQuery] bool includeHidden = false)
    {
        try
        {
            var categories = includeHidden
                ? await _categoryRepository.GetAllAsync()
                : await _categoryRepository.GetVisibleAsync();

            var dtos = categories.Select(c => new MenuCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder,
                IsVisible = c.IsVisible,
                IsBeverage = c.IsBeverage,
                ButtonColor = c.ButtonColor
            });

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu categories");
            return StatusCode(500, new { Message = "Error retrieving menu categories" });
        }
    }

    /// <summary>
    /// Get a specific menu category by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuCategoryDto>> GetCategory(Guid id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
                return NotFound(new { Message = "Category not found" });

            var dto = new MenuCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder,
                IsVisible = category.IsVisible,
                IsBeverage = category.IsBeverage,
                ButtonColor = category.ButtonColor
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu category {CategoryId}", id);
            return StatusCode(500, new { Message = "Error retrieving menu category" });
        }
    }
}
