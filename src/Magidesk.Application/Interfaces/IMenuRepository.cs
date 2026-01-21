using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IMenuRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuItem>> GetByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<MenuModifier?> GetModifierByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ComboDefinition?> GetComboDefinitionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Core CRUD
    Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuItem menuItem, CancellationToken cancellationToken = default);
}
