using System;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents money deposited from the cash drawer (cash drop).
/// </summary>
public class CashDrop
{
    public Guid Id { get; private set; }
    public Guid CashSessionId { get; private set; }
    public Money Amount { get; private set; }
    public string? Reason { get; private set; }
    public UserId ProcessedBy { get; private set; } = null!;
    public DateTime ProcessedAt { get; private set; }

    private CashDrop()
    {
        Amount = Money.Zero();
    }

    public static CashDrop Create(
        Guid cashSessionId,
        Money amount,
        UserId processedBy,
        string? reason = null)
    {
        if (amount <= Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Cash drop amount must be greater than zero.");
        }

        return new CashDrop
        {
            Id = Guid.NewGuid(),
            CashSessionId = cashSessionId,
            Amount = amount,
            Reason = reason,
            ProcessedBy = processedBy,
            ProcessedAt = DateTime.UtcNow
        };
    }
}

