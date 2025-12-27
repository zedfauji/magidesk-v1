using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

public sealed partial class TableMapPage : Page
{
    public TableMapViewModel ViewModel { get; }

    public TableMapPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TableMapViewModel>();
        this.Name = "RootPage"; // For ElementName binding
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Guid ticketId)
        {
            ViewModel.SetContext(ticketId);
        }
        else
        {
            ViewModel.SetContext(null);
        }

        await ViewModel.LoadTablesCommand.ExecuteAsync(null);
    }
}
