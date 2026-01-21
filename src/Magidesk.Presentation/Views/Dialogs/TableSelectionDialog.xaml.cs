using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class TableSelectionDialog : ContentDialog
{
    public TableSelectionViewModel ViewModel { get; set; }

    public TableSelectionDialog()
    {
        InitializeComponent();
    }
}
