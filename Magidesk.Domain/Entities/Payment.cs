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
}

