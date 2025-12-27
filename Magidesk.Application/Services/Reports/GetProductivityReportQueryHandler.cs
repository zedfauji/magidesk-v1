using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetProductivityReportQueryHandler : IQueryHandler<GetProductivityReportQuery, ProductivityReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetProductivityReportQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductivityReportDto> HandleAsync(GetProductivityReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetServerProductivityAsync(query.StartDate, query.EndDate, query.UserId, cancellationToken);
    }
}
