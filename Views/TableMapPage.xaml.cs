using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Controls;
using Magidesk.Application.DTOs;
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

    private async void OnTableClicked(object sender, TableActionEventArgs e)
    {
        if (e.Table != null)
        {
            await ViewModel.SelectTableCommand.ExecuteAsync(e.Table);
        }
    }

    private void OnTableRightClicked(object sender, TableActionEventArgs e)
    {
        // Context menu is handled by the EnhancedTableControl itself
        // This event is provided for additional handling if needed
    }

    private async void OnServerAssigned(object sender, ServerAssignmentEventArgs e)
    {
        if (e != null)
        {
            await ViewModel.AssignServerCommand.ExecuteAsync(e);
        }
    }
}
