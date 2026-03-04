using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.Presentation.ViewModels;
using Magidesk.Presentation.Views;
using Magidesk.Presentation.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// ViewModel for the redesigned Order Page.
/// Manages order entry workflow with modern UI patterns.
/// </summary>
public partial class OrderPageViewModel : ViewModelBase
{
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>> _getMenuItemsHandler;
    private readonly IQueryHandler<GetTableQuery, TableDto?> _getTableHandler;
    private readonly ICommandHandler<AddOrderLineCommand, AddOrderLineResult> _addOrderLineHandler;
    private readonly ICommandHandler<RemoveOrderLineCommand> _removeOrderLineHandler;
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;
    private readonly ICommandHandler<StartTableSessionCommand, StartTableSessionResult> _startTableSessionHandler;
    private readonly ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> _pauseTableSessionHandler;
    private readonly ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> _resumeTableSessionHandler;
    private readonly ICommandHandler<EndTableSessionCommand, EndTableSessionResult> _endTableSessionHandler;
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly IUserContextService _userContextService;
    private readonly ITerminalContext _terminalContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDialogService _dialogService;
    private readonly ILogger<OrderPageViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;

    private Guid? _ticketId;
    private TicketDto? _ticket;
    private Guid? _tableId;
    private System.Timers.Timer? _timeUpdateTimer;
    private System.Timers.Timer? _sessionDurationTimer;
    private List<ProductViewModel> _allProducts = new();
    private Microsoft.UI.Xaml.XamlRoot? _xamlRoot; // Store XamlRoot for dialogs

