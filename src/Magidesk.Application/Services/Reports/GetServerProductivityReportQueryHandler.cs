using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetServerProductivityReportQueryHandler : IQueryHandler<GetServerProductivityReportQuery, ServerProductivityReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetServerProductivityReportQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<ServerProductivityReportDto> HandleAsync(GetServerProductivityReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetServerProductivityReportAsync(
            query.StartDate, 
            query.EndDate, 
            query.UserIdFilter, 
            cancellationToken);
    }
}
