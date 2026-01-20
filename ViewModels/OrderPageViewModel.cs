using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    private Guid? _ticketId;
    private TicketDto? _ticket;
    private Guid? _tableId;
    private System.Timers.Timer? _timeUpdateTimer;
    private List<ProductViewModel> _allProducts = new();

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
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(WaitTime));
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
        _ticketId = ticketId;
        _tableId = tableId;

        await LoadCategoriesAsync();
        await LoadProductsAsync();

        if (_ticketId.HasValue)
        {
            await LoadTicketAsync();
        }

        if (_tableId.HasValue)
        {
            await LoadTableAsync();
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
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load ticket {TicketId}", _ticketId);
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
                var getTableHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetTableQuery, TableDto?>>();
                var table = await getTableHandler.HandleAsync(new GetTableQuery { TableId = _tableId.Value });

                if (table != null)
                {
                    TableNumber = $"TABLE {table.TableNumber} (GUESTS: {GuestCount})";
                    _logger.LogInformation("Loaded table {TableNumber}", table.TableNumber);
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
            // TODO: Implement category loading from repository
            // For now, create sample categories
            Categories.Clear();
            Categories.Add(new ProductCategoryViewModel { Name = "Food", IconName = "restaurant" });
            Categories.Add(new ProductCategoryViewModel { Name = "Drinks", IconName = "local_bar" });
            Categories.Add(new ProductCategoryViewModel { Name = "Desserts", IconName = "cake" });
            Categories.Add(new ProductCategoryViewModel { Name = "Sides", IconName = "fastfood" });
            Categories.Add(new ProductCategoryViewModel { Name = "Popular", IconName = "star" });
            Categories.Add(new ProductCategoryViewModel { Name = "Retail", IconName = "shopping_bag" });

            // Select first category by default
            if (Categories.Any())
            {
                SelectedCategory = Categories.First();
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load categories");
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var getMenuItemsHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetMenuItemsQuery, List<MenuItemDto>>>();
                var menuRepository = scope.ServiceProvider.GetRequiredService<IMenuRepository>();
                
                var menuItems = await getMenuItemsHandler.HandleAsync(new GetMenuItemsQuery { IsActive = true });

                _allProducts.Clear();
                foreach (var item in menuItems)
                {
                    // Get the full menu item to check for modifiers
                    var menuItem = await menuRepository.GetByIdAsync(item.Id);
                    bool hasModifiers = menuItem?.ModifierGroups.Any() ?? false;
                    
                    _allProducts.Add(new ProductViewModel
                    {
                        ProductId = item.Id,
                        Name = item.Name,
                        SKU = item.Id.ToString().Substring(0, 8), // Use first 8 chars of GUID as SKU
                        Price = item.Price,
                        CategoryName = item.CategoryName ?? "Uncategorized",
                        SubcategoryName = string.Empty,
                        HasModifiers = hasModifiers,
                        IsAvailable = item.IsActive
                    });
                }

                // Apply initial filter
                FilterProducts();

                _logger.LogInformation("Loaded {Count} products", _allProducts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load products");
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
            FilteredProducts.Clear();

            var query = _allProducts.AsEnumerable();

            // Filter by search query (name or SKU, case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.SKU.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filter by category
            if (SelectedCategory != null && SelectedCategory.Name != "Popular")
            {
                query = query.Where(p => p.CategoryName.Equals(SelectedCategory.Name, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by subcategory
            if (!string.IsNullOrWhiteSpace(SelectedSubcategory))
            {
                query = query.Where(p => p.SubcategoryName.Equals(SelectedSubcategory, StringComparison.OrdinalIgnoreCase));
            }

            // Apply filtered results
            foreach (var product in query)
            {
                FilteredProducts.Add(product);
            }

            _logger.LogDebug("Filtered products: Category={Category}, Subcategory={Subcategory}, Search={Search}, Results={Count}",
                SelectedCategory?.Name, SelectedSubcategory, SearchQuery, FilteredProducts.Count);
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
            // TODO: Show table selection dialog
            _logger.LogInformation("Select table requested");
            
            await _dialogService.ShowMessageAsync(
                "Select Table",
                "Table selection feature is coming soon.\n\nThis will allow you to select a table and specify guest count.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show table selection dialog");
        }
    }

    private void OnSearchProduct()
    {
        FilterProducts();
    }

    private async Task OnAddProductAsync(ProductViewModel? product)
    {
        if (product == null) return;

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
                        if (Microsoft.UI.Xaml.Window.Current?.Content is Microsoft.UI.Xaml.FrameworkElement element)
                        {
                            dialog.XamlRoot = element.XamlRoot;
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
                        "Error",
                        "Product not found. Please try again.");
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
                return;
            }

            if (_terminalContext.TerminalId == null)
            {
                _logger.LogError("Cannot create ticket: no terminal context");
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ticket");
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
                return;
            }

            var orderLine = _ticket.OrderLines.FirstOrDefault(ol => ol.Id == item.OrderItemId);
            if (orderLine == null)
            {
                _logger.LogError("Order line {OrderLineId} not found in ticket", item.OrderItemId);
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
                        "Error",
                        "Product not found. Please try again.");
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove order item {ItemId}", item.OrderItemId);
        }
    }

    private void OnSelectCategory(ProductCategoryViewModel? category)
    {
        if (category == null) return;

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
        
        // Clear subcategory selection when category changes
        SelectedSubcategory = null;
        
        FilterProducts();
        
        _logger.LogDebug("Selected category: {CategoryName} with {SubcategoryCount} subcategories", 
            category.Name, Subcategories.Count);
    }

    private void OnSelectSubcategory(string? subcategory)
    {
        SelectedSubcategory = subcategory;
        FilterProducts();
        
        _logger.LogDebug("Selected subcategory: {SubcategoryName}", subcategory);
    }

    private async Task OnSplitOrderAsync()
    {
        try
        {
            _logger.LogInformation("Split order requested");
            
            await _dialogService.ShowMessageAsync(
                "Split Order",
                "Split order feature is coming soon.\n\nThis will allow you to split the order by seat or item.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show split order dialog");
        }
    }

    private async Task OnMergeOrderAsync()
    {
        try
        {
            _logger.LogInformation("Merge order requested");
            
            await _dialogService.ShowMessageAsync(
                "Merge Order",
                "Merge order feature is coming soon.\n\nThis will allow you to merge multiple tickets together.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show merge order dialog");
        }
    }

    private async Task OnAddNoteAsync()
    {
        try
        {
            _logger.LogInformation("Add note requested");
            
            await _dialogService.ShowMessageAsync(
                "Add Note",
                "Add note feature is coming soon.\n\nThis will allow you to add special instructions to the order.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show add note dialog");
        }
    }

    private async Task OnPrintOrderAsync()
    {
        try
        {
            _logger.LogInformation("Print order requested");
            
            await _dialogService.ShowMessageAsync(
                "Print Order",
                "Print order feature is coming soon.\n\nThis will print the order ticket.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print order");
        }
    }

    private async Task OnNavigateToSettleAsync()
    {
        if (!_ticketId.HasValue)
        {
            _logger.LogWarning("Cannot navigate to settle: no ticket");
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
        }
    }

    private async Task OnPayNowAsync()
    {
        try
        {
            _logger.LogInformation("Pay now requested");
            
            await _dialogService.ShowMessageAsync(
                "Pay Now",
                "Quick payment feature is coming soon.\n\nThis will initiate immediate payment processing.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate payment");
        }
    }

    private async Task OnStartSessionAsync()
    {
        try
        {
            _logger.LogInformation("Start session requested");
            
            await _dialogService.ShowMessageAsync(
                "Start Session",
                "Session management feature is coming soon.\n\nThis will start a new POS session.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start session");
        }
    }

    private async Task OnEndSessionAsync()
    {
        try
        {
            _logger.LogInformation("End session requested");
            
            await _dialogService.ShowMessageAsync(
                "End Session",
                "Session management feature is coming soon.\n\nThis will end the current POS session.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to end session");
        }
    }

    private async Task OnReprintAsync()
    {
        try
        {
            _logger.LogInformation("Reprint requested");
            
            await _dialogService.ShowMessageAsync(
                "Reprint",
                "Reprint feature is coming soon.\n\nThis will reprint the last ticket.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reprint");
        }
    }

    private async Task OnVoidTicketAsync()
    {
        try
        {
            _logger.LogInformation("Void ticket requested");
            
            await _dialogService.ShowMessageAsync(
                "Void Ticket",
                "Void ticket feature is coming soon.\n\nThis will void the current ticket.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to void ticket");
        }
    }

    private async Task OnApplyDiscountAsync()
    {
        try
        {
            _logger.LogInformation("Apply discount requested");
            
            await _dialogService.ShowMessageAsync(
                "Apply Discount",
                "Discount feature is coming soon.\n\nThis will allow you to apply promotional discounts.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show discount dialog");
        }
    }

    private async Task OnFireTicketAsync()
    {
        try
        {
            _logger.LogInformation("Fire ticket requested");
            
            await _dialogService.ShowMessageAsync(
                "Fire Ticket",
                "Fire ticket feature is coming soon.\n\nThis will send the order to the kitchen.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fire ticket");
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
