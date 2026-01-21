using System;

namespace Magidesk.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Unit { get; private set; } = "unit"; // e.g., oz, lbs, kg, count
    public decimal StockQuantity { get; private set; }
    public decimal ReorderPoint { get; private set; }
    public bool IsActive { get; private set; }

    private InventoryItem() { } // EF Core

    public static InventoryItem Create(string name, string unit, decimal stockQuantity, decimal reorderPoint)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
        
        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Unit = unit,
            StockQuantity = stockQuantity,
            ReorderPoint = reorderPoint,
            IsActive = true
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
        Name = name;
    }

    public void UpdateUnit(string unit) => Unit = unit;
    
    public void AdjustStock(decimal quantityDelta)
    {
        StockQuantity += quantityDelta;
    }

    public void SetReorderPoint(decimal point)
    {
        ReorderPoint = point;
    }

    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}
