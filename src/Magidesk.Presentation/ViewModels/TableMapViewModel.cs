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

public partial class TableMapViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTableMapQuery, GetTableMapResult> _getTableMap;
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTable;
    private readonly NavigationService _navigationService;
    private Timer? _refreshTimer;
    private Microsoft.UI.Xaml.DispatcherTimer? _uiRefreshTimer;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly OrderPageNavigationHelper _orderPageNavigationHelper;

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
    private readonly IUserContextService _userContextService;
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
        IUserContextService userContextService,
        ITicketCreationService ticketCreationService,
        IServiceScopeFactory serviceScopeFactory,
        ITerminalContext terminalContext,
        Services.LocalizationService localizationService,
        IServiceProvider serviceProvider,
        OrderPageNavigationHelper orderPageNavigationHelper)
    {
        _getTableMap = getTableMap;
        _changeTable = changeTable;
        _navigationService = navigationService;
        _userService = userService;
        _userContextService = userContextService;
        _ticketCreationService = ticketCreationService;
        _serviceScopeFactory = serviceScopeFactory;
        _terminalContext = terminalContext;
        Localization = localizationService;
        _serviceProvider = serviceProvider;
        _orderPageNavigationHelper = orderPageNavigationHelper;
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

}
