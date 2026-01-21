using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Enumerations;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for audit log operations.
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<(List<AuditLogDto> AuditLogs, int TotalCount)> GetAuditLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? userId = null,
        AuditEventType? eventType = null,
        string? entityType = null,
        string? searchText = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var auditQuery = _context.AuditEvents.AsNoTracking();

        // Apply filters
        if (startDate.HasValue)
        {
            auditQuery = auditQuery.Where(a => a.Timestamp >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            auditQuery = auditQuery.Where(a => a.Timestamp <= endDate.Value);
        }

        if (userId.HasValue)
        {
            auditQuery = auditQuery.Where(a => a.UserId == userId.Value);
        }

        if (eventType.HasValue)
        {
            auditQuery = auditQuery.Where(a => a.EventType == eventType.Value);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            auditQuery = auditQuery.Where(a => a.EntityType == entityType);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var searchLower = searchText.ToLower();
            auditQuery = auditQuery.Where(a =>
                a.Description.ToLower().Contains(searchLower) ||
                a.EntityType.ToLower().Contains(searchLower) ||
                a.AfterState.ToLower().Contains(searchLower) ||
                (a.BeforeState != null && a.BeforeState.ToLower().Contains(searchLower))
            );
        }

        // Get total count before pagination
        var totalCount = await auditQuery.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var auditLogs = await auditQuery
            .OrderByDescending(a => a.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                EventType = a.EventType,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                UserId = a.UserId,
                UserName = string.Empty, // Will be populated separately
                Timestamp = a.Timestamp,
                BeforeState = a.BeforeState,
                AfterState = a.AfterState,
                Description = a.Description,
                CorrelationId = a.CorrelationId
            })
            .ToListAsync(cancellationToken);

        // Populate user names
        var userIds = auditLogs.Select(a => a.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Username })
            .ToListAsync(cancellationToken);

        var userDict = users.ToDictionary(u => u.Id, u => u.Username);

        foreach (var log in auditLogs)
        {
            if (userDict.TryGetValue(log.UserId, out var username))
            {
                log.UserName = username;
            }
        }

        return (auditLogs, totalCount);
    }
}
