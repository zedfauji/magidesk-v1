using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.SystemConfig;

namespace Magidesk.Application.Services.SystemConfig;

public class GetPrinterMappingsQueryHandler : IQueryHandler<GetPrinterMappingsQuery, GetPrinterMappingsResult>
{
    private readonly IPrinterMappingRepository _repository;
    private readonly IPrinterGroupRepository _groupRepository;

    public GetPrinterMappingsQueryHandler(IPrinterMappingRepository repository, IPrinterGroupRepository groupRepository)
    {
        _repository = repository;
        _groupRepository = groupRepository;
    }

    public async Task<GetPrinterMappingsResult> HandleAsync(GetPrinterMappingsQuery query, CancellationToken cancellationToken = default)
    {
        var mappings = await _repository.GetByTerminalIdAsync(query.TerminalId, cancellationToken);
        var groups = await _groupRepository.GetAllAsync(cancellationToken);
        var groupsDict = groups.ToDictionary(g => g.Id, g => g.Name);

        return new GetPrinterMappingsResult(mappings.Select(m => new PrinterMappingDto
        {
            Id = m.Id,
            TerminalId = m.TerminalId,
            PrinterGroupId = m.PrinterGroupId,
            PhysicalPrinterName = m.PhysicalPrinterName,
            PrinterGroupName = groupsDict.GetValueOrDefault(m.PrinterGroupId, "Unknown Group")
        }).ToList());
    }
}
