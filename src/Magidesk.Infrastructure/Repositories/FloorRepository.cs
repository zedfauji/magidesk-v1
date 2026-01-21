using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Magidesk.Infrastructure.Data;
using Magidesk.Application.Interfaces;

namespace Magidesk.Infrastructure.Repositories;

public class FloorRepository : IFloorRepository
{
    private readonly ApplicationDbContext _context;

    public FloorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Floor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Floors
            .Include(f => f.TableLayouts)
            .ThenInclude(l => l.Tables)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Floor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Floors
            .Include(f => f.TableLayouts)
            .Where(f => f.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Floor?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Floors
            .Include(f => f.TableLayouts)
            .FirstOrDefaultAsync(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task AddAsync(Floor entity, CancellationToken cancellationToken = default)
    {
        await _context.Floors.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(Floor entity, CancellationToken cancellationToken = default)
    {
        _context.Floors.Update(entity);
    }

    public async Task DeleteAsync(Floor entity, CancellationToken cancellationToken = default)
    {
        _context.Floors.Remove(entity);
    }
}

