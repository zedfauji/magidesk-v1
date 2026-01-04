using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<LoginViewModel>();
        DataContext = ViewModel;
        
        // Ensure Page can receive focus
        this.Loaded += (s, e) => this.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Ensure focus when navigating to this page
        this.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }

    private void Page_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        // Prevent bubbling if we handle it? Maybe.
        // Map Keys
        string? digit = null;

        if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
        {
            digit = (e.Key - Windows.System.VirtualKey.Number0).ToString();
        }
        else if (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9)
        {
            digit = (e.Key - Windows.System.VirtualKey.NumberPad0).ToString();
        }

        if (digit != null)
        {
            if (ViewModel.AppendDigitCommand.CanExecute(digit))
            {
                ViewModel.AppendDigitCommand.Execute(digit);
                e.Handled = true;
            }
        }
        else if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.LoginCommand.CanExecute(null))
            {
                ViewModel.LoginCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (e.Key == Windows.System.VirtualKey.Back)
        {
            if (ViewModel.RemoveLastDigitCommand.CanExecute(null))
            {
                ViewModel.RemoveLastDigitCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
