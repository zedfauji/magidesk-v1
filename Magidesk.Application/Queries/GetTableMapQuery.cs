using MediatR;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

public class GetTableMapQuery : IRequest<GetTableMapResult>
{
    public Guid? FloorId { get; set; }
}

public class GetTableMapResult
{
    public IEnumerable<TableDto> Tables { get; set; } = new List<TableDto>();
}
