using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.TableSessions;

/// <summary>
/// Handler for getting all active table sessions.
/// </summary>
public class GetActiveSessionsQueryHandler : IQueryHandler<GetActiveSessionsQuery, IEnumerable<ActiveSessionDto>>
{
    private readonly ITableSessionRepository _sessionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ILogger<GetActiveSessionsQueryHandler> _logger;

    public GetActiveSessionsQueryHandler(
        ITableSessionRepository sessionRepository,
        ITableRepository tableRepository,
        ILogger<GetActiveSessionsQueryHandler> logger)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _tableRepository = tableRepository ?? throw new ArgumentNullException(nameof(tableRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ActiveSessionDto>> HandleAsync(
        GetActiveSessionsQuery query,
        CancellationToken cancellationToken = default)
    {
        // 1. Get all active sessions
        var sessions = await _sessionRepository.GetActiveSessionsAsync();

        // 2. Get all tables for lookup
        var tables = await _tableRepository.GetAllAsync();
        var tableDict = tables.ToDictionary(t => t.Id);

        // 3. Map to DTOs
        var result = sessions.Select(session =>
        {
            var table = tableDict.TryGetValue(session.TableId, out var t) ? t : null;

            return new ActiveSessionDto
            {
                SessionId = session.Id,
                TableId = session.TableId,
                TableNumber = table?.TableNumber ?? 0,
                TableName = table != null ? $"Table {table.TableNumber}" : "Unknown",
                CustomerId = session.CustomerId,
                CustomerName = null, // TODO: Get customer name from customer repository
                StartTime = session.StartTime,
                Status = session.Status,
                HourlyRate = session.HourlyRate,
                PausedDuration = session.TotalPausedDuration
            };
        })
        .OrderBy(s => s.StartTime) // Sort by start time (oldest first)
        .ToList();

        _logger.LogInformation("Retrieved {Count} active sessions", result.Count);

        return result;
    }
}
