using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for Shift entity.
/// </summary>
public interface IShiftRepository
{
    /// <summary>
    /// Gets a shift by ID.
    /// </summary>
    Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active shifts.
    /// </summary>
    Task<IEnumerable<Shift>> GetActiveShiftsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all shifts.
    /// </summary>
    Task<IEnumerable<Shift>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current shift based on current time.
    /// </summary>
    Task<Shift?> GetCurrentShiftAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new shift.
    /// </summary>
    Task AddAsync(Shift shift, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing shift.
    /// </summary>
    Task UpdateAsync(Shift shift, CancellationToken cancellationToken = default);
}

