using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries;

/// <summary>
/// Handler for retrieving audit logs with filtering and pagination.
/// </summary>
public class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, GetAuditLogsResult>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
    }

    public async Task<GetAuditLogsResult> HandleAsync(GetAuditLogsQuery query, CancellationToken cancellationToken = default)
    {
        var (auditLogs, totalCount) = await _auditLogRepository.GetAuditLogsAsync(
            startDate: query.StartDate,
            endDate: query.EndDate,
            userId: query.UserId,
            eventType: query.EventType,
            entityType: query.EntityType,
            searchText: query.SearchText,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken
        );

        return new GetAuditLogsResult
        {
            AuditLogs = auditLogs,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
