using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Represents a product category tab.
/// </summary>
public class ProductCategoryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
    public ObservableCollection<string> Subcategories { get; set; } = new();
}
