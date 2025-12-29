using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Magidesk.Views
{
    public sealed partial class AuthorizationCodeDialog : ContentDialog
    {
        public AuthorizationCodeViewModel ViewModel { get; }

        public AuthorizationCodeDialog()
        {
            this.InitializeComponent();
            ViewModel = App.Services.GetRequiredService<AuthorizationCodeViewModel>();
            this.DataContext = ViewModel;
        }

        private void CardType_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string cardType)
            {
                ViewModel.SelectedCardType = cardType;
            }
        }
    }
}
