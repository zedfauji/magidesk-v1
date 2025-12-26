using System;

using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Queries;

public class GetSalesBalanceQuery
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public GetSalesBalanceQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
