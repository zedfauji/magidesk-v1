using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for Inventory Management — core pagination and loading functionality.
/// Manages ObservableCollections of inventory items and categories, pagination state,
/// and delegates to Application layer handlers for data operations.
/// </summary>
public partial class InventoryViewModel : ViewModelBase
{
    private readonly ICommandHandler<GetInventoryItemsPagedQuery, InventoryItemPagedResultDto> _getPagedHandler;
    private readonly ICommandHandler<GetInventoryCategoriesQuery, IReadOnlyList<InventoryCategoryDto>> _getCategoriesHandler;
    private readonly ICommandHandler<BulkUpdateInventoryItemsCommand> _bulkUpdateHandler;

    public ObservableCollection<InventoryItemDto> InventoryItems { get; } = new();
    public ObservableCollection<InventoryCategoryDto> Categories { get; } = new();

    [ObservableProperty]
    private InventoryCategoryDto? selectedCategory;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private int currentPage;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private int pageSize = 50;

    public bool HasNextPage => (CurrentPage + 1) * PageSize < TotalCount;
    public bool HasPreviousPage => CurrentPage > 0;

    public InventoryViewModel(
        ICommandHandler<GetInventoryItemsPagedQuery, InventoryItemPagedResultDto> getPagedHandler,
        ICommandHandler<GetInventoryCategoriesQuery, IReadOnlyList<InventoryCategoryDto>> getCategoriesHandler,
        ICommandHandler<BulkUpdateInventoryItemsCommand> bulkUpdateHandler)
    {
        _getPagedHandler = getPagedHandler;
        _getCategoriesHandler = getCategoriesHandler;
        _bulkUpdateHandler = bulkUpdateHandler;
        Title = "Inventory Management";
    }

    partial void OnSelectedCategoryChanged(InventoryCategoryDto? value)
    {
        CurrentPage = 0;
        LoadPageCommand.Execute(null);
    }

    partial void OnIsBusyChanged(bool value)
    {
        NotifyCrudCommandsCanExecuteChanged();
    }

    [RelayCommand]
    private async Task LoadPageAsync()
    {
        IsBusy = true;
        try
        {
            var query = new GetInventoryItemsPagedQuery(
                SearchText,
                ActiveFilter,
                SelectedCategory?.Id,
                CurrentPage,
                PageSize);

            var result = await _getPagedHandler.HandleAsync(query);

            InventoryItems.Clear();
            foreach (var item in result.Items)
            {
                InventoryItems.Add(item);
            }

            TotalCount = result.TotalCount;
            StatusMessage = $"Loaded {InventoryItems.Count} items (Page {CurrentPage + 1})";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading inventory: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        try
        {
            var result = await _getCategoriesHandler.HandleAsync(new GetInventoryCategoriesQuery());

            Categories.Clear();
            foreach (var category in result)
            {
                Categories.Add(category);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading categories: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (HasNextPage)
        {
            CurrentPage++;
            await LoadPageCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (HasPreviousPage)
        {
            CurrentPage--;
            await LoadPageCommand.ExecuteAsync(null);
        }
    }
}
