using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a historical record of stock changes for a menu item.
/// Immutable audit log of inventory movements.
/// </summary>
public class StockMovement
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int QuantityChange { get; private set; }
    public StockMovementType Type { get; private set; }
    public string Reference { get; private set; } // Order Number, PO Number, "Manual Adjustment"
    public DateTime Timestamp { get; private set; }
    public Guid? UserId { get; private set; } // Who performed the action

    /// <summary>
    /// EF Core Constructor
    /// </summary>
    protected StockMovement() { }

    private StockMovement(
        Guid menuItemId,
        int quantityChange,
        StockMovementType type,
        string reference,
        Guid? userId)
    {
        Id = Guid.NewGuid();
        MenuItemId = menuItemId;
        QuantityChange = quantityChange;
        Type = type;
        Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        Timestamp = DateTime.UtcNow;
        UserId = userId;
    }

    public static StockMovement Create(
        Guid menuItemId,
        int quantityChange,
        StockMovementType type,
        string reference,
        Guid? userId)
    {
        if (menuItemId == Guid.Empty)
            throw new ArgumentException("MenuItemId cannot be empty.", nameof(menuItemId));
            
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Reference/Reason is required.", nameof(reference));

        return new StockMovement(menuItemId, quantityChange, type, reference, userId);
    }
}
