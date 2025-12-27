using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public class GetJournalReportQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public string? EntityType { get; } // Optional filter
    public Guid? UserId { get; } // Optional filter

    public GetJournalReportQuery(DateTime startDate, DateTime endDate, string? entityType = null, Guid? userId = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        EntityType = entityType;
        UserId = userId;
    }
}
