using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryAuditEventRepository : IAuditEventRepository
{
    public List<AuditEvent> Events { get; } = new();

    public Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        Events.Add(auditEvent);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<AuditEvent>> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        var result = Events.Where(e => e.EntityType == entityType && e.EntityId == entityId).ToList();
        return Task.FromResult<IEnumerable<AuditEvent>>(result);
    }

    public Task<IEnumerable<AuditEvent>> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        var result = Events.Where(e => e.CorrelationId == correlationId).ToList();
        return Task.FromResult<IEnumerable<AuditEvent>>(result);
    }
}
