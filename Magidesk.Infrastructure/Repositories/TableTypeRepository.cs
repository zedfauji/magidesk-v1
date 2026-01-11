using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TableType entity.
/// </summary>
public class TableTypeRepository : ITableTypeRepository
{
    private readonly ApplicationDbContext _context;

    public TableTypeRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TableType?> GetByIdAsync(Guid id)
    {
        return await _context.TableTypes
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TableType>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.TableTypes.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<TableType?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        // Use .ToLower() for case-insensitive comparison (PostgreSQL compatible)
        var normalizedName = name.Trim().ToLower();
        return await _context.TableTypes
            .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedName);
    }

    public async Task<TableType> AddAsync(TableType tableType)
    {
        if (tableType == null)
        {
            throw new ArgumentNullException(nameof(tableType));
        }

        _context.TableTypes.Add(tableType);
        await _context.SaveChangesAsync();
        return tableType;
    }

    public async Task UpdateAsync(TableType tableType)
    {
        if (tableType == null)
        {
            throw new ArgumentNullException(nameof(tableType));
        }

        _context.TableTypes.Update(tableType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tableType = await GetByIdAsync(id);
        if (tableType != null)
        {
            _context.TableTypes.Remove(tableType);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string name, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var normalizedName = name.Trim().ToLower();
        var query = _context.TableTypes
            .Where(t => t.Name.ToLower() == normalizedName);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
