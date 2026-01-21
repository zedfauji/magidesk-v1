namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get menu items.
/// </summary>
public class GetMenuItemsQuery
{
    public Guid? CategoryId { get; set; }
    public bool? IsActive { get; set; } = true;
}

/// <summary>
/// Menu item DTO for queries.
/// </summary>
public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal TaxRate { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public bool IsBeverage { get; set; }
}

