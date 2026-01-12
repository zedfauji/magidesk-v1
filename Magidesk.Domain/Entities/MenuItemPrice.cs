using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a specific price for a menu item at a specific price level.
/// </summary>
public class MenuItemPrice
{
    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public Guid PriceLevelId { get; private set; }
    public Money Price { get; private set; }
    
    // Navigation properties
    public virtual MenuItem MenuItem { get; private set; } = null!;
    public virtual PriceLevel PriceLevel { get; private set; } = null!;

    // Private constructor for EF Core
    private MenuItemPrice() { }

    public static MenuItemPrice Create(Guid menuItemId, Guid priceLevelId, Money price)
    {
        if (menuItemId == Guid.Empty) throw new ArgumentException("MenuItemId cannot be empty", nameof(menuItemId));
        if (priceLevelId == Guid.Empty) throw new ArgumentException("PriceLevelId cannot be empty", nameof(priceLevelId));
        if (price is null) throw new ArgumentNullException(nameof(price));

        return new MenuItemPrice
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId,
            PriceLevelId = priceLevelId,
            Price = price
        };
    }

    public void UpdatePrice(Money price)
    {
        if (price is null) throw new ArgumentNullException(nameof(price));
        Price = price;
    }
}
