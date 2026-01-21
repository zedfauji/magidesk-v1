using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for reporting and tracking errors for management dashboard.
/// </summary>
public interface IErrorReportingService
{
    /// <summary>
    /// Reports an error to the management dashboard.
    /// </summary>
    Task ReportErrorAsync(ErrorReportDto errorReport);

    /// <summary>
    /// Gets recent errors for dashboard display.
    /// </summary>
    Task<IEnumerable<ErrorReportDto>> GetRecentErrorsAsync(int count = 50);

    /// <summary>
    /// Gets errors by category for analysis.
    /// </summary>
    Task<IEnumerable<ErrorReportDto>> GetErrorsByCategoryAsync(ErrorCategory category, DateTime? since = null);

    /// <summary>
    /// Marks an error as resolved.
    /// </summary>
    Task ResolveErrorAsync(Guid errorId, string resolvedBy, string? resolution = null);

    /// <summary>
    /// Gets error statistics for dashboard.
    /// </summary>
    Task<ErrorStatisticsDto> GetErrorStatisticsAsync(DateTime? since = null);
}

/// <summary>
/// Error statistics for management dashboard.
/// </summary>
public class ErrorStatisticsDto
{
    public int TotalErrors { get; set; }
    public int CriticalErrors { get; set; }
    public int UnresolvedErrors { get; set; }
    public Dictionary<ErrorCategory, int> ErrorsByCategory { get; set; } = new();
    public Dictionary<string, int> ErrorsByTerminal { get; set; } = new();
    public TimeSpan AverageResolutionTime { get; set; }
}