using System.Collections.ObjectModel;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels;

public class TableMapViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetAvailableTablesQuery, GetAvailableTablesResult> _getAvailableTables;
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTable;
    private readonly NavigationService _navigationService;
    private readonly TicketViewModel _ticketViewModel;

    public ObservableCollection<TableDto> Tables { get; } = new();

    private TableDto? _selectedTable;
    public TableDto? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }
    
    // Mode Logic
    private Guid? _sourceTicketId;
    public Guid? SourceTicketId
    {
        get => _sourceTicketId;
        set => SetProperty(ref _sourceTicketId, value);
    }
    
    public string HeaderText => SourceTicketId.HasValue ? "Select New Table" : "Table Map";

    public AsyncRelayCommand LoadTablesCommand { get; }
    public AsyncRelayCommand<TableDto> SelectTableCommand { get; }

    public TableMapViewModel(
        IQueryHandler<GetAvailableTablesQuery, GetAvailableTablesResult> getAvailableTables,
        ICommandHandler<ChangeTableCommand, ChangeTableResult> changeTable,
        NavigationService navigationService,
        TicketViewModel ticketViewModel)
    {
        _getAvailableTables = getAvailableTables;
        _changeTable = changeTable;
        _navigationService = navigationService;
        _ticketViewModel = ticketViewModel;

        Title = "Table Map";
        LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
        SelectTableCommand = new AsyncRelayCommand<TableDto>(SelectTableAsync);
    }
    
    public void SetContext(Guid? sourceTicketId)
    {
        SourceTicketId = sourceTicketId;
        OnPropertyChanged(nameof(HeaderText));
    }

    private async Task LoadTablesAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _getAvailableTables.HandleAsync(new GetAvailableTablesQuery());
            Tables.Clear();
            foreach (var table in result.Tables)
            {
                Tables.Add(table);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SelectTableAsync(TableDto? table)
    {
        if (table == null) return;
        
        SelectedTable = table;

        if (SourceTicketId.HasValue)
        {
            // F-0080: Move Table Logic
            if (table.Status != TableStatus.Available)
            {
                 // TODO: Show Error or Offer Merge
                 return;
            }
            
            IsBusy = true;
            try
            {
                var result = await _changeTable.HandleAsync(new ChangeTableCommand
                {
                    TicketId = SourceTicketId.Value,
                    NewTableId = table.Id,
                    UserId = new UserId(Guid.Parse("00000000-0000-0000-0000-000000000001")) // TODO: Current User
                });

                if (result.Success)
                {
                     // Return to Ticket Page
                     _ticketViewModel.TicketIdText = SourceTicketId.Value.ToString();
                     await _ticketViewModel.LoadTicketCommand.ExecuteAsync(null);
                     _navigationService.Navigate(typeof(TicketPage));
                     
                     // Reset Context
                     SetContext(null);
                }
                else
                {
                    // Show error? For now just log/ignore
                }
            }
            finally
            {
                IsBusy = false;
            }
            
            return;
        }

        // F-0082: Normal Navigation Logic
        if (table.Status == TableStatus.Seat && table.CurrentTicketId.HasValue)
        {
             // Resume existing ticket
             _ticketViewModel.TicketIdText = table.CurrentTicketId.Value.ToString();
             await _ticketViewModel.LoadTicketCommand.ExecuteAsync(null);
             _navigationService.Navigate(typeof(TicketPage));
        }
        else if (table.Status == TableStatus.Available)
        {
             _navigationService.Navigate(typeof(TicketPage));
        }
    }
}
