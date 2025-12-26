using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class KitchenOrderRepository : IKitchenOrderRepository
{
    private readonly ApplicationDbContext _context;

    public KitchenOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(KitchenOrder kitchenOrder)
    {
        await _context.KitchenOrders.AddAsync(kitchenOrder);
        await _context.SaveChangesAsync();
    }

    public async Task<KitchenOrder?> GetByIdAsync(Guid id)
    {
        return await _context.KitchenOrders
            .Include(ko => ko.Items)
            .FirstOrDefaultAsync(ko => ko.Id == id);
    }

    public async Task UpdateAsync(KitchenOrder kitchenOrder)
    {
        _context.KitchenOrders.Update(kitchenOrder);
        await _context.SaveChangesAsync();
    }
}
