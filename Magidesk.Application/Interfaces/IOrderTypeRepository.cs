using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for OrderType entity.
/// </summary>
public interface IOrderTypeRepository
{
    /// <summary>
    /// Gets an order type by ID.
    /// </summary>
    Task<OrderType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order type by name.
    /// </summary>
    Task<OrderType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active order types.
    /// </summary>
    Task<IEnumerable<OrderType>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all order types.
    /// </summary>
    Task<IEnumerable<OrderType>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new order type.
    /// </summary>
    Task AddAsync(OrderType orderType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order type.
    /// </summary>
    Task UpdateAsync(OrderType orderType, CancellationToken cancellationToken = default);
}

