using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IMenuGroupRepository
{
    Task<MenuGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuGroup>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuGroup>> GetVisibleAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuGroup>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    
    // CRUD operations
    Task AddAsync(MenuGroup group, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuGroup group, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuGroup group, CancellationToken cancellationToken = default);
}
