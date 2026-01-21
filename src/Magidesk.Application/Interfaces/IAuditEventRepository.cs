using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for AuditEvent entities.
/// </summary>
public interface IAuditEventRepository
{
    /// <summary>
    /// Adds a new audit event.
    /// </summary>
    Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit events for an entity.
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit events by correlation ID.
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
}

