using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Magidesk.Views
{
    public sealed partial class SwipeCardDialog : ContentDialog
    {
        public SwipeCardViewModel ViewModel { get; }

        public SwipeCardDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<SwipeCardViewModel>();
            this.DataContext = ViewModel;
            this.Opened += SwipeCardDialog_Opened;
        }

        private void SwipeCardDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Focus the hidden input box to capture swipe
            SwipeInputBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }

        private void SwipeInputBox_PasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // We can capture data here if needed, but PasswordBox doesn't bind easily.
            // We'll rely on KeyDown/Enter or just reading the Password property on submit.
             if (ViewModel != null)
            {
                ViewModel.SwipeData = SwipeInputBox.Password;
            }
        }

        private void SwipeInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Swipe completed (usually ends with Enter)
                // Assume success for now and close with "Primary" result equivalent (or a Custom one if we could).
                // For ContentDialog, we usually simulate a button press or just Hide().
                // Here we will treat 'Enter' as a successful swipe capture if data is present.
                if (!string.IsNullOrEmpty(SwipeInputBox.Password))
                {
                    // In a real app we'd validate track data structure.
                    Hide(); // Closes dialog, result will be None unless we set it. 
                            // This needs better handling in a real production app (Custom Result).
                }
            }
        }
    }
}
