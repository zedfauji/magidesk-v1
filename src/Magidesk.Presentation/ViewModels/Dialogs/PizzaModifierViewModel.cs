using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public enum PizzaSection
{
    None,
    Whole,
    Left,
    Right
}

public class PizzaModifierItemViewModel : ObservableObject
{
    private PizzaSection _section = PizzaSection.None;

    public Guid ModifierId { get; }
    public string Name { get; }
    public decimal Price { get; }
    public decimal TaxRate { get; }

    public PizzaSection Section
    {
        get => _section;
        set
        {
            if (SetProperty(ref _section, value))
            {
                OnPropertyChanged(nameof(IsWhole));
                OnPropertyChanged(nameof(IsLeft));
                OnPropertyChanged(nameof(IsRight));
            }
        }
    }

    public bool IsWhole
    {
        get => Section == PizzaSection.Whole;
        set { if (value) Section = PizzaSection.Whole; else if (Section == PizzaSection.Whole) Section = PizzaSection.None; }
    }

    public bool IsLeft
    {
        get => Section == PizzaSection.Left;
        set { if (value) Section = PizzaSection.Left; else if (Section == PizzaSection.Left) Section = PizzaSection.None; }
    }

    public bool IsRight
    {
        get => Section == PizzaSection.Right;
        set { if (value) Section = PizzaSection.Right; else if (Section == PizzaSection.Right) Section = PizzaSection.None; }
    }

    public PizzaModifierItemViewModel(Guid modifierId, string name, decimal price, decimal taxRate)
    {
        ModifierId = modifierId;
        Name = name;
        Price = price;
        TaxRate = taxRate;
    }
}

public class PizzaModifierGroupViewModel : ObservableObject
{
    public string Name { get; }
    public ObservableCollection<PizzaModifierItemViewModel> Modifiers { get; } = new();

    public PizzaModifierGroupViewModel(string name)
    {
        Name = name;
    }
}

public partial class PizzaModifierViewModel : ObservableObject
{
    private readonly IMenuRepository _menuRepository;
    private readonly OrderLineDto _orderLine;

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<PizzaModifierGroupViewModel> Groups { get; } = new();

    public bool IsConfirmed { get; private set; }
    public List<OrderLineModifierDto> ResultModifiers { get; private set; } = new();

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public Action? CloseAction { get; set; }

    public PizzaModifierViewModel(IMenuRepository menuRepository, OrderLineDto orderLine)
    {
        _menuRepository = menuRepository;
        _orderLine = orderLine;

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
                foreach (var menuItemModifierGroup in menuItem.ModifierGroups)
                {
                    var groupVm = new PizzaModifierGroupViewModel(menuItemModifierGroup.ModifierGroup?.Name ?? "Modifiers");
                    
                    if (menuItemModifierGroup.ModifierGroup != null)
                    {
                        foreach (var modifier in menuItemModifierGroup.ModifierGroup.Modifiers)
                        {
                            var modVm = new PizzaModifierItemViewModel(
                                modifier.Id, 
                                modifier.Name, 
                                modifier.BasePrice.Amount, // Fixed: Price -> BasePrice
                                0 // Tax rate simplified
                            );

                            // Check if already selected in OrderLine
                            var existing = _orderLine.Modifiers.FirstOrDefault(m => m.ModifierId == modifier.Id);
                            if (existing != null)
                            {
                                if (existing.SectionName == "Left") modVm.IsLeft = true;
                                else if (existing.SectionName == "Right") modVm.IsRight = true;
                                else modVm.IsWhole = true; // Default or explicitly Whole
                            }

                            groupVm.Modifiers.Add(modVm);
                        }
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
        ResultModifiers.Clear();

        foreach (var group in Groups)
        {
            foreach (var mod in group.Modifiers)
            {
                if (mod.Section != PizzaSection.None)
                {
                    ResultModifiers.Add(new OrderLineModifierDto
                    {
                        ModifierId = mod.ModifierId,
                        Name = mod.Name,
                        ModifierType = ModifierType.Normal, // Fixed: AddNo -> Normal
                        ItemCount = 1,
                        UnitPrice = mod.Price,
                        SectionName = mod.Section.ToString(),
                        TaxRate = mod.TaxRate,
                        ShouldPrintToKitchen = true
                    });
                }
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
