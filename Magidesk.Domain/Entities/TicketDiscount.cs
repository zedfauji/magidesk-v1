using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a discount applied to an entire ticket (immutable snapshot).
/// </summary>
public class TicketDiscount
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public Guid DiscountId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public Money? MinimumAmount { get; private set; }
    public Money Amount { get; private set; }
    public DateTime AppliedAt { get; private set; }

    private TicketDiscount()
    {
        Amount = Money.Zero();
    }

    public static TicketDiscount Create(
        Guid ticketId,
        Guid discountId,
        string name,
        DiscountType type,
        decimal value,
        Money amount,
        Money? minimumAmount = null)
    {
        return new TicketDiscount
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            DiscountId = discountId,
            Name = name,
            Type = type,
            Value = value,
            Amount = amount,
            MinimumAmount = minimumAmount,
            AppliedAt = DateTime.UtcNow
        };
    }
}

