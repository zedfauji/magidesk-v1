using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for holding a ticket with a reason.
/// </summary>
public sealed partial class HoldTicketDialog : ContentDialog
{
    public HoldTicketDialogViewModel ViewModel { get; }

    public HoldTicketDialog(HoldTicketDialogViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    /// <summary>
    /// Gets the result of the hold operation.
    /// </summary>
    public bool IsSuccess => ViewModel.IsSuccess;
}
