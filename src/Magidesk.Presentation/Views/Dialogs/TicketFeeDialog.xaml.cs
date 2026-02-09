using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class TicketFeeDialog : ContentDialog
{
    public TicketFeeViewModel ViewModel { get; set; }
    public Action? CloseAction { get; set; }

    public TicketFeeDialog()
    {
        this.InitializeComponent();
        ViewModel = new TicketFeeViewModel();
    }

    public TicketFeeDialog(TicketFeeViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
    }
}
