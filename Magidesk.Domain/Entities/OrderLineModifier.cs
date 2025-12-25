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
    public Guid ModifierId { get; private set; }
    public Guid? MenuItemModifierGroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ModifierType ModifierType { get; private set; }
    public int ItemCount { get; private set; }
    public Money UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money SubtotalAmount { get; private set; }
    public Money TotalAmount { get; private set; }
    public bool ShouldPrintToKitchen { get; private set; }
    public bool PrintedToKitchen { get; private set; }
    public string? MultiplierName { get; private set; }
    public string? SectionName { get; private set; }
    public bool IsSectionWisePrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private OrderLineModifier()
    {
        UnitPrice = Money.Zero();
        TaxAmount = Money.Zero();
        SubtotalAmount = Money.Zero();
        TotalAmount = Money.Zero();
    }

    public static OrderLineModifier Create(
        Guid orderLineId,
        Guid modifierId,
        string name,
        ModifierType modifierType,
        int itemCount,
        Money unitPrice,
        decimal taxRate = 0m)
    {
        if (itemCount <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Item count must be greater than zero.");
        }

        if (unitPrice < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Unit price cannot be negative.");
        }

        var modifier = new OrderLineModifier
        {
            Id = Guid.NewGuid(),
            OrderLineId = orderLineId,
            ModifierId = modifierId,
            Name = name,
            ModifierType = modifierType,
            ItemCount = itemCount,
            UnitPrice = unitPrice,
            TaxRate = taxRate,
            CreatedAt = DateTime.UtcNow
        };

        modifier.CalculateTotals();
        return modifier;
    }

    private void CalculateTotals()
    {
        SubtotalAmount = UnitPrice * ItemCount;
        TaxAmount = SubtotalAmount * TaxRate;
        TotalAmount = SubtotalAmount + TaxAmount;
    }
}

