using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for hierarchical category tree view (Feature G.4).
/// </summary>
public partial class CategoryTreeViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<CategoryNodeDto> _rootCategories = new();

    [ObservableProperty]
    private CategoryNodeDto? _selectedCategory;

    [ObservableProperty]
    private bool _isLoading;

    public CategoryTreeViewModel(IMediator mediator, IDialogService dialogService)
    {
        _mediator = mediator;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        try
        {
            IsLoading = true;
            var query = new GetCategoryHierarchyQuery();
            var result = await _mediator.Send(query);

            RootCategories.Clear();
            foreach (var category in result.RootCategories)
            {
                RootCategories.Add(category);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Failed to load categories",  $"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangeParentAsync(Tuple<Guid, Guid?> parameters)
    {
        var (categoryId, newParentId) = parameters;
        
        try
        {
            var command = new SetCategoryParentCommand(categoryId, newParentId);
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                await LoadCategoriesAsync(); // Refresh tree
            }
            else
            {
                await _dialogService.ShowErrorAsync("Cannot change parent", result.Message);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Failed to change category parent", $"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddSubcategoryAsync(Guid parentId)
    {
        // TODO: Show dialog to create new category with parent set
        await _dialogService.ShowMessageAsync("Feature", "Add subcategory functionality will be implemented in category editor dialog.");
    }

    [RelayCommand]
    private async Task DeleteCategoryAsync(Guid categoryId)
    {
        try
        {
            // Find the category to check if it has children
            var category = FindCategoryById(categoryId, RootCategories);
            if (category == null)
            {
                await _dialogService.ShowErrorAsync("Category not found", "The selected category could not be found.");
                return;
            }

            // Validation: Prevent deletion if has children
            if (category.Children.Count > 0)
            {
                await _dialogService.ShowErrorAsync(
                    "Cannot delete category",
                    $"Category '{category.Name}' has {category.Children.Count} subcategories. Please delete or move the subcategories first.");
                return;
            }

            // Confirm deletion
            var confirmed = await _dialogService.ShowConfirmationAsync(
                $"Delete '{category.Name}'?",
                "This action cannot be undone.");

            if (!confirmed)
                return;

            // TODO: Send delete command to backend
            await _dialogService.ShowMessageAsync("Feature", "Delete command will be implemented with backend support.");
            await LoadCategoriesAsync(); // Refresh
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Failed to delete category", $"Error: {ex.Message}");
        }
    }

    private CategoryNodeDto? FindCategoryById(Guid id, IEnumerable<CategoryNodeDto> categories)
    {
        foreach (var category in categories)
        {
            if (category.Id == id)
                return category;

            var found = FindCategoryById(id, category.Children);
            if (found != null)
                return found;
        }
        return null;
    }
}
