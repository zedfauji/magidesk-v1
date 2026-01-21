using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class EndSessionDialog : ContentDialog
{
    public EndSessionDialogViewModel ViewModel { get; }

    public EndSessionDialog(EndSessionDialogViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        
        this.InitializeComponent();

        // Wire up RequestClose event
        ViewModel.RequestClose += (s, e) => Hide();
    }
}
