using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IKitchenOrderRepository
{
    Task AddAsync(KitchenOrder kitchenOrder);
    Task<KitchenOrder?> GetByIdAsync(Guid id);
    Task UpdateAsync(KitchenOrder kitchenOrder);
    Task<IEnumerable<KitchenOrder>> GetActiveOrdersAsync();
    Task<IEnumerable<KitchenOrder>> GetCompletedOrdersAsync(int limit = 50);
}
