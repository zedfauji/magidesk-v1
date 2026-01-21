using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.ViewModels.Dialogs;

public partial class ComboSelectionViewModel : ObservableObject
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
    private string _titleText = string.Empty;
    
    [ObservableProperty]
    private string _comboName = string.Empty;
    
    [ObservableProperty]
    private string? _validationError;

    public ObservableCollection<ComboGroupViewModel> SelectionGroups { get; } = new();

    public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public Action? CloseAction { get; set; }
    
    public bool IsConfirmed { get; private set; }
    public List<ComboSelectionResultDto> ResultSelections { get; private set; } = new();

    public ComboSelectionViewModel(IMenuRepository menuRepository, OrderLineDto parentOrderLine)
    {
        _menuRepository = menuRepository;
        _parentOrderLine = parentOrderLine;
        _titleText = "Customize Your Combo";
        
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            var menuItem = await _menuRepository.GetByIdAsync(_parentOrderLine.MenuItemId);
            if (menuItem?.ComboDefinitionId == null) return;
            
            var comboDefinition = await GetComboDefinition(menuItem.ComboDefinitionId.Value);
            if (comboDefinition == null) return;

            _comboName = comboDefinition.Name;

            // Load selection groups
            SelectionGroups.Clear();
            foreach (var group in comboDefinition.Groups.OrderBy(g => g.SortOrder))
            {
                var groupVm = new ComboGroupViewModel(group);
                
                // Load items for this group
                foreach (var item in group.Items.OrderBy(i => i.MenuItemId))
                {
                    var itemMenuItem = await _menuRepository.GetByIdAsync(item.MenuItemId);
                    if (itemMenuItem != null)
                    {
                        var itemVm = new ComboItemViewModel(
                            item.Id,
                            itemMenuItem.Name,
                            itemMenuItem.Price.Amount,
                            item.Upcharge.Amount,
                            group.Id
                        );
                        
                        // Subscribe to selection changes
                        itemVm.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == nameof(ComboItemViewModel.IsSelected))
                            {
                                // Enforce single selection per group
                                if (itemVm.IsSelected)
                                {
                                    foreach (var otherItem in groupVm.Items.Where(i => i != itemVm))
                                    {
                                        otherItem.IsSelected = false;
                                    }
                                }
                                
                                // Revalidate when selection changes
                                ValidateSelections();
                            }
                        };
                        
                        groupVm.Items.Add(itemVm);
                    }
                }
                
                SelectionGroups.Add(groupVm);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<ComboDefinition?> GetComboDefinition(Guid comboDefinitionId)
    {
        // For now, we'll need to implement this in the repository
        // As a placeholder, return null - in a real implementation this would fetch from database
        // For the scope of F-0040, we'll create a mock implementation
        return null; // Placeholder - would need repository method
    }

    private bool ValidateSelections()
    {
        ValidationError = null;
        
        foreach (var group in SelectionGroups)
        {
            var hasSelection = group.Items.Any(i => i.IsSelected);
            
            // For now, assume all groups are required
            // In a real implementation, ComboGroup would have an IsRequired property
            if (!hasSelection)
            {
                ValidationError = $"Please select an item from {group.GroupName}";
                return false;
            }
        }
        
        return true;
    }

    private void Confirm()
    {
        if (!ValidateSelections()) return;

        ResultSelections.Clear();

        foreach (var group in SelectionGroups)
        {
            var selectedItem = group.Items.FirstOrDefault(i => i.IsSelected);
            if (selectedItem != null)
            {
                ResultSelections.Add(new ComboSelectionResultDto
                {
                    ComboGroupId = group.GroupId,
                    GroupName = group.GroupName,
                    MenuItemId = selectedItem.MenuItemId,
                    ItemName = selectedItem.ItemName,
                    BasePrice = selectedItem.BasePrice,
                    Upcharge = selectedItem.Upcharge
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

public partial class ComboGroupViewModel : ObservableObject
{
    public Guid GroupId { get; }
    public string GroupName { get; }
    public string SelectionPrompt { get; }

    public ObservableCollection<ComboItemViewModel> Items { get; } = new();

    public ComboGroupViewModel(ComboGroup group)
    {
        GroupId = group.Id;
        GroupName = group.Name;
        SelectionPrompt = $"Choose your {group.Name}";
    }
}

public partial class ComboItemViewModel : ObservableObject
{
    public Guid Id { get; }
    public Guid MenuItemId { get; }
    public string ItemName { get; }
    public decimal BasePrice { get; }
    public decimal Upcharge { get; }
    public Guid GroupId { get; }

    [ObservableProperty]
    private bool _isSelected;

    public string PriceDisplay
    {
        get
        {
            var totalPrice = BasePrice + Upcharge;
            return totalPrice > 0 ? $"+{totalPrice:C}" : "Included";
        }
    }

    public ComboItemViewModel(Guid id, string itemName, decimal basePrice, decimal upcharge, Guid groupId)
    {
        Id = id;
        MenuItemId = id;
        ItemName = itemName;
        BasePrice = basePrice;
        Upcharge = upcharge;
        GroupId = groupId;
    }
}

public class ComboSelectionResultDto
{
    public Guid ComboGroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public Guid MenuItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal Upcharge { get; set; }
}
