using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public class GetProductivityReportQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public Guid? UserId { get; } // Optional filter

    public GetProductivityReportQuery(DateTime startDate, DateTime endDate, Guid? userId = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        UserId = userId;
    }
}
