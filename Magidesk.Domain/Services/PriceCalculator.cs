using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service to calculate prices for complex menu models like Pizza (Fractional) and Combos.
/// </summary>
public class PriceCalculator
{
    /// <summary>
    /// Recalculates unit prices for fractional modifiers on an order line based on the strategy.
    /// </summary>
    /// <param name="modifiers">The list of order line modifiers involved in the fractional set (e.g. halves of a pizza).</param>
    /// <param name="strategy">The pricing strategy to apply.</param>
    public void RecalculateFractionalPrices(List<OrderLineModifier> modifiers, PriceStrategy strategy)
    {
        if (modifiers == null || !modifiers.Any()) return;

        // Filter for fractional portions (IsSectionWisePrice is a good proxy, or PortionValue < 1)
        var fractionalModifiers = modifiers.Where(m => m.IsSectionWisePrice).ToList();
        if (!fractionalModifiers.Any()) return;

        // Group by strategy if mixed? TDD suggests strategy is per modifier definition.
        // But usually a Pizza has ONE strategy.
        // We use the passed 'strategy' as the governing rule for this recalculation context.
        // Or we should check each modifier's strategy? 
        // If passed strategy is global for the recalculation, we use it.

        switch (strategy)
        {
            case PriceStrategy.SumOfHalves:
            case PriceStrategy.AverageOfHalves:
                // Price = BasePrice * Portion
                foreach (var mod in fractionalModifiers)
                {
                    var newPrice = mod.BasePrice * mod.PortionValue;
                    mod.UpdateUnitPrice(newPrice);
                }
                break;

            case PriceStrategy.HighestHalf:
            case PriceStrategy.WholePie:
                // Find highest BasePrice
                if (!fractionalModifiers.Any()) break;
                
                var maxBasePrice = fractionalModifiers.Max(m => m.BasePrice);
                if (maxBasePrice == null) maxBasePrice = Money.Zero();

                foreach (var mod in fractionalModifiers)
                {
                    var newPrice = maxBasePrice * mod.PortionValue;
                    mod.UpdateUnitPrice(newPrice);
                }
                break;
        }
    }
    
    /// <summary>
    /// Calculates the upsell price for a combo group selection.
    /// </summary>
    public Money CalculateComboItemPrice(Money baseModifierPrice, Money upcharge)
    {
        // For combos, the item price itself is usually 0, plus upcharge.
        // If the item has a base price (e.g. fancy burger), the upcharge might be the difference?
        // Usually Combo Definition says: "Burger A included. Burger B +$1."
        // So the price on the line should be +$1.
        return upcharge; 
    }
}
