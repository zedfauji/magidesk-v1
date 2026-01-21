using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.Interfaces;

namespace Magidesk.ViewModels.Dialogs;

public partial class ModifierSelectionViewModel : ObservableObject
{
    private readonly IMenuRepository _menuRepository;
    private readonly OrderLineDto _orderLine;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
    
    [ObservableProperty]
    private string _menuItemName = string.Empty;

    public ObservableCollection<ModifierGroupViewModel> Groups { get; } = new();

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public Action? CloseAction { get; set; }
    
    public bool IsConfirmed { get; private set; }
    public List<OrderLineModifierDto> ResultModifiers { get; private set; } = new();

    public ModifierSelectionViewModel(IMenuRepository menuRepository, OrderLineDto orderLine)
    {
        _menuRepository = menuRepository;
        _orderLine = orderLine;
        _menuItemName = orderLine.MenuItemName;
        
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            var menuItem = await _menuRepository.GetByIdAsync(_orderLine.MenuItemId);
            if (menuItem != null)
            {
                foreach (var menuItemModifierGroup in menuItem.ModifierGroups.OrderBy(mg => mg.DisplayOrder))
                {
                    if (menuItemModifierGroup.ModifierGroup == null) continue;

                    var groupVm = new ModifierGroupViewModel(menuItemModifierGroup.ModifierGroup);
                    
                    foreach (var modifier in menuItemModifierGroup.ModifierGroup.Modifiers)
                    {
                        var modVm = new ModifierItemViewModel(
                            modifier.Id, 
                            modifier.Name, 
                            modifier.BasePrice.Amount
                        );
                        
                        // Check if already selected
                        if (_orderLine.Modifiers.Any(m => m.ModifierId == modifier.Id))
                        {
                            modVm.IsSelected = true;
                        }

                        // Subscribe to selection changes to enforce logic
                        modVm.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(ModifierItemViewModel.IsSelected))
                            {
                                groupVm.EnforceSelectionLogic(modVm);
                                ((RelayCommand)ConfirmCommand).NotifyCanExecuteChanged();
                            }
                        };
                        
                        groupVm.Modifiers.Add(modVm);
                    }
                    
                    if (groupVm.Modifiers.Any())
                    {
                        Groups.Add(groupVm);
                    }
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Confirm()
    {
        if (!CanConfirm()) return;

        ResultModifiers.Clear();

        foreach (var group in Groups)
        {
            foreach (var mod in group.Modifiers)
            {
                if (mod.IsSelected)
                {
                    ResultModifiers.Add(new OrderLineModifierDto
                    {
                        ModifierId = mod.ModifierId,
                        Name = mod.Name,
                        ModifierType = ModifierType.Normal,
                        ItemCount = 1,
                        UnitPrice = mod.Price,
                        TaxRate = 0, // Simplified
                        ShouldPrintToKitchen = true
                    });
                }
            }
        }

        IsConfirmed = true;
        CloseAction?.Invoke();
    }

    private bool CanConfirm()
    {
        return Groups.All(g => g.Validate());
    }

    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}
