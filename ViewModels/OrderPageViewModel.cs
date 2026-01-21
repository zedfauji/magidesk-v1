using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Magidesk.Presentation.ViewModels.Dialogs;
using Magidesk.Presentation.Views.Dialogs;
using Magidesk.ViewModels;
using Magidesk.Views;
using Magidesk.Views.Dialogs;
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
    private readonly NavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDialogService _dialogService;
    private readonly ILogger<OrderPageViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;

    private Guid? _ticketId;
    private TicketDto? _ticket;
    private Guid? _tableId;
    private System.Timers.Timer? _timeUpdateTimer;
    private List<ProductViewModel> _allProducts = new();
    private Microsoft.UI.Xaml.XamlRoot? _xamlRoot; // Store XamlRoot for dialogs

    public OrderPageViewModel(
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>> getMenuItemsHandler,
        IQueryHandler<GetTableQuery, TableDto?> getTableHandler,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        ICommandHandler<RemoveOrderLineCommand> removeOrderLineHandler,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        NavigationService navigationService,
        IUserService userService,
        ITerminalContext terminalContext,
        IServiceScopeFactory serviceScopeFactory,
        IDialogService dialogService,
        ILogger<OrderPageViewModel> logger)
    {
        _getTicketHandler = getTicketHandler ?? throw new ArgumentNullException(nameof(getTicketHandler));
        _getMenuItemsHandler = getMenuItemsHandler ?? throw new ArgumentNullException(nameof(getMenuItemsHandler));
        _getTableHandler = getTableHandler ?? throw new ArgumentNullException(nameof(getTableHandler));
        _addOrderLineHandler = addOrderLineHandler ?? throw new ArgumentNullException(nameof(addOrderLineHandler));
        _removeOrderLineHandler = removeOrderLineHandler ?? throw new ArgumentNullException(nameof(removeOrderLineHandler));
        _createTicketHandler = createTicketHandler ?? throw new ArgumentNullException(nameof(createTicketHandler));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _terminalContext = terminalContext ?? throw new ArgumentNullException(nameof(terminalContext));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
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
        StartSessionCommand = new AsyncRelayCommand(OnStartSessionAsync);
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

        _logger.LogInformation("OrderPageViewModel constructor - All commands initialized");
    }

    #region Properties

    // Table Information
    [ObservableProperty]
    private string _tableNumber = "No Table";

    [ObservableProperty]
    private int _guestCount;

    // Ticket Information
    public string TicketNumber => _ticket != null ? $"Ticket #{_ticket.TicketNumber}" : "New Order";
    
    public DateTime TicketStartTime => _ticket?.CreatedAt ?? DateTime.Now;
    
    public TimeSpan WaitTime => DateTime.Now - TicketStartTime;

    public bool HasTicket => _ticketId.HasValue && _ticket != null;

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

    // Session Information
    public string TerminalName => _terminalContext.TerminalIdentity ?? "Terminal";
    
    public string UserName => _userService.CurrentUser?.FullName ?? "User";
    
    public string SystemStatus => "ONLINE"; // TODO: Implement actual status check
    
    public DateTime CurrentTime => DateTime.Now;

    // Statistics
    public int TotalItemCount => OrderItems.Sum(item => item.Quantity);

    #endregion

    #region Commands

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
    public AsyncRelayCommand StartSessionCommand { get; }
    public AsyncRelayCommand EndSessionCommand { get; }
    public AsyncRelayCommand ReprintCommand { get; }
    public AsyncRelayCommand VoidTicketCommand { get; }
    public AsyncRelayCommand ApplyDiscountCommand { get; }
    public AsyncRelayCommand FireTicketCommand { get; }

    #endregion

    #region Public Methods

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
    
    /// <summary>
    /// Sets the XamlRoot for dialogs. Must be called from the View after it's loaded.
    /// </summary>
    public void SetXamlRoot(Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        _xamlRoot = xamlRoot;
    }

    /// <summary>
    /// Refreshes the current ticket data from the repository.
    /// Used when navigating back from SettlePageView to reload any changes.
    /// </summary>
    public async Task RefreshTicketAsync()
    {
        if (_ticketId.HasValue)
        {
            await LoadTicketAsync();
        }
    }

    #endregion

    #region Private Methods

    private async Task LoadTicketAsync()
    {
        if (!_ticketId.HasValue) return;

        try
        {
            IsBusy = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTicketHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTicketQuery, TicketDto?>>();
                _ticket = await getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = _ticketId.Value });

                if (_ticket != null)
                {
                    // Load order items
                    OrderItems.Clear();
                    foreach (var line in _ticket.OrderLines)
                    {
                        OrderItems.Add(new OrderItemViewModel
                        {
                            OrderItemId = line.Id,
                            ProductName = line.MenuItemName,
                            Quantity = (int)line.Quantity,
                            UnitPrice = line.UnitPrice,
                            LineTotal = line.TotalAmount,
                            Modifiers = new ObservableCollection<string>(
                                line.Modifiers?.Select(m => m.Name) ?? Enumerable.Empty<string>()
                            )
                        });
                    }

                    RecalculateTotals();

                    // Notify property changes
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(TicketStartTime));
                    OnPropertyChanged(nameof(WaitTime));
                    OnPropertyChanged(nameof(TotalItemCount));

                    _logger.LogInformation("Loaded ticket {TicketId} with {ItemCount} items", _ticketId, OrderItems.Count);
                }
                else
                {
                    _logger.LogWarning("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowWarningAsync(
                        "Ticket Not Found",
                        $"Ticket {_ticketId} could not be found. It may have been deleted or moved.");
                }
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while loading ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync(
                "Error Loading Ticket",
                $"An error occurred while loading the ticket:\n\n{ex.Message}",
                ex.ToString());
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadTableAsync()
    {
        if (!_tableId.HasValue) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getTableHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTableQuery, GetTableResult>>();
                var result = await getTableHandler.HandleAsync(new GetTableQuery { TableId = _tableId.Value });

                if (result?.Table != null)
                {
                    TableNumber = $"TABLE {result.Table.TableNumber} (GUESTS: {GuestCount})";
                    _logger.LogInformation("Loaded table {TableNumber}", result.Table.TableNumber);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load table {TableId}", _tableId);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("LoadCategoriesAsync starting");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var menuCategoryRepository = scope.ServiceProvider.GetRequiredService<IMenuCategoryRepository>();
                
                var dbCategories = await menuCategoryRepository.GetAllAsync();
                _logger.LogInformation("Loaded {Count} categories from database", dbCategories?.Count() ?? 0);
                
                Categories.Clear();
                
                // Add "Popular" as first category (special category that shows all)
                Categories.Add(new ProductCategoryViewModel { Name = "Popular", IconName = "\uE734" }); // FavoriteStar
                
                // Add categories from database
                if (dbCategories != null)
                {
                    _logger.LogInformation("Adding {Count} categories from database:", dbCategories.Count());
                    foreach (var category in dbCategories.Where(c => c.IsActive).OrderBy(c => c.SortOrder))
                    {
                        _logger.LogInformation("  Category from DB: '{Name}'", category.Name);
                        Categories.Add(new ProductCategoryViewModel 
                        { 
                            Name = category.Name, 
                            IconName = GetIconForCategory(category.Name) 
                        });
                    }
                }
                
                // Fallback: if no categories in database, add default ones
                if (Categories.Count == 1) // Only "Popular"
                {
                    _logger.LogWarning("No categories found in database, using defaults");
                    Categories.Add(new ProductCategoryViewModel { Name = "Food", IconName = "\uE787" }); // Restaurant
                    Categories.Add(new ProductCategoryViewModel { Name = "Drinks", IconName = "\uE8C4" }); // Coffee
                    Categories.Add(new ProductCategoryViewModel { Name = "Desserts", IconName = "\uE7E3" }); // Cake
                    Categories.Add(new ProductCategoryViewModel { Name = "Sides", IconName = "\uE7E8" }); // Food
                    Categories.Add(new ProductCategoryViewModel { Name = "Retail", IconName = "\uE719" }); // ShoppingCart
                }

                // Select first category by default (Popular)
                if (Categories.Any())
                {
                    SelectedCategory = Categories.First();
                    _logger.LogInformation("Selected default category: {CategoryName}", SelectedCategory.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
            
            // Fallback to default categories on error
            Categories.Clear();
            Categories.Add(new ProductCategoryViewModel { Name = "Popular", IconName = "\uE734" }); // FavoriteStar
            Categories.Add(new ProductCategoryViewModel { Name = "All Items", IconName = "\uE787" }); // Restaurant
            
            if (Categories.Any())
            {
                SelectedCategory = Categories.First();
            }
        }
    }
    
    private string GetIconForCategory(string categoryName)
    {
        // Map category names to Segoe MDL2 Assets icon glyphs (Unicode characters)
        var lowerName = categoryName.ToLowerInvariant();
        
        // Popular/Star
        if (lowerName.Contains("popular"))
            return "\uE734"; // FavoriteStar
        // Food/Meal
        if (lowerName.Contains("food") || lowerName.Contains("meal") || lowerName.Contains("អាហារ") || lowerName.Contains("ម្ហូប"))
            return "\uE787"; // Restaurant
        // Drinks/Beverages
        if (lowerName.Contains("drink") || lowerName.Contains("beverage") || lowerName.Contains("ភេសជ្ជៈ"))
            return "\uE8C4"; // Drink (Coffee)
        // Desserts
        if (lowerName.Contains("dessert") || lowerName.Contains("sweet") || lowerName.Contains("បង្អែម"))
            return "\uE7E3"; // Cake
        // Appetizers/Starters
        if (lowerName.Contains("appetizer") || lowerName.Contains("starter"))
            return "\uE7E8"; // Food
        // Sides
        if (lowerName.Contains("side"))
            return "\uE7E8"; // Food
        // Burgers
        if (lowerName.Contains("burger"))
            return "\uE7E8"; // Food
        // Pizza
        if (lowerName.Contains("pizza"))
            return "\uE7E8"; // Food
        // Salads
        if (lowerName.Contains("salad"))
            return "\uE7E8"; // Food
        // Combos
        if (lowerName.Contains("combo"))
            return "\uE7E8"; // Food
        // Retail/Merchandise
        if (lowerName.Contains("retail") || lowerName.Contains("merchandise"))
            return "\uE719"; // ShoppingCart
        // Misc
        if (lowerName.Contains("misc"))
            return "\uE8FD"; // More
            
        return "\uE787"; // Default: Restaurant icon
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            _logger.LogInformation("LoadProductsAsync starting");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getMenuItemsHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>>>();
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                
                _logger.LogInformation("Calling GetMenuItemsQuery");
                var menuItems = await getMenuItemsHandler.HandleAsync(new GetMenuItemsQuery { IsActive = true });
                _logger.LogInformation("GetMenuItemsQuery returned {Count} items", menuItems?.Count ?? 0);

                _allProducts.Clear();
                foreach (var item in menuItems)
                {
                    // Get the full menu item to check for modifiers and get group/category info
                    var menuItem = await menuRepository.GetByIdAsync(item.Id);
                    bool hasModifiers = menuItem?.ModifierGroups.Any() ?? false;
                    
                    // Get category and group (subcategory) names
                    string categoryName = menuItem?.Category?.Name ?? item.CategoryName ?? "Uncategorized";
                    string groupName = menuItem?.Group?.Name ?? string.Empty;
                    
                    _allProducts.Add(new ProductViewModel
                    {
                        ProductId = item.Id,
                        Name = item.Name,
                        SKU = item.Id.ToString().Substring(0, 8), // Use first 8 chars of GUID as SKU
                        Price = item.Price,
                        CategoryName = categoryName,
                        SubcategoryName = groupName, // Group is the subcategory
                        HasModifiers = hasModifiers,
                        IsAvailable = item.IsActive
                    });
                }
                
                // Log first 5 products with their actual category names for debugging
                // _logger.LogInformation("Sample products loaded:");
                // foreach (var product in _allProducts.Take(5))
                // {
                //     _logger.LogInformation("  Product: {Name}, Category: '{Category}', Subcategory: '{Subcategory}'", 
                //         product.Name, product.CategoryName, product.SubcategoryName);
                // }

                // Apply initial filter
                FilterProducts();

                _logger.LogInformation("Loaded {Count} products, filtered to {FilteredCount}", _allProducts.Count, FilteredProducts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load products");
            await _dialogService.ShowErrorAsync(
                "Error Loading Products",
                $"Failed to load menu items:\n\n{ex.Message}",
                ex.ToString());
        }
    }

    private void RecalculateTotals()
    {
        Subtotal = OrderItems.Sum(item => item.LineTotal);
        TaxAmount = Subtotal * TaxRate;
        Total = Subtotal + TaxAmount;

        OnPropertyChanged(nameof(TotalItemCount));

        _logger.LogDebug("Recalculated totals: Subtotal={Subtotal}, Tax={Tax}, Total={Total}",
            Subtotal, TaxAmount, Total);
    }

    private void FilterProducts()
    {
        try
        {
            _logger.LogInformation("FilterProducts called - SelectedCategory: {Category}, SelectedSubcategory: {Subcategory}, SearchQuery: {Search}, TotalProducts: {Total}",
                SelectedCategory?.Name ?? "null", SelectedSubcategory ?? "null", SearchQuery ?? "null", _allProducts.Count);
            
            FilteredProducts.Clear();

            var query = _allProducts.AsEnumerable();

            // Filter by search query (name or SKU, case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.SKU.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                );
                _logger.LogInformation("After search filter: {Count} products", query.Count());
            }

            // Filter by category
            if (SelectedCategory != null && SelectedCategory.Name != "Popular")
            {
                var beforeCount = query.Count();
                query = query.Where(p => p.CategoryName.Equals(SelectedCategory.Name, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation("Category filter '{Category}': {Before} -> {After} products", 
                    SelectedCategory.Name, beforeCount, query.Count());
                
                // Debug: Log first few products and their categories
                // var sampleProducts = query.Take(3).ToList();
                // foreach (var p in sampleProducts)
                // {
                //     _logger.LogInformation("  Sample product: {Name}, Category: {Category}, Subcategory: {Subcategory}", 
                //         p.Name, p.CategoryName, p.SubcategoryName);
                // }
            }

            // Filter by subcategory
            if (!string.IsNullOrWhiteSpace(SelectedSubcategory))
            {
                var beforeCount = query.Count();
                query = query.Where(p => p.SubcategoryName.Equals(SelectedSubcategory, StringComparison.OrdinalIgnoreCase));
                _logger.LogInformation("Subcategory filter '{Subcategory}': {Before} -> {After} products", 
                    SelectedSubcategory, beforeCount, query.Count());
            }

            // Apply filtered results
            foreach (var product in query)
            {
                FilteredProducts.Add(product);
            }

            _logger.LogInformation("FilterProducts completed: {Count} products in FilteredProducts", FilteredProducts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to filter products");
        }
    }

    private async Task OnSelectTableAsync()
    {
        try
        {
            _logger.LogInformation("Select table requested");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
                
                // Create ViewModel for table selection dialog with required repository
                var viewModel = new TableSelectionViewModel(tableRepository);
                await viewModel.InitializeAsync();
                
                // Create Dialog
                var dialog = new Magidesk.Views.Dialogs.TableSelectionDialog
                {
                    DataContext = viewModel
                };
                
                // Set XamlRoot for the dialog
                if (_xamlRoot != null)
                {
                    dialog.XamlRoot = _xamlRoot;
                }
                else
                {
                    _logger.LogError("XamlRoot is null - dialog may not display correctly");
                    throw new InvalidOperationException("XamlRoot must be set before showing dialogs. Call SetXamlRoot() from the View.");
                }
                
                // Set close action
                viewModel.CloseAction = () => dialog.Hide();
                
                await dialog.ShowAsync();

                // If user confirmed selection
                if (viewModel.IsConfirmed && viewModel.SelectedTable != null)
                {
                    _tableId = viewModel.SelectedTable.Id;
                    TableNumber = $"TABLE {viewModel.SelectedTable.TableNumber}";
                    
                    _logger.LogInformation("Selected table {TableNumber}", 
                        viewModel.SelectedTable.TableNumber);
                    
                    // If we have a ticket, assign the table to it
                    if (_ticketId.HasValue)
                    {
                        var assignTableHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AssignTableToTicketCommand, AssignTableToTicketResult>>();
                        
                        var command = new AssignTableToTicketCommand
                        {
                            TicketId = _ticketId.Value,
                            TableId = _tableId.Value
                        };
                        
                        await assignTableHandler.HandleAsync(command);
                        await LoadTicketAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show table selection dialog");
            await _dialogService.ShowErrorAsync("Error", $"Failed to open table selection dialog: {ex.Message}");
        }
    }

    private void OnSearchProduct()
    {
        FilterProducts();
    }

    private async Task OnAddProductAsync(ProductViewModel? product)
    {
        if (product == null) return;

        // Check if product is available
        if (!product.IsAvailable)
        {
            _logger.LogWarning("Cannot add product {ProductName}: product not available", product.Name);
            await _dialogService.ShowWarningAsync(
                "Product Unavailable",
                $"{product.Name} is currently unavailable and cannot be added to the order.");
            return;
        }

        try
        {
            // Create ticket if it doesn't exist
            if (!_ticketId.HasValue)
            {
                await CreateTicketAsync();
            }

            if (!_ticketId.HasValue)
            {
                _logger.LogError("Failed to create ticket");
                await _dialogService.ShowErrorAsync(
                    "Error",
                    "Failed to create ticket. Please try again.");
                return;
            }

            List<MenuModifier> selectedModifiers = new();

            // Check if product has modifiers
            if (product.HasModifiers)
            {
                _logger.LogInformation("Product {ProductName} has modifiers, showing dialog", product.Name);
                
                // Get the full menu item to check for modifiers
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                    var menuItem = await menuRepository.GetByIdAsync(product.ProductId);
                    
                    if (menuItem == null)
                    {
                        _logger.LogError("Menu item {ProductId} not found", product.ProductId);
                        await _dialogService.ShowErrorAsync(
                            "Product Not Found",
                            $"{product.Name} could not be found in the menu. It may have been removed.");
                        return;
                    }
                    
                    if (menuItem != null && menuItem.ModifierGroups.Any())
                    {
                        // Create a temporary order line DTO for the modifier dialog
                        var tempOrderLine = new OrderLineDto
                        {
                            Id = Guid.NewGuid(),
                            MenuItemId = product.ProductId,
                            MenuItemName = product.Name,
                            Quantity = 1,
                            UnitPrice = product.Price,
                            TaxRate = TaxRate,
                            Modifiers = new List<OrderLineModifierDto>()
                        };

                        // Show modifier selection dialog
                        var modifierViewModel = new Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel(
                            menuRepository, 
                            tempOrderLine);
                        
                        var dialog = new Magidesk.Views.Dialogs.ModifierSelectionDialog(modifierViewModel);
                        
                        // Set XamlRoot for the dialog
                        if (_xamlRoot != null)
                        {
                            dialog.XamlRoot = _xamlRoot;
                        }
                        else
                        {
                            _logger.LogError("XamlRoot is null - cannot show modifier dialog");
                            throw new InvalidOperationException("XamlRoot must be set before showing dialogs.");
                        }
                        
                        await dialog.ShowAsync();

                        // If user confirmed, get the selected modifiers
                        if (modifierViewModel.IsConfirmed)
                        {
                            // Convert OrderLineModifierDto to MenuModifier entities
                            foreach (var modDto in modifierViewModel.ResultModifiers)
                            {
                                if (modDto.ModifierId.HasValue)
                                {
                                    var modifier = await menuRepository.GetModifierByIdAsync(modDto.ModifierId.Value);
                                    if (modifier != null)
                                    {
                                        selectedModifiers.Add(modifier);
                                    }
                                }
                            }
                            
                            _logger.LogInformation("User selected {Count} modifiers for {ProductName}", 
                                selectedModifiers.Count, product.Name);
                        }
                        else
                        {
                            // User cancelled the modifier selection
                            _logger.LogInformation("User cancelled modifier selection for {ProductName}", product.Name);
                            return;
                        }
                    }
                }
            }

            // Add order line with modifiers
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var addOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddOrderLineCommand, AddOrderLineResult>>();
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                
                // Get the menu item to get accurate pricing and details
                var menuItem = await menuRepository.GetByIdAsync(product.ProductId);
                if (menuItem == null)
                {
                    _logger.LogError("Menu item {ProductId} not found", product.ProductId);
                    await _dialogService.ShowErrorAsync(
                        "Product Not Found",
                        $"{product.Name} could not be found in the menu. It may have been removed.");
                    return;
                }
                
                var command = new AddOrderLineCommand
                {
                    TicketId = _ticketId.Value,
                    MenuItemId = product.ProductId,
                    MenuItemName = product.Name,
                    Quantity = 1,
                    UnitPrice = menuItem.Price,
                    TaxRate = menuItem.TaxRate,
                    CategoryName = menuItem.Category?.Name,
                    GroupName = menuItem.Group?.Name,
                    AddedBy = _userService.CurrentUser != null 
                        ? new UserId(_userService.CurrentUser.Id) 
                        : null,
                    Modifiers = selectedModifiers
                };

                var result = await addOrderLineHandler.HandleAsync(command);

                // Reload ticket to get updated order lines
                await LoadTicketAsync();
                
                _logger.LogInformation("Added product {ProductName} to ticket {TicketId} with {ModifierCount} modifiers",
                    product.Name, _ticketId, selectedModifiers.Count);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while adding product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to add {product.Name} to the order:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while adding product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add product {ProductName}", product.Name);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to add product: {ex.Message}");
        }
    }

    private async Task CreateTicketAsync()
    {
        try
        {
            if (_userService.CurrentUser == null)
            {
                _logger.LogError("Cannot create ticket: no user logged in");
                await _dialogService.ShowErrorAsync(
                    "Authentication Error",
                    "No user is currently logged in. Please log in and try again.");
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                _logger.LogError("Cannot create ticket: no terminal context");
                await _dialogService.ShowErrorAsync(
                    "Terminal Error",
                    "Terminal context is not available. Please restart the application.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Check if there's an active session
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();
                var activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);
                
                if (activeSession == null)
                {
                    _logger.LogError("Cannot create ticket: no active session");
                    
                    var startSession = await _dialogService.ShowConfirmationAsync(
                        "No Active Session",
                        "There is no active POS session. You must start a session before creating orders.\n\nWould you like to start a session now?",
                        "Start Session", "Cancel");
                    
                    if (startSession)
                    {
                        await OnStartSessionAsync();
                        // After starting session, try again
                        activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);
                        if (activeSession == null)
                        {
                            // Session start failed
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                
                var createTicketHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTicketCommand, CreateTicketResult>>();
                
                var command = new CreateTicketCommand
                {
                    TableId = _tableId,
                    CreatedBy = new UserId(_userService.CurrentUser.Id),
                    TerminalId = _terminalContext.TerminalId.Value
                };

                var result = await createTicketHandler.HandleAsync(command);

                _ticketId = result.TicketId;
                _logger.LogInformation("Created new ticket {TicketId}", _ticketId);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while creating ticket");
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to create ticket:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while creating ticket");
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ticket");
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to create ticket: {ex.Message}");
        }
    }

    private async Task OnEditOrderItemAsync(OrderItemViewModel? item)
    {
        if (item == null || !_ticketId.HasValue) return;

        try
        {
            _logger.LogInformation("Edit order item {ItemId} requested", item.OrderItemId);

            // Get the current order line from the ticket
            if (_ticket == null)
            {
                _logger.LogError("Cannot edit item: ticket not loaded");
                await _dialogService.ShowErrorAsync(
                    "Error",
                    "Ticket is not loaded. Please refresh and try again.");
                return;
            }

            var orderLine = _ticket.OrderLines.FirstOrDefault(ol => ol.Id == item.OrderItemId);
            if (orderLine == null)
            {
                _logger.LogError("Order line {OrderLineId} not found in ticket", item.OrderItemId);
                await _dialogService.ShowErrorAsync(
                    "Item Not Found",
                    "The order item could not be found. It may have been removed.");
                return;
            }

            // Get the menu item to check for modifiers
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                var menuItem = await menuRepository.GetByIdAsync(orderLine.MenuItemId);
                
                if (menuItem == null)
                {
                    _logger.LogError("Menu item {MenuItemId} not found", orderLine.MenuItemId);
                    await _dialogService.ShowErrorAsync(
                        "Product Not Found",
                        "The product could not be found in the menu. It may have been removed.");
                    return;
                }

                // Check if the menu item has modifiers
                if (!menuItem.ModifierGroups.Any())
                {
                    _logger.LogInformation("Menu item {MenuItemName} has no modifiers to edit", menuItem.Name);
                    await _dialogService.ShowMessageAsync(
                        "Edit Item",
                        "This item has no modifiers to edit.");
                    return;
                }

                // Create order line DTO for the modifier dialog
                var orderLineDto = new OrderLineDto
                {
                    Id = orderLine.Id,
                    MenuItemId = orderLine.MenuItemId,
                    MenuItemName = orderLine.MenuItemName,
                    Quantity = orderLine.Quantity,
                    UnitPrice = orderLine.UnitPrice,
                    TaxRate = orderLine.TaxRate,
                    Modifiers = orderLine.Modifiers.Select(m => new OrderLineModifierDto
                    {
                        ModifierId = m.ModifierId,
                        Name = m.Name,
                        ModifierType = m.ModifierType,
                        ItemCount = m.ItemCount,
                        UnitPrice = m.UnitPrice,
                        TaxRate = m.TaxRate,
                        SectionName = m.SectionName,
                        ShouldPrintToKitchen = m.ShouldPrintToKitchen
                    }).ToList()
                };

                // Show modifier selection dialog
                var modifierViewModel = new Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel(
                    menuRepository, 
                    orderLineDto);
                
                var dialog = new Magidesk.Views.Dialogs.ModifierSelectionDialog(modifierViewModel);
                
                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }
                
                await dialog.ShowAsync();

                // If user confirmed, update the order line with new modifiers
                if (modifierViewModel.IsConfirmed)
                {
                    _logger.LogInformation("User confirmed modifier changes for order item {ItemId}", item.OrderItemId);

                    // Execute ModifyOrderLineCommand with new modifiers
                    var modifyOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ModifyOrderLineCommand>>();
                    
                    var command = new ModifyOrderLineCommand
                    {
                        TicketId = _ticketId.Value,
                        OrderLineId = item.OrderItemId,
                        Quantity = orderLine.Quantity, // Keep the same quantity
                        Modifiers = modifierViewModel.ResultModifiers
                    };

                    await modifyOrderLineHandler.HandleAsync(command);

                    // Reload ticket to get updated order lines and recalculated totals
                    await LoadTicketAsync();
                    
                    _logger.LogInformation("Updated modifiers for order item {ItemId} in ticket {TicketId}",
                        item.OrderItemId, _ticketId);
                }
                else
                {
                    _logger.LogInformation("User cancelled modifier changes for order item {ItemId}", item.OrderItemId);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while editing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to edit item:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while editing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to edit order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to edit item: {ex.Message}");
        }
    }

    private async Task OnRemoveOrderItemAsync(OrderItemViewModel? item)
    {
        if (item == null || !_ticketId.HasValue) return;

        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var removeOrderLineHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<RemoveOrderLineCommand>>();
                
                var command = new RemoveOrderLineCommand
                {
                    TicketId = _ticketId.Value,
                    OrderLineId = item.OrderItemId
                };

                await removeOrderLineHandler.HandleAsync(command);

                // Reload ticket to get updated order lines
                await LoadTicketAsync();
                
                _logger.LogInformation("Removed order item {ItemId} from ticket {TicketId}",
                    item.OrderItemId, _ticketId);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while removing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Invalid Operation",
                $"Unable to remove item:\n\n{ex.Message}");
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while removing order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Network Error",
                "Unable to connect to the server. Please check your network connection and try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove order item {ItemId}", item.OrderItemId);
            await _dialogService.ShowErrorAsync(
                "Error",
                $"Failed to remove item: {ex.Message}");
        }
    }

    private void OnSelectCategory(ProductCategoryViewModel? category)
    {
        if (category == null)
        {
            _logger.LogWarning("OnSelectCategory called with null category");
            return;
        }

        _logger.LogInformation("OnSelectCategory called with category: {CategoryName}", category.Name);
        
        SelectedCategory = category;
        
        // Update subcategories based on selected category
        Subcategories.Clear();
        
        // Get unique subcategories from products in the selected category
        var subcategories = _allProducts
            .Where(p => p.CategoryName.Equals(category.Name, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.SubcategoryName)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .OrderBy(s => s);
        
        foreach (var subcategory in subcategories)
        {
            Subcategories.Add(subcategory);
        }
        
        _logger.LogInformation("Found {Count} subcategories for category {CategoryName}: {Subcategories}", 
            Subcategories.Count, category.Name, string.Join(", ", Subcategories));
        
        // Clear subcategory selection when category changes
        SelectedSubcategory = null;
        
        FilterProducts();
        
        _logger.LogInformation("OnSelectCategory completed for: {CategoryName}", category.Name);
    }

    private void OnSelectSubcategory(string? subcategory)
    {
        SelectedSubcategory = subcategory;
        FilterProducts();
        
        _logger.LogDebug("Selected subcategory: {SubcategoryName}", subcategory);
    }

    private async Task OnSplitOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot split order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before splitting.");
            return;
        }

        if (OrderItems.Count < 2)
        {
            _logger.LogWarning("Cannot split order: insufficient items");
            await _dialogService.ShowWarningAsync(
                "Insufficient Items",
                "You need at least 2 items to split an order.");
            return;
        }

        try
        {
            _logger.LogInformation("Split order requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                
                // Load the ticket
                var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);
                
                if (ticket == null)
                {
                    _logger.LogError("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        "The ticket could not be found. It may have been deleted.");
                    return;
                }
                
                // For now, show a simple confirmation and split evenly
                var confirmed = await _dialogService.ShowConfirmationAsync(
                    "Split Order",
                    $"Split this order into 2 separate tickets?\n\nCurrent ticket: #{ticket.TicketNumber}\nItems: {OrderItems.Count}\n\nThis will create a new ticket with half the items.",
                    "Split", "Cancel");
                
                if (confirmed)
                {
                    var splitTicketHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<SplitTicketCommand, SplitTicketResult>>();
                    
                    // Split the ticket - take half the order lines for the new ticket
                    var orderLinesToMove = ticket.OrderLines.Take(ticket.OrderLines.Count / 2).Select(ol => ol.Id).ToList();
                    
                    var command = new SplitTicketCommand
                    {
                        OriginalTicketId = _ticketId.Value,
                        OrderLineIdsToSplit = orderLinesToMove,
                        SplitBy = new UserId(_userService.CurrentUser!.Id),
                        TerminalId = _terminalContext.TerminalId!.Value,
                        ShiftId = Guid.Empty, // TODO: Get actual shift ID
                        OrderTypeId = Guid.Empty // TODO: Get actual order type ID
                    };
                    
                    var result = await splitTicketHandler.HandleAsync(command);
                    
                    _logger.LogInformation("Ticket {TicketId} split into new ticket {NewTicketId}", 
                        _ticketId, result.NewTicketId);
                    
                    // Reload the current ticket
                    await LoadTicketAsync();
                    
                    await _dialogService.ShowMessageAsync(
                        "Order Split",
                        $"Order has been split.\n\nOriginal Ticket: #{ticket.TicketNumber}\nNew Ticket: #{result.NewTicketNumber}\n\n{orderLinesToMove.Count} items moved to new ticket.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to split order for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to split order: {ex.Message}");
        }
    }

    private async Task OnMergeOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot merge order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before merging.");
            return;
        }

        try
        {
            _logger.LogInformation("Merge order requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                
                // Get all open tickets except the current one
                var openTickets = await ticketRepository.GetOpenTicketsAsync();
                var availableTickets = openTickets.Where(t => t.Id != _ticketId.Value).ToList();
                
                if (!availableTickets.Any())
                {
                    _logger.LogWarning("No other open tickets available to merge");
                    await _dialogService.ShowWarningAsync(
                        "No Tickets Available",
                        "There are no other open tickets to merge with.");
                    return;
                }
                
                // For now, show a simple message listing available tickets
                // In a full implementation, you would show a ticket selection dialog
                var ticketList = string.Join("\n", availableTickets.Select(t => $"Ticket #{t.TicketNumber} - {t.OrderLines.Count} items"));
                
                await _dialogService.ShowMessageAsync(
                    "Merge Order",
                    $"Merge order feature is available.\n\nAvailable tickets to merge:\n\n{ticketList}\n\nFull merge dialog coming soon.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show merge order dialog");
            await _dialogService.ShowErrorAsync("Error", $"Failed to show merge options: {ex.Message}");
        }
    }

    private async Task OnAddNoteAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot add note: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before adding a note.");
            return;
        }

        try
        {
            _logger.LogInformation("Add note requested for ticket {TicketId}", _ticketId);
            
            // Create a simple text input dialog for the note
            var inputDialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Add Note",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Primary
            };
            
            var textBox = new Microsoft.UI.Xaml.Controls.TextBox
            {
                PlaceholderText = "Enter special instructions or notes...",
                AcceptsReturn = true,
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                Height = 120,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0)
            };
            
            inputDialog.Content = textBox;
            
            // Set XamlRoot for the dialog
            if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
            {
                inputDialog.XamlRoot = element.XamlRoot;
            }
            
            var result = await inputDialog.ShowAsync();
            
            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                    
                    // Load the ticket
                    var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);
                    
                    if (ticket != null)
                    {
                        // Add note to the ticket (assuming there's a Notes property or similar)
                        // For now, we'll add it as an instruction to the last order line
                        if (ticket.OrderLines.Any())
                        {
                            var lastOrderLine = ticket.OrderLines.Last();
                            
                            var addInstructionHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddOrderLineInstructionCommand>>();
                            
                            var command = new AddOrderLineInstructionCommand
                            {
                                TicketId = _ticketId.Value,
                                OrderLineId = lastOrderLine.Id,
                                Instruction = textBox.Text
                            };
                            
                            await addInstructionHandler.HandleAsync(command);
                            
                            _logger.LogInformation("Note added to ticket {TicketId}: {Note}", _ticketId, textBox.Text);
                            
                            // Reload ticket
                            await LoadTicketAsync();
                            
                            await _dialogService.ShowMessageAsync(
                                "Note Added",
                                $"Note has been added to the order:\n\n{textBox.Text}");
                        }
                        else
                        {
                            await _dialogService.ShowWarningAsync(
                                "No Items",
                                "Please add items to the order before adding notes.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add note to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to add note: {ex.Message}");
        }
    }

    private async Task OnPrintOrderAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot print order: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before printing.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot print order: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before printing.");
            return;
        }

        try
        {
            _logger.LogInformation("Print order requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printToKitchenHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult>>();
                
                var command = new PrintToKitchenCommand
                {
                    TicketId = _ticketId.Value
                };
                
                var result = await printToKitchenHandler.HandleAsync(command);
                
                if (result.Success)
                {
                    _logger.LogInformation("Order ticket printed for ticket {TicketId}", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Order Printed",
                        $"Order ticket has been printed.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    var errorMsg = result.Errors.Any() ? string.Join("\n", result.Errors) : result.Message;
                    _logger.LogError("Failed to print order: {Error}", errorMsg);
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        $"Failed to print order:\n\n{errorMsg}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print order for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to print order: {ex.Message}");
        }
    }

    private async Task OnNavigateToSettleAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot navigate to settle: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please add items to the order before settling.");
            return;
        }

        // Check if there are any items in the order
        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot navigate to settle: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before settling.");
            return;
        }

        try
        {
            _logger.LogInformation("Navigating to settle page for ticket {TicketId}", _ticketId);
            
            // Navigate to settle page with ticket ID
            _navigationService.Navigate(typeof(Views.SettlePageView), _ticketId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to settle page");
            await _dialogService.ShowErrorAsync(
                "Navigation Error",
                $"Failed to open settle page: {ex.Message}");
        }
    }

    private async Task OnPayNowAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot pay now: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please add items to the order before processing payment.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot pay now: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before processing payment.");
            return;
        }

        try
        {
            _logger.LogInformation("Pay now requested for ticket {TicketId}", _ticketId);
            
            // Quick payment flow - navigate directly to settle page
            _navigationService.Navigate(typeof(Views.SettlePageView), _ticketId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate payment for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to initiate payment: {ex.Message}");
        }
    }

    private async Task OnStartSessionAsync()
    {
        try
        {
            _logger.LogInformation("Start session requested");
            
            if (_userService.CurrentUser == null)
            {
                _logger.LogError("Cannot start session: no user logged in");
                await _dialogService.ShowErrorAsync(
                    "Authentication Error",
                    "No user is currently logged in. Please log in and try again.");
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                _logger.LogError("Cannot start session: no terminal context");
                await _dialogService.ShowErrorAsync(
                    "Terminal Error",
                    "Terminal context is not available. Please restart the application.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();
                
                // Check if there's already an active session
                var activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);
                
                if (activeSession != null)
                {
                    _logger.LogWarning("Session already active for terminal {TerminalId}", _terminalContext.TerminalId);
                    await _dialogService.ShowWarningAsync(
                        "Session Already Active",
                        $"There is already an active session on this terminal.\n\nSession started: {activeSession.OpenedAt:g}");
                    return;
                }
                
                // Prompt for starting cash amount - use fully qualified name to avoid ambiguity
                var cashEntryDialog = new Magidesk.Presentation.Views.Dialogs.CashEntryDialog();
                
                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    cashEntryDialog.XamlRoot = element.XamlRoot;
                }
                
                cashEntryDialog.ViewModel.Title = "Start Session - Opening Cash";
                cashEntryDialog.ViewModel.Message = "Enter the opening cash amount for this session";
                
                var dialogResult = await cashEntryDialog.ShowAsync();
                
                if (dialogResult == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    var openingCash = cashEntryDialog.ViewModel.TotalAmount;
                    
                    var openSessionHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<OpenCashSessionCommand, OpenCashSessionResult>>();
                    
                    var command = new OpenCashSessionCommand
                    {
                        TerminalId = _terminalContext.TerminalId.Value,
                        UserId = new UserId(_userService.CurrentUser.Id),
                        OpeningBalance = new Money(openingCash, "USD"),
                        ShiftId = Guid.Empty // TODO: Get actual shift ID
                    };
                    
                    var result = await openSessionHandler.HandleAsync(command);
                    
                    _logger.LogInformation("Session {SessionId} started with opening cash {OpeningCash}", 
                        result.CashSessionId, openingCash);
                    
                    await _dialogService.ShowMessageAsync(
                        "Session Started",
                        $"POS session has been started.\n\nOpening Cash: {openingCash:C2}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to start session: {ex.Message}");
        }
    }

    private async Task OnEndSessionAsync()
    {
        try
        {
            _logger.LogInformation("End session requested");
            
            if (_userService.CurrentUser == null)
            {
                _logger.LogError("Cannot end session: no user logged in");
                await _dialogService.ShowErrorAsync(
                    "Authentication Error",
                    "No user is currently logged in. Please log in and try again.");
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                _logger.LogError("Cannot end session: no terminal context");
                await _dialogService.ShowErrorAsync(
                    "Terminal Error",
                    "Terminal context is not available. Please restart the application.");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var cashSessionRepository = scope.ServiceProvider.GetRequiredService<ICashSessionRepository>();
                
                // Check if there's an active session
                var activeSession = await cashSessionRepository.GetOpenSessionByTerminalIdAsync(_terminalContext.TerminalId.Value);
                
                if (activeSession == null)
                {
                    _logger.LogWarning("No active session for terminal {TerminalId}", _terminalContext.TerminalId);
                    await _dialogService.ShowWarningAsync(
                        "No Active Session",
                        "There is no active session on this terminal.");
                    return;
                }
                
                // Confirm session end
                var confirmed = await _dialogService.ShowConfirmationAsync(
                    "End Session",
                    $"End the current POS session?\n\nSession started: {activeSession.OpenedAt:g}\n\nThis will close the cash drawer and generate a session report.",
                    "End Session", "Cancel");
                
                if (!confirmed)
                {
                    return;
                }
                
                // Prompt for ending cash amount - use fully qualified name to avoid ambiguity
                var cashEntryDialog = new Magidesk.Presentation.Views.Dialogs.CashEntryDialog();
                
                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    cashEntryDialog.XamlRoot = element.XamlRoot;
                }
                
                cashEntryDialog.ViewModel.Title = "End Session - Closing Cash";
                cashEntryDialog.ViewModel.Message = "Enter the closing cash amount for this session";
                
                var dialogResult = await cashEntryDialog.ShowAsync();
                
                if (dialogResult == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    var closingCash = cashEntryDialog.ViewModel.TotalAmount;
                    
                    var closeSessionHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CloseCashSessionCommand, CloseCashSessionResult>>();
                    
                    var command = new CloseCashSessionCommand
                    {
                        CashSessionId = activeSession.Id,
                        ClosedBy = new UserId(_userService.CurrentUser.Id),
                        ActualCash = new Money(closingCash, "USD")
                    };
                    
                    var result = await closeSessionHandler.HandleAsync(command);
                    
                    _logger.LogInformation("Session {SessionId} ended with closing cash {ClosingCash}", 
                        activeSession.Id, closingCash);
                    
                    var variance = closingCash - result.ExpectedCash.Amount;
                    var varianceMessage = variance == 0 
                        ? "Cash drawer balanced perfectly!" 
                        : $"Cash variance: {variance:C2}";
                    
                    await _dialogService.ShowMessageAsync(
                        "Session Ended",
                        $"POS session has been closed.\n\nExpected Cash: {result.ExpectedCash.Amount:C2}\nActual Cash: {closingCash:C2}\n\n{varianceMessage}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end session");
            await _dialogService.ShowErrorAsync("Error", $"Failed to end session: {ex.Message}");
        }
    }

    private async Task OnReprintAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot reprint: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "There is no active ticket to reprint.");
            return;
        }

        try
        {
            _logger.LogInformation("Reprint requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printReceiptHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintReceiptCommand, PrintReceiptResult>>();
                
                var command = new PrintReceiptCommand
                {
                    TicketId = _ticketId.Value
                };
                
                var result = await printReceiptHandler.HandleAsync(command);
                
                if (result.Success)
                {
                    _logger.LogInformation("Receipt reprinted for ticket {TicketId}", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Receipt Reprinted",
                        $"Receipt has been reprinted.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    _logger.LogError("Failed to reprint receipt");
                    await _dialogService.ShowErrorAsync(
                        "Print Error",
                        "Failed to reprint receipt. Please check the printer and try again.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reprint receipt for ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to reprint receipt: {ex.Message}");
        }
    }

    private async Task OnVoidTicketAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot void ticket: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "There is no active ticket to void.");
            return;
        }

        try
        {
            _logger.LogInformation("Void ticket requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var voidTicketHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<VoidTicketCommand>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                
                // Load the ticket to pass to the dialog
                var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);
                
                if (ticket == null)
                {
                    _logger.LogError("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        "The ticket could not be found. It may have been deleted.");
                    return;
                }
                
                // Convert domain ticket to DTO
                var ticketDto = new TicketDto
                {
                    Id = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    TotalAmount = ticket.TotalAmount.Amount,
                    Status = ticket.Status
                };
                
                // Create ViewModel for void ticket dialog with required dependencies
                var viewModel = new VoidTicketViewModel(voidTicketHandler, userService);
                viewModel.Initialize(ticketDto);
                
                // Create Dialog
                var dialog = new VoidTicketDialog
                {
                    DataContext = viewModel
                };
                
                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }
                
                var result = await dialog.ShowAsync();

                // If the void was successful (dialog handles the void operation internally)
                if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary && !viewModel.HasError)
                {
                    _logger.LogInformation("Ticket {TicketId} voided successfully", _ticketId);
                    
                    // Clear the current ticket and reset the page
                    _ticketId = null;
                    _ticket = null;
                    OrderItems.Clear();
                    RecalculateTotals();
                    OnPropertyChanged(nameof(TicketNumber));
                    OnPropertyChanged(nameof(HasTicket));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to void ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to void ticket: {ex.Message}");
        }
    }

    private async Task OnApplyDiscountAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot apply discount: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before applying a discount.");
            return;
        }

        try
        {
            _logger.LogInformation("Apply discount requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var discountRepository = scope.ServiceProvider.GetRequiredService<IDiscountRepository>();
                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var applyDiscountHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ApplyDiscountCommand>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var managerPinDialog = scope.ServiceProvider.GetRequiredService<ManagerPinDialogViewModel>();
                
                // Load the ticket
                var ticket = await ticketRepository.GetByIdAsync(_ticketId.Value);
                
                if (ticket == null)
                {
                    _logger.LogError("Ticket {TicketId} not found", _ticketId);
                    await _dialogService.ShowErrorAsync(
                        "Ticket Not Found",
                        "The ticket could not be found. It may have been deleted.");
                    return;
                }
                
                // Create ViewModel for discount selection dialog with all required dependencies
                var viewModel = new DiscountSelectionViewModel(
                    discountRepository,
                    applyDiscountHandler,
                    userService,
                    managerPinDialog);
                
                // Set ticket information
                viewModel.TicketId = _ticketId.Value;
                viewModel.TicketTotal = ticket.TotalAmount;
                
                // Load available discounts
                await viewModel.LoadDiscountsAsync();
                
                // Create Dialog
                var dialog = new DiscountSelectionDialog(viewModel);
                
                // Set XamlRoot for the dialog
                if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                {
                    dialog.XamlRoot = element.XamlRoot;
                }
                
                await dialog.ShowAsync();

                // If discount was applied successfully
                if (viewModel.IsSuccess)
                {
                    _logger.LogInformation("Discount applied to ticket {TicketId}", _ticketId);
                    
                    // Reload ticket to get updated totals
                    await LoadTicketAsync();
                    
                    await _dialogService.ShowMessageAsync(
                        "Discount Applied",
                        $"Discount has been applied to the order.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply discount to ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to apply discount: {ex.Message}");
        }
    }

    private async Task OnFireTicketAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot fire ticket: no ticket");
            await _dialogService.ShowWarningAsync(
                "No Ticket",
                "Please create an order before sending to kitchen.");
            return;
        }

        if (OrderItems.Count == 0)
        {
            _logger.LogWarning("Cannot fire ticket: no items in order");
            await _dialogService.ShowWarningAsync(
                "Empty Order",
                "Please add items to the order before sending to kitchen.");
            return;
        }

        try
        {
            _logger.LogInformation("Fire ticket requested for ticket {TicketId}", _ticketId);
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var printToKitchenHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult>>();
                
                var command = new PrintToKitchenCommand
                {
                    TicketId = _ticketId.Value
                };
                
                var result = await printToKitchenHandler.HandleAsync(command);
                
                if (result.Success)
                {
                    _logger.LogInformation("Ticket {TicketId} sent to kitchen", _ticketId);
                    
                    await _dialogService.ShowMessageAsync(
                        "Order Sent",
                        $"Order has been sent to the kitchen.\n\nTicket #{_ticket?.TicketNumber}");
                }
                else
                {
                    var errorMsg = result.Errors.Any() ? string.Join("\n", result.Errors) : result.Message;
                    _logger.LogError("Failed to fire ticket: {Error}", errorMsg);
                    await _dialogService.ShowErrorAsync(
                        "Kitchen Print Error",
                        $"Failed to send order to kitchen:\n\n{errorMsg}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fire ticket {TicketId}", _ticketId);
            await _dialogService.ShowErrorAsync("Error", $"Failed to send order to kitchen: {ex.Message}");
        }
    }

    #endregion

    #region Cleanup

    public void Cleanup()
    {
        _timeUpdateTimer?.Stop();
        _timeUpdateTimer?.Dispose();
    }

    #endregion
}

/// <summary>
/// Represents an order item in the list.
/// </summary>
public partial class OrderItemViewModel : ObservableObject
{
    public Guid OrderItemId { get; set; }
    
    [ObservableProperty]
    private string _productName = string.Empty;
    
    [ObservableProperty]
    private int _quantity;
    
    [ObservableProperty]
    private decimal _unitPrice;
    
    [ObservableProperty]
    private decimal _lineTotal;
    
    public ObservableCollection<string> Modifiers { get; set; } = new();
    
    [ObservableProperty]
    private string? _specialNote;
    
    public bool HasModifiers => Modifiers.Any();
    
    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>
/// Represents a product in the catalog grid.
/// </summary>
public class ProductViewModel
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SubcategoryName { get; set; } = string.Empty;
    public bool HasModifiers { get; set; }
    public bool IsAvailable { get; set; }
}

/// <summary>
/// Represents a product category tab.
/// </summary>
public class ProductCategoryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string IconName { get; set; } = string.Empty;
    public ObservableCollection<string> Subcategories { get; set; } = new();
}
