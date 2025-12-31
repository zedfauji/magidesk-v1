using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for CashSession aggregate root.
/// </summary>
public class CashSessionRepository : ICashSessionRepository
{
    private readonly ApplicationDbContext _context;

    public CashSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CashSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Include(cs => cs.Payments)
            .Include(cs => cs.Payouts)
            .Include(cs => cs.CashDrops)
            .Include(cs => cs.DrawerBleeds)
            .FirstOrDefaultAsync(cs => cs.Id == id, cancellationToken);
    }

    public async Task<CashSession?> GetOpenSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Include(cs => cs.Payments)
            .Include(cs => cs.Payouts)
            .Include(cs => cs.CashDrops)
            .Include(cs => cs.DrawerBleeds)
            .FirstOrDefaultAsync(
                cs => cs.UserId == new UserId(userId) && cs.Status == CashSessionStatus.Open,
                cancellationToken);
    }

    public async Task<CashSession?> GetOpenSessionByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Include(cs => cs.Payments)
            .Include(cs => cs.Payouts)
            .Include(cs => cs.CashDrops)
            .Include(cs => cs.DrawerBleeds)
            .FirstOrDefaultAsync(
                cs => cs.TerminalId == terminalId && cs.Status == CashSessionStatus.Open,
                cancellationToken);
    }

    public async Task<IEnumerable<CashSession>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default)
    {
        return await _context.CashSessions
            .Where(cs => cs.ShiftId == shiftId)
            .Include(cs => cs.Payments)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CashSession cashSession, CancellationToken cancellationToken = default)
    {
        await _context.CashSessions.AddAsync(cashSession, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CashSession cashSession, CancellationToken cancellationToken = default)
    {
        _context.CashSessions.Update(cashSession);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Domain.Exceptions.ConcurrencyException(
                $"Cash session {cashSession.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }
}

