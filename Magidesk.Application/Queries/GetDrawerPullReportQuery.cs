using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get drawer pull report for a cash session.
/// </summary>
public class GetDrawerPullReportQuery
{
    public Guid CashSessionId { get; set; }
}

/// <summary>
/// Result of getting drawer pull report.
/// </summary>
public class GetDrawerPullReportResult
{
    public DrawerPullReportDto Report { get; set; } = null!;
}

