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
using Magidesk.ViewModels.Dialogs;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.ViewModels;

public partial class OrderEntryViewModel : ViewModelBase
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
    private readonly IUserService _userService;
    private readonly ITerminalContext _terminalContext;
    private readonly ICommandHandler<ChangeSeatCommand> _changeSeatHandler;
    private readonly ICommandHandler<MergeTicketsCommand> _mergeTicketsHandler;
    private readonly ICommandHandler<ChangeTableCommand, ChangeTableResult> _changeTableHandler;
    private readonly ICommandHandler<SetCustomerCommand, SetCustomerResult> _setCustomerHandler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKitchenRoutingService _kitchenRoutingService;

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

    public ICommand SettleCommand { get; }
    public ICommand QuickPayCommand { get; }

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
    
    public bool HasTicketWithItems => Ticket != null && Ticket.OrderLines.Any();
    
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
        IPrintingService printingService,
        IUserService userService,
        ITerminalContext terminalContext,
        ICommandHandler<ChangeSeatCommand> changeSeatHandler,
        ICommandHandler<MergeTicketsCommand> mergeTicketsHandler,
        ICommandHandler<ChangeTableCommand, ChangeTableResult> changeTableHandler,
        ICommandHandler<SetCustomerCommand, SetCustomerResult> setCustomerHandler,
        IServiceProvider serviceProvider,
        IKitchenRoutingService kitchenRoutingService)
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
        _userService = userService;
        _terminalContext = terminalContext;
        _changeSeatHandler = changeSeatHandler;
        _mergeTicketsHandler = mergeTicketsHandler;
        _changeTableHandler = changeTableHandler;
        _setCustomerHandler = setCustomerHandler;
        _serviceProvider = serviceProvider;
        _kitchenRoutingService = kitchenRoutingService;

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
        AddMiscItemCommand = new AsyncRelayCommand(AddMiscItemAsync);
        AddFeeCommand = new AsyncRelayCommand(AddFeeAsync);
        AddCookingInstructionCommand = new AsyncRelayCommand<OrderLineDto>(AddCookingInstructionAsync);
        AddPizzaModifiersCommand = new AsyncRelayCommand<OrderLineDto>(AddPizzaModifiersAsync);
        ChangeSeatCommand = new AsyncRelayCommand<OrderLineDto>(ChangeSeatAsync);
        MergeTicketsCommand = new AsyncRelayCommand(MergeTicketsAsync);
        ChangeTableCommand = new AsyncRelayCommand(ChangeTableAsync);
        AssignCustomerCommand = new AsyncRelayCommand(AssignCustomerAsync);
        SettleCommand = new AsyncRelayCommand(SettleAsync);
        QuickPayCommand = new AsyncRelayCommand(QuickPayAsync);
    }

    // ... InitializeAsync ...

    public ICommand AddMiscItemCommand { get; }
    public ICommand AddFeeCommand { get; }
    
    // F-0036: Cooking Instructions
    public ICommand AddCookingInstructionCommand { get; }
    public ICommand ChangeSeatCommand { get; }
    public ICommand MergeTicketsCommand { get; }
    public ICommand ChangeTableCommand { get; }
    public ICommand AssignCustomerCommand { get; }



    private async Task MergeTicketsAsync()
    {
        if (Ticket == null) return;
        
        // F-0040: Merge current ticket INTO another ticket.
        // We need GetOpenTicketsHandler for the dialog, but the VM handles it internally if we instantiate correctly.
        // The dialog VM needs IQueryHandler<GetOpenTicketsQuery...>. 
        // We can resolve it from App.Services for the dialog since it's a transient VM inside a dialog.
        
        try
        {
             var queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<GetOpenTicketsQuery, IEnumerable<TicketDto>>>();
             var vm = new Magidesk.Presentation.ViewModels.Dialogs.MergeTicketsViewModel(queryHandler, Ticket.Id);
             
             var dialog = new Magidesk.Views.Dialogs.MergeTicketsDialog { ViewModel = vm };
             dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
             
             vm.CloseAction = () => dialog.Hide();
             
             await dialog.ShowAsync();
             
             if (vm.IsConfirmed && vm.SelectedTargetTicket != null)
             {
                 if (_userService.CurrentUser?.Id == null) return;

                 await _mergeTicketsHandler.HandleAsync(new MergeTicketsCommand
                 {
                     SourceTicketId = Ticket.Id,
                     TargetTicketId = vm.SelectedTargetTicket.Id,
                     ProcessedBy = _userService.CurrentUser.Id
                 });
                 
                 // Current ticket is now void/merged. Navigate back.
                 _navigationService.GoBack();
             }
        }
        catch (Exception ex)
        {
             var errorDialog = new Microsoft.UI.Xaml.Controls.ContentDialog
             {
                 Title = "Merge Error",
                 Content = ex.Message,
                 CloseButtonText = "OK",
                 XamlRoot = App.MainWindowInstance.Content.XamlRoot
             };
             await _navigationService.ShowDialogAsync(errorDialog);
        }
        }


    private async Task AssignCustomerAsync()
    {
        if (Ticket == null) return;

        // F-0077: Assign Customer (Stub)
        var dialog = new Magidesk.Views.Dialogs.CustomerSelectionDialog();
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        // ViewModel is auto-created by the view

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.ViewModel.IsConfirmed)
        {
             if (_userService.CurrentUser?.Id == null) return;

             var cmd = new SetCustomerCommand
             {
                 TicketId = Ticket.Id,
                 GuestName = dialog.ViewModel.GuestName,
                 PhoneNumber = dialog.ViewModel.PhoneNumber,
                 ModifiedBy = _userService.CurrentUser.Id
             };

             var outcome = await _setCustomerHandler.HandleAsync(cmd);
             
             if (outcome.Success)
             {
                 await LoadTicketAsync(Ticket.Id);
             }
             else
             {
                 // Handle Error
                 System.Diagnostics.Debug.WriteLine($"Error assigning customer: {outcome.ErrorMessage}");
             }
        }
    }

    private async Task ChangeTableAsync()
    {
        if (Ticket == null) return;

        // F-0080: Change Table Action
        // Use injected ServiceProvider to resolve transient VM with dependencies
        var vm = _serviceProvider.GetRequiredService<Magidesk.Presentation.ViewModels.Dialogs.TableSelectionViewModel>();
        await vm.InitializeAsync();

        var dialog = new Magidesk.Views.Dialogs.TableSelectionDialog { ViewModel = vm };
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        vm.CloseAction = () => dialog.Hide();

        await dialog.ShowAsync();

        if (vm.IsConfirmed && vm.SelectedTable != null)
        {
            if (_userService.CurrentUser?.Id == null) return;

            var result = await _changeTableHandler.HandleAsync(new ChangeTableCommand
            {
                TicketId = Ticket.Id,
                NewTableId = vm.SelectedTable.Id,
                UserId = _userService.CurrentUser.Id
            });

            if (result.Success)
            {
                await LoadTicketAsync(Ticket.Id);
            }
            else
            {
                 var errorDialog = new ContentDialog
                 {
                     Title = "Change Table Error",
                     Content = result.ErrorMessage,
                     CloseButtonText = "OK",
                     XamlRoot = App.MainWindowInstance.Content.XamlRoot
                 };
                 await _navigationService.ShowDialogAsync(errorDialog);
            }
        }
    }

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

    [RelayCommand]
    private async Task AddOnSelectionAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

        // Show add-on selection dialog
        var vm = new Magidesk.ViewModels.Dialogs.AddOnSelectionViewModel(_menuRepository, line);
        var dialog = new Magidesk.Views.Dialogs.AddOnSelectionDialog(vm)
        {
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };

        vm.CloseAction = () => dialog.Hide();

        await dialog.ShowAsync();

        if (vm.IsConfirmed && vm.ResultAddOns.Any())
        {
            // Add selected add-ons as separate order lines
            foreach (var addOn in vm.ResultAddOns)
            {
                if (addOn.ModifierId.HasValue)
                {
                    await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
                    {
                        TicketId = Ticket.Id,
                        MenuItemId = addOn.ModifierId.Value,
                        MenuItemName = addOn.Name,
                        Quantity = 1, // Add-ons are typically single quantity
                        UnitPrice = new Magidesk.Domain.ValueObjects.Money(addOn.UnitPrice),
                        TaxRate = addOn.TaxRate,
                        Modifiers = new List<Magidesk.Domain.Entities.MenuModifier>(), // No modifiers on add-ons
                        CategoryName = "Add-on", // Special category for add-ons
                        GroupName = "Upsell"
                    });
                }
            }

            await LoadTicketAsync(Ticket.Id);
        }
    }

    [RelayCommand]
    private async Task ComboSelectionAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

        // Show combo selection dialog
        var vm = new Magidesk.ViewModels.Dialogs.ComboSelectionViewModel(_menuRepository, line);
        var dialog = new Magidesk.Views.Dialogs.ComboSelectionDialog(vm)
        {
            XamlRoot = App.MainWindowInstance.Content.XamlRoot
        };

        vm.CloseAction = () => dialog.Hide();

        await dialog.ShowAsync();

        if (vm.IsConfirmed && vm.ResultSelections.Any())
        {
            // For now, we'll just add the selected combo items as separate order lines
            // In a real implementation, this would create a proper combo structure
            foreach (var selection in vm.ResultSelections)
            {
                await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
                {
                    TicketId = Ticket.Id,
                    MenuItemId = selection.MenuItemId,
                    MenuItemName = selection.ItemName,
                    Quantity = 1,
                    UnitPrice = new Magidesk.Domain.ValueObjects.Money(selection.BasePrice + selection.Upcharge),
                    TaxRate = 0, // Simplified, would use actual tax rate
                    Modifiers = new List<Magidesk.Domain.Entities.MenuModifier>(),
                    CategoryName = "Combo",
                    GroupName = selection.GroupName
                });
            }

            // Remove the original combo item (placeholder implementation)
            await _removeOrderLineHandler.HandleAsync(new RemoveOrderLineCommand
            {
                TicketId = Ticket.Id,
                OrderLineId = line.Id
            });

            await LoadTicketAsync(Ticket.Id);
        }
    }

    private async Task<bool> IsPizza(OrderLineDto line)
    {
        var item = await _menuRepository.GetByIdAsync(line.MenuItemId);
        if (item == null) return false;
        
        var category = Categories.FirstOrDefault(c => c.Id == item.CategoryId);
        if (category?.Name.Contains("Pizza", StringComparison.OrdinalIgnoreCase) == true) return true;

        var group = Groups.FirstOrDefault(g => g.Id == item.GroupId);
        // If group not in memory (e.g. browsing another category), fetch it
        if (group == null && item.GroupId.HasValue)
        {
             group = await _groupRepository.GetByIdAsync(item.GroupId.Value);
        }
        
        return group?.Name.Contains("Pizza", StringComparison.OrdinalIgnoreCase) ?? false;
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

            if (_userService.CurrentUser?.Id == null)
            {
                return;
            }

            var processedBy = _userService.CurrentUser.Id;

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

    private async Task ChangeSeatAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
        
        // F-0036: Seat Assignment Dialog
        var vm = new Magidesk.Presentation.ViewModels.Dialogs.SeatSelectionViewModel();
        var dialog = new Magidesk.Views.Dialogs.SeatSelectionDialog { ViewModel = vm };
        dialog.XamlRoot = App.MainWindowInstance.Content.XamlRoot;
        
        vm.CloseAction = () => dialog.Hide();
        
        await dialog.ShowAsync();
        
        if (vm.IsConfirmed && vm.ResultSeatNumber.HasValue)
        {
             try 
            {
                 await _changeSeatHandler.HandleAsync(new ChangeSeatCommand
                 {
                     TicketId = Ticket.Id,
                     OrderLineId = line.Id,
                     SeatNumber = vm.ResultSeatNumber.Value
                 });
                 
                 await LoadTicketAsync(Ticket.Id);
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Seat Assignment Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };
                await _navigationService.ShowDialogAsync(errorDialog);
            }
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

        if (_userService.CurrentUser?.Id == null || _terminalContext.TerminalId == null)
        {
            return;
        }

        // F-0041: Edge case validation
        if (!Ticket.OrderLines.Any())
        {
            // No items - disable button should prevent this, but add safety check
            return;
        }

        if (Ticket.TotalAmount <= 0)
        {
            // Zero balance - skip payment view, just close ticket
            await LoadTicketAsync(Ticket.Id);
            Ticket = null;
            _navigationService.GoBack();
            return;
        }

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
                TenderType = "CASH",
                ProcessedBy = _userService.CurrentUser.Id,
                TerminalId = _terminalContext.TerminalId.Value
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
             var dialog = new Magidesk.Presentation.Views.SplitTicketDialog();
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
            // Use Routing Service to create Kitchen Orders
            await _kitchenRoutingService.RouteToKitchenAsync(Ticket);
            
            // In future: Mark items as "Sent" in DB or Ticket state if the routing service doesn't do it implicitly.
            // For now, we assume successful route is enough to count as "Sent".
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Send To Kitchen Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
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
            // Ticket creation requires authoritative runtime context (user/terminal/open cash session).
            // The supported entry point for this is Switchboard -> New Ticket.
            var dialog = new ContentDialog
            {
                Title = "Action Required",
                Content = "Create Ticket is not available from Order Entry. Use Switchboard -> New Ticket.",
                CloseButtonText = "OK"
            };
            await _navigationService.ShowDialogAsync(dialog);
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

        Money effectivePrice = new Money(fullItem.Price.Amount, fullItem.Price.Currency);

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
            // Check for remaining modifier groups (excluding size group which was already handled)
            var remainingGroups = fullItem.ModifierGroups
                .Where(mg => mg.ModifierGroup != null && 
                            mg.ModifierGroup.IsActive && 
                            mg.ModifierGroup.Id != sizeGroup?.Id)
                .ToList();

            if (remainingGroups.Any())
            {
                // Create a temporary OrderLineDto for the dialog
                var tempLine = new OrderLineDto
                {
                    MenuItemId = fullItem.Id,
                    MenuItemName = fullItem.Name,
                    Modifiers = selectedModifiers.Select(m => new OrderLineModifierDto
                    {
                        ModifierId = m.Id,
                        Name = m.Name,
                        ModifierType = ModifierType.Normal,
                        ItemCount = 1,
                        UnitPrice = m.BasePrice.Amount,
                        TaxRate = 0,
                        ShouldPrintToKitchen = true
                    }).ToList()
                };

                // Show modifier selection dialog
                var vm = new Magidesk.ViewModels.Dialogs.ModifierSelectionViewModel(_menuRepository, tempLine);
                var dialog = new Magidesk.Views.Dialogs.ModifierSelectionDialog(vm)
                {
                    XamlRoot = App.MainWindowInstance.Content.XamlRoot
                };

                // Wire interaction
                vm.CloseAction = () => dialog.Hide();

                await dialog.ShowAsync();

                if (vm.IsConfirmed)
                {
                    // Convert dialog results back to MenuModifier list
                    foreach (var modifierDto in vm.ResultModifiers)
                    {
                        if (modifierDto.ModifierId.HasValue)
                        {
                            var modifier = await _menuRepository.GetModifierByIdAsync(modifierDto.ModifierId.Value);
                            if (modifier != null)
                            {
                                selectedModifiers.Add(modifier);
                            }
                        }
                    }
                }
                else if (remainingGroups.Any(mg => mg.ModifierGroup.IsRequired || mg.ModifierGroup.MinSelections > 0))
                {
                    // User cancelled but there were required groups -> Abort item addition
                    return;
                }
            }
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

        // F-0040: Show combo selection prompt for combo items
        if (fullItem.ComboDefinitionId.HasValue)
        {
            // Get the newly added order line to prompt for combo selections
            var newOrderLine = Ticket?.OrderLines
                .OrderByDescending(ol => ol.CreatedAt)
                .FirstOrDefault(ol => ol.MenuItemId == fullItem.Id);
                
            if (newOrderLine != null)
            {
                await ComboSelectionAsync(newOrderLine);
            }
        }

        // F-0039: Show add-on selection prompt for eligible items
        // Skip for beverages to maintain quick-add speed
        var addOnCategory = Categories.FirstOrDefault(c => c.Id == fullItem.CategoryId);
        bool isAddOnBeverage = addOnCategory?.IsBeverage ?? false;
        
        if (!isAddOnBeverage)
        {
            // Get the newly added order line to prompt for add-ons
            var addOnOrderLine = Ticket?.OrderLines
                .OrderByDescending(ol => ol.CreatedAt)
                .FirstOrDefault(ol => ol.MenuItemId == fullItem.Id);
                
            if (addOnOrderLine != null)
            {
                await AddOnSelectionAsync(addOnOrderLine);
            }
        }
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

    private async Task SettleAsync()
    {
        if (Ticket == null) return;
        
        // Navigate to Settle View with TicketId
        _navigationService.Navigate(typeof(Magidesk.Presentation.Views.SettlePage), Ticket.Id);
    }

    private async Task QuickPayAsync()
    {
        if (Ticket == null) return;
        if (Ticket.DueAmount <= 0) return; // Nothing to pay

        IsBusy = true;
        try
        {
            await _payNowHandler.HandleAsync(new PayNowCommand
            {
                TicketId = Ticket.Id,
                ProcessedBy = _userService.CurrentUser!.Id,
                TerminalId = _terminalContext.TerminalId!.Value
            });

            // If we reach here, it succeeded (exceptions handled in catch)
            // Ticket should be closed by the handler if fully paid.
            
            // Navigate back
            _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Quick Pay Error: {ex.Message}");
            // Ideally show error dialog
        }
        finally
        {
            IsBusy = false;
        }
    }
}
