using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IModifierGroupRepository
{
    Task<ModifierGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModifierGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ModifierGroup>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(ModifierGroup modifierGroup, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModifierGroup modifierGroup, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
