using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for exporting reports to various formats (PDF, Excel).
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// Exports report data to PDF format with templates and branding.
    /// </summary>
    /// <typeparam name="T">Type of report data</typeparam>
    /// <param name="reportData">The report data to export</param>
    /// <param name="templateName">Name of the PDF template to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file as byte array</returns>
    Task<byte[]> ExportToPdfAsync<T>(T reportData, string templateName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports report data to Excel format with formulas and formatting.
    /// </summary>
    /// <typeparam name="T">Type of report data</typeparam>
    /// <param name="reportData">The report data to export</param>
    /// <param name="templateName">Name of the Excel template to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> ExportToExcelAsync<T>(T reportData, string templateName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports multiple reports in batch operation.
    /// </summary>
    /// <param name="requests">Collection of export requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch export result with individual file results</returns>
    Task<BatchExportResult> BatchExportAsync(IEnumerable<ExportRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates export format and parameters.
    /// </summary>
    /// <param name="format">Export format (PDF, Excel)</param>
    /// <param name="templateName">Template name to validate</param>
    /// <param name="reportType">Type of report being exported</param>
    /// <returns>Validation result</returns>
    Task<ExportValidationResult> ValidateExportFormatAsync(ExportFormat format, string templateName, Type reportType);
}

/// <summary>
/// Represents an export request for batch operations.
/// </summary>
public record ExportRequest(
    object ReportData,
    Type ReportType,
    ExportFormat Format,
    string TemplateName,
    string FileName
);

/// <summary>
/// Result of batch export operation.
/// </summary>
public record BatchExportResult(
    bool IsSuccess,
    IEnumerable<ExportFileResult> Files,
    IEnumerable<string> Errors
);

/// <summary>
/// Individual file result from export operation.
/// </summary>
public record ExportFileResult(
    string FileName,
    byte[] FileData,
    ExportFormat Format,
    bool IsSuccess,
    string? ErrorMessage = null
);

/// <summary>
/// Export format validation result.
/// </summary>
public record ExportValidationResult(
    bool IsValid,
    IEnumerable<string> ValidationErrors
);

/// <summary>
/// Supported export formats.
/// </summary>
public enum ExportFormat
{
    Pdf,
    Excel
}