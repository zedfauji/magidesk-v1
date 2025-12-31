using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class ShiftEndDialog : ContentDialog
{
    public ShiftEndViewModel ViewModel { get; }

    public ShiftEndDialog(ShiftEndViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        this.DataContext = ViewModel;
        
        ViewModel.CloseAction = () =>
        {
            this.Hide();
        };
    }
}
