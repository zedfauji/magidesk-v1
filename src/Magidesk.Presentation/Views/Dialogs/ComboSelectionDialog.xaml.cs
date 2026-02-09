using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

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
