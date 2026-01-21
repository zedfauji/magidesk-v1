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

public class MenuCategoryRepository : IMenuCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public MenuCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<MenuCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuCategory>> GetVisibleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuCategories
            .AsNoTracking()
            .Where(c => c.IsActive && c.IsVisible)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        await _context.MenuCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        _context.MenuCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuCategory category, CancellationToken cancellationToken = default)
    {
        _context.MenuCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
