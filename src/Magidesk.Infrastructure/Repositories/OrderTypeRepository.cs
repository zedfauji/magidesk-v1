using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for OrderType entity.
/// </summary>
public class OrderTypeRepository : IOrderTypeRepository
{
    private readonly ApplicationDbContext _context;

    public OrderTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OrderTypes
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<OrderType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.OrderTypes
            .FirstOrDefaultAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<OrderType>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrderTypes
            .Where(o => o.IsActive)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrderType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrderTypes
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OrderType orderType, CancellationToken cancellationToken = default)
    {
        await _context.OrderTypes.AddAsync(orderType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrderType orderType, CancellationToken cancellationToken = default)
    {
        _context.OrderTypes.Update(orderType);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Domain.Exceptions.ConcurrencyException(
                $"Order type {orderType.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }
}

