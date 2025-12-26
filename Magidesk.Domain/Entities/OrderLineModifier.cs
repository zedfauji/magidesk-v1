using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a modifier or add-on for an order line.
/// </summary>
public class OrderLineModifier
{
    public Guid Id { get; private set; }
    public Guid OrderLineId { get; private set; }
    public Guid? ModifierId { get; private set; }
    public Guid? ModifierGroupId { get; private set; }
    public Guid? MenuItemModifierGroupId { get; private set; }
    public Guid? ParentOrderLineModifierId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ModifierType ModifierType { get; private set; }
    public int ItemCount { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money BasePrice { get; private set; } // Snapshot of original price
    public decimal PortionValue { get; private set; } = 1.0m;
    public decimal TaxRate { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money SubtotalAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    public bool ShouldPrintToKitchen { get; private set; }
    public bool PrintedToKitchen { get; private set; }
    public string? MultiplierName { get; private set; }
    public string? SectionName { get; private set; }
    public bool IsSectionWisePrice { get; private set; }
    public PriceStrategy? PriceStrategy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private OrderLineModifier()
    {
        UnitPrice = Money.Zero();
        BasePrice = Money.Zero();
        TaxAmount = Money.Zero();
        SubtotalAmount = Money.Zero();
        TotalAmount = Money.Zero();
    }

    public static OrderLineModifier Create(
        Guid orderLineId,
        Guid? modifierId,
        string name,
        ModifierType modifierType,
        int itemCount,
        Money unitPrice,
        Money basePrice, // Snapshot
        decimal portionValue = 1.0m,
        decimal taxRate = 0m,
        Guid? menuItemModifierGroupId = null,
        Guid? modifierGroupId = null,
        bool shouldPrintToKitchen = true,
        string? sectionName = null,
        string? multiplierName = null,

        bool isSectionWisePrice = false,
        Guid? parentOrderLineModifierId = null,
        PriceStrategy? priceStrategy = null)
    {
        if (itemCount <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Item count must be greater than zero.");
        }

        if (unitPrice < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Unit price cannot be negative.");
        }

        if (!modifierId.HasValue && modifierType != ModifierType.InfoOnly)
        {
             // Typically we want ID unless it's info only.
             // But for manual modifiers (open item modifier), maybe null ID is okay too?
             // Let's enforce: If ModifierId is null, it acts as ad-hoc/instruction.
        }

        var modifier = new OrderLineModifier
        {
            Id = Guid.NewGuid(),
            OrderLineId = orderLineId,
            ModifierId = modifierId,
            ModifierGroupId = modifierGroupId,
            MenuItemModifierGroupId = menuItemModifierGroupId,
            Name = name,
            ModifierType = modifierType,
            ItemCount = itemCount,
            UnitPrice = unitPrice,
            BasePrice = basePrice,
            PortionValue = portionValue,
            TaxRate = taxRate,
            ShouldPrintToKitchen = shouldPrintToKitchen,
            SectionName = sectionName,
            MultiplierName = multiplierName,
            IsSectionWisePrice = isSectionWisePrice,
            ParentOrderLineModifierId = parentOrderLineModifierId,
            PriceStrategy = priceStrategy,
            CreatedAt = DateTime.UtcNow
        };

        modifier.CalculateTotals();
        return modifier;
    }

    /// <summary>
    /// Creates a pizza-style section modifier.
    /// </summary>
    public static OrderLineModifier CreatePizzaSectionModifier(
        Guid orderLineId,
        Guid modifierId,
        string name,
        ModifierType modifierType,
        int itemCount,
        Money unitPrice,
        Money basePrice, // Requires BasePrice
        string sectionName,
        string multiplierName,
        decimal taxRate = 0m)
    {
        return Create(
            orderLineId,
            modifierId,
            name,
            modifierType,
            itemCount,
            unitPrice,
            basePrice, // Passed through
            1.0m, // Default Portion (Handled via UnitPrice logic externally usually, or update logic)
            taxRate,
            null,
            shouldPrintToKitchen: true,
            sectionName: sectionName,
            multiplierName: multiplierName,
            isSectionWisePrice: true);
    }

    private void CalculateTotals()
    {
        SubtotalAmount = UnitPrice * ItemCount;
        TaxAmount = SubtotalAmount * TaxRate;
        TotalAmount = SubtotalAmount + TaxAmount;
    }

    /// <summary>
    /// Updates the unit price. Used by PriceCalculator.
    /// </summary>
    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice < Money.Zero())
             throw new Exceptions.BusinessRuleViolationException("Unit price cannot be negative.");
             
        UnitPrice = newUnitPrice;
        CalculateTotals();
    }

    /// <summary>
    ///  Marks this modifier as printed to kitchen.
    /// </summary>
    public void MarkPrintedToKitchen()
    {
        if (!ShouldPrintToKitchen)
        {
            throw new Exceptions.BusinessRuleViolationException("Modifier is not configured to print to kitchen.");
        }

        PrintedToKitchen = true;
    }

    /// <summary>
    /// Creates a cooking instruction modifier (free text, info only).
    /// </summary>
    public static OrderLineModifier CreateInstruction(
        Guid orderLineId,
        string instruction)
    {
        if (string.IsNullOrWhiteSpace(instruction))
        {
            throw new Exceptions.BusinessRuleViolationException("Instruction text cannot be empty.");
        }

        return Create(
            orderLineId: orderLineId,
            modifierId: null,
            name: instruction.Trim().ToUpperInvariant(),
            modifierType: ModifierType.InfoOnly,
            itemCount: 1,
            unitPrice: Money.Zero(),
            basePrice: Money.Zero(), // Base Price is Zero
            portionValue: 1.0m,
            taxRate: 0m,
            menuItemModifierGroupId: null,
            modifierGroupId: null,
            shouldPrintToKitchen: true,
            sectionName: null,
            multiplierName: null,
            isSectionWisePrice: false,
            parentOrderLineModifierId: null
        );
    }
}

