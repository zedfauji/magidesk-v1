using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Represents an order item in the list.
/// </summary>
public partial class OrderItemViewModel : ObservableObject
{
    public Guid OrderItemId { get; set; }

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private decimal _unitPrice;

    [ObservableProperty]
    private decimal _lineTotal;

    public ObservableCollection<string> Modifiers { get; set; } = new();

    [ObservableProperty]
    private string? _specialNote;

    public bool HasModifiers => Modifiers.Any();

    [ObservableProperty]
    private bool _isSelected;
}
