using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class ModifierSelectionDialog : ContentDialog
{
    public ModifierSelectionViewModel ViewModel { get; }

    public ModifierSelectionDialog(ModifierSelectionViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        ViewModel.CloseAction = () => Hide();
        
        // Load data when dialog opens
        Loaded += async (s, e) => await ViewModel.InitializeAsync();
    }
}
