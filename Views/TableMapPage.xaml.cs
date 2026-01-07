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
        DataContext = ViewModel;
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
        
        ViewModel.RequestShiftStart += ViewModel_RequestShiftStart;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.RequestShiftStart -= ViewModel_RequestShiftStart;
    }

    private async void ViewModel_RequestShiftStart(object? sender, EventArgs e)
    {
        var dialog = new Dialogs.ShiftStartDialog();
        dialog.XamlRoot = this.XamlRoot;
        await dialog.ShowAsync();
    }
}
