using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a gift certificate payment transaction.
/// </summary>
public class GiftCertificatePayment : Payment
{
    public string GiftCertificateNumber { get; protected set; } = null!;
    public Money OriginalAmount { get; protected set; }
    public Money RemainingBalance { get; protected set; }

    protected GiftCertificatePayment()
    {
        OriginalAmount = Money.Zero();
        RemainingBalance = Money.Zero();
    }

    protected GiftCertificatePayment(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string giftCertificateNumber,
        Money originalAmount,
        Money remainingBalance,
        string? globalId = null)
        : base(ticketId, PaymentType.GiftCertificate, amount, processedBy, terminalId, globalId)
    {
        if (string.IsNullOrWhiteSpace(giftCertificateNumber))
        {
            throw new ArgumentException("Gift certificate number cannot be null or empty.", nameof(giftCertificateNumber));
        }

        GiftCertificateNumber = giftCertificateNumber;
        OriginalAmount = originalAmount;
        RemainingBalance = remainingBalance;
        IsAuthorizable = false; // Gift certificates are not authorizable
    }

    /// <summary>
    /// Creates a new gift certificate payment.
    /// </summary>
    public static GiftCertificatePayment Create(
        Guid ticketId,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string giftCertificateNumber,
        Money originalAmount,
        Money remainingBalance,
        string? globalId = null)
    {
        if (amount > remainingBalance)
        {
            throw new Exceptions.BusinessRuleViolationException(
                $"Payment amount ({amount}) exceeds remaining balance ({remainingBalance}).");
        }

        return new GiftCertificatePayment(
            ticketId,
            amount,
            processedBy,
            terminalId,
            giftCertificateNumber,
            originalAmount,
            remainingBalance,
            globalId);
    }

    /// <summary>
    /// Updates the remaining balance after payment.
    /// </summary>
    public void UpdateRemainingBalance(Money newBalance)
    {
        if (newBalance < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Remaining balance cannot be negative.");
        }

        RemainingBalance = newBalance;
    }
}

