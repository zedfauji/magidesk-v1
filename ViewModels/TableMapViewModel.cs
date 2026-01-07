using System.Collections.ObjectModel;
using System.Threading;
using Magidesk.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Commands;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Services;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace Magidesk.Presentation.ViewModels;

public class TableMapViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTableMapQuery, GetTableMapResult> _getTableMap;
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTable;
    private readonly NavigationService _navigationService;
    private Timer? _refreshTimer;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ObservableCollection<TableDto> Tables { get; } = new();

    private TableDto? _selectedTable;
    public TableDto? SelectedTable
    {
        get => _selectedTable;
        set => SetProperty(ref _selectedTable, value);
    }

    private bool _isRealTimeEnabled = true;
    public bool IsRealTimeEnabled
    {
        get => _isRealTimeEnabled;
        set => SetProperty(ref _isRealTimeEnabled, value);
    }

    private int _refreshInterval = 5000; // 5 seconds
    public int RefreshInterval
    {
        get => _refreshInterval;
        set => SetProperty(ref _refreshInterval, value);
    }

    private DateTime _lastRefresh = DateTime.MinValue;
    public DateTime LastRefresh
    {
        get => _lastRefresh;
        set => SetProperty(ref _lastRefresh, value);
    }
    
    // Mode Logic
    private Guid? _sourceTicketId;
    public Guid? SourceTicketId
    {
        get => _sourceTicketId;
        set => SetProperty(ref _sourceTicketId, value);
    }
    
    public string HeaderText => SourceTicketId.HasValue ? "TM_SelectNewTable" : "TM_Title";

    public AsyncRelayCommand LoadTablesCommand { get; }
    public AsyncRelayCommand RefreshTablesCommand { get; }
    public AsyncRelayCommand ToggleRealTimeCommand { get; }
    public AsyncRelayCommand<TableDto> SelectTableCommand { get; }

    private readonly IUserService _userService;
    private readonly ITicketCreationService _ticketCreationService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

    public Services.LocalizationService Localization { get; }

    public TableMapViewModel(
        IQueryHandler<GetTableMapQuery, GetTableMapResult> getTableMap,
        ICommandHandler<ChangeTableCommand, ChangeTableResult> changeTable,
        NavigationService navigationService,
        IUserService userService,
        ITicketCreationService ticketCreationService,
        IServiceScopeFactory serviceScopeFactory,
        Services.LocalizationService localizationService)
    {
        _getTableMap = getTableMap;
        _changeTable = changeTable;
        _navigationService = navigationService;
        _userService = userService;
        _ticketCreationService = ticketCreationService;
        _serviceScopeFactory = serviceScopeFactory;
        Localization = localizationService;
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
        RefreshTablesCommand = new AsyncRelayCommand(RefreshTablesAsync);
        ToggleRealTimeCommand = new AsyncRelayCommand(ToggleRealTimeAsync);
        SelectTableCommand = new AsyncRelayCommand<TableDto>(SelectTableAsync);

        Title = "Table Map";
        
        // Start real-time polling with initial delay
        StartRealTimePolling();
    }

    public event EventHandler? RequestShiftStart;
    
    public void SetContext(Guid? sourceTicketId)
    {
        SourceTicketId = sourceTicketId;
        OnPropertyChanged(nameof(HeaderText));
    }

    private double _canvasWidth = 2000;
    public double CanvasWidth
    {
        get => _canvasWidth;
        set => SetProperty(ref _canvasWidth, value);
    }

    private double _canvasHeight = 2000;
    public double CanvasHeight
    {
        get => _canvasHeight;
        set => SetProperty(ref _canvasHeight, value);
    }

    private async Task LoadTablesAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _getTableMap.HandleAsync(new GetTableMapQuery());
            Tables.Clear();
            
            double maxX = 2000;
            double maxY = 2000;

            foreach (var table in result.Tables)
            {
                Tables.Add(table);
                
                // Track max extent (+ padding) to resize canvas dynamically
                double tableRight = table.X + (table.Width > 0 ? table.Width : 150);
                double tableBottom = table.Y + (table.Height > 0 ? table.Height : 150);
                
                if (tableRight > maxX) maxX = tableRight;
                if (tableBottom > maxY) maxY = tableBottom;
            }
            
            // Add margin
            CanvasWidth = maxX + 200;
            CanvasHeight = maxY + 200;
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
                     _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(SourceTicketId.Value, true));
                     
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
             _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(table.CurrentTicketId.Value, true));
        }
        else if (table.Status == TableStatus.Available)
        {
             // Create new ticket using shared service
             try 
             {
                 IsBusy = true;
                 
                 if (_userService.CurrentUser?.Id == null) return;
                 
                 var ticketId = await _ticketCreationService.CreateTicketForTableAsync(table.Id, _userService.CurrentUser.Id);

                 // Navigate with new Ticket ID
                 _navigationService.Navigate(typeof(OrderEntryPage), new OrderEntryNavigationContext(ticketId, true));
             }
             catch (Exception ex)
             {
                 // TODO: Show visual error
                 System.Diagnostics.Debug.WriteLine($"Failed to create ticket from map: {ex.Message}");
             }
             finally
             {
                 IsBusy = false;
             }
        }    
    }

    private void StartRealTimePolling()
    {
        if (IsRealTimeEnabled) // IsBusy check removed as we use separate scope
        {
            // Initial delay to avoid collision with page load
            _refreshTimer = new Timer(async _ => await RefreshTableStatusAsync(), 
                                     null, TimeSpan.FromMilliseconds(RefreshInterval), TimeSpan.FromMilliseconds(RefreshInterval));
        }
    }

    private void StopRealTimePolling()
    {
        _refreshTimer?.Dispose();
    }

    private async Task RefreshTableStatusAsync()
    {
        if (!IsRealTimeEnabled) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTableMap = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTableMapQuery, GetTableMapResult>>();
                var result = await getTableMap.HandleAsync(new GetTableMapQuery());
            
                // Marshall back to UI thread if needed, or update ObservableCollection carefully.
                // Since this is updating the ObservableCollection properties (Status, CurrentTicketId),
                // we should do this on the UI thread to avoid "The application called an interface that was marshalled for a different thread."
                
                _dispatcherQueue.TryEnqueue(() => 
                {
                    // Update only changed tables for performance
                    foreach (var updatedTable in result.Tables)
                    {
                        var existingTable = Tables.FirstOrDefault(t => t.Id == updatedTable.Id);
                        if (existingTable != null && (existingTable.Status != updatedTable.Status || existingTable.CurrentTicketId != updatedTable.CurrentTicketId))
                        {
                            existingTable.Status = updatedTable.Status;
                            existingTable.CurrentTicketId = updatedTable.CurrentTicketId;
                        }
                    }
                });
            }
            
            _dispatcherQueue.TryEnqueue(() => LastRefresh = DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            // Log error but don't crash the polling
            System.Diagnostics.Debug.WriteLine($"Error refreshing table status: {ex.Message}");
        }
    }

    private async Task RefreshTablesAsync()
    {
        await LoadTablesAsync();
        LastRefresh = DateTime.UtcNow;
    }

    private async Task ToggleRealTimeAsync()
    {
        IsRealTimeEnabled = !IsRealTimeEnabled;
        
        if (IsRealTimeEnabled)
        {
            StartRealTimePolling();
        }
        else
        {
            StopRealTimePolling();
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        StopRealTimePolling();
        _cancellationTokenSource.Dispose();
    }
}
