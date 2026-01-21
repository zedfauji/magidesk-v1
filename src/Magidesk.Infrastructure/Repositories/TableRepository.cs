using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Table entity.
/// </summary>
public class TableRepository : ITableRepository
{
    private readonly ApplicationDbContext _context;

    public TableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Table?> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.CurrentTicketId == ticketId, cancellationToken);
    }

    public async Task<Table?> GetByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.TableNumber == tableNumber, cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByFloorIdAsync(Guid? floorId, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.FloorId == floorId)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.Status == status)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.IsActive && t.Status == TableStatus.Available)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .Where(t => t.IsActive)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tables
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Table table, CancellationToken cancellationToken = default)
    {
        await _context.Tables.AddAsync(table, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Table table, CancellationToken cancellationToken = default)
    {
        _context.Tables.Update(table);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Domain.Exceptions.ConcurrencyException(
                $"Table {table.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }
}

