namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Represents a product in the catalog grid.
/// </summary>
public class ProductViewModel
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SubcategoryName { get; set; } = string.Empty;
    public bool HasModifiers { get; set; }
    public bool IsAvailable { get; set; }
}
