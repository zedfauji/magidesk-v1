using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries.Reports;

public class GetDeliveryReportQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public GetDeliveryReportQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
