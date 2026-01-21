using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for service charge calculations.
/// Service charges are typically calculated as a percentage of subtotal (after discounts).
/// </summary>
public class ServiceChargeDomainService
{
    /// <summary>
    /// Calculates service charge based on a percentage rate.
    /// </summary>
    /// <param name="subtotal">Subtotal amount (after discounts)</param>
    /// <param name="serviceChargeRate">Service charge rate as decimal (e.g., 0.15 for 15%)</param>
    /// <returns>Service charge amount</returns>
    public Money CalculateServiceCharge(Money subtotal, decimal serviceChargeRate)
    {
        if (subtotal == null)
        {
            throw new ArgumentNullException(nameof(subtotal));
        }

        if (serviceChargeRate < 0 || serviceChargeRate > 1)
        {
            throw new Exceptions.BusinessRuleViolationException("Service charge rate must be between 0 and 1 (0% to 100%).");
        }

        if (subtotal <= Money.Zero())
        {
            return Money.Zero();
        }

        return subtotal * serviceChargeRate;
    }

    /// <summary>
    /// Calculates service charge based on a percentage rate for a ticket.
    /// Uses the ticket's subtotal after discounts.
    /// </summary>
    public Money CalculateServiceChargeForTicket(Ticket ticket, decimal serviceChargeRate)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        // Service charge is calculated on subtotal after discounts
        var subtotalAfterDiscounts = ticket.SubtotalAmount - ticket.DiscountAmount;
        return CalculateServiceCharge(subtotalAfterDiscounts, serviceChargeRate);
    }

    /// <summary>
    /// Calculates service charge based on a fixed amount per guest.
    /// </summary>
    public Money CalculateServiceChargePerGuest(int numberOfGuests, Money chargePerGuest)
    {
        if (numberOfGuests <= 0)
        {
            throw new Exceptions.BusinessRuleViolationException("Number of guests must be greater than zero.");
        }

        if (chargePerGuest == null)
        {
            throw new ArgumentNullException(nameof(chargePerGuest));
        }

        if (chargePerGuest < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Charge per guest cannot be negative.");
        }

        return chargePerGuest * numberOfGuests;
    }
}

