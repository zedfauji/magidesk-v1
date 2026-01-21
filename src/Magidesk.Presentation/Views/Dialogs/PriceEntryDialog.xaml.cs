using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Views.Dialogs;

public sealed partial class PriceEntryDialog : ContentDialog
{
    public PriceEntryViewModel ViewModel { get; set; }

    public PriceEntryDialog()
    {
        this.InitializeComponent();
    }
}
