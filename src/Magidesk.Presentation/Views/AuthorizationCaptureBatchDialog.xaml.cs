using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation;

namespace Magidesk.Views
{
    public sealed partial class AuthorizationCaptureBatchDialog : ContentDialog
    {
        public AuthorizationCaptureBatchViewModel ViewModel { get; }

        public AuthorizationCaptureBatchDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<AuthorizationCaptureBatchViewModel>();
            this.DataContext = ViewModel;
            
            this.Opened += AuthorizationCaptureBatchDialog_Opened;
        }

        private async void AuthorizationCaptureBatchDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Auto-start the batch when dialog opens
            await ViewModel.StartBatchCommand.ExecuteAsync(null);
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // Prevent closing if processing is active
            if (ViewModel.IsProcessing)
            {
                args.Cancel = true;
            }
        }
    }
}
