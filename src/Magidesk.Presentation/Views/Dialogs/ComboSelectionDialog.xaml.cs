using Microsoft.UI.Xaml.Controls;
using Magidesk.ViewModels.Dialogs;

namespace Magidesk.Views.Dialogs;

public sealed partial class ComboSelectionDialog : ContentDialog
{
    public ComboSelectionViewModel ViewModel { get; }

    public ComboSelectionDialog(ComboSelectionViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        ViewModel.CloseAction = () => Hide();
        
        // Load data when dialog opens
        Loaded += async (s, e) => await ViewModel.InitializeAsync();
    }
}