    public OrderPageViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>> getMenuItemsHandler,
        IQueryHandler<GetTableQuery, TableDto?> getTableHandler,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        ICommandHandler<RemoveOrderLineCommand> removeOrderLineHandler,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        ICommandHandler<StartTableSessionCommand, StartTableSessionResult> startTableSessionHandler,
        ICommandHandler<PauseTableSessionCommand, PauseTableSessionResult> pauseTableSessionHandler,
        ICommandHandler<ResumeTableSessionCommand, ResumeTableSessionResult> resumeTableSessionHandler,
        ICommandHandler<EndTableSessionCommand, EndTableSessionResult> endTableSessionHandler,
        NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        IServiceScopeFactory serviceScopeFactory,
        IDialogService dialogService,
        IUserContextService userContextService,
        ILogger<OrderPageViewModel> logger)
    {
        _getTicketHandler = getTicketHandler ?? throw new ArgumentNullException(nameof(getTicketHandler));
        _getMenuItemsHandler = getMenuItemsHandler ?? throw new ArgumentNullException(nameof(getMenuItemsHandler));
        _getTableHandler = getTableHandler ?? throw new ArgumentNullException(nameof(getTableHandler));
        _addOrderLineHandler = addOrderLineHandler ?? throw new ArgumentNullException(nameof(addOrderLineHandler));
        _removeOrderLineHandler = removeOrderLineHandler ?? throw new ArgumentNullException(nameof(removeOrderLineHandler));
        _createTicketHandler = createTicketHandler ?? throw new ArgumentNullException(nameof(createTicketHandler));
        _startTableSessionHandler = startTableSessionHandler ?? throw new ArgumentNullException(nameof(startTableSessionHandler));
        _pauseTableSessionHandler = pauseTableSessionHandler ?? throw new ArgumentNullException(nameof(pauseTableSessionHandler));
        _resumeTableSessionHandler = resumeTableSessionHandler ?? throw new ArgumentNullException(nameof(resumeTableSessionHandler));
        _endTableSessionHandler = endTableSessionHandler ?? throw new ArgumentNullException(nameof(endTableSessionHandler));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _terminalContext = terminalContext ?? throw new ArgumentNullException(nameof(terminalContext));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Get the dispatcher queue for the current thread (must be called from UI thread)
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (_dispatcherQueue == null)
        {
            throw new InvalidOperationException("OrderPageViewModel must be constructed on the UI thread");
        }

        // Initialize collections
        OrderItems = new ObservableCollection<OrderItemViewModel>();
        Categories = new ObservableCollection<ProductCategoryViewModel>();
        Subcategories = new ObservableCollection<string>();
        FilteredProducts = new ObservableCollection<ProductViewModel>();

        // Initialize commands
        SelectTableCommand = new AsyncRelayCommand(OnSelectTableAsync);
        SearchProductCommand = new RelayCommand(OnSearchProduct);
        AddProductCommand = new AsyncRelayCommand<ProductViewModel>(OnAddProductAsync);
        EditOrderItemCommand = new AsyncRelayCommand<OrderItemViewModel>(OnEditOrderItemAsync);
        RemoveOrderItemCommand = new AsyncRelayCommand<OrderItemViewModel>(OnRemoveOrderItemAsync);
        SelectCategoryCommand = new RelayCommand<ProductCategoryViewModel>(OnSelectCategory);
        SelectSubcategoryCommand = new RelayCommand<string>(OnSelectSubcategory);
        SplitOrderCommand = new AsyncRelayCommand(OnSplitOrderAsync);
        MergeOrderCommand = new AsyncRelayCommand(OnMergeOrderAsync);
        AddNoteCommand = new AsyncRelayCommand(OnAddNoteAsync);
        PrintOrderCommand = new AsyncRelayCommand(OnPrintOrderAsync);
        NavigateToSettleCommand = new AsyncRelayCommand(OnNavigateToSettleAsync);
        PayNowCommand = new AsyncRelayCommand(OnPayNowAsync);
        ToggleSessionCommand = new AsyncRelayCommand(OnToggleSessionAsync);
        EndSessionCommand = new AsyncRelayCommand(OnEndSessionAsync);
        ReprintCommand = new AsyncRelayCommand(OnReprintAsync);
        VoidTicketCommand = new AsyncRelayCommand(OnVoidTicketAsync);
        ApplyDiscountCommand = new AsyncRelayCommand(OnApplyDiscountAsync);
        FireTicketCommand = new AsyncRelayCommand(OnFireTicketAsync);

        // Initialize time update timer
        _timeUpdateTimer = new System.Timers.Timer(1000); // Update every second
        _timeUpdateTimer.Elapsed += (s, e) =>
        {
            // Marshal property changes to UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                OnPropertyChanged(nameof(CurrentTime));
                OnPropertyChanged(nameof(WaitTime));
            });
        };
        _timeUpdateTimer.Start();

        // Initialize session duration timer
        _sessionDurationTimer = new System.Timers.Timer(1000); // Update every second
        _sessionDurationTimer.Elapsed += (s, e) =>
        {
            // Marshal property changes to UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                OnPropertyChanged(nameof(SessionDurationDisplay));
            });
        };
        _sessionDurationTimer.Start();

        _logger.LogInformation("OrderPageViewModel constructor - All commands initialized");
    }

    // Table Information
    [ObservableProperty]
    private string _tableNumber = "No Table";

    [ObservableProperty]
    private int _guestCount;

    // Order Items
    public ObservableCollection<OrderItemViewModel> OrderItems { get; }

    // Financial Calculations
    [ObservableProperty]
    private decimal _subtotal;

    [ObservableProperty]
    private decimal _taxAmount;

    public decimal TaxRate => 0.08m; // 8% tax rate - TODO: Get from configuration

    [ObservableProperty]
    private decimal _total;

    // Product Catalog
    public ObservableCollection<ProductCategoryViewModel> Categories { get; }

    [ObservableProperty]
    private ProductCategoryViewModel? _selectedCategory;

    public ObservableCollection<string> Subcategories { get; }

    [ObservableProperty]
    private string? _selectedSubcategory;

    public ObservableCollection<ProductViewModel> FilteredProducts { get; }

    // Search
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public AsyncRelayCommand SelectTableCommand { get; }
    public RelayCommand SearchProductCommand { get; }
    public AsyncRelayCommand<ProductViewModel> AddProductCommand { get; }
    public AsyncRelayCommand<OrderItemViewModel> EditOrderItemCommand { get; }
    public AsyncRelayCommand<OrderItemViewModel> RemoveOrderItemCommand { get; }
    public RelayCommand<ProductCategoryViewModel> SelectCategoryCommand { get; }
    public RelayCommand<string> SelectSubcategoryCommand { get; }
    public AsyncRelayCommand SplitOrderCommand { get; }
    public AsyncRelayCommand MergeOrderCommand { get; }
    public AsyncRelayCommand AddNoteCommand { get; }
    public AsyncRelayCommand PrintOrderCommand { get; }
    public AsyncRelayCommand NavigateToSettleCommand { get; }
    public AsyncRelayCommand PayNowCommand { get; }
    public AsyncRelayCommand ToggleSessionCommand { get; }
    public AsyncRelayCommand EndSessionCommand { get; }
    public AsyncRelayCommand ReprintCommand { get; }
    public AsyncRelayCommand VoidTicketCommand { get; }
    public AsyncRelayCommand ApplyDiscountCommand { get; }
    public AsyncRelayCommand FireTicketCommand { get; }

    /// <summary>
    /// Initializes the ViewModel with optional ticket and table IDs.
    /// </summary>
    public async Task InitializeAsync(Guid? ticketId = null, Guid? tableId = null)
    {
        try
        {
            _logger.LogInformation("InitializeAsync called with ticketId: {TicketId}, tableId: {TableId}", ticketId, tableId);

            _ticketId = ticketId;
            _tableId = tableId;

            await LoadCategoriesAsync();
            _logger.LogInformation("Categories loaded");

            await LoadProductsAsync();
            _logger.LogInformation("Products loaded: {Count}", _allProducts.Count);

            if (_ticketId.HasValue)
            {
                await LoadTicketAsync();
            }

            if (_tableId.HasValue)
            {
                await LoadTableAsync();
            }

            _logger.LogInformation("InitializeAsync completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize OrderPageViewModel");
            await _dialogService.ShowErrorAsync(
                "Initialization Error",
                $"Failed to load order page data:\n\n{ex.Message}",
                ex.ToString());
        }
    }

    private async Task OnSelectTableAsync()
    {
        // Handle table selection here
        await Task.CompletedTask;
    }
}
