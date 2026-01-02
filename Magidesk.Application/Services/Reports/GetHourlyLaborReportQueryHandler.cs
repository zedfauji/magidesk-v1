using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetHourlyLaborReportQueryHandler : IQueryHandler<GetHourlyLaborReportQuery, HourlyLaborReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetHourlyLaborReportQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<HourlyLaborReportDto> HandleAsync(GetHourlyLaborReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetHourlyLaborReportAsync(
            query.StartDate, 
            query.EndDate, 
            query.EmployeeIdFilter, 
            cancellationToken);
    }
}
