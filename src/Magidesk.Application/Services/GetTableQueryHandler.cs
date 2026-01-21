using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetTableQuery.
/// </summary>
public class GetTableQueryHandler : IQueryHandler<GetTableQuery, GetTableResult>
{
    private readonly ITableRepository _tableRepository;

    public GetTableQueryHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<GetTableResult> HandleAsync(GetTableQuery query, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(query.TableId, cancellationToken);
        
        return new GetTableResult
        {
            Table = table != null ? MapToDto(table) : null
        };
    }

    private static TableDto MapToDto(Domain.Entities.Table table)
    {
        return new TableDto
        {
            Id = table.Id,
            TableNumber = table.TableNumber,
            FloorId = table.FloorId,
            Capacity = table.Capacity,
            X = table.X,
            Y = table.Y,
            Status = table.Status,
            CurrentTicketId = table.CurrentTicketId,
            IsActive = table.IsActive
        };
    }
}

