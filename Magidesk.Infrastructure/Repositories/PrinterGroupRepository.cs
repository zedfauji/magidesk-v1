using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class PrinterGroupRepository : IPrinterGroupRepository
{
    private readonly ApplicationDbContext _context;

    public PrinterGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PrinterGroup>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrinterGroups
            .Include(g => g.ReceiptTemplate)
            .Include(g => g.KitchenTemplate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrinterGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrinterGroups
            .Include(g => g.ReceiptTemplate)
            .Include(g => g.KitchenTemplate)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddAsync(PrinterGroup group, CancellationToken cancellationToken = default)
    {
        await _context.PrinterGroups.AddAsync(group, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PrinterGroup group, CancellationToken cancellationToken = default)
    {
        _context.PrinterGroups.Update(group);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PrinterGroup group, CancellationToken cancellationToken = default)
    {
        _context.PrinterGroups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
