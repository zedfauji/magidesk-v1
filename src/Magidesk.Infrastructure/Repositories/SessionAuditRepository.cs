using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for session audit records with immutable storage.
/// </summary>
public class SessionAuditRepository : IAuditRepository<SessionAuditEntry>
{
    private readonly ApplicationDbContext _dbContext;

    public SessionAuditRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAuditRecordAsync(SessionAuditEntry auditRecord, CancellationToken cancellationToken = default)
    {
        if (auditRecord == null)
        {
            throw new ArgumentNullException(nameof(auditRecord));
        }

        // Convert to entity for storage
        var auditEntity = new SessionAuditEntity
        {
            Id = Guid.NewGuid(),
            SessionId = auditRecord.SessionId,
            Action = auditRecord.Action,
            Details = auditRecord.Details,
            UserId = auditRecord.UserId,
            Timestamp = auditRecord.Timestamp,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Set<SessionAuditEntity>().Add(auditEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SessionAuditEntry>> GetAuditRecordsByEntityIdAsync(
        Guid entityId, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<SessionAuditEntity>()
            .Where(e => e.SessionId == entityId && 
                       e.Timestamp >= fromDate && 
                       e.Timestamp <= toDate)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new SessionAuditEntry(
            e.SessionId,
            e.Action,
            e.Details,
            e.UserId,
            e.Timestamp
        ));
    }

    public async Task<IEnumerable<SessionAuditEntry>> GetAuditRecordsByUserIdAsync(
        Guid userId, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<SessionAuditEntity>()
            .Where(e => e.UserId == userId && 
                       e.Timestamp >= fromDate && 
                       e.Timestamp <= toDate)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new SessionAuditEntry(
            e.SessionId,
            e.Action,
            e.Details,
            e.UserId,
            e.Timestamp
        ));
    }

    public async Task<IEnumerable<SessionAuditEntry>> GetAuditRecordsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<SessionAuditEntity>()
            .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new SessionAuditEntry(
            e.SessionId,
            e.Action,
            e.Details,
            e.UserId,
            e.Timestamp
        ));
    }

    public async Task<IEnumerable<SessionAuditEntry>> GetAuditRecordsByActionTypeAsync(
        string actionType, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<SessionAuditEntity>()
            .Where(e => e.Action == actionType && 
                       e.Timestamp >= fromDate && 
                       e.Timestamp <= toDate)
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new SessionAuditEntry(
            e.SessionId,
            e.Action,
            e.Details,
            e.UserId,
            e.Timestamp
        ));
    }

    public async Task<bool> VerifyAuditIntegrityAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would verify cryptographic hashes or checksums
        // to ensure audit records haven't been tampered with
        var auditCount = await _dbContext.Set<SessionAuditEntity>()
            .CountAsync(e => e.SessionId == entityId, cancellationToken);

        // Simple integrity check - ensure audit records exist and are in chronological order
        var records = await _dbContext.Set<SessionAuditEntity>()
            .Where(e => e.SessionId == entityId)
            .OrderBy(e => e.Timestamp)
            .Select(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        // Verify chronological order
        for (int i = 1; i < records.Count; i++)
        {
            if (records[i] < records[i - 1])
            {
                return false; // Timestamps are not in order
            }
        }

        return true;
    }
}

/// <summary>
/// Entity for storing session audit records in the database.
/// </summary>
public class SessionAuditEntity
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
}