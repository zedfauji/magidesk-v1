using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class InventoryCategoryRepository : IInventoryCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InventoryCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.InventoryCategories
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        return categories.AsReadOnly();
    }

    public async Task<InventoryCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCategories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(InventoryCategory category, CancellationToken cancellationToken = default)
    {
        await _context.InventoryCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryCategory category, CancellationToken cancellationToken = default)
    {
        _context.InventoryCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCategories
            .FirstOrDefaultAsync(x => EF.Functions.ILike(x.Name, name), cancellationToken);
    }

    public async Task<int> CountActiveChildCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryCategories
            .Where(x => x.IsActive && x.ParentCategoryId == parentCategoryId)
            .CountAsync(cancellationToken);
    }
}
