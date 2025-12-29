using Magidesk.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class TicketFeeDialog : ContentDialog
{
    public TicketFeeViewModel ViewModel { get; } = new();

    public TicketFeeDialog()
    {
        this.InitializeComponent();
    }
}
