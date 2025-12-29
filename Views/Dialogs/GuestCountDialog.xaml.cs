using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class GuestCountDialog : ContentDialog
{
    public GuestCountViewModel ViewModel { get; }

    public GuestCountDialog(GuestCountViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
    }
}
