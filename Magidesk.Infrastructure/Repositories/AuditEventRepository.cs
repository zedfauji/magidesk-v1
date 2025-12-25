using Microsoft.EntityFrameworkCore;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AuditEvent entities.
/// </summary>
public class AuditEventRepository : IAuditEventRepository
{
    private readonly ApplicationDbContext _context;

    public AuditEventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        await _context.AuditEvents.AddAsync(auditEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditEvent>> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditEvents
            .Where(ae => ae.EntityType == entityType && ae.EntityId == entityId)
            .OrderByDescending(ae => ae.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditEvent>> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditEvents
            .Where(ae => ae.CorrelationId == correlationId)
            .OrderBy(ae => ae.Timestamp)
            .ToListAsync(cancellationToken);
    }
}

