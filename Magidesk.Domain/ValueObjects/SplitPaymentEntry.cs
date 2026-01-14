using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a single payment entry in a split payment transaction.
/// </summary>
public record SplitPaymentEntry
{
    public PaymentType Method { get; init; }
    public Money Amount { get; init; }

    public SplitPaymentEntry(PaymentType method, Money amount)
    {
        if (amount <= Money.Zero())
        {
            throw new ArgumentException("Payment amount must be positive.", nameof(amount));
        }

        Method = method;
        Amount = amount;
    }
}
