using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;

namespace Magidesk.Application.Services.Reports;

public class GetSalesBalanceQueryHandler : IQueryHandler<GetSalesBalanceQuery, SalesBalanceReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetSalesBalanceQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<SalesBalanceReportDto> HandleAsync(GetSalesBalanceQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetSalesBalanceAsync(query.StartDate, query.EndDate, cancellationToken);
    }
}
