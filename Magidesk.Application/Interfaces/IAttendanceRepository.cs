using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface IAttendanceRepository : IRepository<AttendanceHistory>
{
    Task<AttendanceHistory?> GetOpenByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
}
