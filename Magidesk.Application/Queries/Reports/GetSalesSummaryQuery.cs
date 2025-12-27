using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries;

public class GetSalesSummaryQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public bool IncludeGroups { get; }

    public GetSalesSummaryQuery(DateTime startDate, DateTime endDate, bool includeGroups = true)
    {
        StartDate = startDate;
        EndDate = endDate;
        IncludeGroups = includeGroups;
    }
}
