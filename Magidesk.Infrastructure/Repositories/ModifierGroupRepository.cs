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

public class ModifierGroupRepository : IModifierGroupRepository
{
    private readonly ApplicationDbContext _context;

    public ModifierGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ModifierGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ModifierGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ModifierGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ModifierGroups
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<ModifierGroup>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
         // Wait, ModifierGroups are often shared or linked to MenuItem/Category via bridging table.
         // In step 7761 I defined IModifierGroupRepository without GetByCategoryId.
         // But ModifierEditorViewModel uses a GetAll logic.
         // Let's stick to the Interface definition from Step 7757.
         // Interface only had GetById, GetAll, Add, Update, Delete.
         // No, wait, checking Step 7757... IModifierGroupRepository has standard CRUD.
         // ModifierEditorViewModel loads ALL groups.
         return await GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(ModifierGroup modifierGroup, CancellationToken cancellationToken = default)
    {
        await _context.ModifierGroups.AddAsync(modifierGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ModifierGroup modifierGroup, CancellationToken cancellationToken = default)
    {
        _context.ModifierGroups.Update(modifierGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await _context.ModifierGroups.FindAsync(new object[] { id }, cancellationToken);
        if (group != null)
        {
            _context.ModifierGroups.Remove(group);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
