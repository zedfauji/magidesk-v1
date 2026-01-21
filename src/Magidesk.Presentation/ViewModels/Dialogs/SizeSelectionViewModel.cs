using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Magidesk.Domain.Entities;
using Magidesk.ViewModels;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public class SizeSelectionViewModel : ViewModelBase
{
    private readonly ModifierGroup _sizeGroup;

    public ObservableCollection<MenuModifier> Sizes { get; } = new();

    private MenuModifier? _selectedSize;
    public MenuModifier? SelectedSize
    {
        get => _selectedSize;
        set => SetProperty(ref _selectedSize, value);
    }

    public ICommand SelectSizeCommand { get; }
    public ICommand CancelCommand { get; }
    
    // Action to close dialog with success
    public Action? CloseAction { get; set; }

    public SizeSelectionViewModel(ModifierGroup sizeGroup)
    {
        _sizeGroup = sizeGroup;
        Title = $"Select {sizeGroup.Name}";

        foreach (var modifier in sizeGroup.Modifiers.OrderBy(m => m.DisplayOrder))
        {
            Sizes.Add(modifier);
        }

        SelectSizeCommand = new RelayCommand<MenuModifier>(SelectSize);
        CancelCommand = new RelayCommand(() => CloseAction?.Invoke()); // Or handle as cancel
    }

    private void SelectSize(MenuModifier? size)
    {
        if (size == null) return;
        SelectedSize = size;
        CloseAction?.Invoke();
    }
}
