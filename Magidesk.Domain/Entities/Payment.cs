using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a payment transaction.
/// Base class for all payment types using Table Per Hierarchy (TPH) pattern.
/// </summary>
public abstract class Payment
{
    public Guid Id { get; protected set; }
    public string? GlobalId { get; protected set; }
    public Guid TicketId { get; protected set; }
    public TransactionType TransactionType { get; protected set; }
    public PaymentType PaymentType { get; protected set; }
    public Money Amount { get; protected set; }
    public Money TipsAmount { get; protected set; }
    public Money TipsExceedAmount { get; private set; }
    public Money TenderAmount { get; protected set; }
    public Money ChangeAmount { get; protected set; }
    
    protected void SetTipsAmount(Money tipsAmount)
    {
        TipsAmount = tipsAmount;
    }

    /// <summary>
    /// Sets tender amount for cash payments.
    /// Kept as a protected method so Application layer can set via domain behavior (used by ProcessPaymentCommandHandler).
    /// </summary>
    protected void SetTenderAmount(Money tenderAmount)
    {
        TenderAmount = tenderAmount;
    }

    /// <summary>
    /// Sets calculated change amount for cash payments.
    /// </summary>
    protected void SetChangeAmount(Money changeAmount)
    {
        ChangeAmount = changeAmount;
    }

    /// <summary>
    /// Associates this payment with a cash session.
    /// </summary>
    protected void SetCashSessionId(Guid cashSessionId)
    {
        CashSessionId = cashSessionId;
    }
    public DateTime TransactionTime { get; protected set; }
    public UserId ProcessedBy { get; protected set; } = null!;
    public Guid TerminalId { get; protected set; }
    public bool IsCaptured { get; protected set; }
    public bool IsVoided { get; protected set; }
    public bool IsAuthorizable { get; protected set; }
    public Guid? CashSessionId { get; protected set; }
    public string? Note { get; protected set; }

    protected Payment()
    {
        Amount = Money.Zero();
        TipsAmount = Money.Zero();
        TipsExceedAmount = Money.Zero();
        TenderAmount = Money.Zero();
        ChangeAmount = Money.Zero();
    }

    protected Payment(
        Guid ticketId,
        PaymentType paymentType,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null)
    {
        Id = Guid.NewGuid();
        GlobalId = globalId;
        TicketId = ticketId;
        PaymentType = paymentType;
        TransactionType = TransactionType.Credit; // Payments are credits
        Amount = amount;
        TipsAmount = Money.Zero();
        TipsExceedAmount = Money.Zero();
        TenderAmount = Money.Zero();
        ChangeAmount = Money.Zero();
        TransactionTime = DateTime.UtcNow;
        ProcessedBy = processedBy;
        TerminalId = terminalId;
        IsCaptured = false;
        IsVoided = false;
        IsAuthorizable = false;
    }

    /// <summary>
    /// Factory method to create cash payment instances.
    /// For other payment types, use the specific Create methods (CreditCardPayment.Create, etc.).
    /// </summary>
    public static Payment Create(
        Guid ticketId,
        PaymentType paymentType,
        Money amount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null)
    {
        if (paymentType != PaymentType.Cash)
        {
            throw new InvalidOperationException(
                $"Use specific payment type Create methods for {paymentType}. " +
                $"For cash, use CashPayment.Create().");
        }

        return CashPayment.Create(ticketId, amount, processedBy, terminalId, globalId);
    }

    public void Void()
    {
        if (IsVoided)
        {
            throw new Exceptions.InvalidOperationException("Payment is already voided.");
        }

        IsVoided = true;
    }

    public void Capture()
    {
        if (!IsAuthorizable)
        {
            throw new Exceptions.InvalidOperationException("Payment cannot be captured (not authorizable).");
        }

        if (IsCaptured)
        {
            throw new Exceptions.InvalidOperationException("Payment is already captured.");
        }

        IsCaptured = true;
    }

    /// <summary>
    /// Adds tips to the payment (for card payments).
    /// </summary>
    public virtual void AddTips(Money tipsAmount)
    {
        if (tipsAmount < Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Tips amount cannot be negative.");
        }

        if (IsVoided)
        {
            throw new Exceptions.InvalidOperationException("Cannot add tips to a voided payment.");
        }

        SetTipsAmount(tipsAmount);
    }

    /// <summary>
    /// Creates a refund payment for this payment.
    /// Refunds are represented as Payment entities with TransactionType.Debit.
    /// </summary>
    public static Payment CreateRefund(
        Payment originalPayment,
        Money refundAmount,
        UserId processedBy,
        Guid terminalId,
        string? reason = null,
        string? globalId = null)
    {
        if (originalPayment == null)
        {
            throw new ArgumentNullException(nameof(originalPayment));
        }

        if (refundAmount <= Money.Zero())
        {
            throw new Exceptions.BusinessRuleViolationException("Refund amount must be greater than zero.");
        }

        if (refundAmount > originalPayment.Amount)
        {
            throw new Exceptions.BusinessRuleViolationException(
                $"Refund amount ({refundAmount}) cannot exceed original payment amount ({originalPayment.Amount}).");
        }

        if (originalPayment.IsVoided)
        {
            throw new Exceptions.InvalidOperationException("Cannot refund a voided payment.");
        }

        // Create refund payment based on original payment type
        // Use reflection to set TransactionType to Debit after creation
        Payment refundPayment = originalPayment.PaymentType switch
        {
            PaymentType.Cash => CashPayment.Create(
                originalPayment.TicketId,
                refundAmount,
                processedBy,
                terminalId,
                globalId),
            PaymentType.CreditCard => CreditCardPayment.Create(
                originalPayment.TicketId,
                refundAmount,
                processedBy,
                terminalId,
                globalId: globalId),
            PaymentType.DebitCard => DebitCardPayment.Create(
                originalPayment.TicketId,
                refundAmount,
                processedBy,
                terminalId,
                globalId: globalId),
            PaymentType.GiftCertificate => 
                originalPayment is GiftCertificatePayment gcPayment
                    ? GiftCertificatePayment.Create(
                        originalPayment.TicketId,
                        refundAmount,
                        processedBy,
                        terminalId,
                        gcPayment.GiftCertificateNumber,
                        gcPayment.OriginalAmount,
                        gcPayment.RemainingBalance + refundAmount, // Restore balance
                        globalId)
                    : throw new Exceptions.InvalidOperationException("Cannot refund gift certificate payment without certificate details."),
            PaymentType.CustomPayment =>
                originalPayment is CustomPayment customPayment
                    ? CustomPayment.Create(
                        originalPayment.TicketId,
                        refundAmount,
                        processedBy,
                        terminalId,
                        customPayment.PaymentName,
                        null,
                        null,
                        globalId)
                    : throw new Exceptions.InvalidOperationException("Cannot refund custom payment without payment details."),
            _ => throw new Exceptions.InvalidOperationException($"Refund not supported for payment type {originalPayment.PaymentType}.")
        };

        // Set transaction type to Debit for refund
        var transactionTypeProperty = typeof(Payment).GetProperty("TransactionType",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        transactionTypeProperty?.SetValue(refundPayment, TransactionType.Debit);

        // Set note with refund information
        var noteProperty = typeof(Payment).GetProperty("Note",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        noteProperty?.SetValue(refundPayment, $"Refund of payment {originalPayment.Id}. Reason: {reason ?? "N/A"}");

        return refundPayment;
    }
}

