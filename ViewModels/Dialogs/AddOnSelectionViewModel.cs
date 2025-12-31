using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.ViewModels.Dialogs;

public partial class AddOnSelectionViewModel : ObservableObject
{
    private readonly IMenuRepository _menuRepository;
    private readonly OrderLineDto _parentOrderLine;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
    
    [ObservableProperty]
    private string _promptText = string.Empty;

    public ObservableCollection<AddOnItemViewModel> AddOnItems { get; } = new();

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public Action? CloseAction { get; set; }
    
    public bool IsConfirmed { get; private set; }
    public List<OrderLineModifierDto> ResultAddOns { get; private set; } = new();

    public AddOnSelectionViewModel(IMenuRepository menuRepository, OrderLineDto parentOrderLine)
    {
        _menuRepository = menuRepository;
        _parentOrderLine = parentOrderLine;
        _promptText = $"Would you like to add anything to your {parentOrderLine.MenuItemName}?";
        
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            var menuItem = await _menuRepository.GetByIdAsync(_parentOrderLine.MenuItemId);
            if (menuItem != null)
            {
                // For now, we'll use a simple approach: look for menu items that are marked as add-ons
                // In a real implementation, this would be based on a proper add-on relationship
                var allMenuItems = await _menuRepository.GetAllAsync();
                
                // Simple heuristic: items with "Add" in name or low price items as potential add-ons
                var potentialAddOns = allMenuItems
                    .Where(item => item.IsAvailable && 
                                   item.IsVisible &&
                                   (item.Name.Contains("Add", StringComparison.OrdinalIgnoreCase) ||
                                    item.Price.Amount <= 3.00m)) // Low-priced items as add-ons
                    .OrderBy(item => item.Price.Amount)
                    .Take(12) // Limit to reasonable number
                    .ToList();

                foreach (var addOnItem in potentialAddOns)
                {
                    var addOnVm = new AddOnItemViewModel(
                        addOnItem.Id,
                        addOnItem.Name,
                        addOnItem.Price.Amount
                    );
                    
                    AddOnItems.Add(addOnVm);
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
        ResultAddOns.Clear();

        foreach (var addOn in AddOnItems)
        {
            if (addOn.IsSelected)
            {
                ResultAddOns.Add(new OrderLineModifierDto
                {
                    ModifierId = addOn.MenuItemId,
                    Name = addOn.Name,
                    ModifierType = ModifierType.AddOn,
                    ItemCount = 1,
                    UnitPrice = addOn.Price,
                    TaxRate = 0, // Simplified, would use actual tax rate
                    ShouldPrintToKitchen = true
                });
            }
        }

        IsConfirmed = true;
        CloseAction?.Invoke();
    }

    private void Cancel()
    {
        IsConfirmed = false;
        CloseAction?.Invoke();
    }
}

public partial class AddOnItemViewModel : ObservableObject
{
    public Guid MenuItemId { get; }
    public string Name { get; }
    public decimal Price { get; }

    [ObservableProperty]
    private bool _isSelected;

    public AddOnItemViewModel(Guid menuItemId, string name, decimal price)
    {
        MenuItemId = menuItemId;
        Name = name;
        Price = price;
    }
}
