using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

/// <summary>
/// Query to get table utilization report for a specific date range.
/// </summary>
public record GetTableUtilizationReportQuery(
    DateTime StartDate, 
    DateTime EndDate
) : IQuery<TableUtilizationReportDto>;