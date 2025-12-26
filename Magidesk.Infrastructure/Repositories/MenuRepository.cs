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

public class MenuRepository : IMenuRepository
{
    private readonly ApplicationDbContext _context;

    public MenuRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .Include(m => m.ModifierGroups)
                .ThenInclude(mmg => mmg.ModifierGroup)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .Where(m => m.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MenuItem>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .Where(m => m.GroupId == groupId && m.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<MenuModifier?> GetModifierByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MenuModifiers
            .FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<ComboDefinition?> GetComboDefinitionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ComboDefinition>()
            .Include(c => c.Groups)
                .ThenInclude(g => g.Items) // Ensure Items are loaded if we mapped them
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        await _context.MenuItems.AddAsync(menuItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
    {
        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
