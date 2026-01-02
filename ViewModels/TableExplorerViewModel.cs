using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.Views;

namespace Magidesk.Presentation.ViewModels;

public class TableExplorerViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetAvailableTablesQuery, GetAvailableTablesResult> _getAvailableTables;
    private readonly NavigationService _navigationService;

    public ObservableCollection<TableDto> AllTables { get; } = new();
    public ObservableCollection<TableDto> FilteredTables { get; } = new();

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                ApplyFilter();
            }
        }
    }

    public AsyncRelayCommand LoadTablesCommand { get; }
    public IRelayCommand<TableDto> SelectTableCommand { get; }

    public TableExplorerViewModel(
        IQueryHandler<GetAvailableTablesQuery, GetAvailableTablesResult> getAvailableTables,
        NavigationService navigationService)
    {
        _getAvailableTables = getAvailableTables;
        _navigationService = navigationService;

        Title = "Table Explorer";
        LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
        SelectTableCommand = new RelayCommand<TableDto>(SelectTable);
    }

    private async Task LoadTablesAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _getAvailableTables.HandleAsync(new GetAvailableTablesQuery());
            AllTables.Clear();
            foreach (var table in result.Tables)
            {
                AllTables.Add(table);
            }
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilter()
    {
        FilteredTables.Clear();
        var query = AllTables.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            query = query.Where(t => t.TableNumber.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var table in query.OrderBy(t => t.TableNumber))
        {
            FilteredTables.Add(table);
        }
    }

    private void SelectTable(TableDto? table)
    {
        if (table == null) return;

        // F-0082: Resume/Create logic
        if (table.Status == Domain.Enumerations.TableStatus.Seat && table.CurrentTicketId.HasValue)
        {
            _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(table.CurrentTicketId.Value, true));
        }
        else
        {
            // For now, same as Map behavior: pass null ticket for new order if available
            _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(null, true));
        }
    }
}
