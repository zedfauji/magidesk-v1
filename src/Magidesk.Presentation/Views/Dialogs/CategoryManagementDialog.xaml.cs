using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for managing inventory categories (CRUD operations).
/// </summary>
public sealed partial class CategoryManagementDialog : ContentDialog
{
    /// <summary>
    /// Gets the ViewModel for this dialog.
    /// </summary>
    public CategoryManagementViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryManagementDialog"/> class.
    /// </summary>
    /// <param name="viewModel">The ViewModel to bind to this dialog.</param>
    public CategoryManagementDialog(CategoryManagementViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        
        // Load categories when dialog is initialized
        _ = ViewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }
}
