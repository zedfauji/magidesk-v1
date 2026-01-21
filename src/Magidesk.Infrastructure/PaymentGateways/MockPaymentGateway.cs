using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.PaymentGateways;

/// <summary>
/// Mock payment gateway for development and testing.
/// Simulates card payment processing without actual external API calls.
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    private readonly Random _random = new();

    public Task<AuthorizationResult> AuthorizeAsync(
        CreditCardPayment payment,
        string cardNumber,
        string? cardHolderName = null,
        string? expirationDate = null,
        string? cvv = null,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(100);

        // Validate card number (basic validation)
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 13)
        {
            return Task.FromResult(new AuthorizationResult
            {
                Success = false,
                ErrorMessage = "Invalid card number"
            });
        }

        // Simulate occasional failures (5% failure rate)
        if (_random.Next(100) < 5)
        {
            return Task.FromResult(new AuthorizationResult
            {
                Success = false,
                ErrorMessage = "Authorization declined by bank"
            });
        }

        // Generate mock authorization code
        var authCode = GenerateAuthCode();
        var refNumber = GenerateReferenceNumber();
        var cardType = DetermineCardType(cardNumber);
        var lastFour = cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "****";

        return Task.FromResult(new AuthorizationResult
        {
            Success = true,
            AuthorizationCode = authCode,
            ReferenceNumber = refNumber,
            CardType = cardType,
            LastFourDigits = lastFour
        });
    }

    public Task<CaptureResult> CaptureAsync(
        CreditCardPayment payment,
        Money? amount = null,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(100);

        if (string.IsNullOrEmpty(payment.AuthorizationCode))
        {
            return Task.FromResult(new CaptureResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before capture"
            });
        }

        if (payment.IsCaptured)
        {
            return Task.FromResult(new CaptureResult
            {
                Success = false,
                ErrorMessage = "Payment is already captured"
            });
        }

        // Simulate occasional failures (2% failure rate)
        if (_random.Next(100) < 2)
        {
            return Task.FromResult(new CaptureResult
            {
                Success = false,
                ErrorMessage = "Capture failed"
            });
        }

        var captureCode = GenerateCaptureCode();
        var refNumber = GenerateReferenceNumber();

        return Task.FromResult(new CaptureResult
        {
            Success = true,
            CaptureCode = captureCode,
            ReferenceNumber = refNumber
        });
    }

    public Task<VoidResult> VoidAsync(
        CreditCardPayment payment,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(100);

        if (string.IsNullOrEmpty(payment.AuthorizationCode))
        {
            return Task.FromResult(new VoidResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before void"
            });
        }

        if (payment.IsCaptured)
        {
            return Task.FromResult(new VoidResult
            {
                Success = false,
                ErrorMessage = "Cannot void a captured payment. Use refund instead."
            });
        }

        if (payment.IsVoided)
        {
            return Task.FromResult(new VoidResult
            {
                Success = false,
                ErrorMessage = "Payment is already voided"
            });
        }

        var voidCode = GenerateVoidCode();

        return Task.FromResult(new VoidResult
        {
            Success = true,
            VoidCode = voidCode
        });
    }

    public Task<RefundResult> RefundAsync(
        CreditCardPayment payment,
        Money refundAmount,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(100);

        if (!payment.IsCaptured)
        {
            return Task.FromResult(new RefundResult
            {
                Success = false,
                ErrorMessage = "Payment must be captured before refund"
            });
        }

        if (refundAmount > payment.Amount)
        {
            return Task.FromResult(new RefundResult
            {
                Success = false,
                ErrorMessage = $"Refund amount ({refundAmount}) cannot exceed payment amount ({payment.Amount})"
            });
        }

        // Simulate occasional failures (3% failure rate)
        if (_random.Next(100) < 3)
        {
            return Task.FromResult(new RefundResult
            {
                Success = false,
                ErrorMessage = "Refund declined by bank"
            });
        }

        var refundCode = GenerateRefundCode();
        var refNumber = GenerateReferenceNumber();

        return Task.FromResult(new RefundResult
        {
            Success = true,
            RefundCode = refundCode,
            ReferenceNumber = refNumber
        });
    }

    public Task<AddTipsResult> AddTipsAsync(
        CreditCardPayment payment,
        Money tipsAmount,
        CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(100);

        if (string.IsNullOrEmpty(payment.AuthorizationCode))
        {
            return Task.FromResult(new AddTipsResult
            {
                Success = false,
                ErrorMessage = "Payment must be authorized before adding tips"
            });
        }

        if (payment.IsCaptured)
        {
            return Task.FromResult(new AddTipsResult
            {
                Success = false,
                ErrorMessage = "Cannot add tips to a captured payment"
            });
        }

        // For tips, we typically need to re-authorize with the new amount
        // In a real gateway, this would be a tip adjustment operation
        var authCode = GenerateAuthCode();

        return Task.FromResult(new AddTipsResult
        {
            Success = true,
            AuthorizationCode = authCode
        });
    }

    private string GenerateAuthCode() => $"AUTH{_random.Next(100000, 999999)}";
    private string GenerateCaptureCode() => $"CAP{_random.Next(100000, 999999)}";
    private string GenerateVoidCode() => $"VOID{_random.Next(100000, 999999)}";
    private string GenerateRefundCode() => $"REF{_random.Next(100000, 999999)}";
    private string GenerateReferenceNumber() => $"REF{DateTime.UtcNow:yyyyMMddHHmmss}{_random.Next(100, 999)}";

    private string DetermineCardType(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return "Unknown";

        var firstDigit = cardNumber[0];
        return firstDigit switch
        {
            '4' => "Visa",
            '5' => "MasterCard",
            '3' => "Amex",
            '6' => "Discover",
            _ => "Unknown"
        };
    }

    public Task<BatchCloseResult> CloseBatchAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        // Simulate network delay
        Thread.Sleep(500);

        return Task.FromResult(new BatchCloseResult
        {
             Success = true,
             GatewayBatchId = $"BATCH-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(1000, 9999)}",
             TotalAmount = Money.Zero(), // In a real mock, we might track this?
             TransactionCount = 0
        });
    }
}

