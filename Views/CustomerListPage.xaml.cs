using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class CustomerListPage : Page
{
    public CustomerListViewModel ViewModel { get; }

    public CustomerListPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CustomerListViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.LoadCommand.Execute(null);
    }
}
