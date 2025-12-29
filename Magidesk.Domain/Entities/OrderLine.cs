using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a single item in an order.
/// </summary>
public class OrderLine
{
    private readonly List<OrderLineModifier> _modifiers = new();
    private readonly List<OrderLineModifier> _addOns = new();
    private readonly List<OrderLineDiscount> _discounts = new();

    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = string.Empty;
    public string? CategoryName { get; private set; }
    public string? GroupName { get; private set; }
    
    // Quantity
    public decimal Quantity { get; private set; }
    public int ItemCount { get; private set; }
    public string? ItemUnitName { get; private set; }
    public bool IsFractionalUnit { get; private set; }
    
    // Pricing
    public Money UnitPrice { get; private set; }
    public Money SubtotalAmount { get; private set; }
    public Money SubtotalAmountWithoutModifiers { get; private set; }
    public Money DiscountAmount { get; private set; }
    public decimal TaxRate { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money TaxAmountWithoutModifiers { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money TotalAmountWithoutModifiers { get; private set; }
    
    // Flags
    public bool IsBeverage { get; private set; }
    public bool ShouldPrintToKitchen { get; private set; }
    public bool PrintedToKitchen { get; private set; }
    
    // F-0036: Cooking Instructions
    public string? Instructions { get; private set; }
    
    // Seat
    public int? SeatNumber { get; private set; }
    public bool TreatAsSeat { get; private set; }
    
    // Collections
    public IReadOnlyCollection<OrderLineModifier> Modifiers => _modifiers.AsReadOnly();
    public IReadOnlyCollection<OrderLineModifier> AddOns => _addOns.AsReadOnly();
    public IReadOnlyCollection<OrderLineDiscount> Discounts => _discounts.AsReadOnly();
    public OrderLineModifier? SizeModifier { get; private set; }
    
    // References
    public Guid? PrinterGroupId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Private constructor for EF Core
    private OrderLine()
    {
        UnitPrice = Money.Zero();
        SubtotalAmount = Money.Zero();
        SubtotalAmountWithoutModifiers = Money.Zero();
        DiscountAmount = Money.Zero();
        TaxAmount = Money.Zero();
        TaxAmountWithoutModifiers = Money.Zero();
        TotalAmount = Money.Zero();
        TotalAmountWithoutModifiers = Money.Zero();
    }

    /// <summary>
    /// Splits modifiers into _modifiers and _addOns based on ModifierType.
    /// Called after EF Core materialization.
    /// </summary>
    internal void SplitModifiersAfterLoad()
    {
        // EF Core loads all OrderLineModifiers into _modifiers
        // We need to split them into _modifiers (Normal) and _addOns (Extra)
        var allModifiers = _modifiers.ToList();
        _modifiers.Clear();
        _addOns.Clear();

        foreach (var modifier in allModifiers)
        {
            if (modifier.ModifierType == Enumerations.ModifierType.Extra)
            {
                _addOns.Add(modifier);
            }
            else
            {
                _modifiers.Add(modifier);
            }
        }
    }

    /// <summary>
    /// Creates a new order line.
    /// </summary>
    public static OrderLine Create(
        Guid ticketId,
        Guid menuItemId,
        string menuItemName,
        decimal quantity,
        Money unitPrice,
        decimal taxRate = 0m,
        string? categoryName = null,
        string? groupName = null)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleViolationException("Quantity must be greater than zero.");
        }

        if (unitPrice < Money.Zero())
        {
            throw new BusinessRuleViolationException("Unit price cannot be negative.");
        }

        var orderLine = new OrderLine
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            MenuItemId = menuItemId,
            MenuItemName = menuItemName,
            CategoryName = categoryName,
            GroupName = groupName,
            Quantity = quantity,
            ItemCount = (int)Math.Ceiling(quantity), // For discrete items
            UnitPrice = unitPrice,
            TaxRate = taxRate,
            CreatedAt = DateTime.UtcNow
        };

