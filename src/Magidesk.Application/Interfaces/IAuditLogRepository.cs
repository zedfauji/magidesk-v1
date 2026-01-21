using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for audit log operations.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Gets audit logs with filtering and pagination.
    /// </summary>
    Task<(List<AuditLogDto> AuditLogs, int TotalCount)> GetAuditLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? userId = null,
        AuditEventType? eventType = null,
        string? entityType = null,
        string? searchText = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
