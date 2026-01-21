using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetCashOutReportQueryHandler : IQueryHandler<GetCashOutReportQuery, CashOutReportDto>
{
    private readonly ISalesReportRepository _reportRepository;

    public GetCashOutReportQueryHandler(ISalesReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<CashOutReportDto> HandleAsync(GetCashOutReportQuery query, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetCashOutReportAsync(query.StartDate, query.EndDate, query.UserIdFilter, cancellationToken);
    }
}
