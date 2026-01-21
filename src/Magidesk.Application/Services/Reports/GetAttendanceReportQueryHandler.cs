using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.Reports;

namespace Magidesk.Application.Services.Reports;

public class GetAttendanceReportQueryHandler : IQueryHandler<GetAttendanceReportQuery, AttendanceReportDto>
{
    private readonly ISalesReportRepository _reportRepository;

    public GetAttendanceReportQueryHandler(ISalesReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<AttendanceReportDto> HandleAsync(GetAttendanceReportQuery query, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetAttendanceReportAsync(query.StartDate, query.EndDate, query.UserIdFilter, cancellationToken);
    }
}
