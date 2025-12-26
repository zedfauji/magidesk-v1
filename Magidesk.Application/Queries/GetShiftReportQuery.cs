using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get a shift report with tickets and cash sessions.
/// </summary>
public class GetShiftReportQuery
{
    public Guid ShiftId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Result of getting shift report.
/// </summary>
public class GetShiftReportResult
{
    public ShiftReportDto Report { get; set; } = null!;
}

