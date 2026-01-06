using Magidesk.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

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
