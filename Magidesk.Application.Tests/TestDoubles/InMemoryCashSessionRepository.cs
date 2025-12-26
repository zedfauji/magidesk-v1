using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryCashSessionRepository : ICashSessionRepository
{
    private readonly Dictionary<Guid, CashSession> _sessions = new();

    public Task AddAsync(CashSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CashSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task<CashSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(id, out var session);
        return Task.FromResult(session);
    }

    public Task<CashSession?> GetOpenSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = _sessions.Values.FirstOrDefault(s => s.UserId.Value == userId && s.ClosedAt == null);
        return Task.FromResult(session);
    }

    public Task<IEnumerable<CashSession>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default)
    {
        var result = _sessions.Values.Where(s => s.ShiftId == shiftId).ToList();
        return Task.FromResult<IEnumerable<CashSession>>(result);
    }
}
