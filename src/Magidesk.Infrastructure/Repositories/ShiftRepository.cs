using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Shift entity.
/// </summary>
public class ShiftRepository : IShiftRepository
{
    private readonly ApplicationDbContext _context;

    public ShiftRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetActiveShiftsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .Where(s => s.IsActive)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Shift>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Shift?> GetCurrentShiftAsync(CancellationToken cancellationToken = default)
    {
        var activeShifts = await GetActiveShiftsAsync(cancellationToken);
        return Shift.GetCurrentShift(activeShifts);
    }

    public async Task AddAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        await _context.Shifts.AddAsync(shift, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Update(shift);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Domain.Exceptions.ConcurrencyException(
                $"Shift {shift.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }
}

