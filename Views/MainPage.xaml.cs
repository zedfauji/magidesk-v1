using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.Views
{
    /// <summary>
    /// Utility page used as a fallback when no ticket is selected for editing.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly NavigationService _navigation;

        public MainPage()
        {
            this.InitializeComponent();
            _navigation = App.Services.GetRequiredService<NavigationService>();
        }

        private void OnGoBackClicked(object sender, RoutedEventArgs e)
        {
            if (_navigation.CanGoBack)
            {
                _navigation.GoBack();
            }
        }
    }
}
