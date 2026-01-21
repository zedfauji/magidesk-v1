using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class CustomerSelectionDialog : ContentDialog
{
    public CustomerSelectionViewModel ViewModel { get; set; } = new();

    public CustomerSelectionDialog()
    {
        InitializeComponent();
    }
}
