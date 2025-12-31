using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using CommunityToolkit.Mvvm.Input;

namespace Magidesk.Presentation.ViewModels;

public sealed class OrderTypeExplorerViewModel : ViewModelBase
{
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly ICommandHandler<CreateOrderTypeCommand, CreateOrderTypeResult> _create;
    private readonly ICommandHandler<UpdateOrderTypeCommand, UpdateOrderTypeResult> _update;

    public ObservableCollection<OrderType> OrderTypes { get; } = new();

    private OrderType? _selectedOrderType;
    public OrderType? SelectedOrderType
    {
        get => _selectedOrderType;
        set
        {
            if (SetProperty(ref _selectedOrderType, value))
            {
                HydrateEditorFromSelection();
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    public bool HasSelection => SelectedOrderType != null;

    private string _editingName = string.Empty;
    public string EditingName
    {
        get => _editingName;
        set => SetProperty(ref _editingName, value);
    }

    private bool _editingIsActive;
    public bool EditingIsActive
    {
        get => _editingIsActive;
        set => SetProperty(ref _editingIsActive, value);
    }

    private bool _editingCloseOnPaid;
    public bool EditingCloseOnPaid
    {
        get => _editingCloseOnPaid;
        set => SetProperty(ref _editingCloseOnPaid, value);
    }

    private bool _editingAllowSeatBasedOrder;
    public bool EditingAllowSeatBasedOrder
    {
        get => _editingAllowSeatBasedOrder;
        set => SetProperty(ref _editingAllowSeatBasedOrder, value);
    }

    private bool _editingAllowToAddTipsLater;
    public bool EditingAllowToAddTipsLater
    {
        get => _editingAllowToAddTipsLater;
        set => SetProperty(ref _editingAllowToAddTipsLater, value);
    }

    private bool _editingIsBarTab;
    public bool EditingIsBarTab
    {
        get => _editingIsBarTab;
        set => SetProperty(ref _editingIsBarTab, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeactivateCommand { get; }

    public OrderTypeExplorerViewModel(
        IOrderTypeRepository orderTypeRepository,
        ICommandHandler<CreateOrderTypeCommand, CreateOrderTypeResult> create,
        ICommandHandler<UpdateOrderTypeCommand, UpdateOrderTypeResult> update)
    {
        _orderTypeRepository = orderTypeRepository;
        _create = create;
        _update = update;

        Title = "Order Type Explorer";

        LoadCommand = new AsyncRelayCommand(LoadAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedOrderType != null);
        DeactivateCommand = new AsyncRelayCommand(DeactivateAsync, () => SelectedOrderType != null);
    }

    private void HydrateEditorFromSelection()
    {
        if (SelectedOrderType == null)
        {
            EditingName = string.Empty;
            EditingIsActive = false;
            EditingCloseOnPaid = false;
            EditingAllowSeatBasedOrder = false;
            EditingAllowToAddTipsLater = false;
            EditingIsBarTab = false;
            return;
        }

        EditingName = SelectedOrderType.Name;
        EditingIsActive = SelectedOrderType.IsActive;
        EditingCloseOnPaid = SelectedOrderType.CloseOnPaid;
        EditingAllowSeatBasedOrder = SelectedOrderType.AllowSeatBasedOrder;
        EditingAllowToAddTipsLater = SelectedOrderType.AllowToAddTipsLater;
        EditingIsBarTab = SelectedOrderType.IsBarTab;
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var all = await _orderTypeRepository.GetAllAsync();
            OrderTypes.Clear();
            foreach (var ot in all)
            {
                OrderTypes.Add(ot);
            }

            StatusMessage = "Loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Load Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddAsync()
    {
        IsBusy = true;
        try
        {
            var result = await _create.HandleAsync(new CreateOrderTypeCommand
            {
                Name = "New Order Type",
                CloseOnPaid = false,
                AllowSeatBasedOrder = false,
                AllowToAddTipsLater = false,
                IsBarTab = false,
                IsActive = true
            });

            await LoadAsync();
            SelectedOrderType = OrderTypes.FirstOrDefault(x => x.Id == result.OrderTypeId);
            StatusMessage = "Order type created.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Create Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedOrderType == null) return;

        IsBusy = true;
        try
        {
            await _update.HandleAsync(new UpdateOrderTypeCommand
            {
                OrderTypeId = SelectedOrderType.Id,
                Name = EditingName,
                IsActive = EditingIsActive,
                CloseOnPaid = EditingCloseOnPaid,
                AllowSeatBasedOrder = EditingAllowSeatBasedOrder,
                AllowToAddTipsLater = EditingAllowToAddTipsLater,
                IsBarTab = EditingIsBarTab
            });

            await LoadAsync();
            SelectedOrderType = OrderTypes.FirstOrDefault(x => x.Id == SelectedOrderType.Id);
            StatusMessage = "Saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeactivateAsync()
    {
        if (SelectedOrderType == null) return;

        IsBusy = true;
        try
        {
            await _update.HandleAsync(new UpdateOrderTypeCommand
            {
                OrderTypeId = SelectedOrderType.Id,
                IsActive = false
            });

            await LoadAsync();
            SelectedOrderType = null;
            StatusMessage = "Deactivated.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Deactivate Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
