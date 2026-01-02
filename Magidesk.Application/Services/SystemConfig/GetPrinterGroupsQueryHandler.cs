using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.SystemConfig;

namespace Magidesk.Application.Services.SystemConfig;

public class GetPrinterGroupsQueryHandler : IQueryHandler<GetPrinterGroupsQuery, GetPrinterGroupsResult>
{
    private readonly IPrinterGroupRepository _repository;

    public GetPrinterGroupsQueryHandler(IPrinterGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetPrinterGroupsResult> HandleAsync(GetPrinterGroupsQuery query, CancellationToken cancellationToken = default)
    {
        var groups = await _repository.GetAllAsync(cancellationToken);
        return new GetPrinterGroupsResult(groups.Select(g => new PrinterGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            PrinterType = g.Type
        }).ToList());
    }
}
