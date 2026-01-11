using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

/// <summary>
/// Dialog for starting a new table session.
/// </summary>
public sealed partial class StartSessionDialog : ContentDialog
{
    public StartSessionDialogViewModel ViewModel { get; }

    public StartSessionDialog(StartSessionDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        
        InitializeComponent();

        // Wire up RequestClose event
        ViewModel.RequestClose += (s, e) => Hide();
    }
}
