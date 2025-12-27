using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;

namespace Magidesk.Presentation.ViewModels;

public class OrderTypeSelectionViewModel : ViewModelBase
{
    private readonly IOrderTypeRepository _orderTypeRepository;
    private OrderType? _selectedOrderType;

    public ObservableCollection<OrderType> OrderTypes { get; } = new();

    public OrderType? SelectedOrderType
    {
        get => _selectedOrderType;
        set => SetProperty(ref _selectedOrderType, value);
    }

    public OrderTypeSelectionViewModel(IOrderTypeRepository orderTypeRepository)
    {
        _orderTypeRepository = orderTypeRepository;
    }

    public async Task LoadOrderTypesAsync()
    {
        IsBusy = true;
        try
        {
            var types = await _orderTypeRepository.GetActiveAsync();
            OrderTypes.Clear();
            foreach (var type in types)
            {
                OrderTypes.Add(type);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
