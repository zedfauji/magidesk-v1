using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
namespace Magidesk.Application.Services.Reports;

public class GetSalesDetailQueryHandler : IQueryHandler<GetSalesDetailQuery, SalesDetailReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetSalesDetailQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<SalesDetailReportDto> HandleAsync(GetSalesDetailQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetSalesDetailAsync(
            query.StartDate, 
            query.EndDate, 
            query.CategoryFilter, 
            query.GroupFilter, 
            query.ItemFilter, 
            cancellationToken);
    }
}
