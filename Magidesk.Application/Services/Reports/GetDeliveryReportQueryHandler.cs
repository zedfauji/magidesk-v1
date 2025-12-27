using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetDeliveryReportQueryHandler : IQueryHandler<GetDeliveryReportQuery, DeliveryReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetDeliveryReportQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeliveryReportDto> HandleAsync(GetDeliveryReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetDeliveryReportAsync(query.StartDate, query.EndDate, cancellationToken);
    }
}
