using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems.FindAsync(new object[] { id }, cancellationToken);
        if (item != null)
        {
            item.Deactivate(); // Soft delete preferred usually, looking at Interface...
            // Interface said "DeleteAsync". BaseEntity usually implies soft delete?
            // InventoryItem has IsActive. I should set IsActive = false;
            // But if I want true delete:
            // _context.InventoryItems.Remove(item);

            // Let's do Soft Delete to be safe with IsActive
            item.Deactivate();
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<(IReadOnlyList<InventoryItem> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        InventoryFilterType filter,
        Guid? categoryId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        IQueryable<InventoryItem> query = _context.InventoryItems.Where(x => x.IsActive);

        if (categoryId != null)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, $"%{searchTerm}%") ||
                (x.SkuCode != null && EF.Functions.ILike(x.SkuCode, $"%{searchTerm}%")));
        }

        query = filter switch
        {
            InventoryFilterType.LowStock => query.Where(x => x.StockQuantity <= x.ReorderPoint && x.StockQuantity > 0),
            InventoryFilterType.OutOfStock => query.Where(x => x.StockQuantity == 0),
            InventoryFilterType.RecentlyAdded => query.Where(x => x.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-30)),
            InventoryFilterType.None => query,
            _ => query
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items.AsReadOnly(), totalCount);
    }

    public async Task<InventoryItem?> GetBySkuCodeAsync(string skuCode, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.SkuCode != null && EF.Functions.ILike(i.SkuCode, skuCode), cancellationToken);
    }

    public async Task<int> CountActiveItemsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems
            .Where(i => i.IsActive && i.CategoryId == categoryId)
            .CountAsync(cancellationToken);
    }
}
