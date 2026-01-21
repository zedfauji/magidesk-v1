using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetTipReportQueryHandler : IQueryHandler<GetTipReportQuery, TipReportDto>
{
    private readonly ISalesReportRepository _reportRepository;

    public GetTipReportQueryHandler(ISalesReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<TipReportDto> HandleAsync(GetTipReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _reportRepository.GetTipReportAsync(query.StartDate, query.EndDate, query.UserIdFilter, cancellationToken);
    }
}
