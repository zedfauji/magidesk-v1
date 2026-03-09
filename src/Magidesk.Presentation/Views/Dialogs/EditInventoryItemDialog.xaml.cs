using System;
using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for editing existing inventory items.
/// </summary>
public sealed partial class EditInventoryItemDialog : ContentDialog
{
    /// <summary>
    /// Gets the ViewModel for this dialog.
    /// </summary>
    public EditInventoryItemViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditInventoryItemDialog"/> class.
    /// </summary>
    /// <param name="viewModel">The ViewModel to bind to this dialog.</param>
    /// <param name="itemId">The ID of the item to edit.</param>
    public EditInventoryItemDialog(EditInventoryItemViewModel viewModel, Guid itemId)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
        
        // Load item data when dialog is initialized
        _ = ViewModel.LoadItemAsync(itemId);
    }
}
