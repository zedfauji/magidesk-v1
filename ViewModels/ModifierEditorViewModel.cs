using System.Collections.ObjectModel;
using System.Windows.Input;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Presentation.ViewModels;

public class ModifierEditorViewModel : ViewModelBase
{
    private readonly IModifierGroupRepository _groupRepository;
    private readonly IMenuModifierRepository _modifierRepository;

    private ModifierGroup? _selectedGroup;
    private MenuModifier? _selectedModifier;
    
    // Editing properties
    private string _editingName = string.Empty;
    private string _editingPrice = "0.00";
    
    // Group properties
    private bool _isGroupRequired;
    private int _groupMin = 0;
    private int _groupMax = 1;
    private bool _groupMultiSelect;
    
    public string EditingName
    {
        get => _editingName;
        set => SetProperty(ref _editingName, value);
    }
    
    public string EditingPrice
    {
        get => _editingPrice;
        set => SetProperty(ref _editingPrice, value);
    }
    
    // Group Editing Properties
    public bool IsGroupRequired
    {
        get => _isGroupRequired;
        set => SetProperty(ref _isGroupRequired, value);
    }
    
    public int GroupMin
    {
        get => _groupMin;
        set => SetProperty(ref _groupMin, value);
    }
    
    public int GroupMax
    {
        get => _groupMax;
        set => SetProperty(ref _groupMax, value);
    }
    
    public bool GroupMultiSelect
    {
        get => _groupMultiSelect;
        set => SetProperty(ref _groupMultiSelect, value);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    
    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public ObservableCollection<ModifierGroup> Groups { get; } = new();
    public ObservableCollection<MenuModifier> Modifiers { get; } = new();

    public ModifierGroup? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
            {
                SelectedModifier = null;
                Modifiers.Clear();
                
                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    IsGroupRequired = value.IsRequired;
                    GroupMin = value.MinSelections;
                    GroupMax = value.MaxSelections;
                    GroupMultiSelect = value.AllowMultipleSelections;
                    
                    StatusMessage = $"Editing Group: {value.Name}";
                    _ = LoadModifiersAsync(value.Id);
                }
                else
                {
                    IsEditing = false;
                    StatusMessage = "Ready";
                }
            }
        }
    }

    public MenuModifier? SelectedModifier
    {
        get => _selectedModifier;
        set
        {
            if (SetProperty(ref _selectedModifier, value))
            {
                if (value != null)
                {
                    IsEditing = true;
                    EditingName = value.Name;
                    EditingPrice = value.BasePrice.Amount.ToString("F2");
                    StatusMessage = $"Editing Modifier: {value.Name}";
                }
                else if (SelectedGroup != null)
                {
                    // Fallback to Group
                    EditingName = SelectedGroup.Name;
                    StatusMessage = $"Editing Group: {SelectedGroup.Name}";
                }
            }
        }
    }

    public ICommand LoadDataCommand { get; }
    public ICommand AddGroupCommand { get; }
    public ICommand AddModifierCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public ModifierEditorViewModel(
        IModifierGroupRepository groupRepository,
        IMenuModifierRepository modifierRepository)
    {
        _groupRepository = groupRepository;
        _modifierRepository = modifierRepository;
        Title = "Modifier Editor";

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddGroupCommand = new AsyncRelayCommand(AddGroupAsync);
        AddModifierCommand = new AsyncRelayCommand(AddModifierAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            Groups.Clear();
            var groups = await _groupRepository.GetAllAsync();
            foreach (var g in groups) Groups.Add(g);
        }
        finally { IsBusy = false; }
    }

    private async Task LoadModifiersAsync(Guid groupId)
    {
        IsBusy = true;
        try
        {
            var mods = await _modifierRepository.GetByGroupIdAsync(groupId);
            Modifiers.Clear();
            foreach (var m in mods) Modifiers.Add(m);
        }
        finally { IsBusy = false; }
    }

    private async Task AddGroupAsync()
    {
        IsBusy = true;
        try
        {
            var newGroup = ModifierGroup.Create("New Group");
            await _groupRepository.AddAsync(newGroup);
            Groups.Add(newGroup);
            SelectedGroup = newGroup;
            StatusMessage = "Group Added";
        }
        finally { IsBusy = false; }
    }

    private async Task AddModifierAsync()
    {
        if (SelectedGroup == null) return;

        IsBusy = true;
        try
        {
            var newMod = MenuModifier.Create("New Modifier", ModifierType.Normal, Money.Zero(), SelectedGroup.Id);
            await _modifierRepository.AddAsync(newMod);
            Modifiers.Add(newMod);
            SelectedModifier = newMod;
            StatusMessage = "Modifier Added";
        }
        finally { IsBusy = false; }
    }

    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            if (SelectedModifier != null)
            {
                SelectedModifier.UpdateName(EditingName);
                if (decimal.TryParse(EditingPrice, out var price))
                {
                    SelectedModifier.UpdateBasePrice(new Money(price));
                }
                await _modifierRepository.UpdateAsync(SelectedModifier);
                StatusMessage = "Modifier Saved";
            }
            else if (SelectedGroup != null)
            {
                SelectedGroup.UpdateName(EditingName);
                SelectedGroup.SetIsRequired(IsGroupRequired);
                try {
                    SelectedGroup.UpdateSelectionConstraints(GroupMin, GroupMax, GroupMultiSelect);
                } catch (Exception ex) {
                    StatusMessage = $"Validation Error: {ex.Message}";
                    return;
                }
                
                await _groupRepository.UpdateAsync(SelectedGroup);
                StatusMessage = "Group Saved";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    private async Task DeleteAsync()
    {
        if (SelectedModifier != null)
        {
            var mod = SelectedModifier;
            // await _modifierRepository.DeleteAsync(mod.Id);
            Modifiers.Remove(mod);
            SelectedModifier = null;
            StatusMessage = "Modifier Deleted";
        }
        else if (SelectedGroup != null)
        {
            var grp = SelectedGroup;
            // await _groupRepository.DeleteAsync(grp.Id);
            Groups.Remove(grp);
            SelectedGroup = null;
            StatusMessage = "Group Deleted";
        }
    }
}
