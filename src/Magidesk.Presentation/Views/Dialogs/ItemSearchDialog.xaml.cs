using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels.Dialogs;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class ItemSearchDialog : ContentDialog
{
    public ItemSearchViewModel ViewModel { get; set; }

    public ItemSearchDialog()
    {
        this.InitializeComponent();
    }
}
