using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Queries.Reports;

/// <summary>
/// Query to get shift summary report for a specific shift and date range.
/// </summary>
public record GetShiftSummaryReportQuery(
    Guid ShiftId,
    DateTime StartDate,
    DateTime EndDate
) : IQuery<ShiftSummaryReportDto>;