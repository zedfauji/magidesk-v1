using System;
using System.Collections.Generic;
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

    public async Task<System.Collections.Generic.IEnumerable<KitchenOrder>> GetActiveOrdersAsync()
    {
        return await _context.KitchenOrders
            .Include(ko => ko.Items)
            .Where(ko => ko.Status != Magidesk.Domain.Enumerations.KitchenStatus.Done 
                         && ko.Status != Magidesk.Domain.Enumerations.KitchenStatus.Void)
            .OrderBy(ko => ko.Timestamp) // Oldest first
            .ToListAsync();
    }

    public async Task<System.Collections.Generic.IEnumerable<KitchenOrder>> GetCompletedOrdersAsync(int limit = 50)
    {
        return await _context.KitchenOrders
            .Include(ko => ko.Items)
            .Where(ko => ko.Status == Magidesk.Domain.Enumerations.KitchenStatus.Done)
            .OrderByDescending(ko => ko.Timestamp) // Newest first
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> IsTicketItemRoutedAsync(Guid ticketItemId)
    {
        // Check if any kitchen order item exists for this ticket item ID
        // And ensure the parent order is not Voided (if we want to allow re-sending voided items)
        return await _context.Set<Domain.Entities.KitchenOrderItem>()
            .AnyAsync(koi => koi.TicketItemId == ticketItemId);
            
        // Note: Currently we don't join with KitchenOrder to check status because
        // we ideally shouldn't duplicate even if voided without explicit action?
        // But for safety against duplicates, ANY existence is enough to say "it was routed".
        // If a user *wants* to re-send, they might need a specific "Re-send" command that bypasses this,
        // or we rely on the fact that Voiding usually means "Don't Make", not "Make Again".
    }
}
