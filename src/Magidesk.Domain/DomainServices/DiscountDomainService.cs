using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for discount operations.
/// Stateless service that coordinates discount business logic.
/// </summary>
public class DiscountDomainService
{
    /// <summary>
    /// Gets the maximum discount from a list of discounts for a ticket.
    /// Only one discount applies (the maximum one).
    /// </summary>
    public Discount? GetMaxDiscount(IEnumerable<Discount> discounts, Ticket ticket)
    {
        if (discounts == null)
        {
            throw new ArgumentNullException(nameof(discounts));
        }

        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        var eligibleDiscounts = discounts
            .Where(d => IsEligible(d, ticket))
            .ToList();

        if (!eligibleDiscounts.Any())
        {
            return null;
        }

        // Calculate discount amounts and select the maximum
        var discountAmounts = eligibleDiscounts
            .Select(d => new
            {
                Discount = d,
                Amount = CalculateDiscountAmount(d, ticket.SubtotalAmount)
            })
            .ToList();

        return discountAmounts
            .OrderByDescending(x => x.Amount)
            .First()
            .Discount;
    }

    /// <summary>
    /// Gets the maximum discount from a list of discounts for an order line.
    /// Only one discount applies (the maximum one).
    /// </summary>
    public Discount? GetMaxDiscount(IEnumerable<Discount> discounts, OrderLine orderLine)
    {
        if (discounts == null)
        {
            throw new ArgumentNullException(nameof(discounts));
        }

        if (orderLine == null)
        {
            throw new ArgumentNullException(nameof(orderLine));
        }

        var eligibleDiscounts = discounts
            .Where(d => IsEligible(d, orderLine))
            .ToList();

        if (!eligibleDiscounts.Any())
        {
            return null;
        }

        // Calculate discount amounts and select the maximum
        var discountAmounts = eligibleDiscounts
            .Select(d => new
            {
                Discount = d,
                Amount = CalculateDiscountAmount(d, orderLine.SubtotalAmount)
            })
            .ToList();

        return discountAmounts
            .OrderByDescending(x => x.Amount)
            .First()
            .Discount;
    }

    /// <summary>
    /// Calculates the discount amount for a discount applied to a ticket/item.
    /// </summary>
    public Money CalculateDiscountAmount(Discount discount, Money subtotal)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (subtotal <= Money.Zero())
        {
            return Money.Zero();
        }

        return discount.Type switch
        {
            Enumerations.DiscountType.Amount => new Money(Math.Min(discount.Value, subtotal.Amount)),
            Enumerations.DiscountType.Percentage => subtotal * (discount.Value / 100m),
            Enumerations.DiscountType.RePrice => subtotal - new Money(discount.Value),
            Enumerations.DiscountType.AltPrice => subtotal - new Money(discount.Value),
            _ => Money.Zero()
        };
    }

    /// <summary>
    /// Checks if a discount is eligible for a ticket.
    /// </summary>
    public bool IsEligible(Discount discount, Ticket ticket)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (!discount.IsActive)
        {
            return false;
        }

        // Check minimum buy requirement
        if (discount.MinimumBuy != null && ticket.SubtotalAmount < discount.MinimumBuy)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a discount is eligible for an order line.
    /// </summary>
    public bool IsEligible(Discount discount, OrderLine orderLine)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (orderLine == null)
        {
            throw new ArgumentNullException(nameof(orderLine));
        }

        if (!discount.IsActive)
        {
            return false;
        }

        // Check minimum quantity requirement
        if (discount.MinimumQuantity.HasValue && orderLine.ItemCount < discount.MinimumQuantity.Value)
        {
            return false;
        }

        return true;
    }
}

