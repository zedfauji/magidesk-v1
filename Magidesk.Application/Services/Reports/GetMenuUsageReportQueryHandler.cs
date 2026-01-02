using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetMenuUsageReportQueryHandler : IQueryHandler<GetMenuUsageReportQuery, MenuUsageReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetMenuUsageReportQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<MenuUsageReportDto> HandleAsync(GetMenuUsageReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetMenuUsageReportAsync(
            query.StartDate, 
            query.EndDate, 
            query.CategoryFilter, 
            query.OrderTypeFilter, 
            cancellationToken);
    }
}
