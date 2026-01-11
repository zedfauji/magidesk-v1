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
using Magidesk.Application.Commands.TableSessions;

namespace Magidesk.Presentation.ViewModels;

public class TableMapViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTableMapQuery, GetTableMapResult> _getTableMap;
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTable;
    private readonly NavigationService _navigationService;
    private Timer? _refreshTimer;
    private Microsoft.UI.Xaml.DispatcherTimer? _uiRefreshTimer;
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
    
    private bool _canAdjustTime;
    public bool CanAdjustTime
    {
        get => _canAdjustTime;
        set => SetProperty(ref _canAdjustTime, value);
    }
    
    public string HeaderText => SourceTicketId.HasValue ? "TM_SelectNewTable" : "TM_Title";

    public AsyncRelayCommand LoadTablesCommand { get; }
    public AsyncRelayCommand RefreshTablesCommand { get; }
    public AsyncRelayCommand ToggleRealTimeCommand { get; }
    public AsyncRelayCommand<TableDto> SelectTableCommand { get; }
    
    // Session dialog commands
    public AsyncRelayCommand<TableDto> OpenStartSessionDialogCommand { get; }
    public AsyncRelayCommand<TableDto> OpenEndSessionDialogCommand { get; }
    public AsyncRelayCommand<TableDto> PauseSessionCommand { get; }
    public AsyncRelayCommand<TableDto> ResumeSessionCommand { get; }
    public AsyncRelayCommand<TableDto> PerformTimeAdjustmentCommand { get; }

    private readonly IUserService _userService;
    private readonly ITicketCreationService _ticketCreationService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITerminalContext _terminalContext;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
    private readonly IServiceProvider _serviceProvider;

    public Services.LocalizationService Localization { get; }

    public TableMapViewModel(
        IQueryHandler<GetTableMapQuery, GetTableMapResult> getTableMap,
        ICommandHandler<ChangeTableCommand, ChangeTableResult> changeTable,
        NavigationService navigationService,
        IUserService userService,
        ITicketCreationService ticketCreationService,
        IServiceScopeFactory serviceScopeFactory,
        ITerminalContext terminalContext,
        Services.LocalizationService localizationService,
        IServiceProvider serviceProvider)
    {
        _getTableMap = getTableMap;
        _changeTable = changeTable;
        _navigationService = navigationService;
        _userService = userService;
        _ticketCreationService = ticketCreationService;
        _serviceScopeFactory = serviceScopeFactory;
        _terminalContext = terminalContext;
        Localization = localizationService;
        _serviceProvider = serviceProvider;
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
        RefreshTablesCommand = new AsyncRelayCommand(RefreshTablesAsync);
        ToggleRealTimeCommand = new AsyncRelayCommand(ToggleRealTimeAsync);
        SelectTableCommand = new AsyncRelayCommand<TableDto>(SelectTableAsync);
        
        // Session dialog commands
        OpenStartSessionDialogCommand = new AsyncRelayCommand<TableDto>(OpenStartSessionDialogAsync);
        OpenEndSessionDialogCommand = new AsyncRelayCommand<TableDto>(OpenEndSessionDialogAsync);
        PauseSessionCommand = new AsyncRelayCommand<TableDto>(PauseSessionAsync);
        ResumeSessionCommand = new AsyncRelayCommand<TableDto>(ResumeSessionAsync);
        PerformTimeAdjustmentCommand = new AsyncRelayCommand<TableDto>(PerformTimeAdjustmentAsync);
        
        // Check permissions
        _ = CheckPermissionsAsync();

        Title = "Table Map";
        
        // Start real-time polling with initial delay
        StartRealTimePolling();
        
        // Start UI refresh timer for session timers (1 second)
        StartUIRefreshTimer();
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
        
        // Set as selected table for toolbar buttons
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

    private void StartUIRefreshTimer()
    {
        // Create a DispatcherTimer for UI updates (1 second interval)
        _uiRefreshTimer = new Microsoft.UI.Xaml.DispatcherTimer();
        _uiRefreshTimer.Interval = TimeSpan.FromSeconds(1);
        _uiRefreshTimer.Tick += (s, e) =>
        {
            // Force UI update for calculated properties (SessionElapsedTime, SessionRunningCharge)
            // This triggers property change notifications for all tables
            foreach (var table in Tables)
            {
                // Trigger property changed for calculated properties
                // Since TableDto doesn't implement INotifyPropertyChanged, we need to update the collection
                // The UI will re-evaluate bindings on the next layout pass
            }
        };
        _uiRefreshTimer.Start();
    }

    private void StopUIRefreshTimer()
    {
        _uiRefreshTimer?.Stop();
        _uiRefreshTimer = null;
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
                        if (existingTable != null)
                        {
                            // Update table status
                            if (existingTable.Status != updatedTable.Status)
                            {
                                existingTable.Status = updatedTable.Status;
                            }
                            
                            if (existingTable.CurrentTicketId != updatedTable.CurrentTicketId)
                            {
                                existingTable.CurrentTicketId = updatedTable.CurrentTicketId;
                            }
                            
                            // Update session data (for timers and icons)
                            existingTable.SessionId = updatedTable.SessionId;
                            existingTable.SessionStartTime = updatedTable.SessionStartTime;
                            existingTable.SessionStatus = updatedTable.SessionStatus;
                            existingTable.SessionHourlyRate = updatedTable.SessionHourlyRate;
                            existingTable.SessionPausedDuration = updatedTable.SessionPausedDuration;
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

    #region Session Dialog Commands

    private async Task OpenStartSessionDialogAsync(TableDto? table)
    {
        if (table == null) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.StartSessionDialogViewModel>();
            
            // TODO: Get table type information - for now using placeholder
            // This should be fetched from the table or a default table type
            var tableTypeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder
            var tableTypeName = "Standard";
            var hourlyRate = 15.00m; // Default rate
            
            // Initialize dialog
            dialogViewModel.Initialize(
                table.Id, 
                tableTypeId, 
                $"Table {table.TableNumber}", 
                tableTypeName, 
                hourlyRate, 
                ticketId: null, // No ticket yet
                userId: _userService.CurrentUser?.Id,
                terminalId: _terminalContext.TerminalId,
                shiftId: null, // TODO: Get current shift
                orderTypeId: Guid.Parse("00000000-0000-0000-0000-000000000001"), // DEFAULT
                createTicket: true);
            
            // Create and show dialog
            var dialog = new Views.Dialogs.StartSessionDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionStarted += async (s, result) =>
            {
                // Refresh table map to show new session
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening start session dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

    private async Task OpenEndSessionDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.EndSessionDialogViewModel>();
            
            // Calculate session duration and charge
            var duration = table.SessionElapsedTime ?? TimeSpan.Zero;
            var hourlyRate = table.SessionHourlyRate ?? 0m;
            var totalCharge = table.SessionRunningCharge ?? 0m;
            
            // Initialize dialog with session ID and calculated values
            dialogViewModel.Initialize(
                table.SessionId.Value, 
                duration, 
                hourlyRate, 
                totalCharge,
                userId: _userService.CurrentUser?.Id,
                terminalId: _terminalContext.TerminalId,
                hasExistingTicket: true);
            
            // Create and show dialog
            var dialog = new Views.Dialogs.EndSessionDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionEnded += async (s, result) =>
            {
                // Refresh table map to clear session
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening end session dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

    private async Task PauseSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // TODO: Implement PauseTableSessionCommand
            // For now, just log
            System.Diagnostics.Debug.WriteLine($"Pause session not yet implemented for table {table.TableNumber}");
            
            // This will be implemented in ticket FE-A.16-01
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Not Implemented", "Pause/Resume functionality will be available in a future update.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error pausing session: {ex.Message}");
        }
    }

    private async Task ResumeSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // TODO: Implement ResumeTableSessionCommand
            // For now, just log
            System.Diagnostics.Debug.WriteLine($"Resume session not yet implemented for table {table.TableNumber}");
            
            // This will be implemented in ticket FE-A.16-01
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Not Implemented", "Pause/Resume functionality will be available in a future update.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resuming session: {ex.Message}");
        }
    }

    private async Task PerformTimeAdjustmentAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = new ViewModels.Dialogs.TableSessions.AdjustSessionTimeDialogViewModel(
                _serviceProvider.GetRequiredService<ICommandHandler<AdjustSessionTimeCommand, AdjustSessionTimeResult>>(),
                table.SessionId.Value
            );

            // Create and show dialog
            var dialog = new Views.Dialogs.TableSessions.AdjustSessionTimeDialog();
            dialog.DataContext = dialogViewModel;
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;

            await dialog.ShowAsync();
            
            // Refresh tables to show updated time
            await RefreshTablesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adjusting session time: {ex.Message}");
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to open adjustment dialog: {ex.Message}");
        }
    }

    private async Task CheckPermissionsAsync()
    {
        try 
        {
            if (_userService.CurrentUser == null) 
            {
                CanAdjustTime = false;
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var securityService = scope.ServiceProvider.GetRequiredService<ISecurityService>();
                var userId = new UserId(_userService.CurrentUser.Id);
                CanAdjustTime = await securityService.HasPermissionAsync(userId, UserPermission.AdjustSessionTime);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking permissions: {ex.Message}");
            CanAdjustTime = false;
        }
    }

    #endregion

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        StopRealTimePolling();
        StopUIRefreshTimer();
        _cancellationTokenSource.Dispose();
    }
}
