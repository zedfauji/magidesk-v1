using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for bulk edit dialog DataGrid.
/// Provides editable rows for batch updates of stock quantity and reorder points.
/// </summary>
public partial class InventoryBulkEditViewModel : ObservableObject
{
    public ObservableCollection<InventoryBulkEditRow> EditableItems { get; } = new();

    public event EventHandler<IReadOnlyList<BulkUpdateInventoryItemEntryDto>>? Confirmed;
    public event EventHandler? Cancelled;

    public InventoryBulkEditViewModel(IReadOnlyList<InventoryItemDto> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            EditableItems.Add(new InventoryBulkEditRow
            {
                Id = item.Id,
                Name = item.Name,
                NewStockQuantity = item.StockQuantity,
                NewReorderPoint = item.ReorderPoint,
            });
        }
    }

    [RelayCommand]
    private void Confirm()
    {
        var entries = EditableItems
            .Select(row => new BulkUpdateInventoryItemEntryDto(
                row.Id,
                row.NewStockQuantity,
                row.NewReorderPoint))
            .ToList();

        Confirmed?.Invoke(this, entries);
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
}
