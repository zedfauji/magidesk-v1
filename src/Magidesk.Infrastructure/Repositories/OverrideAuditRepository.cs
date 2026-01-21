using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for OverrideAuditEntry entities.
/// Provides immutable audit trail storage for manager override operations.
/// </summary>
public class OverrideAuditRepository : IOverrideAuditRepository
{
    private readonly ApplicationDbContext _context;

    public OverrideAuditRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new override audit entry.
    /// </summary>
    public async Task AddAsync(OverrideAuditEntry auditEntry, CancellationToken cancellationToken = default)
    {
        if (auditEntry == null)
        {
            throw new ArgumentNullException(nameof(auditEntry));
        }

        // Since OverrideAuditEntry is a value object, we need to store it in a way that EF Core can handle
        // For now, we'll create a simple entity to store the audit data
        var auditEntity = new OverrideAuditEntity
        {
            Id = auditEntry.Id,
            SessionId = auditEntry.SessionId,
            OverrideType = auditEntry.OverrideType,
            OriginalValue = auditEntry.OriginalValue,
            NewValue = auditEntry.NewValue,
            Reason = auditEntry.Reason,
            ManagerId = auditEntry.ManagerId,
            Timestamp = auditEntry.Timestamp
        };

        _context.OverrideAuditEntries.Add(auditEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets override audit entries for a specific session.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.OverrideAuditEntries
            .Where(e => e.SessionId == sessionId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToValueObject);
    }

    /// <summary>
    /// Gets override audit entries for a specific manager.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetByManagerIdAsync(Guid managerId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var entities = await _context.OverrideAuditEntries
            .Where(e => e.ManagerId == managerId)
            .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToValueObject);
    }

    /// <summary>
    /// Gets override audit entries within a date range.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var entities = await _context.OverrideAuditEntries
            .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToValueObject);
    }

    /// <summary>
    /// Gets override audit entries by override type within a date range.
    /// </summary>
    public async Task<IEnumerable<OverrideAuditEntry>> GetByOverrideTypeAsync(OverrideType overrideType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var entities = await _context.OverrideAuditEntries
            .Where(e => e.OverrideType == overrideType)
            .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToValueObject);
    }

    /// <summary>
    /// Maps the entity to the value object.
    /// </summary>
    private static OverrideAuditEntry MapToValueObject(OverrideAuditEntity entity)
    {
        return new OverrideAuditEntry(
            Id: entity.Id,
            SessionId: entity.SessionId,
            OverrideType: entity.OverrideType,
            OriginalValue: entity.OriginalValue,
            NewValue: entity.NewValue,
            Reason: entity.Reason,
            ManagerId: entity.ManagerId,
            Timestamp: entity.Timestamp
        );
    }
}

/// <summary>
/// Entity class for storing override audit entries in the database.
/// This is needed because EF Core cannot directly store value objects as entities.
/// </summary>
public class OverrideAuditEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public OverrideType OverrideType { get; set; }
    public string OriginalValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public Guid ManagerId { get; set; }
    public DateTime Timestamp { get; set; }
}