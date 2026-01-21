using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IMenuCategoryRepository
{
    Task<MenuCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuCategory>> GetVisibleAsync(CancellationToken cancellationToken = default);
    
    // CRUD operations
    Task AddAsync(MenuCategory category, CancellationToken cancellationToken = default);
    Task UpdateAsync(MenuCategory category, CancellationToken cancellationToken = default);
    Task DeleteAsync(MenuCategory category, CancellationToken cancellationToken = default);
}
