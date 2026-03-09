using CommunityToolkit.Mvvm.ComponentModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Observable row model for bulk edit DataGrid.
/// Represents a single inventory item row in the bulk edit dialog with editable quantity and reorder point.
/// </summary>
public partial class InventoryBulkEditRow : ObservableObject
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;

    [ObservableProperty]
    private decimal newStockQuantity;

    [ObservableProperty]
    private decimal newReorderPoint;
}
