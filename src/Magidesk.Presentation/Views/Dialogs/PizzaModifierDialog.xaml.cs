using Magidesk.Presentation.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views.Dialogs;

public sealed partial class PizzaModifierDialog : ContentDialog
{
    public PizzaModifierViewModel ViewModel
    {
        get => (PizzaModifierViewModel)DataContext;
        set => DataContext = value;
    }

    public PizzaModifierDialog()
    {
        this.InitializeComponent();
        this.Opened += PizzaModifierDialog_Opened;
        this.PrimaryButtonClick += PizzaModifierDialog_PrimaryButtonClick;
        this.CloseButtonClick += PizzaModifierDialog_CloseButtonClick;
    }

    private async void PizzaModifierDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        if (ViewModel != null)
        {
            await ViewModel.InitializeAsync();
        }
    }

    private void PizzaModifierDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel?.ConfirmCommand.Execute(null);
    }

    private void PizzaModifierDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel?.CancelCommand.Execute(null);
    }
}
