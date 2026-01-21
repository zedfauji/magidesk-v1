using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly ApplicationDbContext _context;

    public VendorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Vendor?> GetByIdAsync(Guid id)
    {
        return await _context.Vendors.FindAsync(id);
    }

    public async Task<IEnumerable<Vendor>> GetAllAsync()
    {
        return await _context.Vendors
            .Where(v => v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Vendor vendor)
    {
        await _context.Vendors.AddAsync(vendor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor != null)
        {
            vendor.Deactivate();
            await _context.SaveChangesAsync();
        }
    }
}

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Vendor)
            .Include(po => po.Lines)
                .ThenInclude(l => l.InventoryItem)
            .FirstOrDefaultAsync(po => po.Id == id);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
    {
        return await _context.PurchaseOrders
            .Include(po => po.Vendor)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseOrder po)
    {
        await _context.PurchaseOrders.AddAsync(po);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseOrder po)
    {
        _context.PurchaseOrders.Update(po);
        await _context.SaveChangesAsync();
    }
}

public class InventoryAdjustmentRepository : IInventoryAdjustmentRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryAdjustmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryAdjustment>> GetByItemIdAsync(Guid itemId)
    {
        return await _context.InventoryAdjustments
            .Where(a => a.InventoryItemId == itemId)
            .OrderByDescending(a => a.AdjustedAt)
            .ToListAsync();
    }

    public async Task AddAsync(InventoryAdjustment adjustment)
    {
        await _context.InventoryAdjustments.AddAsync(adjustment);
        await _context.SaveChangesAsync();
    }
}
