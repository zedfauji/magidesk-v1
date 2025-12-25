using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for payment operations.
/// Stateless service that coordinates payment business logic.
/// </summary>
public class PaymentDomainService
{
    /// <summary>
    /// Calculates change for cash payment.
    /// </summary>
    public Money CalculateChange(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (payment.PaymentType != Enumerations.PaymentType.Cash)
        {
            throw new BusinessRuleViolationException("Change calculation only applies to cash payments.");
        }

        if (payment.TenderAmount < payment.Amount)
        {
            throw new BusinessRuleViolationException("Tender amount must be greater than or equal to payment amount.");
        }

        return payment.TenderAmount - payment.Amount;
    }

    /// <summary>
    /// Validates if a payment can be voided.
    /// </summary>
    public bool CanVoidPayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        return !payment.IsVoided;
    }

    /// <summary>
    /// Validates if a payment can be refunded.
    /// </summary>
    public bool CanRefundPayment(Payment payment, Money refundAmount)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (refundAmount <= Money.Zero())
        {
            throw new BusinessRuleViolationException("Refund amount must be greater than zero.");
        }

        if (payment.IsVoided)
        {
            return false;
        }

        // For card payments, must be captured first
        if (payment.IsAuthorizable && !payment.IsCaptured)
        {
            return false;
        }

        // Refund amount cannot exceed payment amount
        if (refundAmount > payment.Amount)
        {
            throw new BusinessRuleViolationException($"Refund amount ({refundAmount}) cannot exceed payment amount ({payment.Amount}).");
        }

        return true;
    }

    /// <summary>
    /// Validates if a payment can be captured (for card transactions).
    /// </summary>
    public bool CanCapturePayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (!payment.IsAuthorizable)
        {
            return false;
        }

        if (payment.IsCaptured)
        {
            return false;
        }

        if (payment.IsVoided)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates if tips can be added to a payment.
    /// </summary>
    public bool CanAddTips(Payment payment, Money tipsAmount)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (tipsAmount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Tips amount cannot be negative.");
        }

        if (payment.IsVoided)
        {
            return false;
        }

        // Tips can be added to any payment type
        return true;
    }

    /// <summary>
    /// Calculates tips that exceed the payment amount.
    /// </summary>
    public Money CalculateTipsExceedAmount(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (payment.TipsAmount <= payment.Amount)
        {
            return Money.Zero();
        }

        return payment.TipsAmount - payment.Amount;
    }
}

