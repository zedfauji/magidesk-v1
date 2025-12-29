using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _context;

    public AttendanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceHistories
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AttendanceHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceHistories.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        await _context.AttendanceHistories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        _context.AttendanceHistories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AttendanceHistory entity, CancellationToken cancellationToken = default)
    {
        _context.AttendanceHistories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AttendanceHistory?> GetOpenByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Assuming UserId is mapped correctly. If it's a value object, EF Core might treat it as a property if configured, 
        // or we might need to compare the underlying value if it's a primitive column.
        // For now, assuming direct comparison works if conversion is set up in DbContext/Configuration.
        return await _context.AttendanceHistories
            .Where(a => a.UserId == userId && a.ClockOutTime == null)
            .OrderByDescending(a => a.ClockInTime)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
