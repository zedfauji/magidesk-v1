namespace Magidesk.Api.Dtos.Menu;

public class MenuCategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<MenuCategoryDto>? Subcategories { get; set; }
}

public class MenuItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public int? StockQuantity { get; set; }
}

public class ModifierGroupDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinSelection { get; set; }
    public int MaxSelection { get; set; }
    public List<ModifierOptionDto> Options { get; set; } = new();
}

public class ModifierOptionDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal PriceDelta { get; set; }
}
