using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.Views.Dialogs;

public sealed partial class ShiftStartDialog : ContentDialog
{
    public ShiftStartViewModel ViewModel { get; }

    public ShiftStartDialog()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ShiftStartViewModel>();
        this.DataContext = ViewModel;
        
        ViewModel.CloseAction = (result) =>
        {
            this.Hide();
        };

        this.Opened += ShiftStartDialog_Opened;
    }

    private async void ShiftStartDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.InitializeAsync();
    }
}
