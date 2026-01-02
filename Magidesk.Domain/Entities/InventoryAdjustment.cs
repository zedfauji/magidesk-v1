using System;

namespace Magidesk.Domain.Entities;

public class InventoryAdjustment
{
    public Guid Id { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public decimal QuantityDelta { get; private set; }
    public string Reason { get; private set; } = null!;
    public DateTime AdjustedAt { get; private set; }
    public Guid? UserId { get; private set; }

    public virtual InventoryItem InventoryItem { get; private set; } = null!;

    private InventoryAdjustment() { }

    public static InventoryAdjustment Create(Guid itemId, decimal delta, string reason, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required for adjustment");

        return new InventoryAdjustment
        {
            Id = Guid.NewGuid(),
            InventoryItemId = itemId,
            QuantityDelta = delta,
            Reason = reason,
            AdjustedAt = DateTime.UtcNow,
            UserId = userId
        };
    }
}
