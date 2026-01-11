using Magidesk.Presentation.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views
{
    public sealed partial class RefundWizardDialog : ContentDialog
    {
        public RefundWizardDialog()
        {
            this.InitializeComponent();
        }

        private void OnPasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext is RefundWizardViewModel vm && sender is PasswordBox pb)
            {
                vm.ManagerPin = pb.Password;
            }
        }

        private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = true; // Prevent default closing behavior during navigation
            try
            {
                if (DataContext is RefundWizardViewModel vm)
                {
                    if (vm.NextCommand.CanExecute(null))
                    {
                        await vm.NextCommand.ExecuteAsync(null);
                    }
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DataContext is RefundWizardViewModel vm)
            {
                if (vm.BackCommand.CanExecute(null))
                {
                    vm.BackCommand.Execute(null);
                }
            }
        }
    }
}
