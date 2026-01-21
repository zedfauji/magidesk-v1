using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a discount applied to an order line (immutable snapshot).
/// </summary>
public class OrderLineDiscount
{
    public Guid Id { get; private set; }
    public Guid OrderLineId { get; private set; }
    public Guid DiscountId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public int? MinimumQuantity { get; private set; }
    public Money Amount { get; private set; }
    public bool AutoApply { get; private set; }
    public DateTime AppliedAt { get; private set; }

    private OrderLineDiscount()
    {
        Amount = Money.Zero();
    }

    public static OrderLineDiscount Create(
        Guid orderLineId,
        Guid discountId,
        string name,
        DiscountType type,
        decimal value,
        Money amount,
        int? minimumQuantity = null,
        bool autoApply = false)
    {
        return new OrderLineDiscount
        {
            Id = Guid.NewGuid(),
            OrderLineId = orderLineId,
            DiscountId = discountId,
            Name = name,
            Type = type,
            Value = value,
            Amount = amount,
            MinimumQuantity = minimumQuantity,
            AutoApply = autoApply,
            AppliedAt = DateTime.UtcNow
        };
    }
}

