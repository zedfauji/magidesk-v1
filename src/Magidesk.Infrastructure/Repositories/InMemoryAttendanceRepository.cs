using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Repositories;

public class InMemoryAttendanceRepository : IAttendanceRepository
{
    private readonly List<AttendanceHistory> _records = new();

    public Task<AttendanceHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_records.FirstOrDefault(r => r.Id == id));
    }

    public Task AddAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        _records.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        // In-memory update is implicit as we hold reference
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        _records.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<AttendanceHistory?> GetOpenByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_records.FirstOrDefault(r => r.UserId == userId && r.ClockOutTime == null));
    }

    public Task<IEnumerable<AttendanceHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IEnumerable<AttendanceHistory>)_records);
    }
}
