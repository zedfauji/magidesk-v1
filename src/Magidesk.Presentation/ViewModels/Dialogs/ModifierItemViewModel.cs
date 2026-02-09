using CommunityToolkit.Mvvm.ComponentModel;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Presentation.ViewModels.Dialogs;

public partial class ModifierItemViewModel : ObservableObject
{
    public Guid ModifierId { get; }
    public string Name { get; }
    public decimal Price { get; }

    [ObservableProperty]
    private bool _isSelected;

    public ModifierItemViewModel(Guid modifierId, string name, decimal price)
    {
        ModifierId = modifierId;
        Name = name;
        Price = price;
    }
}
