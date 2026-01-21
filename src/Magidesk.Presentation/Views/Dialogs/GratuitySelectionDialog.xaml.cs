using Microsoft.UI.Xaml.Controls;
using Magidesk.ViewModels;

namespace Magidesk.Views.Dialogs;

/// <summary>
/// Dialog for selecting and applying gratuity/tips to a ticket.
/// Provides suggested amounts and custom entry with server assignment.
/// </summary>
public sealed partial class GratuitySelectionDialog : ContentDialog
{
    public GratuitySelectionViewModel ViewModel { get; }

    public GratuitySelectionDialog(GratuitySelectionViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        this.InitializeComponent();
    }
}
