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
    private readonly IQueryHandler<GetTableMapQuery, GetTableMapResult> _getTableMap;
    private readonly NavigationService _navigationService;
    private readonly ITicketCreationService _ticketCreationService;
    private readonly IUserService _userService;

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
    public AsyncRelayCommand<TableDto> SelectTableCommand { get; }

    public TableExplorerViewModel(
        IQueryHandler<GetTableMapQuery, GetTableMapResult> getTableMap,
        NavigationService navigationService,
        ITicketCreationService ticketCreationService,
        IUserService userService)
    {
        _getTableMap = getTableMap;
        _navigationService = navigationService;
        _ticketCreationService = ticketCreationService;
        _userService = userService;

        Title = "Table Explorer";
        LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
        SelectTableCommand = new AsyncRelayCommand<TableDto>(SelectTableAsync);
    }

    private async Task LoadTablesAsync()
    {
        IsBusy = true;
        try
        {
            // Sync with Map: Show all active tables, not just available ones
            var result = await _getTableMap.HandleAsync(new GetTableMapQuery());
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

    private async Task SelectTableAsync(TableDto? table)
    {
        if (table == null) return;

        // F-0082: Resume/Create logic
        if (table.Status == Domain.Enumerations.TableStatus.Seat && table.CurrentTicketId.HasValue)
        {
            // Resume existing ticket
            _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(table.CurrentTicketId.Value, true));
        }
        else if (table.Status == Domain.Enumerations.TableStatus.Available)
        {
            // Create new ticket using shared service
            try
            {
                IsBusy = true;
                
                if (_userService.CurrentUser?.Id == null) return;

                var ticketId = await _ticketCreationService.CreateTicketForTableAsync(table.Id, _userService.CurrentUser.Id);
                
                _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(ticketId, true));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create ticket from explorer: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
