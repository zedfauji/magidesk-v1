using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class UserManagementPage : Page
{
    public UserManagementViewModel ViewModel { get; }

    public UserManagementPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<UserManagementViewModel>();
        DataContext = ViewModel;
    }
}
