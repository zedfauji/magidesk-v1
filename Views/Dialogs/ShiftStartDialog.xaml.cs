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
        // FEH-003: Async Void Barrier
        try
        {
            await ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            try
            {
                var dialogService = App.Services.GetRequiredService<Magidesk.Application.Interfaces.IDialogService>();
                await dialogService.ShowErrorAsync("Shift Start Error", "Failed to initialize shift dialog.", ex.ToString());
                 this.Hide(); 
            }
            catch
            {
                // Fallback if DialogService fails
                this.Hide();
            }
        }
    }
}
