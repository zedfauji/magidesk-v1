using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class StubPaymentGateway : IPaymentGateway
{
    public Task<AuthorizationResult> AuthorizeAsync(CreditCardPayment payment, string cardNumber, string? cardHolderName = null, string? expirationDate = null, string? cvv = null, CancellationToken cancellationToken = default)
        => Task.FromResult(new AuthorizationResult { Success = true, AuthorizationCode = "AUTH", ReferenceNumber = "REF", CardType = "VISA", LastFourDigits = "1111" });

    public Task<CaptureResult> CaptureAsync(CreditCardPayment payment, Money? amount = null, CancellationToken cancellationToken = default)
        => Task.FromResult(new CaptureResult { Success = true, CaptureCode = "CAP", ReferenceNumber = "REF" });

    public Task<VoidResult> VoidAsync(CreditCardPayment payment, CancellationToken cancellationToken = default)
        => Task.FromResult(new VoidResult { Success = true, VoidCode = "VOID" });

    public Task<RefundResult> RefundAsync(CreditCardPayment payment, Money refundAmount, CancellationToken cancellationToken = default)
        => Task.FromResult(new RefundResult { Success = true, RefundCode = "RFND", ReferenceNumber = "REF" });

    public Task<AddTipsResult> AddTipsAsync(CreditCardPayment payment, Money tipsAmount, CancellationToken cancellationToken = default)
        => Task.FromResult(new AddTipsResult { Success = true, AuthorizationCode = "AUTH" });

    public Task<BatchCloseResult> CloseBatchAsync(Guid terminalId, CancellationToken cancellationToken = default)
        => Task.FromResult(new BatchCloseResult { Success = true, GatewayBatchId = "BATCH123", TransactionCount = 10, TotalAmount = new Money(500.00m) });
}
