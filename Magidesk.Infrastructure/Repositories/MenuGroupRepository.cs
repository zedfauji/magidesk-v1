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

public class MenuGroupRepository : IMenuGroupRepository
{
    private readonly ApplicationDbContext _context;

    public MenuGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuGroups
            .AsNoTracking()
            .Include(g => g.Category)
            .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<MenuGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuGroups
            .AsNoTracking()
            .Include(g => g.Category)
            .Where(g => g.IsActive)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuGroup>> GetVisibleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuGroups
            .AsNoTracking()
            .Include(g => g.Category)
            .Where(g => g.IsActive && g.IsVisible)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuGroup>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.MenuGroups
            .AsNoTracking()
            .Include(g => g.Category)
            .Where(g => g.CategoryId == categoryId && g.IsActive && g.IsVisible)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuGroup group, CancellationToken cancellationToken = default)
    {
        await _context.MenuGroups.AddAsync(group, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuGroup group, CancellationToken cancellationToken = default)
    {
        _context.MenuGroups.Update(group);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuGroup group, CancellationToken cancellationToken = default)
    {
        _context.MenuGroups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
