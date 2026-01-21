using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetTableQuery that returns TableDto? directly.
/// This is an adapter for OrderPageViewModel which expects TableDto? instead of GetTableResult.
/// </summary>
public class GetTableDtoQueryHandler : IQueryHandler<GetTableQuery, TableDto?>
{
    private readonly ITableRepository _tableRepository;

    public GetTableDtoQueryHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableDto?> HandleAsync(GetTableQuery query, CancellationToken cancellationToken = default)
    {
        var table = await _tableRepository.GetByIdAsync(query.TableId, cancellationToken);
        
        if (table == null)
        {
            return null;
        }

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
