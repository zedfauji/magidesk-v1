using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IMenuModifierRepository
{
    Task<MenuModifier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuModifier>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task AddAsync(MenuModifier modifier, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuModifier modifier, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
