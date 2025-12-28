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
    }
}
