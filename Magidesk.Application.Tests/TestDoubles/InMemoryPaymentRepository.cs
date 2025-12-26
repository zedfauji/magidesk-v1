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
}
