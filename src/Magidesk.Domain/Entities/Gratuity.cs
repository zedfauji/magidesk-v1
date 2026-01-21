using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents tips/gratuity on a ticket.
/// </summary>
public class Gratuity
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Money Amount { get; private set; }
    public bool Paid { get; private set; }
    public bool Refunded { get; private set; }
    public Guid TerminalId { get; private set; }
    public UserId OwnerId { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Gratuity()
    {
        Amount = Money.Zero();
    }

    public static Gratuity Create(
        Guid ticketId,
        Money amount,
        Guid terminalId,
        UserId ownerId)
    {
        if (amount < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Gratuity amount cannot be negative.");
        }

        return new Gratuity
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Amount = amount,
            TerminalId = terminalId,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            Paid = false,
            Refunded = false
        };
    }

    /// <summary>
    /// Adds to the existing gratuity amount.
    /// Used when accumulating multiple tips for the same ticket.
    /// </summary>
    [Obsolete("Use UpdateAmount instead to replace the gratuity amount.")]
    public void AddToAmount(Money additionalAmount)
    {
        if (additionalAmount < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Additional gratuity amount cannot be negative.");
        }

        Amount = Amount + additionalAmount;
    }

    /// <summary>
    /// Updates the gratuity amount to a new value.
    /// Used when changing the tip amount (e.g., from 20% to 25%).
    /// </summary>
    public void UpdateAmount(Money newAmount)
    {
        if (newAmount < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Gratuity amount cannot be negative.");
        }

        Amount = newAmount;
    }

    public void MarkAsPaid()
    {
        Paid = true;
    }

    public void MarkAsRefunded()
    {
        Refunded = true;
    }
}

