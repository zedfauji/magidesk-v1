using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for Table entity.
/// </summary>
public interface ITableRepository
{
    /// <summary>
    /// Gets a table by ID.
    /// </summary>
    Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a table by table number.
    /// </summary>
    Task<Table?> GetByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tables for a floor.
    /// </summary>
    Task<IEnumerable<Table>> GetByFloorIdAsync(Guid? floorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tables with a specific status.
    /// </summary>
    Task<IEnumerable<Table>> GetByStatusAsync(TableStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available tables.
    /// </summary>
    Task<IEnumerable<Table>> GetAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active tables.
    /// </summary>
    Task<IEnumerable<Table>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tables.
    /// </summary>
    Task<IEnumerable<Table>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new table.
    /// </summary>
    Task AddAsync(Table table, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing table.
    /// </summary>
    Task UpdateAsync(Table table, CancellationToken cancellationToken = default);
}

