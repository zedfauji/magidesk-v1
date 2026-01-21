using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

public class PrintTemplateRepository : IPrintTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public PrintTemplateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrintTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PrintTemplates.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<PrintTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PrintTemplates.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PrintTemplate entity, CancellationToken cancellationToken = default)
    {
        await _context.PrintTemplates.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PrintTemplate entity, CancellationToken cancellationToken = default)
    {
        _context.PrintTemplates.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PrintTemplate entity, CancellationToken cancellationToken = default)
    {
        _context.PrintTemplates.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PrintTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.PrintTemplates
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<List<PrintTemplate>> GetSystemTemplatesAsync(CancellationToken cancellationToken)
    {
        return await _context.PrintTemplates
            .Where(t => t.IsSystem)
            .ToListAsync(cancellationToken);
    }
}
