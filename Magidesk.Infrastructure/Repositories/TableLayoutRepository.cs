using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class TableLayoutRepository : ITableLayoutRepository
{
    private readonly ApplicationDbContext _context;

    public TableLayoutRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TableLayout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TableLayouts
            .Include(tl => tl.Tables)
            .FirstOrDefaultAsync(tl => tl.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TableLayout>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TableLayouts
            .Include(tl => tl.Tables)
            .Where(tl => tl.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<TableLayoutDto?> GetLayoutByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var layout = await _context.TableLayouts
            .Include(tl => tl.Tables)
            .FirstOrDefaultAsync(tl => tl.Name.Equals(name, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (layout == null) return null;

        return new TableLayoutDto
        {
            Id = layout.Id,
            Name = layout.Name,
            FloorId = layout.FloorId,
            Tables = layout.Tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Status = t.Status,
                IsActive = t.IsActive
            }).ToList(),
            CreatedAt = layout.CreatedAt,
            UpdatedAt = layout.UpdatedAt,
            IsActive = layout.IsActive,
            Version = layout.Version
        };
    }

    public async Task<List<TableLayoutDto>> GetLayoutsByFloorAsync(Guid floorId, CancellationToken cancellationToken = default)
    {
        var layouts = await _context.TableLayouts
            .Include(tl => tl.Tables)
            .Where(tl => tl.FloorId == floorId && tl.IsActive)
            .ToListAsync(cancellationToken);

        return layouts.Select(layout => new TableLayoutDto
        {
            Id = layout.Id,
            Name = layout.Name,
            FloorId = layout.FloorId,
            Tables = layout.Tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Status = t.Status,
                IsActive = t.IsActive
            }).ToList(),
            CreatedAt = layout.CreatedAt,
            UpdatedAt = layout.UpdatedAt,
            IsActive = layout.IsActive,
            Version = layout.Version
        }).ToList();
    }

    public async Task<TableLayoutDto?> GetLayoutWithTablesAsync(Guid layoutId, CancellationToken cancellationToken = default)
    {
        var layout = await _context.TableLayouts
            .Include(tl => tl.Tables)
            .FirstOrDefaultAsync(tl => tl.Id == layoutId, cancellationToken);

        if (layout == null) return null;

        return new TableLayoutDto
        {
            Id = layout.Id,
            Name = layout.Name,
            FloorId = layout.FloorId,
            Tables = layout.Tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Status = t.Status,
                IsActive = t.IsActive
            }).ToList(),
            CreatedAt = layout.CreatedAt,
            UpdatedAt = layout.UpdatedAt,
            IsActive = layout.IsActive,
            Version = layout.Version
        };
    }

    public async Task<bool> IsLayoutNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.TableLayouts
            .Where(tl => tl.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (excludeId.HasValue)
        {
            query = query.Where(tl => tl.Id != excludeId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(TableLayout entity, CancellationToken cancellationToken = default)
    {
        await _context.TableLayouts.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(TableLayout entity, CancellationToken cancellationToken = default)
    {
        _context.TableLayouts.Update(entity);
    }

    public async Task DeleteAsync(TableLayout entity, CancellationToken cancellationToken = default)
    {
        _context.TableLayouts.Remove(entity);
    }
}
