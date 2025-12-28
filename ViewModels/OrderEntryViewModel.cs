using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Microsoft.UI.Xaml.Controls;

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
                 OnPropertyChanged(nameof(TotalsText));
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

    public string TotalsText => Ticket == null
        ? "$0.00"
        : $"{Ticket.TotalAmount:C}"; // Simple format for now

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
        NavigationService navigationService)
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
        _navigationService = navigationService;

        Title = "Order Entry";

        SelectCategoryCommand = new RelayCommand<MenuCategory>(c => SelectedCategory = c);
        SelectGroupCommand = new RelayCommand<MenuGroup>(g => SelectedGroup = g);
        AddItemCommand = new AsyncRelayCommand<MenuItem>(AddItemAsync);

        BackToCategoriesCommand = new RelayCommand(() => SelectedCategory = null);
        SearchItemCommand = new AsyncRelayCommand(SearchItemAsync);
        IncrementQuantityCommand = new AsyncRelayCommand<OrderLineDto>(IncrementQuantityAsync);
        DecrementQuantityCommand = new AsyncRelayCommand<OrderLineDto>(DecrementQuantityAsync);
        RemoveItemCommand = new AsyncRelayCommand<OrderLineDto>(RemoveItemAsync);
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

    private async Task SplitTicketAsync()
    {
        if (Ticket == null) return;

        IsBusy = true;
        try
        {
             var dialog = new Magidesk.Presentation.Views.SplitTicketDialog(Ticket);
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

        // F-0038 Integration: Check for modifiers
        List<MenuModifier> selectedModifiers = new();
        if (item.ModifierGroups.Any(mg => mg.ModifierGroup.IsActive))
        {
                 var dialog = new Magidesk.Presentation.Views.ModifierSelectionDialog(item);
                 var result = await _navigationService.ShowDialogAsync(dialog);

                 if (result != ContentDialogResult.Primary) return;
                 
                 selectedModifiers = dialog.ViewModel.GetSelectedModifiers();
        }

        await _addOrderLineHandler.HandleAsync(new AddOrderLineCommand
        {
            TicketId = Ticket.Id,
            MenuItemId = item.Id,
            MenuItemName = item.Name,
            Quantity = 1,
            UnitPrice = item.Price,
            TaxRate = item.TaxRate,
            Modifiers = selectedModifiers
        });

        await LoadTicketAsync(Ticket.Id);
    }

    private async Task SearchItemAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText) || Ticket == null) return;

        IsBusy = true;
        try
        {
            // In-memory search as per plan
            var allItems = await _menuRepository.GetAllAsync();
            
            // Try Barcode Exact Match first
            var match = allItems.FirstOrDefault(i => string.Equals(i.Barcode, SearchText, StringComparison.OrdinalIgnoreCase));
            
            // Fallback to Name contains
            if (match == null)
            {
                match = allItems.FirstOrDefault(i => i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (match != null)
            {
                await AddItemAsync(match);
                SearchText = string.Empty; // Clear on success
            }
            else
            {
               // TODO: Show "Not Found" concept?
            }
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine($"Search Error: {ex.Message}");
        }
        finally { IsBusy = false; }
    }

    private async Task IncrementQuantityAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
        await ModifyQuantityAsync(line, line.Quantity + 1);
    }

    private async Task DecrementQuantityAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;
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

    private async Task RemoveItemAsync(OrderLineDto? line)
    {
        if (line == null || Ticket == null) return;

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
