using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

/// <summary>
/// Query to get time-based revenue analytics report for a specific date range.
/// Separates time charges from product sales and provides detailed breakdown.
/// </summary>
public record GetTimeRevenueReportQuery(
    DateTime StartDate, 
    DateTime EndDate
) : IQuery<TimeRevenueReportDto>;