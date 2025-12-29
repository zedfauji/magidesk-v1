using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Domain.Entities;

namespace Magidesk.ViewModels.Dialogs;

public partial class ModifierGroupViewModel : ObservableObject
{
    public string Name { get; }
    public bool IsRequired { get; }
    public int MinSelections { get; }
    public int MaxSelections { get; }
    public bool AllowMultipleSelections { get; }

    public ObservableCollection<ModifierItemViewModel> Modifiers { get; } = new();

    public ModifierGroupViewModel(ModifierGroup group)
    {
        Name = group.Name;
        IsRequired = group.IsRequired;
        MinSelections = group.MinSelections;
        MaxSelections = group.MaxSelections;
        AllowMultipleSelections = group.AllowMultipleSelections;
    }

    public bool Validate()
    {
        var selectedCount = Modifiers.Count(m => m.IsSelected);
        if (IsRequired && selectedCount < MinSelections) return false;
        if (selectedCount > MaxSelections) return false;
        return true;
    }
    
    public void EnforceSelectionLogic(ModifierItemViewModel changedItem)
    {
        if (changedItem.IsSelected)
        {
            // If MaxSelections is 1, unselect others
            if (MaxSelections == 1)
            {
                foreach (var mod in Modifiers)
                {
                    if (mod != changedItem && mod.IsSelected)
                    {
                        mod.IsSelected = false;
                    }
                }
            }
        }
    }
}
