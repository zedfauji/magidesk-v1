using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

/// <summary>
/// Query to get server performance report for a specific date range.
/// Tracks sales volume, tip metrics, and performance comparisons.
/// </summary>
public record GetServerPerformanceReportQuery(
    DateTime StartDate, 
    DateTime EndDate
) : IQuery<ServerPerformanceReportDto>;