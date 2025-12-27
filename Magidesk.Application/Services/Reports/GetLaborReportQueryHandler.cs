using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetLaborReportQueryHandler : IQueryHandler<GetLaborReportQuery, LaborReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetLaborReportQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<LaborReportDto> HandleAsync(GetLaborReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetLaborReportAsync(query.StartDate, query.EndDate, cancellationToken);
    }
}
