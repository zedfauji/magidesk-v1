using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Services;
using Magidesk.Presentation.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Magidesk.Presentation.ViewModels;

public partial class OrderEntryViewModel : ViewModelBase
{
    private readonly MenuApiService _menuApiService;
    private readonly IMenuRepository _menuRepository;
    private readonly IQueryHandler<GetTicketQuery, TicketDto?> _getTicketHandler;
    private readonly ICommandHandler<AddOrderLineCommand, AddOrderLineResult> _addOrderLineHandler;
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private TicketDto? _currentTicket;

    [ObservableProperty]
    private MenuCategoryDto? _selectedCategory;

    [ObservableProperty]
    private MenuGroupDto? _selectedGroup;

    public ObservableCollection<MenuCategoryDto> MenuCategories { get; } = new();
    public ObservableCollection<MenuGroupDto> MenuGroups { get; } = new();
    public ObservableCollection<MenuItem> MenuItems { get; } = new();

    public ICommand SelectCategoryCommand { get; }
    public ICommand SelectGroupCommand { get; }
    public ICommand SelectItemCommand { get; }

    public OrderEntryViewModel(
        MenuApiService menuApiService,
        IMenuRepository menuRepository,
        IQueryHandler<GetTicketQuery, TicketDto?> getTicketHandler,
        ICommandHandler<AddOrderLineCommand, AddOrderLineResult> addOrderLineHandler,
        NavigationService navigationService)
    {
        _menuApiService = menuApiService;
        _menuRepository = menuRepository;
        _getTicketHandler = getTicketHandler;
        _addOrderLineHandler = addOrderLineHandler;
        _navigationService = navigationService;

        Title = "Order Entry";

        SelectCategoryCommand = new AsyncRelayCommand<MenuCategoryDto>(SelectCategoryAsync);
        SelectGroupCommand = new AsyncRelayCommand<MenuGroupDto>(SelectGroupAsync);
        SelectItemCommand = new AsyncRelayCommand<MenuItem>(SelectItemAsync);

        _ = LoadMenuCategoriesAsync();
    }

    private async Task LoadMenuCategoriesAsync()
    {
        IsBusy = true;
        try
        {
            var categories = await _menuApiService.GetMenuCategoriesAsync();
            MenuCategories.Clear();
            foreach (var category in categories)
            {
                MenuCategories.Add(category);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SelectCategoryAsync(MenuCategoryDto? category)
    {
        SelectedCategory = category;
        MenuGroups.Clear();
        MenuItems.Clear();

        if (category == null) return;

        IsBusy = true;
        try
        {
            var groups = await _menuApiService.GetMenuGroupsAsync(category.Id);
            foreach (var group in groups)
            {
                MenuGroups.Add(group);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SelectGroupAsync(MenuGroupDto? group)
    {
        SelectedGroup = group;
        MenuItems.Clear();

        if (group == null) return;

        IsBusy = true;
        try
        {
            var items = await _menuRepository.GetByGroupAsync(group.Id);
            foreach (var item in items)
            {
                MenuItems.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SelectItemAsync(MenuItem? item)
    {
        if (item == null || CurrentTicket == null) return;

        var command = new AddOrderLineCommand
        {
            TicketId = CurrentTicket.Id,
            MenuItemId = item.Id,
            MenuItemName = item.Name,
            Quantity = 1,
            UnitPrice = item.Price,
            TaxRate = item.TaxRate
        };

        await _addOrderLineHandler.HandleAsync(command);

        // Refresh the ticket
        var updatedTicket = await _getTicketHandler.HandleAsync(
            new GetTicketQuery { TicketId = CurrentTicket.Id });
        CurrentTicket = updatedTicket;
    }
}