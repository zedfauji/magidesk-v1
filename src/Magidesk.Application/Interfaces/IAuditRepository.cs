using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for immutable audit record storage.
/// </summary>
/// <typeparam name="T">Type of audit record</typeparam>
public interface IAuditRepository<T> where T : class
{
    /// <summary>
    /// Adds an immutable audit record.
    /// </summary>
    /// <param name="auditRecord">Audit record to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task AddAuditRecordAsync(T auditRecord, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit records by entity ID within a date range.
    /// </summary>
    /// <param name="entityId">ID of the entity being audited</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit records</returns>
    Task<IEnumerable<T>> GetAuditRecordsByEntityIdAsync(
        Guid entityId, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit records by user ID within a date range.
    /// </summary>
    /// <param name="userId">ID of the user who performed the action</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit records</returns>
    Task<IEnumerable<T>> GetAuditRecordsByUserIdAsync(
        Guid userId, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all audit records within a date range.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit records</returns>
    Task<IEnumerable<T>> GetAuditRecordsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit records by action type within a date range.
    /// </summary>
    /// <param name="actionType">Type of action</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of audit records</returns>
    Task<IEnumerable<T>> GetAuditRecordsByActionTypeAsync(
        string actionType, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the integrity of audit records (ensures no tampering).
    /// </summary>
    /// <param name="entityId">ID of the entity to verify</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if integrity is maintained, false otherwise</returns>
    Task<bool> VerifyAuditIntegrityAsync(Guid entityId, CancellationToken cancellationToken = default);
}