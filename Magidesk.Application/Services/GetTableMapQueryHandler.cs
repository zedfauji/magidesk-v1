using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

public class GetTableMapQueryHandler : IQueryHandler<GetTableMapQuery, GetTableMapResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly ITableSessionRepository _sessionRepository;

    public GetTableMapQueryHandler(
        ITableRepository tableRepository,
        ITableSessionRepository sessionRepository)
    {
        _tableRepository = tableRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<GetTableMapResult> HandleAsync(GetTableMapQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Entities.Table> tables;

        if (query.FloorId.HasValue)
        {
            var allTables = await _tableRepository.GetByFloorIdAsync(query.FloorId, cancellationToken);
            tables = allTables.Where(t => t.IsActive);
        }
        else
        {
            tables = await _tableRepository.GetActiveAsync(cancellationToken);
        }

        // Get all active sessions for efficient lookup
        var activeSessions = await _sessionRepository.GetActiveSessionsAsync();
        var sessionsByTableId = activeSessions.ToDictionary(s => s.TableId);

        var tableDtos = tables.Select(t =>
        {
            var dto = new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                FloorId = t.FloorId,
                Capacity = t.Capacity,
                X = t.X,
                Y = t.Y,
                Width = t.Width,
                Height = t.Height,
                Status = t.Status,
                CurrentTicketId = t.CurrentTicketId,
                IsActive = t.IsActive,
                Shape = t.Shape
            };

            // Add session data if table has an active session
            if (sessionsByTableId.TryGetValue(t.Id, out var session))
            {
                dto.SessionId = session.Id;
                dto.SessionStartTime = session.StartTime;
                dto.SessionStatus = session.Status;
                dto.SessionHourlyRate = session.HourlyRate;
                dto.SessionPausedDuration = session.TotalPausedDuration;
            }

            return dto;
        }).ToList();

        return new GetTableMapResult
        {
            Tables = tableDtos
        };
    }
}
