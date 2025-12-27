using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetJournalReportQueryHandler : IQueryHandler<GetJournalReportQuery, JournalReportDto>
{
    private readonly ISalesReportRepository _repository;

    public GetJournalReportQueryHandler(ISalesReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<JournalReportDto> HandleAsync(GetJournalReportQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetJournalReportAsync(query.StartDate, query.EndDate, query.EntityType, query.UserId, cancellationToken);
    }
}
