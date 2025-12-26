using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly Dictionary<Guid, Payment> _payments = new();

    public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _payments.TryGetValue(id, out var payment);
        return Task.FromResult(payment);
    }

    public Task<IEnumerable<Payment>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var result = _payments.Values.Where(p => p.TicketId == ticketId).ToList();
        return Task.FromResult<IEnumerable<Payment>>(result);
    }

    public Task<IEnumerable<Payment>> GetByCashSessionIdAsync(Guid cashSessionId, CancellationToken cancellationToken = default)
    {
        var result = _payments.Values.Where(p => p.CashSessionId == cashSessionId).ToList();
        return Task.FromResult<IEnumerable<Payment>>(result);
    }

    public Task<IEnumerable<Payment>> GetUnbatchedCapturedPaymentsAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        // Simplistic assuming Capture means PaymentStatus.Paid and CardPayment.
        // And unbatched means BatchId is null.
        // Assuming Payment entity has TerminalId or relies on Session.
        // For Stub, simply return empty or mock data.
        // Actually, let's see Payment entity properties if needed?
        // Stub implementation:
        var result = _payments.Values
            .OfType<CreditCardPayment>()
            .Where(p => p.IsCaptured && p.BatchId == null)
             .Cast<Payment>()
            .ToList();
            
        return Task.FromResult<IEnumerable<Payment>>(result);
    }
}
