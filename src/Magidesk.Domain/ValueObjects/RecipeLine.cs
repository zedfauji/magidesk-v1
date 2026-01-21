using System;
using System.Collections.Generic;

namespace Magidesk.Domain.ValueObjects;

public class RecipeLine
{
    public Guid InventoryItemId { get; private set; }
    public decimal Quantity { get; private set; }

    public RecipeLine(Guid inventoryItemId, decimal quantity)
    {
        if (inventoryItemId == Guid.Empty) throw new ArgumentException("Invalid Inventory Item ID");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

        InventoryItemId = inventoryItemId;
        Quantity = quantity;
    }
}
