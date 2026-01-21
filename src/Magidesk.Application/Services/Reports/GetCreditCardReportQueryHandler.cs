using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetCreditCardReportQueryHandler : IQueryHandler<GetCreditCardReportQuery, CreditCardReportDto>
{
    private readonly ISalesReportRepository _salesReportRepository;

    public GetCreditCardReportQueryHandler(ISalesReportRepository salesReportRepository)
    {
        _salesReportRepository = salesReportRepository;
    }

    public async Task<CreditCardReportDto> HandleAsync(GetCreditCardReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _salesReportRepository.GetCreditCardReportAsync(
            query.StartDate, 
            query.EndDate, 
            query.CardTypeFilter, 
            query.TransactionTypeFilter, 
            cancellationToken);
    }
}
