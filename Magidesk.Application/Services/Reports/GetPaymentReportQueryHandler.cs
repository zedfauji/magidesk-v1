using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetPaymentReportQueryHandler : IQueryHandler<GetPaymentReportQuery, PaymentReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetPaymentReportQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<PaymentReportDto> HandleAsync(GetPaymentReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetPaymentReportAsync(
            query.StartDate, 
            query.EndDate, 
            query.TerminalFilter, 
            cancellationToken);
    }
}
