using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

/// <summary>
/// Query to get daily sales report for a specific date.
/// </summary>
public record GetDailySalesReportQuery(DateTime Date) : IQuery<DailySalesReportDto>;