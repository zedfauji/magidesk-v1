using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

public class GetExceptionsReportQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public GetExceptionsReportQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
