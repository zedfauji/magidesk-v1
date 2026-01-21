using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetAvailableTablesQuery.
/// </summary>
public class GetAvailableTablesQueryHandler : IQueryHandler<GetAvailableTablesQuery, GetAvailableTablesResult>
{
    private readonly ITableRepository _tableRepository;

    public GetAvailableTablesQueryHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<GetAvailableTablesResult> HandleAsync(GetAvailableTablesQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Entities.Table> tables;

        if (query.FloorId.HasValue)
        {
            var allTables = await _tableRepository.GetByFloorIdAsync(query.FloorId, cancellationToken);
            tables = allTables.Where(t => t.IsAvailable());
        }
        else
        {
            tables = await _tableRepository.GetAvailableAsync(cancellationToken);
        }

        var tableDtos = tables.Select(t => new TableDto
        {
            Id = t.Id,
            TableNumber = t.TableNumber,
            FloorId = t.FloorId,
            Capacity = t.Capacity,
            X = t.X,
            Y = t.Y,
            Width = t.Width,
            Height = t.Height,
            Shape = t.Shape,
            Status = t.Status,
            CurrentTicketId = t.CurrentTicketId,
            IsActive = t.IsActive
        }).ToList();

        return new GetAvailableTablesResult
        {
            Tables = tableDtos
        };
    }
}

