using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Magidesk.Presentation.ViewModels;

namespace Magidesk.Presentation.Views;

public sealed partial class OrderTypeExplorerPage : Page
{
    public OrderTypeExplorerViewModel ViewModel { get; }

    public OrderTypeExplorerPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<OrderTypeExplorerViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (ViewModel.LoadCommand.CanExecute(null))
        {
            ViewModel.LoadCommand.Execute(null);
        }
    }
}
