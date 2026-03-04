using System;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing discount management methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Applies a discount to the ticket.
    /// </summary>
    public void ApplyDiscount(TicketDiscount discount)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        if (discount.TicketId != Id)
        {
            throw new BusinessRuleViolationException("Discount does not belong to this ticket.");
        }

        _discounts.Add(discount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }

    /// <summary>
    /// Applies a discount to the ticket with authorization support.
    /// </summary>
    /// <param name="discount">The discount definition to apply</param>
    /// <param name="appliedBy">User applying the discount</param>
    /// <param name="authorizedBy">Manager who authorized the discount (required for discounts > 50%)</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket status doesn't allow discounts</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown if discount would result in negative total or authorization is missing</exception>
    public void ApplyDiscount(Discount discount, UserId appliedBy, UserId? authorizedBy = null)
    {
        if (discount == null)
        {
            throw new ArgumentNullException(nameof(discount));
        }

        if (appliedBy == null)
        {
            throw new ArgumentNullException(nameof(appliedBy));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        if (!discount.IsActive)
        {
            throw new BusinessRuleViolationException("Cannot apply inactive discount.");
        }

        // Calculate discount amount
        var discountAmount = discount.CalculateDiscount(SubtotalAmount);

        // Validate discount doesn't result in negative total
        var newTotal = TotalAmount - discountAmount;
        if (newTotal < Money.Zero())
        {
            throw new BusinessRuleViolationException("Discount would result in negative total.");
        }

        // Check if discount requires authorization (> 50% of subtotal)
        var discountPercentage = SubtotalAmount.Amount > 0
            ? (discountAmount.Amount / SubtotalAmount.Amount) * 100m
            : 0m;

        if (discountPercentage > 50m && authorizedBy == null)
        {
            throw new BusinessRuleViolationException("Discounts greater than 50% require manager authorization.");
        }

        // Create ticket discount snapshot
        var ticketDiscount = TicketDiscount.Create(
            ticketId: Id,
            discountId: discount.Id,
            name: discount.Name,
            type: discount.Type,
            value: discount.Value,
            amount: discountAmount,
            appliedBy: appliedBy,
            authorizedBy: authorizedBy,
            minimumAmount: discount.MinimumBuy
        );

        _discounts.Add(ticketDiscount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();

        // TODO: Raise DiscountAppliedEvent (Task 2.1.4)
    }

    /// <summary>
    /// Removes a discount from the ticket.
    /// </summary>
    /// <param name="discountId">The ID of the TicketDiscount to remove</param>
    /// <exception cref="DomainInvalidOperationException">Thrown if ticket status doesn't allow discount removal</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown if discount not found</exception>
    public void RemoveDiscount(Guid discountId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot remove discount from ticket in {Status} status.");
        }

        var discount = _discounts.FirstOrDefault(d => d.Id == discountId);
        if (discount == null)
        {
            throw new BusinessRuleViolationException($"Discount {discountId} not found on this ticket.");
        }

        _discounts.Remove(discount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();

        // TODO: Raise DiscountRemovedEvent (Task 2.1.4)
    }

    /// <summary>
    /// Applies a discount to a specific order line in the ticket.
    /// </summary>
    public void ApplyLineDiscount(Guid orderLineId, OrderLineDiscount discount)
    {
        if (discount == null) throw new ArgumentNullException(nameof(discount));

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot apply discount to ticket in {Status} status.");
        }

        var line = _orderLines.FirstOrDefault(x => x.Id == orderLineId);
        if (line == null)
            throw new BusinessRuleViolationException($"OrderLine {orderLineId} not found in this ticket.");

        line.ApplyDiscount(discount);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }
}
