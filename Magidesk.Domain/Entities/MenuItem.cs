using System;
using System.Collections.Generic;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a product effectively on the menu.
/// </summary>
public class MenuItem
{
    private readonly List<MenuItemModifierGroup> _modifierGroups = new();
    private readonly Dictionary<string, string> _properties = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Barcode { get; private set; }
    public Money Price { get; private set; }
    public decimal TaxRate { get; private set; }
    
    // Organization
    public Guid? CategoryId { get; private set; } // References Category (Reference Data)
    public Guid? GroupId { get; private set; }    // References Group (Reference Data)
    public Guid? ComboDefinitionId { get; private set; } // Links to Combo Structure
    public int DisplayOrder { get; private set; }
    
    // Configuration
    public bool IsVisible { get; private set; }
    public bool IsAvailable { get; private set; } // 86'd status
    public bool ShowInKiosk { get; private set; }
    public bool IsStockItem { get; private set; }
    
    // Kitchen Routing
    public bool ShouldPrintToKitchen { get; private set; }
    public Guid? PrinterGroupId { get; private set; }
    
    // Collections
    public IReadOnlyCollection<MenuItemModifierGroup> ModifierGroups => _modifierGroups.AsReadOnly();
    public IReadOnlyDictionary<string, string> Properties => _properties.AsReadOnly();
    
    // Concurrency
    public int Version { get; private set; }
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private MenuItem()
    {
        Name = string.Empty;
        Price = Money.Zero();
    }

    public static MenuItem Create(
        string name,
        Money price,
        decimal taxRate = 0m)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Name cannot be empty.");
            
        return new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            TaxRate = taxRate,
            IsVisible = true,
            IsAvailable = true,
            ShouldPrintToKitchen = true,
            IsActive = true,
            Version = 1
        };
    }

    public void UpdatePrice(Money price)
    {
        if (price < Money.Zero())
             throw new Exceptions.BusinessRuleViolationException("Price cannot be negative.");
        Price = price;
    }
    
    public void SetPrinterGroup(Guid? printerGroupId)
    {
        PrinterGroupId = printerGroupId;
    }

    public void SetComboDefinition(Guid? comboDefinitionId)
    {
        ComboDefinitionId = comboDefinitionId;
    }

    public void AddModifierGroup(ModifierGroup group, int displayOrder = 0)
    {
        // Logic to add relation
        // Typically handled via aggregate roots or dedicated service if simpler,
        // but DDD prefers methods here.
        // Assuming MenuItemModifierGroup is the join entity.
    }
}
