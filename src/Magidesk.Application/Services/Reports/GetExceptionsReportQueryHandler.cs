using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetExceptionsReportQueryHandler : IQueryHandler<GetExceptionsReportQuery, ExceptionsReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetExceptionsReportQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExceptionsReportDto> HandleAsync(GetExceptionsReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetExceptionsReportAsync(query.StartDate, query.EndDate, cancellationToken);
    }
}
