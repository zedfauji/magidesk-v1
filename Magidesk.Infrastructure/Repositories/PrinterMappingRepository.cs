using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

public class PrinterMappingRepository : IPrinterMappingRepository
{
    private readonly ApplicationDbContext _context;

    public PrinterMappingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PrinterMapping>> GetByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        return await _context.PrinterMappings
            .Where(m => m.TerminalId == terminalId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PrinterMapping mapping, CancellationToken cancellationToken = default)
    {
        await _context.PrinterMappings.AddAsync(mapping, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PrinterMapping mapping, CancellationToken cancellationToken = default)
    {
        _context.PrinterMappings.Update(mapping);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PrinterMapping mapping, CancellationToken cancellationToken = default)
    {
        _context.PrinterMappings.Remove(mapping);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
