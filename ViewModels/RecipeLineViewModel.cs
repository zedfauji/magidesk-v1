using System;
using Magidesk.Domain.Entities;

namespace Magidesk.Presentation.ViewModels;

public class RecipeLineViewModel : ViewModelBase
{
    private decimal _quantity;
    
    public Guid InventoryItemId { get; }
    public string Name { get; }
    public string Unit { get; }
    
    // Reference to parent VM or logic to update entity could be handled here or by parent.
    // Ideally parent handles "Save" which commits changes associated with the MenuItem.
    
    public decimal Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public RecipeLineViewModel(InventoryItem item, decimal quantity)
    {
        InventoryItemId = item.Id;
        Name = item.Name;
        Unit = item.Unit;
        Quantity = quantity;
    }
}
