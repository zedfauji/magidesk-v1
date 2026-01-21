using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Views.Dialogs;

public sealed partial class SizeSelectionDialog : ContentDialog
{
    public SizeSelectionViewModel ViewModel { get; set; }

    public SizeSelectionDialog()
    {
        this.InitializeComponent();
    }
}
