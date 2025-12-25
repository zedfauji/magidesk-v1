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

    public void MarkAsPaid()
    {
        Paid = true;
    }

    public void MarkAsRefunded()
    {
        Refunded = true;
    }
}

