using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for CashSession aggregate root.
/// </summary>
public interface ICashSessionRepository
{
    /// <summary>
    /// Gets a cash session by ID.
    /// </summary>
    Task<CashSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the open cash session for a user.
    /// </summary>
    Task<CashSession?> GetOpenSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the open cash session for a terminal.
    /// </summary>
    Task<CashSession?> GetOpenSessionByTerminalIdAsync(Guid terminalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all cash sessions for a shift.
    /// </summary>
    Task<IEnumerable<CashSession>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new cash session.
    /// </summary>
    Task AddAsync(CashSession cashSession, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cash session.
    /// </summary>
    Task UpdateAsync(CashSession cashSession, CancellationToken cancellationToken = default);
}

