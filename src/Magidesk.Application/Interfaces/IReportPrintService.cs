using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service interface for printing various reports.
/// Handles formatting and printing of management reports.
/// </summary>
public interface IReportPrintService
{
    /// <summary>
    /// Prints a drawer pull report.
    /// </summary>
    /// <param name="report">The drawer pull report to print.</param>
    /// <param name="userId">The user initiating the print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintDrawerPullReportAsync(DrawerPullReportDto report, Guid? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints a cash reconciliation report.
    /// </summary>
    /// <param name="sessionId">The cash session ID.</param>
    /// <param name="expectedAmount">Expected cash amount.</param>
    /// <param name="actualAmount">Actual cash amount.</param>
    /// <param name="variance">Variance amount.</param>
    /// <param name="userId">The user initiating the print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintCashReconciliationReportAsync(Guid sessionId, decimal expectedAmount, decimal actualAmount, decimal variance, Guid? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints a sales summary report.
    /// </summary>
    /// <param name="startDate">Start date for the report.</param>
    /// <param name="endDate">End date for the report.</param>
    /// <param name="userId">The user initiating the print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintSalesSummaryReportAsync(DateTime startDate, DateTime endDate, Guid? userId = null, CancellationToken cancellationToken = default);
}