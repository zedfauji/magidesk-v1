using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TableSession entity.
/// </summary>
public class TableSessionRepository : ITableSessionRepository
{
    private readonly ApplicationDbContext _context;

    public TableSessionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TableSession?> GetByIdAsync(Guid id)
    {
        return await _context.TableSessions
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<TableSession?> GetActiveSessionByTableIdAsync(Guid tableId)
    {
        return await _context.TableSessions
            .Where(s => s.TableId == tableId)
            .Where(s => s.Status == TableSessionStatus.Active || s.Status == TableSessionStatus.Paused)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync();
    }

    public async Task<TableSession?> GetActiveSessionByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _context.TableSessions
            .Where(s => s.TicketId == ticketId)
            .Where(s => s.Status == TableSessionStatus.Active || s.Status == TableSessionStatus.Paused)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TableSession>> GetActiveSessionsAsync()
    {
        return await _context.TableSessions
            .Where(s => s.Status == TableSessionStatus.Active || s.Status == TableSessionStatus.Paused)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<TableSession>> GetSessionsByTableIdAsync(Guid tableId)
    {
        return await _context.TableSessions
            .Where(s => s.TableId == tableId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<TableSession>> GetSessionsByCustomerIdAsync(Guid customerId)
    {
        return await _context.TableSessions
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<TableSession>> GetSessionsByStatusAsync(TableSessionStatus status)
    {
        return await _context.TableSessions
            .Where(s => s.Status == status)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<TableSession> AddAsync(TableSession session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        _context.TableSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(TableSession session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        _context.TableSessions.Update(session);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Domain.Exceptions.ConcurrencyException(
                $"TableSession {session.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await GetByIdAsync(id);
        if (session != null)
        {
            _context.TableSessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }
}
