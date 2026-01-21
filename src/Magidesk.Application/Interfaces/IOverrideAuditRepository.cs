using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for OverrideAuditEntry entities.
/// Provides immutable audit trail storage for manager override operations.
/// </summary>
public interface IOverrideAuditRepository
{
    /// <summary>
    /// Adds a new override audit entry.
    /// </summary>
    /// <param name="auditEntry">The audit entry to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(OverrideAuditEntry auditEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets override audit entries for a specific session.
    /// </summary>
    /// <param name="sessionId">ID of the session</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of override audit entries for the session</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets override audit entries for a specific manager.
    /// </summary>
    /// <param name="managerId">ID of the manager</param>
    /// <param name="fromDate">Start date for the query</param>
    /// <param name="toDate">End date for the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of override audit entries for the manager</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetByManagerIdAsync(Guid managerId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets override audit entries within a date range.
    /// </summary>
    /// <param name="fromDate">Start date for the query</param>
    /// <param name="toDate">End date for the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of override audit entries within the date range</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets override audit entries by override type within a date range.
    /// </summary>
    /// <param name="overrideType">Type of override to filter by</param>
    /// <param name="fromDate">Start date for the query</param>
    /// <param name="toDate">End date for the query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of override audit entries of the specified type</returns>
    Task<IEnumerable<OverrideAuditEntry>> GetByOverrideTypeAsync(Domain.Enumerations.OverrideType overrideType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}