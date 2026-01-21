using Magidesk.Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Magidesk.Presentation.Views;

public sealed partial class TableExplorerPage : Page
{
    public TableExplorerViewModel ViewModel { get; }

    public TableExplorerPage()
    {
        this.InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TableExplorerViewModel>();
        this.Name = "RootPage"; // For ElementName binding
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadTablesCommand.ExecuteAsync(null);
    }
}
