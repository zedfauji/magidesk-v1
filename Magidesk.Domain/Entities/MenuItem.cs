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
    
    // Navigation Properties
    public virtual MenuCategory? Category { get; private set; }
    public virtual MenuGroup? Group { get; private set; }

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

    // F-0031: Menu Item Button View Support
    public string? ColorCode => Properties.TryGetValue("ColorCode", out var color) ? color : null;
    
    // F-0035: Variable Price Support
    public bool IsVariablePrice => Properties.TryGetValue("IsVariablePrice", out var val) && bool.TryParse(val, out var result) && result;

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
    
    private readonly List<RecipeLine> _recipeLines = new();
    public IReadOnlyCollection<RecipeLine> RecipeLines => _recipeLines.AsReadOnly();
    
    // G.8 Pricing Tiers
    private readonly List<MenuItemPrice> _menuItemPrices = new();
    public virtual IReadOnlyCollection<MenuItemPrice> MenuItemPrices => _menuItemPrices.AsReadOnly();

    public void AddRecipeLine(Guid inventoryItemId, decimal quantity)
    {
        var line = new RecipeLine(inventoryItemId, quantity);
        _recipeLines.Add(line);
    }

    public void RemoveRecipeLine(Guid inventoryItemId)
    {
        _recipeLines.RemoveAll(x => x.InventoryItemId == inventoryItemId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.BusinessRuleViolationException("Name cannot be empty.");
        Name = name;
    }
    
    // Stock Tracking (G.2)
    public int StockQuantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public bool TrackStock { get; private set; }

    public void EnableStockTracking(int initialQuantity, int minimumLevel = 0)
    {
        TrackStock = true;
        StockQuantity = initialQuantity;
        MinimumStockLevel = minimumLevel;
    }

    public void DisableStockTracking()
    {
        TrackStock = false;
    }

    public void AdjustStock(int quantityChange)
    {
        if (!TrackStock) return;
        
        StockQuantity += quantityChange;
        
        // Note: We allow negative stock temporarily? 
        // Or should we enforce non-negative?
        // Usually systems allow negative to account for data entry lag, 
        // but "AddOrderLine" might block sale.
        // Let's enforce strictly non-negative for sales, but allow adjustments?
        // For now, simple arithmetic. Logic in CommandHandler will enforce availability.
    }

    public void DeductStock(int quantity)
    {
        if (!TrackStock) return;
        if (quantity < 0) throw new Exceptions.BusinessRuleViolationException("Cannot deduct negative quantity.");
        
        if (StockQuantity < quantity)
        {
             throw new Exceptions.BusinessRuleViolationException($"Insufficient stock for item '{Name}'. Available: {StockQuantity}, Requested: {quantity}");
        }

        StockQuantity -= quantity;
    }
    
    public void ReturnStock(int quantity)
    {
         if (!TrackStock) return;
         if (quantity < 0) throw new Exceptions.BusinessRuleViolationException("Cannot return negative quantity.");
         
         StockQuantity += quantity;
    }

    public void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("Invalid Category ID");
        CategoryId = categoryId;
    }

    public void SetGroup(Guid groupId)
    {
        if (groupId == Guid.Empty) throw new ArgumentException("Invalid Group ID");
        GroupId = groupId;
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

    public void SetPriceForLevel(PriceLevel level, Money price)
    {
        if (level == null) throw new ArgumentNullException(nameof(level));
        if (price == null) throw new ArgumentNullException(nameof(price));

        var existing = _menuItemPrices.Find(x => x.PriceLevelId == level.Id);
        if (existing != null)
        {
            existing.UpdatePrice(price);
        }
        else
        {
            _menuItemPrices.Add(MenuItemPrice.Create(Id, level.Id, price));
        }
    }

    /// <summary>
    /// Gets the price for a specific price level.
    /// Fallback logic:
    /// 1. Specific Level Price
    /// 2. Default Level Price (if defaultLevelId provided)
    /// 3. Base Price
    /// </summary>
    public Money GetPriceForLevel(Guid levelId, Guid? defaultLevelId = null)
    {
        // 1. Check specific level
        var specificPrice = _menuItemPrices.Find(x => x.PriceLevelId == levelId);
        if (specificPrice != null) return specificPrice.Price;

        // 2. Check default level
        if (defaultLevelId.HasValue && defaultLevelId.Value != levelId)
        {
            var defaultPrice = _menuItemPrices.Find(x => x.PriceLevelId == defaultLevelId.Value);
            if (defaultPrice != null) return defaultPrice.Price;
        }

        // 3. Fallback to base price
        return Price;
    }
}
