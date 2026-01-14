using Magidesk.Application.DTOs;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to retrieve audit logs with filtering and pagination.
/// </summary>
public record GetAuditLogsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    Guid? UserId = null,
    AuditEventType? EventType = null,
    string? EntityType = null,
    string? SearchText = null,
    int PageNumber = 1,
    int PageSize = 50
);

/// <summary>
/// Result containing paginated audit logs.
/// </summary>
public class GetAuditLogsResult
{
    public List<AuditLogDto> AuditLogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
