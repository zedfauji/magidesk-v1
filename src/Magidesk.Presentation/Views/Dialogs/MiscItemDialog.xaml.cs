using Magidesk.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class MiscItemDialog : ContentDialog
{
    public MiscItemViewModel ViewModel { get; set; }

    public MiscItemDialog()
    {
        this.InitializeComponent();
        ViewModel = new MiscItemViewModel();
    }

    public MiscItemDialog(MiscItemViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
    }
}
