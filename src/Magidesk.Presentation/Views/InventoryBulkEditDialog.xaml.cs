using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class InventoryBulkEditDialog : ContentDialog
{
    public InventoryBulkEditViewModel? ViewModel { get; set; }

    public InventoryBulkEditDialog()
    {
        this.InitializeComponent();
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel?.ConfirmCommand.Execute(null);
    }

    private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel?.CancelCommand.Execute(null);
    }
}
