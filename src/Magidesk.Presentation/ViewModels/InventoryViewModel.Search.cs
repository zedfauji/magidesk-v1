using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// InventoryViewModel partial — search and filter functionality.
/// Manages SearchText with debouncing, ActiveFilter state, and filter chip visibility properties.
/// </summary>
public partial class InventoryViewModel
{
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private InventoryFilterType activeFilter = InventoryFilterType.None;

    public bool IsFilterAll => ActiveFilter == InventoryFilterType.None;
    public bool IsFilterLowStock => ActiveFilter == InventoryFilterType.LowStock;
    public bool IsFilterOutOfStock => ActiveFilter == InventoryFilterType.OutOfStock;
    public bool IsFilterRecentlyAdded => ActiveFilter == InventoryFilterType.RecentlyAdded;

    partial void OnSearchTextChanged(string value)
    {
        CurrentPage = 0;
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();

        var cts = _searchCts;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(300, cts.Token);
                if (!cts.Token.IsCancellationRequested)
                {
                    await LoadPageCommand.ExecuteAsync(null);
                }
            }
            catch (OperationCanceledException)
            {
                // Search was cancelled, do nothing
            }
        });
    }

    partial void OnActiveFilterChanged(InventoryFilterType value)
    {
        OnPropertyChanged(nameof(IsFilterAll));
        OnPropertyChanged(nameof(IsFilterLowStock));
        OnPropertyChanged(nameof(IsFilterOutOfStock));
        OnPropertyChanged(nameof(IsFilterRecentlyAdded));
    }

    [RelayCommand]
    private void SetFilter(InventoryFilterType filter)
    {
        ActiveFilter = filter;
        CurrentPage = 0;
        LoadPageCommand.Execute(null);
    }
}
