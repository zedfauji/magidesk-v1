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

public class MenuModifierRepository : IMenuModifierRepository
{
    private readonly ApplicationDbContext _context;

    public MenuModifierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuModifier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
         return await _context.MenuModifiers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
    
    public async Task<IEnumerable<MenuModifier>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.MenuModifiers
            .AsNoTracking()
            .Where(m => m.ModifierGroupId == groupId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuModifier modifier, CancellationToken cancellationToken = default)
    {
        await _context.MenuModifiers.AddAsync(modifier, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuModifier modifier, CancellationToken cancellationToken = default)
    {
        _context.MenuModifiers.Update(modifier);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var modifier = await _context.MenuModifiers.FindAsync(new object[] { id }, cancellationToken);
        if (modifier != null)
        {
            _context.MenuModifiers.Remove(modifier);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
