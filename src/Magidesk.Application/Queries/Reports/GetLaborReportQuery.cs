using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public class GetLaborReportQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public GetLaborReportQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
