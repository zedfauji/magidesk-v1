using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IKitchenOrderRepository
{
    Task AddAsync(KitchenOrder kitchenOrder);
    Task<KitchenOrder?> GetByIdAsync(Guid id);
    Task UpdateAsync(KitchenOrder kitchenOrder);
    // Add other methods as needed: e.g., GetActiveOrdersAsync
}
