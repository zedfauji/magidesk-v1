using System;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Commands.Inventory;
using Magidesk.Application.Queries;
using Microsoft.Extensions.Logging;

namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// Command execution logic for CategoryManagementViewModel.
/// </summary>
public partial class CategoryManagementViewModel
{
    private async Task ExecuteLoadCategoriesAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = null;

            var query = new GetInventoryCategoriesQuery();
            var categories = await _getCategoriesHandler.HandleAsync(query);

            Categories.Clear();
            foreach (var category in categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                Categories.Add(category);
            }

            _logger.LogInformation("Loaded {Count} categories", categories.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
            StatusMessage = "Failed to load categories. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExecuteCreateCategoryAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = null;

            var command = new CreateCategoryCommand(
                NewCategoryName,
                NewCategorySortOrder,
                null); // No parent category support in this version

            var categoryId = await _mediator.Send(command);

            _logger.LogInformation("Created category {CategoryId} with name {Name}", categoryId, NewCategoryName);

            // Clear form
            NewCategoryName = string.Empty;
            NewCategorySortOrder = 0;
            _validationErrors.Clear();

            // Reload categories
            await LoadCategoriesCommand.ExecuteAsync(null);

            StatusMessage = "Category created successfully";
            CategoriesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create category: {Message}", ex.Message);
            StatusMessage = $"Create failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating category");
            StatusMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanExecuteCreateCategory()
    {
        return CanCreateCategory && !IsBusy;
    }

    private async Task ExecuteUpdateCategoryAsync()
    {
        if (SelectedCategory == null)
        {
            StatusMessage = "No category selected";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = null;

            var command = new UpdateCategoryCommand(
                SelectedCategory.Id,
                EditCategoryName,
                EditCategorySortOrder,
                SelectedCategory.ParentCategoryId);

            await _mediator.Send(command);

            _logger.LogInformation("Updated category {CategoryId} with name {Name}", 
                SelectedCategory.Id, EditCategoryName);

            // Exit edit mode
            IsEditMode = false;
            SelectedCategory = null;
            _validationErrors.Clear();

            // Reload categories
            await LoadCategoriesCommand.ExecuteAsync(null);

            StatusMessage = "Category updated successfully";
            CategoriesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update category: {Message}", ex.Message);
            StatusMessage = $"Update failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating category");
            StatusMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanExecuteUpdateCategory()
    {
        return CanUpdateCategory && !IsBusy && IsEditMode;
    }

    private async Task ExecuteDeleteCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            StatusMessage = "Invalid category ID";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = null;

            var command = new DeleteCategoryCommand(categoryId);
            await _mediator.Send(command);

            _logger.LogInformation("Deleted category {CategoryId}", categoryId);

            // Exit edit mode if the deleted category was selected
            if (SelectedCategory?.Id == categoryId)
            {
                IsEditMode = false;
                SelectedCategory = null;
            }

            // Reload categories
            await LoadCategoriesCommand.ExecuteAsync(null);

            StatusMessage = "Category deleted successfully";
            CategoriesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete category: {Message}", ex.Message);
            StatusMessage = $"Delete failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting category");
            StatusMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ExecuteEnterEditMode()
    {
        if (SelectedCategory == null)
        {
            StatusMessage = "Please select a category to edit";
            return;
        }

        IsEditMode = true;
        EditCategoryName = SelectedCategory.Name;
        EditCategorySortOrder = SelectedCategory.SortOrder;
        _validationErrors.Clear();
        StatusMessage = null;

        UpdateCategoryCommand.NotifyCanExecuteChanged();
    }

    private bool CanExecuteEnterEditMode()
    {
        return SelectedCategory != null && !IsEditMode && !IsBusy;
    }

    private void ExecuteCancelEdit()
    {
        IsEditMode = false;
        EditCategoryName = string.Empty;
        EditCategorySortOrder = 0;
        _validationErrors.Clear();
        StatusMessage = null;

        UpdateCategoryCommand.NotifyCanExecuteChanged();
    }
}
