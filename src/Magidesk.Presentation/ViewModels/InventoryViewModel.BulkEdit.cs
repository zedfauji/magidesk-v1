using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// InventoryViewModel partial — bulk edit selection and commitment.
/// Manages SelectedItems collection, bulk edit bar visibility, and delegates bulk update to Application layer.
/// </summary>
public partial class InventoryViewModel
{
    public ObservableCollection<InventoryItemDto> SelectedItems { get; } = new();

    public bool IsBulkEditBarVisible => SelectedItems.Count >= 2;

    public event EventHandler<IReadOnlyList<InventoryItemDto>>? BulkEditRequested;

    [RelayCommand]
    private void ToggleItemSelection(InventoryItemDto item)
    {
        if (SelectedItems.Contains(item))
        {
            SelectedItems.Remove(item);
        }
        else
        {
            SelectedItems.Add(item);
        }

        OnPropertyChanged(nameof(IsBulkEditBarVisible));
    }

    [RelayCommand]
    private void OpenBulkEdit()
    {
        BulkEditRequested?.Invoke(this, SelectedItems.ToList());
    }

    public async Task CommitBulkEditAsync(IReadOnlyList<BulkUpdateInventoryItemEntryDto> entries)
    {
        IsBusy = true;
        try
        {
            var command = new BulkUpdateInventoryItemsCommand(entries, "Bulk edit");
            await _bulkUpdateHandler.HandleAsync(command);

            SelectedItems.Clear();
            OnPropertyChanged(nameof(IsBulkEditBarVisible));

            await LoadPageCommand.ExecuteAsync(null);
            StatusMessage = "Bulk update completed successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Bulk update failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
