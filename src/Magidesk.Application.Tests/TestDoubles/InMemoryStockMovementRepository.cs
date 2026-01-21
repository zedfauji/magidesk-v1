using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryStockMovementRepository : IRepository<StockMovement>
{
    private readonly Dictionary<Guid, StockMovement> _movements = new();

    public Task<StockMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _movements.TryGetValue(id, out var movement);
        return Task.FromResult(movement);
    }

    public Task<IEnumerable<StockMovement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<StockMovement>>(_movements.Values);
    }

    public Task AddAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        _movements[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        _movements[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        _movements.Remove(entity.Id);
        return Task.CompletedTask;
    }
}