using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services.Reports;

public class GetSalesSummaryQueryHandler : IQueryHandler<GetSalesSummaryQuery, SalesSummaryReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetSalesSummaryQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<SalesSummaryReportDto> HandleAsync(GetSalesSummaryQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetSalesSummaryAsync(query.StartDate, query.EndDate, query.IncludeGroups, cancellationToken);
    }
}
