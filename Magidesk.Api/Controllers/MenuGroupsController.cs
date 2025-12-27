using Microsoft.AspNetCore.Mvc;
using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using System.Linq;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuGroupsController : ControllerBase
{
    private readonly IMenuGroupRepository _groupRepository;
    private readonly ILogger<MenuGroupsController> _logger;

    public MenuGroupsController(
        IMenuGroupRepository groupRepository,
        ILogger<MenuGroupsController> logger)
    {
        _groupRepository = groupRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all menu groups, optionally filtered by category.
    /// </summary>
    /// <param name="categoryId">Optional category ID to filter groups</param>
    /// <param name="includeHidden">Include hidden groups in the response</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuGroupDto>>> GetGroups(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool includeHidden = false)
    {
        try
        {
            IEnumerable<Domain.Entities.MenuGroup> groups;

            if (categoryId.HasValue)
            {
                groups = await _groupRepository.GetByCategoryIdAsync(categoryId.Value);
            }
            else
            {
                groups = includeHidden
                    ? await _groupRepository.GetAllAsync()
                    : await _groupRepository.GetVisibleAsync();
            }

            var dtos = groups.Select(g => new MenuGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                CategoryId = g.CategoryId,
                CategoryName = g.Category?.Name ?? string.Empty,
                SortOrder = g.SortOrder,
                IsVisible = g.IsVisible,
                ButtonColor = g.ButtonColor
            });

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu groups");
            return StatusCode(500, new { Message = "Error retrieving menu groups" });
        }
    }

    /// <summary>
    /// Get a specific menu group by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuGroupDto>> GetGroup(Guid id)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(id);
            
            if (group == null)
                return NotFound(new { Message = "Group not found" });

            var dto = new MenuGroupDto
            {
                Id = group.Id,
                Name = group.Name,
                CategoryId = group.CategoryId,
                CategoryName = group.Category?.Name ?? string.Empty,
                SortOrder = group.SortOrder,
                IsVisible = group.IsVisible,
                ButtonColor = group.ButtonColor
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu group {GroupId}", id);
            return StatusCode(500, new { Message = "Error retrieving menu group" });
        }
    }
}
