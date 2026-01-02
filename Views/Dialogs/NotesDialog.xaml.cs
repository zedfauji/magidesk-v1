using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class NotesDialog : ContentDialog
{
    public object ViewModel { get; }

    public NotesDialog(object viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;

        // Hook up close event
        if (viewModel is NotesDialogViewModel vm)
        {
            vm.RequestClose += (s, e) => Hide();
        }
    }
}