        orderLine.CalculatePrice();
        return orderLine;
    }

    /// <summary>
    /// Recalculates the line totals.
    /// </summary>
    public void CalculatePrice()
    {
        // Calculate subtotal without modifiers
        SubtotalAmountWithoutModifiers = UnitPrice * Quantity;

        // Add modifiers
        var modifierTotal = _modifiers.Aggregate(Money.Zero(), (sum, m) => sum + m.TotalAmount);
        var addOnTotal = _addOns.Aggregate(Money.Zero(), (sum, a) => sum + a.TotalAmount);
        
        // Add size modifier if present
        var sizeModifierTotal = SizeModifier?.TotalAmount ?? Money.Zero();

        SubtotalAmount = SubtotalAmountWithoutModifiers + modifierTotal + addOnTotal + sizeModifierTotal;

        // Calculate tax
        TaxAmountWithoutModifiers = SubtotalAmountWithoutModifiers * TaxRate;
        var modifierTax = _modifiers.Aggregate(Money.Zero(), (sum, m) => sum + m.TaxAmount);
        var addOnTax = _addOns.Aggregate(Money.Zero(), (sum, a) => sum + a.TaxAmount);
        var sizeModifierTax = SizeModifier?.TaxAmount ?? Money.Zero();
        
        TaxAmount = TaxAmountWithoutModifiers + modifierTax + addOnTax + sizeModifierTax;

        // Apply discounts
        DiscountAmount = _discounts.Aggregate(Money.Zero(), (sum, d) => sum + d.Amount);
        
        if (DiscountAmount > SubtotalAmount)
        {
            DiscountAmount = SubtotalAmount; // Cap discount at subtotal
        }

        // Calculate total
        TotalAmountWithoutModifiers = SubtotalAmountWithoutModifiers + TaxAmountWithoutModifiers - DiscountAmount;
        TotalAmount = SubtotalAmount + TaxAmount - DiscountAmount;

        // Ensure non-negative
        if (TotalAmount < Money.Zero())
        {
            TotalAmount = Money.Zero();
        }
    }

    /// <summary>
    /// Updates the quantity.
    /// </summary>
    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleViolationException("Quantity must be greater than zero.");
        }

        Quantity = quantity;
        ItemCount = (int)Math.Ceiling(quantity);
        CalculatePrice();
    }

    /// <summary>
    /// Checks if this order line can be merged with another.
    /// </summary>
    public bool CanMerge(OrderLine other)
    {
        if (other == null) return false;
        if (MenuItemId != other.MenuItemId) return false;
        if (UnitPrice != other.UnitPrice) return false;
        if (TaxRate != other.TaxRate) return false;
        if (_modifiers.Count != other._modifiers.Count) return false;
        if (_addOns.Count != other._addOns.Count) return false;
        // Could add more detailed comparison of modifiers/add-ons
        
        return true;
    }

    /// <summary>
    /// Merges another order line into this one.
    /// </summary>
    public void Merge(OrderLine other)
    {
        if (!CanMerge(other))
        {
            throw new BusinessRuleViolationException("Cannot merge order lines that are not compatible.");
        }

        Quantity += other.Quantity;
        ItemCount += other.ItemCount;
        CalculatePrice();
    }

    /// <summary>
    /// Adds a modifier to the order line.
    /// </summary>
    public void AddModifier(OrderLineModifier modifier)
    {
        if (modifier == null) throw new ArgumentNullException(nameof(modifier));

        if (modifier.ModifierType == Enumerations.ModifierType.Extra)
        {
            _addOns.Add(modifier);
        }
        else
        {
            _modifiers.Add(modifier);
        }

        CalculatePrice();
    }

    /// <summary>
    /// Removes a modifier from the order line.
    /// </summary>
    public void RemoveModifier(OrderLineModifier modifier)
    {
         if (modifier == null) throw new ArgumentNullException(nameof(modifier));

        if (modifier.ModifierType == Enumerations.ModifierType.Extra)
        {
            _addOns.Remove(modifier);
        }
        else
        {
            _modifiers.Remove(modifier);
        }

        CalculatePrice();
    }

    /// <summary>
    /// Marks this order line as printed to kitchen.
    /// </summary>
    public void MarkPrintedToKitchen()
    {
        if (!ShouldPrintToKitchen)
        {
            throw new BusinessRuleViolationException("Order line is not configured to print to kitchen.");
        }

        PrintedToKitchen = true;

        // Also mark modifiers as printed
        foreach (var modifier in _modifiers.Where(m => m.ShouldPrintToKitchen))
        {
            modifier.MarkPrintedToKitchen();
        }

        foreach (var addOn in _addOns.Where(a => a.ShouldPrintToKitchen))
        {
            addOn.MarkPrintedToKitchen();
        }
    }

    /// <summary>
    /// Sets the printer group for kitchen routing.
    /// </summary>
    public void SetPrinterGroup(Guid? printerGroupId)
    {
        PrinterGroupId = printerGroupId;
    }

    /// <summary>
    /// Sets cooking instructions.
    /// </summary>
    public void SetInstructions(string? instructions)
    {
        Instructions = instructions;
    }

    /// <summary>
    /// Updates the modifiers list (replacing existing ones).
    /// </summary>
    public void UpdateModifiers(IEnumerable<OrderLineModifier> newModifiers)
    {
        _modifiers.Clear();
        _addOns.Clear();

        foreach (var modifier in newModifiers)
        {
            AddModifier(modifier);
        }
        
        CalculatePrice();
    }
}

