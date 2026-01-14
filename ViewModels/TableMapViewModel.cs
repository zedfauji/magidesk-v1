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
using Magidesk.Presentation.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Magidesk.Application.Commands.TableSessions;
using Microsoft.UI.Xaml.Controls;

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

    private int _refreshInterval = 60000; // 1 minute for billing data
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
    
    // Enhanced session management commands
    public AsyncRelayCommand<TableDto> OpenSessionControlDialogCommand { get; }
    public AsyncRelayCommand<TableDto> OpenManagerOverrideDialogCommand { get; }
    public AsyncRelayCommand<TableDto> OpenTableOperationsDialogCommand { get; }
    
    // Table action commands for context menu
    public AsyncRelayCommand<TableDto> StartSessionCommand { get; }
    public AsyncRelayCommand<TableDto> ViewDetailsCommand { get; }
    public AsyncRelayCommand<TableDto> EndSessionCommand { get; }
    
    // Server assignment command
    public AsyncRelayCommand<ServerAssignmentEventArgs> AssignServerCommand { get; }

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
        
        // Enhanced session management commands
        OpenSessionControlDialogCommand = new AsyncRelayCommand<TableDto>(OpenSessionControlDialogAsync);
        OpenManagerOverrideDialogCommand = new AsyncRelayCommand<TableDto>(OpenManagerOverrideDialogAsync);
        OpenTableOperationsDialogCommand = new AsyncRelayCommand<TableDto>(OpenTableOperationsDialogAsync);
        
        // Table action commands for context menu
        StartSessionCommand = new AsyncRelayCommand<TableDto>(StartSessionAsync);
        ViewDetailsCommand = new AsyncRelayCommand<TableDto>(ViewDetailsAsync);
        EndSessionCommand = new AsyncRelayCommand<TableDto>(EndSessionAsync);
        
        // Server assignment command
        AssignServerCommand = new AsyncRelayCommand<ServerAssignmentEventArgs>(AssignServerAsync);
        
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
            // This triggers property change notifications for all tables with active sessions
            var tablesWithSessions = Tables.Where(t => t.SessionId.HasValue && t.SessionStatus == TableSessionStatus.Active).ToList();
            
            if (tablesWithSessions.Any())
            {
                // Create a new collection to trigger UI updates for calculated properties
                // This is necessary because TableDto doesn't implement INotifyPropertyChanged
                var updatedTables = new List<TableDto>();
                
                foreach (var table in Tables)
                {
                    if (table.SessionId.HasValue && table.SessionStatus == TableSessionStatus.Active)
                    {
                        // Create a copy with updated calculated values to trigger UI refresh
                        var updatedTable = new TableDto
                        {
                            Id = table.Id,
                            TableNumber = table.TableNumber,
                            Status = table.Status,
                            X = table.X,
                            Y = table.Y,
                            Width = table.Width,
                            Height = table.Height,
                            Shape = table.Shape,
                            CurrentTicketId = table.CurrentTicketId,
                            SessionId = table.SessionId,
                            SessionStartTime = table.SessionStartTime,
                            SessionStatus = table.SessionStatus,
                            SessionHourlyRate = table.SessionHourlyRate,
                            SessionPausedDuration = table.SessionPausedDuration,
                            FloorId = table.FloorId,
                            LayoutId = table.LayoutId,
                            Capacity = table.Capacity,
                            IsActive = table.IsActive,
                            IsSelected = table.IsSelected,
                            IsLocked = table.IsLocked
                        };
                        updatedTables.Add(updatedTable);
                    }
                }
                
                // Update the tables in the collection to trigger UI refresh
                foreach (var updatedTable in updatedTables)
                {
                    var index = Tables.ToList().FindIndex(t => t.Id == updatedTable.Id);
                    if (index >= 0)
                    {
                        Tables[index] = updatedTable;
                    }
                }
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

    #region Table Action Commands

    private async Task StartSessionAsync(TableDto? table)
    {
        if (table == null) return;
        await OpenStartSessionDialogAsync(table);
    }

    private async Task ViewDetailsAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;
        
        // Navigate to session details or show session control dialog
        await OpenSessionControlDialogAsync(table);
    }

    private async Task EndSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;
        await OpenEndSessionDialogAsync(table);
    }

    #endregion

    #region Server Assignment

    private async Task AssignServerAsync(ServerAssignmentEventArgs? args)
    {
        if (args == null || args.Table == null) return;

        try
        {
            // TODO: Implement server assignment logic
            // This would call a command handler to assign the server to the table/session
            
            System.Diagnostics.Debug.WriteLine($"Assigning server {args.ServerName} (ID: {args.ServerId}) to table {args.Table.TableNumber}");
            
            // For now, just show a success message
            // In a full implementation, this would:
            // 1. Call a command handler to update the table/session with the server assignment
            // 2. Refresh the table map to show the updated assignment
            // 3. Show a toast notification for success/failure
            
            await RefreshTablesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error assigning server: {ex.Message}");
            // TODO: Show error to user via IDialogService or IToastNotificationService
        }
    }

    #endregion

    #region Context Menu Generation

    /// <summary>
    /// Generates context menu items based on table status
    /// </summary>
    /// <param name="table">The table to generate menu items for</param>
    /// <returns>Collection of menu flyout items</returns>
    public ObservableCollection<MenuFlyoutItemBase> GetContextMenuItems(TableDto table)
    {
        var items = new ObservableCollection<MenuFlyoutItemBase>();

        if (table == null) return items;

        // Available table actions
        if (table.Status == TableStatus.Available)
        {
            items.Add(CreateMenuFlyoutItem(
                "Start Session",
                Symbol.Play,
                StartSessionCommand,
                table
            ));
        }

        // Occupied table actions
        if (table.Status == TableStatus.Seat && table.SessionId.HasValue)
        {
            items.Add(CreateMenuFlyoutItem(
                "View Details",
                Symbol.View,
                ViewDetailsCommand,
                table
            ));

            items.Add(new MenuFlyoutSeparator());

            // Pause/Resume based on session status
            if (table.SessionStatus == TableSessionStatus.Active)
            {
                items.Add(CreateMenuFlyoutItem(
                    "Pause Session",
                    Symbol.Pause,
                    PauseSessionCommand,
                    table
                ));
            }
            else if (table.SessionStatus == TableSessionStatus.Paused)
            {
                items.Add(CreateMenuFlyoutItem(
                    "Resume Session",
                    Symbol.Play,
                    ResumeSessionCommand,
                    table
                ));
            }

            items.Add(new MenuFlyoutSeparator());

            items.Add(CreateMenuFlyoutItem(
                "End Session",
                Symbol.Stop,
                EndSessionCommand,
                table
            ));
        }

        return items;
    }

    private MenuFlyoutItem CreateMenuFlyoutItem(string text, Symbol icon, ICommand command, TableDto table)
    {
        var item = new MenuFlyoutItem
        {
            Text = text,
            Icon = new SymbolIcon(icon),
            Command = command,
            CommandParameter = table
        };
        return item;
    }

    #endregion

    #region Session Dialog Commands

    private async Task OpenStartSessionDialogAsync(TableDto? table)
    {
        if (table == null) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.StartSessionDialogViewModel>();
            
            // Get table type information from the table or use default
            var tableTypeRepository = _serviceProvider.GetRequiredService<ITableTypeRepository>();
            
            // For now, use a default table type since TableDto doesn't have TableTypeId
            // In a future version, this should be retrieved from table configuration
            var defaultTableTypes = await tableTypeRepository.GetAllAsync();
            var tableType = defaultTableTypes.FirstOrDefault();
            
            Guid tableTypeId;
            string tableTypeName;
            decimal hourlyRate;
            
            if (tableType != null)
            {
                tableTypeId = tableType.Id;
                tableTypeName = tableType.Name;
                hourlyRate = tableType.HourlyRate;
            }
            else
            {
                // Fallback to default values if no table type found
                tableTypeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Placeholder
                tableTypeName = "Standard";
                hourlyRate = 15.00m; // Default rate
            }
            
            // Get current shift
            var getCurrentShiftHandler = _serviceProvider.GetRequiredService<IQueryHandler<GetCurrentShiftQuery, GetCurrentShiftResult>>();
            var currentShiftResult = await getCurrentShiftHandler.HandleAsync(new GetCurrentShiftQuery());
            var currentShiftId = currentShiftResult.Shift?.Id;
            
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
                shiftId: currentShiftId,
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
            var pauseHandler = _serviceProvider.GetRequiredService<ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult>>();
            var command = new PauseTableSessionCommand(table.SessionId.Value);
            
            var result = await pauseHandler.HandleAsync(command);
            
            // Refresh the table map to show updated status
            await RefreshTablesAsync();
            
            System.Diagnostics.Debug.WriteLine($"Session paused for table {table.TableNumber} at {result.PausedAt}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error pausing session: {ex.Message}");
            
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to pause session: {ex.Message}");
        }
    }

    private async Task ResumeSessionAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            var resumeHandler = _serviceProvider.GetRequiredService<ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult>>();
            var command = new ResumeTableSessionCommand(table.SessionId.Value);
            
            var result = await resumeHandler.HandleAsync(command);
            
            // Refresh the table map to show updated status
            await RefreshTablesAsync();
            
            System.Diagnostics.Debug.WriteLine($"Session resumed for table {table.TableNumber} at {result.ResumedAt}. Total paused duration: {result.TotalPausedDuration}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resuming session: {ex.Message}");
            
            var dialogService = _serviceProvider.GetRequiredService<IDialogService>();
            await dialogService.ShowMessageAsync("Error", $"Failed to resume session: {ex.Message}");
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

    #region Enhanced Session Management Methods

    private async Task OpenSessionControlDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.SessionControlDialogViewModel>();
            
            // Initialize dialog with session information
            dialogViewModel.Initialize(
                table.SessionId.Value,
                $"Table {table.TableNumber}",
                table.SessionStatus ?? TableSessionStatus.Ended,
                4, // TODO: Get actual guest count from session data
                table.SessionElapsedTime ?? TimeSpan.Zero,
                table.SessionPausedDuration ?? TimeSpan.Zero,
                table.SessionRunningCharge ?? 0m
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.SessionControlDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.SessionControlCompleted += async (s, result) =>
            {
                // Refresh table map to show updated session state
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening session control dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

    private async Task OpenManagerOverrideDialogAsync(TableDto? table)
    {
        if (table == null || !table.SessionId.HasValue) return;

        try
        {
            // Show override type selection first
            var overrideTypeDialog = new ContentDialog
            {
                Title = "Select Override Type",
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Cancel",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };

            var overrideTypeSelection = new ComboBox
            {
                PlaceholderText = "Select override type...",
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
            };

            overrideTypeSelection.Items.Add("Time Adjustment");
            overrideTypeSelection.Items.Add("Pricing Override");
            overrideTypeSelection.Items.Add("Force End Session");

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "Select the type of manager override to perform:" });
            stackPanel.Children.Add(overrideTypeSelection);

            overrideTypeDialog.Content = stackPanel;

            var typeResult = await overrideTypeDialog.ShowAsync();
            if (typeResult != ContentDialogResult.Primary || overrideTypeSelection.SelectedItem == null)
                return;

            // Determine override type
            var overrideType = overrideTypeSelection.SelectedItem.ToString() switch
            {
                "Time Adjustment" => ViewModels.Dialogs.ManagerOverrideType.TimeAdjustment,
                "Pricing Override" => ViewModels.Dialogs.ManagerOverrideType.PricingOverride,
                "Force End Session" => ViewModels.Dialogs.ManagerOverrideType.ForceEnd,
                _ => ViewModels.Dialogs.ManagerOverrideType.TimeAdjustment
            };

            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.ManagerOverrideDialogViewModel>();
            
            // Initialize dialog with session and override information
            dialogViewModel.Initialize(
                table.SessionId.Value,
                $"Table {table.TableNumber}",
                overrideType,
                table.SessionElapsedTime ?? TimeSpan.Zero,
                table.SessionRunningCharge ?? 0m
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.ManagerOverrideDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.OverrideCompleted += async (s, result) =>
            {
                // Refresh table map to show updated session state
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening manager override dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
        }
    }

    private async Task OpenTableOperationsDialogAsync(TableDto? table)
    {
        if (table == null) return;

        try
        {
            // Show operation type selection first
            var operationTypeDialog = new ContentDialog
            {
                Title = "Select Table Operation",
                PrimaryButtonText = "Continue",
                SecondaryButtonText = "Cancel",
                XamlRoot = App.MainWindowInstance.Content.XamlRoot
            };

            var operationTypeSelection = new ComboBox
            {
                PlaceholderText = "Select operation type...",
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
            };

            operationTypeSelection.Items.Add("Merge Tables");
            operationTypeSelection.Items.Add("Split Tables");
            if (table.SessionId.HasValue)
            {
                operationTypeSelection.Items.Add("Transfer Session");
            }

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { Text = "Select the table operation to perform:" });
            stackPanel.Children.Add(operationTypeSelection);

            operationTypeDialog.Content = stackPanel;

            var typeResult = await operationTypeDialog.ShowAsync();
            if (typeResult != ContentDialogResult.Primary || operationTypeSelection.SelectedItem == null)
                return;

            // Determine operation type
            var operationType = operationTypeSelection.SelectedItem.ToString() switch
            {
                "Merge Tables" => ViewModels.Dialogs.TableOperationType.Merge,
                "Split Tables" => ViewModels.Dialogs.TableOperationType.Split,
                "Transfer Session" => ViewModels.Dialogs.TableOperationType.Transfer,
                _ => ViewModels.Dialogs.TableOperationType.Merge
            };

            // Resolve dialog ViewModel from DI
            var dialogViewModel = _serviceProvider.GetRequiredService<ViewModels.Dialogs.TableOperationsDialogViewModel>();
            
            // Initialize dialog with table and operation information
            await dialogViewModel.InitializeAsync(
                operationType,
                table.Id,
                $"Table {table.TableNumber}",
                table.SessionId,
                table.SessionRunningCharge ?? 0m,
                table.SessionElapsedTime ?? TimeSpan.Zero,
                4 // TODO: Get actual guest count from session data
            );
            
            // Create and show dialog
            var dialog = new Views.Dialogs.TableOperationsDialog(dialogViewModel);
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Handle dialog result
            dialogViewModel.OperationCompleted += async (s, result) =>
            {
                // Refresh table map to show updated table states
                await RefreshTablesAsync();
            };
            
            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening table operations dialog: {ex.Message}");
            // TODO: Show error to user via IDialogService
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
