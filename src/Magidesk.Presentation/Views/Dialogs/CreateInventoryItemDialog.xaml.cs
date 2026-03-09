using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for creating new inventory items.
/// </summary>
public sealed partial class CreateInventoryItemDialog : ContentDialog
{
    /// <summary>
    /// Gets the ViewModel for this dialog.
    /// </summary>
    public CreateInventoryItemViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInventoryItemDialog"/> class.
    /// </summary>
    /// <param name="viewModel">The ViewModel to bind to this dialog.</param>
    public CreateInventoryItemDialog(CreateInventoryItemViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        
        // Load categories when dialog is initialized
        _ = ViewModel.LoadCategoriesAsync();
    }
}
