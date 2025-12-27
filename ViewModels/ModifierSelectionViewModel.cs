using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Presentation.Services;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Magidesk.Presentation.ViewModels;

public class ModifierSelectionViewModel : ViewModelBase
{
    private readonly IMenuRepository _menuRepository;
    private MenuItem? _menuItem;
    private string? _validationError;

    public ObservableCollection<ModifierGroupViewModel> ModifierGroups { get; } = new();

    public string? ValidationError
    {
        get => _validationError;
        set { SetProperty(ref _validationError, value); OnPropertyChanged(nameof(HasValidationError)); }
    }
    
    public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

    public ModifierSelectionViewModel(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public void LoadModifiers(MenuItem menuItem)
    {
        _menuItem = menuItem;
        ModifierGroups.Clear();

        // Sort groups by DisplayOrder
        var groups = menuItem.ModifierGroups
            .Select(mg => mg.ModifierGroup)
            .OrderBy(g => g.DisplayOrder);

        foreach (var group in groups)
        {
            if (group.IsActive)
            {
                ModifierGroups.Add(new ModifierGroupViewModel(group));
            }
        }
    }

    public bool ValidateSelections()
    {
        ValidationError = null;
        foreach (var groupVm in ModifierGroups)
        {
            var selectedCount = groupVm.Modifiers.Count(m => m.IsSelected);
            
            if (groupVm.Group.IsRequired && selectedCount < groupVm.Group.MinSelections)
            {
                ValidationError = $"{groupVm.Group.Name} requires at least {groupVm.Group.MinSelections} selection(s).";
                return false;
            }
            
            if (selectedCount > groupVm.Group.MaxSelections)
            {
                 // This should be prevented by UI properties, but good to check
                 ValidationError = $"{groupVm.Group.Name} allows max {groupVm.Group.MaxSelections} selection(s).";
                 return false;
            }
        }
        return true;
    }
    
    public List<MenuModifier> GetSelectedModifiers()
    {
        return ModifierGroups
            .SelectMany(g => g.Modifiers)
            .Where(m => m.IsSelected)
            .Select(m => m.Modifier)
            .ToList();
    }
}

public class ModifierGroupViewModel : ViewModelBase
{
    public ModifierGroup Group { get; }
    public ObservableCollection<ModifierViewModel> Modifiers { get; } = new();

    public ModifierGroupViewModel(ModifierGroup group)
    {
        Group = group;
        foreach (var modifier in group.Modifiers.Where(m => m.IsActive).OrderBy(m => m.DisplayOrder))
        {
            Modifiers.Add(new ModifierViewModel(modifier, this));
        }
    }
    
    public void OnModifierSelectionChanged(ModifierViewModel changedModifier)
    {
        // Enforce Single Selection Logic if MaxSelections == 1
        if (Group.MaxSelections == 1 && changedModifier.IsSelected)
        {
            foreach (var mod in Modifiers)
            {
                if (mod != changedModifier && mod.IsSelected)
                {
                    mod.IsSelected = false; 
                }
            }
        }
    }
}

public class ModifierViewModel : ViewModelBase
{
    private bool _isSelected;
    private readonly ModifierGroupViewModel _parentGroup;

    public MenuModifier Modifier { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                _parentGroup.OnModifierSelectionChanged(this);
            }
        }
    }

    public string DisplayName => $"{Modifier.Name} (+{Modifier.BasePrice})";

    public ModifierViewModel(MenuModifier modifier, ModifierGroupViewModel parentGroup)
    {
        Modifier = modifier;
        _parentGroup = parentGroup;
    }
}
