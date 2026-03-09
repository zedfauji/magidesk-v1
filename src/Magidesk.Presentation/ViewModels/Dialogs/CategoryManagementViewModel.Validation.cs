namespace Magidesk.Presentation.ViewModels.Dialogs;

/// <summary>
/// Validation logic for CategoryManagementViewModel.
/// </summary>
public partial class CategoryManagementViewModel
{
    private void ValidateNewCategoryName()
    {
        if (string.IsNullOrWhiteSpace(_newCategoryName))
        {
            _validationErrors[nameof(NewCategoryName)] = "Category name is required.";
        }
        else if (_newCategoryName.Length > 100)
        {
            _validationErrors[nameof(NewCategoryName)] = "Category name must not exceed 100 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(NewCategoryName));
        }
    }

    private void ValidateNewCategorySortOrder()
    {
        if (_newCategorySortOrder < 0)
        {
            _validationErrors[nameof(NewCategorySortOrder)] = "Sort order must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(NewCategorySortOrder));
        }
    }

    private void ValidateEditCategoryName()
    {
        if (string.IsNullOrWhiteSpace(_editCategoryName))
        {
            _validationErrors[nameof(EditCategoryName)] = "Category name is required.";
        }
        else if (_editCategoryName.Length > 100)
        {
            _validationErrors[nameof(EditCategoryName)] = "Category name must not exceed 100 characters.";
        }
        else
        {
            _validationErrors.Remove(nameof(EditCategoryName));
        }
    }

    private void ValidateEditCategorySortOrder()
    {
        if (_editCategorySortOrder < 0)
        {
            _validationErrors[nameof(EditCategorySortOrder)] = "Sort order must be non-negative.";
        }
        else
        {
            _validationErrors.Remove(nameof(EditCategorySortOrder));
        }
    }
}
