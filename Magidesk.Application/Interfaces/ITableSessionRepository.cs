using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for TableSession entity.
/// </summary>
public interface ITableSessionRepository
{
    /// <summary>
    /// Gets a table session by ID.
    /// </summary>
    Task<TableSession?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets the active session for a specific table.
    /// </summary>
    Task<TableSession?> GetActiveSessionByTableIdAsync(Guid tableId);

    /// <summary>
    /// Gets the active session for a specific ticket.
    /// </summary>
    Task<TableSession?> GetActiveSessionByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sessions.
    /// </summary>
    Task<IEnumerable<TableSession>> GetActiveSessionsAsync();

    /// <summary>
    /// Gets all sessions for a specific table.
    /// </summary>
    Task<IEnumerable<TableSession>> GetSessionsByTableIdAsync(Guid tableId);

    /// <summary>
    /// Gets all sessions for a specific customer.
    /// </summary>
    Task<IEnumerable<TableSession>> GetSessionsByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Gets sessions by status.
    /// </summary>
    Task<IEnumerable<TableSession>> GetSessionsByStatusAsync(TableSessionStatus status);

    /// <summary>
    /// Adds a new table session.
    /// </summary>
    Task<TableSession> AddAsync(TableSession session);

    /// <summary>
    /// Updates an existing table session.
    /// </summary>
    Task UpdateAsync(TableSession session);

    /// <summary>
    /// Deletes a table session.
    /// </summary>
    Task DeleteAsync(Guid id);
}
