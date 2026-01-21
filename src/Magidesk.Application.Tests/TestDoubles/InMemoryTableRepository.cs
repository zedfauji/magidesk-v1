using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

public class InMemoryTableRepository : ITableRepository
{
    private readonly List<Table> _tables = new();

    public Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = _tables.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(table);
    }

    public Task<IEnumerable<Table>> GetByFloorIdAsync(Guid? floorId, CancellationToken cancellationToken = default)
    {
        var tables = _tables.Where(t => t.FloorId == floorId).ToList();
        return Task.FromResult<IEnumerable<Table>>(tables);
    }

    public Task<Table?> GetByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default)
    {
        var table = _tables.FirstOrDefault(t => t.TableNumber == tableNumber);
        return Task.FromResult(table);
    }

    public Task<IEnumerable<Table>> GetByStatusAsync(Domain.Enumerations.TableStatus status, CancellationToken cancellationToken = default)
    {
        var tables = _tables.Where(t => t.Status == status).ToList();
        return Task.FromResult<IEnumerable<Table>>(tables);
    }

    public Task<IEnumerable<Table>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Table>>(_tables.ToList());
    }

    public Task AddAsync(Table table, CancellationToken cancellationToken = default)
    {
        _tables.Add(table);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Table table, CancellationToken cancellationToken = default)
    {
        var existing = _tables.FirstOrDefault(t => t.Id == table.Id);
        if (existing != null)
        {
            _tables.Remove(existing);
            _tables.Add(table);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var table = _tables.FirstOrDefault(t => t.Id == id);
        if (table != null)
        {
            _tables.Remove(table);
        }
        return Task.CompletedTask;
    }

    public Task<Table?> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var table = _tables.FirstOrDefault(t => t.CurrentTicketId == ticketId);
        return Task.FromResult(table);
    }

    public Task<IEnumerable<Table>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        var tables = _tables.Where(t => t.CurrentTicketId == null).ToList();
        return Task.FromResult<IEnumerable<Table>>(tables);
    }

    public Task<IEnumerable<Table>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var tables = _tables.Where(t => t.CurrentTicketId != null).ToList();
        return Task.FromResult<IEnumerable<Table>>(tables);
    }
}
