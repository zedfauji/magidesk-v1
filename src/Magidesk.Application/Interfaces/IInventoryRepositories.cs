using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vendor>> GetAllAsync();
    Task AddAsync(Vendor vendor);
    Task UpdateAsync(Vendor vendor);
    Task DeleteAsync(Guid id);
}

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid id);
    Task<IEnumerable<PurchaseOrder>> GetAllAsync();
    Task AddAsync(PurchaseOrder po);
    Task UpdateAsync(PurchaseOrder po);
}

public interface IInventoryAdjustmentRepository
{
    Task<IEnumerable<InventoryAdjustment>> GetByItemIdAsync(Guid itemId);
    Task AddAsync(InventoryAdjustment adjustment);
}
