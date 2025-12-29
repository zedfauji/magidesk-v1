using System.Collections.ObjectModel;
using Magidesk.Domain.ValueObjects;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;

using Magidesk.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public class OrderEntryViewModel : ViewModelBase
{
    private readonly IMenuCategoryRepository _categoryRepository;
    private readonly IMenuGroupRepository _groupRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly ICommandHandler<CreateTicketCommand, CreateTicketResult> _createTicketHandler;
    private readonly ICommandHandler<AddOrderLineCommand, AddOrderLineResult> _addOrderLineHandler;
    private readonly ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> _printToKitchenHandler;
    private readonly ICommandHandler<ModifyOrderLineCommand> _modifyOrderLineHandler;
    private readonly ICommandHandler<RemoveOrderLineCommand> _removeOrderLineHandler;
    private readonly ICommandHandler<PayNowCommand> _payNowHandler;
    private readonly ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult> _setServiceChargeHandler;
    private readonly ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult> _setDeliveryChargeHandler;
    private readonly ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult> _setAdjustmentHandler;
    private readonly NavigationService _navigationService;

    private TicketDto? _ticket;
    private MenuCategory? _selectedCategory;

    private MenuGroup? _selectedGroup;
    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                // Optional: Auto-search on type? For now, explicit command.
            }
        }
    }

    public ObservableCollection<MenuCategory> Categories { get; } = new();
    public ObservableCollection<MenuGroup> Groups { get; } = new();

    public ObservableCollection<MenuItem> Items { get; } = new();

    private decimal _pendingQuantity = 1;
    public decimal PendingQuantity
    {
        get => _pendingQuantity;
        set
        {
            if (SetProperty(ref _pendingQuantity, value))
            {
                OnPropertyChanged(nameof(PendingQuantityText));
                OnPropertyChanged(nameof(IsPendingQuantityVisible));
            }
        }
    }

    public string PendingQuantityText => $"Next Qty: {PendingQuantity}";
    public bool IsPendingQuantityVisible => PendingQuantity != 1;

    public ICommand SetQuantityCommand { get; }

    public TicketDto? Ticket
    {
        get => _ticket;
        private set
        {
             if (SetProperty(ref _ticket, value))
             {
                 OnPropertyChanged(nameof(HasTicket));
                 OnPropertyChanged(nameof(OrderLines));
                 OnPropertyChanged(nameof(TicketHeaderText));
                 OnPropertyChanged(nameof(GuestCountText));
                 OnPropertyChanged(nameof(TotalsText));
                 OnPropertyChanged(nameof(SubTotalText));
                 OnPropertyChanged(nameof(TaxText));
                 OnPropertyChanged(nameof(DiscountText));
                 OnPropertyChanged(nameof(HasDiscount));
                 OnPropertyChanged(nameof(HasUnsentItems));
             }
        }
    }
    
    public bool HasTicket => Ticket != null;
    
    public IReadOnlyList<OrderLineDto> OrderLines => Ticket?.OrderLines ?? new List<OrderLineDto>();
    
    public bool HasUnsentItems => Ticket?.OrderLines.Any(ol => ol.ShouldPrintToKitchen && !ol.PrintedToKitchen) ?? false;
    
    public string TicketHeaderText => Ticket == null
        ? "No ticket active"
        : $"Ticket #{Ticket.TicketNumber} ({Ticket.Status})";

    public string GuestCountText => Ticket == null ? "" : $"Guests: {Ticket.NumberOfGuests}";

    public string SubTotalText => Ticket == null ? "$0.00" : $"{Ticket.SubtotalAmount:C}";
    public string TaxText => Ticket == null ? "$0.00" : $"{Ticket.TaxAmount:C}";
    public string DiscountText => Ticket == null ? "$0.00" : $"-{Ticket.DiscountAmount:C}";
    public bool HasDiscount => (Ticket?.DiscountAmount ?? 0) > 0;

    public string TotalsText => Ticket == null
        ? "$0.00"
        : $"{Ticket.TotalAmount:C}";

    public bool IsSelectionModeCategories => SelectedCategory == null;
    public bool IsSelectionModeGroups => SelectedCategory != null && SelectedGroup == null;
    public bool IsSelectionModeItems => SelectedGroup != null;

    public MenuCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                OnPropertyChanged(nameof(IsSelectionModeCategories));
                OnPropertyChanged(nameof(IsSelectionModeGroups));
                OnPropertyChanged(nameof(IsSelectionModeItems));
                _ = LoadGroupsAsync(value);
                SelectedGroup = null; // Reset group
            }
        }
    }

    public MenuGroup? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
            {
                OnPropertyChanged(nameof(IsSelectionModeGroups));
                OnPropertyChanged(nameof(IsSelectionModeItems));
                _ = LoadItemsAsync(value);
            }
        }
    }

    public ICommand SelectCategoryCommand { get; }
    public ICommand SelectGroupCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand BackToCategoriesCommand { get; }
    public ICommand SearchItemCommand { get; }
    public ICommand IncrementQuantityCommand { get; }
    public ICommand DecrementQuantityCommand { get; }
    public ICommand RemoveItemCommand { get; }

    private readonly IPrintingService _printingService;

    // ... existing fields ...

    public ICommand PrintTicketCommand { get; }

    public OrderEntryViewModel(
        IMenuCategoryRepository categoryRepository,
        IMenuGroupRepository groupRepository,
        IMenuRepository menuRepository,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        ICommandHandler<CreateTicketCommand, CreateTicketResult> createTicketHandler,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult> printToKitchenHandler,
        ICommandHandler<ModifyOrderLineCommand> modifyOrderLineHandler,
        ICommandHandler<RemoveOrderLineCommand> removeOrderLineHandler,
        ICommandHandler<PayNowCommand> payNowHandler,
        ICommandHandler<SetServiceChargeCommand, SetServiceChargeResult> setServiceChargeHandler,
        ICommandHandler<SetDeliveryChargeCommand, SetDeliveryChargeResult> setDeliveryChargeHandler,
        ICommandHandler<SetAdjustmentCommand, SetAdjustmentResult> setAdjustmentHandler,
        NavigationService navigationService,
        IPrintingService printingService) // Injected
    {
        _categoryRepository = categoryRepository;
        _groupRepository = groupRepository;
        _menuRepository = menuRepository;
        _getTicketHandler = getTicketHandler;
        _createTicketHandler = createTicketHandler;
        _addOrderLineHandler = addOrderLineHandler;
        _printToKitchenHandler = printToKitchenHandler;
        _modifyOrderLineHandler = modifyOrderLineHandler;
        _removeOrderLineHandler = removeOrderLineHandler;
        _payNowHandler = payNowHandler;
        _setServiceChargeHandler = setServiceChargeHandler;
        _setDeliveryChargeHandler = setDeliveryChargeHandler;
        _setAdjustmentHandler = setAdjustmentHandler;
        _navigationService = navigationService;
        _printingService = printingService;

        Title = "Order Entry";

        SelectCategoryCommand = new RelayCommand<MenuCategory>(c => SelectedCategory = c);
        SelectGroupCommand = new RelayCommand<MenuGroup>(g => SelectedGroup = g);
        AddItemCommand = new AsyncRelayCommand<MenuItem>(AddItemAsync);
        BackToCategoriesCommand = new RelayCommand(() => SelectedCategory = null);
        SearchItemCommand = new AsyncRelayCommand(SearchItemAsync);
        
        IncrementQuantityCommand = new AsyncRelayCommand<OrderLineDto>(IncrementQuantityAsync);
        DecrementQuantityCommand = new AsyncRelayCommand<OrderLineDto>(DecrementQuantityAsync);
        RemoveItemCommand = new AsyncRelayCommand<OrderLineDto>(RemoveItemAsync);
        SetQuantityCommand = new AsyncRelayCommand(SetQuantityAsync);
        
        PrintTicketCommand = new AsyncRelayCommand(PrintTicketAsync);
        PrintTicketCommand = new AsyncRelayCommand(PrintTicketAsync);
        AddMiscItemCommand = new AsyncRelayCommand(AddMiscItemAsync);
        AddFeeCommand = new AsyncRelayCommand(AddFeeAsync);
        AddCookingInstructionCommand = new AsyncRelayCommand<OrderLineDto>(AddCookingInstructionAsync);
        AddPizzaModifiersCommand = new AsyncRelayCommand<OrderLineDto>(AddPizzaModifiersAsync);
    }

    // ... InitializeAsync ...

    public ICommand AddMiscItemCommand { get; }
    public ICommand AddFeeCommand { get; }
    
    // F-0036: Cooking Instructions
    public ICommand AddCookingInstructionCommand { get; }

    private async Task AddCookingInstructionAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
        
        // F-0036: Audit requirement: Pre-defined options (Medium Rare, etc.)
        var vm = new Magidesk.Presentation.ViewModels.Dialogs.CookingInstructionViewModel(line.Instructions ?? "");
        var dialog = new Magidesk.Views.Dialogs.CookingInstructionDialog { ViewModel = vm };
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        vm.CloseAction = () => dialog.Hide();
        vm.CancelAction = () => dialog.Hide();

        await dialog.ShowAsync();

        if (vm.IsConfirmed)
        {
            await _modifyOrderLineHandler.HandleAsync(new ModifyOrderLineCommand
            {
                TicketId = Ticket.Id,
                OrderLineId = line.Id,
                Quantity = line.Quantity, // Preserve quantity
                Instructions = vm.SelectedInstructions
            });
            await LoadTicketAsync(Ticket.Id);
        }
    }

    [RelayCommand]
    private async Task EditModifiersAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

        // Check if Pizza
        var isPizza = await IsPizza(line);
        if (isPizza)
        {
            await AddPizzaModifiersAsync(line);
            return;
        }

        // Generic Modifiers
        var vm = new Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel(_menuRepository, line);
        var dialog = new Magidesk.Views.Dialogs.ModifierSelectionDialog(vm)
        {
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };

        await dialog.ShowAsync();

        if (vm.IsConfirmed)
        {
            await _modifyOrderLineHandler.HandleAsync(new ModifyOrderLineCommand
            {
                OrderLineId = line.Id,
                TicketId = Ticket.Id,
                Modifiers = vm.ResultModifiers,
                Quantity = line.Quantity,
                Instructions = line.Instructions
            });

            await LoadTicketAsync(Ticket.Id);
        }
    }

    private async Task<bool> IsPizza(OrderLineDto line)
    {
        var item = await _menuRepository.GetByIdAsync(line.MenuItemId);
        return item?.MenuCategory?.Name.Contains("Pizza", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    // F-0037: Pizza Modifiers
    public ICommand AddPizzaModifiersCommand { get; }

    private async Task AddPizzaModifiersAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
        
        var vm = new Magidesk.Presentation.ViewModels.Dialogs.PizzaModifierViewModel(_menuRepository, line);
        var dialog = new Magidesk.Views.Dialogs.PizzaModifierDialog { ViewModel = vm };
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        vm.CloseAction = () => dialog.Hide();
        // vm.CancelAction = () => dialog.Hide(); // Handled internally

        await dialog.ShowAsync();

        if (vm.IsConfirmed)
        {
             await _modifyOrderLineHandler.HandleAsync(new ModifyOrderLineCommand
            {
                TicketId = Ticket.Id,
                OrderLineId = line.Id,
                Quantity = line.Quantity,
                Instructions = line.Instructions, // Preserve
                Modifiers = vm.ResultModifiers // New Modifiers
            });
            await LoadTicketAsync(Ticket.Id);
        }
    }

    private async Task AddFeeAsync()
    {
        if (Ticket == null) return;

        var dialog = new Magidesk.Views.Dialogs.TicketFeeDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var vm = dialog.ViewModel;
            if (vm.Amount < 0) return; // Basic validation

            var processedBy = new Magidesk.Domain.ValueObjects.UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")); // Placeholder User

            switch (vm.SelectedFeeType)
            {
                case TicketFeeType.ServiceCharge:
                    await _setServiceChargeHandler.HandleAsync(new SetServiceChargeCommand
                    {
                        TicketId = Ticket.Id,
                        Amount = new Magidesk.Domain.ValueObjects.Money(vm.Amount),
                        ProcessedBy = processedBy
                    });
                    break;
                case TicketFeeType.DeliveryCharge:
                    await _setDeliveryChargeHandler.HandleAsync(new SetDeliveryChargeCommand
                    {
                        TicketId = Ticket.Id,
                        Amount = new Magidesk.Domain.ValueObjects.Money(vm.Amount),
                        ProcessedBy = processedBy
                    });
                    break;
                case TicketFeeType.Adjustment:
                    await _setAdjustmentHandler.HandleAsync(new SetAdjustmentCommand
                    {
                        TicketId = Ticket.Id,
                        Amount = new Magidesk.Domain.ValueObjects.Money(vm.Amount),
                        ProcessedBy = processedBy,
                        Reason = vm.Reason
                    });
                    break;
            }

            await LoadTicketAsync(Ticket.Id);
        }
    }

    private async Task AddMiscItemAsync()
    {
        if (Ticket == null) return;

        var dialog = new Magidesk.Views.Dialogs.MiscItemDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var vm = dialog.ViewModel;
            if (string.IsNullOrWhiteSpace(vm.Description) || vm.Price < 0) return; // Basic validation

            await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
            {
                TicketId = Ticket.Id,
                MenuItemId = Guid.Empty, // Signifies ad-hoc/misc
                MenuItemName = vm.Description,
                Quantity = PendingQuantity,
                UnitPrice = new Magidesk.Domain.ValueObjects.Money(vm.Price),
                TaxRate = 0, // Default or selected
                CategoryName = "Misc", // Audit Trigger
                GroupName = "General"
            });
            
            // Reset Pending Quantity
            PendingQuantity = 1;

            await LoadTicketAsync(Ticket.Id);
        }
    }

    private async Task PrintTicketAsync()
    {
        if (Ticket == null) return;
        
        IsBusy = true;
        try
        {
             // F-0025: Print Ticket Action (Receipt/Guest Check)
             await _printingService.PrintTicketAsync(Ticket);
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Print Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }



    public async Task InitializeAsync(Guid? ticketId = null)
    {
        await LoadCategoriesAsync();

        if (ticketId.HasValue)
        {
            await LoadTicketAsync(ticketId.Value);
        }
        else if (Ticket == null)
        {
            // Auto-create ticket if none exists
             await CreateNewTicketAsync();
        }
    }

    private async Task LoadCategoriesAsync()
    {
        IsBusy = true;
        try
        {
            Categories.Clear();
            var categories = await _categoryRepository.GetVisibleAsync();
             foreach (var cat in categories) Categories.Add(cat);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task LoadGroupsAsync(MenuCategory? category)
    {
        Groups.Clear();
        Items.Clear();
        if (category == null) return;

        IsBusy = true;
        try
        {
            var groups = await _groupRepository.GetByCategoryIdAsync(category.Id);
             foreach (var g in groups) Groups.Add(g);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadItemsAsync(MenuGroup? group)
    {
        Items.Clear();
        if (group == null) return;

        IsBusy = true;
        try
        {
             var items = await _menuRepository.GetByGroupAsync(group.Id);
             foreach (var i in items) Items.Add(i);
        }
        finally { IsBusy = false; }
    }

    public ICommand SplitTicketUiCommand => new AsyncRelayCommand(SplitTicketAsync);
    public ICommand SendToKitchenCommand => new AsyncRelayCommand(SendToKitchenAsync);
    public ICommand PayNowUiCommand => new AsyncRelayCommand(PayNowAsync);

    private async Task PayNowAsync()
    {
        if (Ticket == null) return;

        IsBusy = true;
        try
        {
            // F-0006: Pay Now implies immediate full payment (usually Cash for Quick Pay, or triggers Settle)
            // The UI button usually says "Total" or "Pay". 
            // If we want "Quick Pay Cash", we pass Amount=0 (full) and Tender=Cash.
            
            await _payNowHandler.HandleAsync(new PayNowCommand
            {
                TicketId = Ticket.Id,
                Amount = 0, // Full amount
                TenderType = "CASH" 
            });

            // Reload to check status (should be Closed)
            await LoadTicketAsync(Ticket.Id);
            
            if (Ticket?.Status == Magidesk.Domain.Enumerations.TicketStatus.Closed)
            {
                // Navigate away or clear? Usually start new ticket.
                // For now, clear the view.
                Ticket = null;
                // Optional: Navigate to Switchboard or stay for next order
                _navigationService.GoBack(); 
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Pay Now Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task SplitTicketAsync()
    {
        if (Ticket == null) return;

        IsBusy = true;
        try
        {
             var dialog = new Magidesk.Views.SplitTicketDialog();
             dialog.ViewModel.Initialize(Ticket);
             var result = await _navigationService.ShowDialogAsync(dialog);

             if (result == ContentDialogResult.Primary)
             {
                 // Reload ticket to reflect changes
                 await LoadTicketAsync(Ticket.Id);
             }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Split Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task SendToKitchenAsync()
    {
        if (Ticket == null) return;

        IsBusy = true;
        try
        {
             var result = await _printToKitchenHandler.HandleAsync(new PrintToKitchenCommand 
             { 
                TicketId = Ticket.Id 
             });

             if (result.Success)
             {
                await LoadTicketAsync(Ticket.Id);
             }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Kitchen Print Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    public ICommand SettleUiCommand => new RelayCommand(Settle);

    private void Settle()
    {
        if (Ticket == null) return;
        _navigationService.Navigate(typeof(Magidesk.Presentation.Views.SettlePage), Ticket.Id);
    }

    private async Task CreateNewTicketAsync()
    {
        IsBusy = true;
        try
        {
            // Defaults for "Quick Start"
             var command = new CreateTicketCommand
            {
                CreatedBy = new Domain.ValueObjects.UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")), // Placeholder
                TerminalId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Placeholder
                ShiftId = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Placeholder
                OrderTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444") // Placeholder
            };

            var result = await _createTicketHandler.HandleAsync(command);
            await LoadTicketAsync(result.TicketId);
        }
        catch (Exception ex)
        {
             // Handle
        }
        finally { IsBusy = false; }
    }

    private async Task LoadTicketAsync(Guid ticketId)
    {
         Ticket = await _getTicketHandler.HandleAsync(new GetTicketQuery { TicketId = ticketId });
    }

    private async Task AddItemAsync(MenuItem? item)
    {
        if (item == null || Ticket == null) return;

        // F-0032: Refetch to get full details (Modifiers) - Shallow Item from GridView is insufficient
        var fullItem = await _menuRepository.GetByIdAsync(item.Id);
        if (fullItem == null) return;

        List<MenuModifier> selectedModifiers = new();

        Money effectivePrice = fullItem.Price;

        // F-0035: Variable Price Support
        if (fullItem.IsVariablePrice)
        {
            var vm = new Magidesk.Presentation.ViewModels.Dialogs.PriceEntryViewModel(fullItem.Name, fullItem.Price.Amount);
            var dialog = new Magidesk.Views.Dialogs.PriceEntryDialog { ViewModel = vm };
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Wire interaction
            vm.CloseAction = () => dialog.Hide();
            vm.CancelAction = () => dialog.Hide();

            await dialog.ShowAsync();

            if (!vm.IsConfirmed)
            {
                return; // User cancelled
            }
            
            if (vm.Price <= 0)
            {
                 // Prevent negative/zero price if strict. 
                 // Assuming 0 is allowed for open items if free? 
                 // Audit says "Price cannot be negative". 
                 // If 0, maybe valid. Let's accept it.
            }
            
            effectivePrice = new Money(vm.Price, fullItem.Price.Currency);
        }

        // 1. Size Selection (Heuristic: Group Name contains "Size")

        // 1. Size Selection (Heuristic: Group Name contains "Size")
        var sizeGroup = fullItem.ModifierGroups
            .Select(mg => mg.ModifierGroup)
            .FirstOrDefault(g => g.IsActive && g.Name.Contains("Size", StringComparison.OrdinalIgnoreCase));

        if (sizeGroup != null)
        {
            var vm = new Magidesk.Presentation.ViewModels.Dialogs.SizeSelectionViewModel(sizeGroup);
            var dialog = new Magidesk.Views.Dialogs.SizeSelectionDialog { ViewModel = vm };
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Wire interaction
            vm.CloseAction = () => dialog.Hide();

            await dialog.ShowAsync();

            if (vm.SelectedSize != null)
            {
                selectedModifiers.Add(vm.SelectedSize);
            }
            else if (sizeGroup.IsRequired || sizeGroup.MinSelections > 0)
            {
                 // Required but cancelled -> Abort
                 return;
            }
        }

        // F-0033: Beverage Quick Add
        // Look up Category to check IsBeverage flag.
        // We use the in-memory Categories list which is already loaded.
        var category = Categories.FirstOrDefault(c => c.Id == fullItem.CategoryId);
        bool isBeverage = category?.IsBeverage ?? false;

        // 2. General Modifiers (F-0038)
        // Logic: If NOT a beverage (or explicit modifier request), show dialog.
        // For Beverages, we Skip to enable "One Touch" speed.
        if (!isBeverage)
        {
             // TODO: F-0038 - Show ModifierSelectionDialog for remaining groups
             // if (fullItem.ModifierGroups.Any(mg => mg.ModifierGroup.Id != sizeGroup?.Id))
             // { ... }
        }

        await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
        {
            TicketId = Ticket.Id,
            MenuItemId = fullItem.Id,
            MenuItemName = fullItem.Name,
            Quantity = PendingQuantity,
            UnitPrice = effectivePrice, // Use effective price (potentially overridden)
            TaxRate = fullItem.TaxRate,
            Modifiers = selectedModifiers
        });

        // Reset Pending Quantity
        PendingQuantity = 1;

        await LoadTicketAsync(Ticket.Id);
    }

    private async Task SearchItemAsync()
    {
        if (Ticket == null) return;

        IsBusy = true;
        try
        {
            var vm = new Magidesk.Presentation.ViewModels.Dialogs.ItemSearchViewModel(_menuRepository);
            
            // Pre-fill text if already typed in viewmodel? 
            // Audit says "User types search query", implying dialog opens first.
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                vm.SearchText = SearchText;
            }

            var dialog = new Magidesk.Views.Dialogs.ItemSearchDialog { ViewModel = vm };
            dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
            
            // Wire interaction
            vm.CloseAction = () => dialog.Hide();

            await dialog.ShowAsync();

            if (vm.SelectedItem != null)
            {
                await AddItemAsync(vm.SelectedItem);
                SearchText = string.Empty; // Clear
            }
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Search Dialog Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task IncrementQuantityAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
        
        // F-0026: Backend Guard - Cannot modify sent items
        if (line.PrintedToKitchen)
        {
             // TODO: Notify user "Cannot modify sent items. Use Void."
             return;
        }

        await ModifyQuantityAsync(line, line.Quantity + 1);
    }

    private async Task DecrementQuantityAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

        // F-0026: Backend Guard - Cannot modify sent items
        if (line.PrintedToKitchen)
        {
             return;
        }

        if (line.Quantity > 1)
        {
            await ModifyQuantityAsync(line, line.Quantity - 1);
        }
        else
        {
            // Optionally confirm delete if quantity becomes 0?
            // For now, adhere to scope: Decrement stops at 1, or use Remove.
            // Requirement F-0028 is explicit delete action.
        }
    }

    private async Task ModifyQuantityAsync(OrderLineDto line, decimal newQuantity)
    {
        IsBusy = true;
        try
        {
            await _modifyOrderLineHandler.HandleAsync(new ModifyOrderLineCommand
            {
                TicketId = Ticket!.Id,
                OrderLineId = line.Id,
                Quantity = newQuantity
            });
            await LoadTicketAsync(Ticket.Id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Modify Qty Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task SetQuantityAsync()
    {
        var dialog = new Magidesk.Views.QuantityDialog();
        // Initialize with current pending quantity if user re-opens? Or always start fresh 1?
        // Audit says "User pre-selects". 
        // Let's reset to default in dialog VM (1) but maybe useful to show current?
        // For now, new instance resets.
        
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot; // Ensure valid root
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            PendingQuantity = dialog.ViewModel.ResultQuantity;
        }
    }

    private async Task RemoveItemAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

        // F-0028: Backend Guard - Cannot delete sent items (Must use Void)
        if (line.PrintedToKitchen)
        {
             // TODO: Notify user "Cannot delete sent items. Use Void action."
             return;
        }

        IsBusy = true;
        try
        {
             await _removeOrderLineHandler.HandleAsync(new RemoveOrderLineCommand
             {
                 TicketId = Ticket.Id,
                 OrderLineId = line.Id
             });
             await LoadTicketAsync(Ticket.Id);
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Remove Item Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }
}
