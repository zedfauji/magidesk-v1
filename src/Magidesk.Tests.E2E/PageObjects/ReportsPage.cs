using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for reporting and audit operations.
/// </summary>
public sealed class ReportsPage : BasePage
{
    // Report generation controls
    private const string StartDatePickerId = "StartDatePicker";
    private const string EndDatePickerId = "EndDatePicker";
    private const string GenerateSalesReportButtonId = "GenerateSalesReportButton";
    private const string GenerateShiftReportButtonId = "GenerateShiftReportButton";
    private const string GenerateDrawerPullReportButtonId = "GenerateDrawerPullReportButton";
    private const string ViewAuditLogButtonId = "ViewAuditLogButton";
    
    // Filter controls
    private const string UsernameFilterTextBoxId = "UsernameFilterTextBox";
    private const string TransactionTypeFilterTextBoxId = "TransactionTypeFilterTextBox";
    private const string ApplyFilterButtonId = "ApplyFilterButton";
    
    // Export controls
    private const string FilenameTextBoxId = "FilenameTextBox";
    private const string ExportToPdfButtonId = "ExportToPdfButton";
    private const string ExportToExcelButtonId = "ExportToExcelButton";
    
    // Report data
    private const string ReportTotalTextBlockId = "ReportTotalTextBlock";
    private const string SessionIdTextBoxId = "SessionIdTextBox";
    private const string ShiftUsernameTextBoxId = "ShiftUsernameTextBox";
    private const string ShiftDatePickerId = "ShiftDatePicker";

    public ReportsPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Generates a sales report for a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    public void GenerateSalesReport(DateTime startDate, DateTime endDate)
    {
        EnterText(StartDatePickerId, startDate.ToString("yyyy-MM-dd"));
        EnterText(EndDatePickerId, endDate.ToString("yyyy-MM-dd"));
        ClickButton(GenerateSalesReportButtonId);
    }

    /// <summary>
    /// Generates a shift report for a specific user and date.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="date">The shift date.</param>
    public void GenerateShiftReport(string username, DateTime date)
    {
        EnterText(ShiftUsernameTextBoxId, username);
        EnterText(ShiftDatePickerId, date.ToString("yyyy-MM-dd"));
        ClickButton(GenerateShiftReportButtonId);
    }

    /// <summary>
    /// Generates a drawer pull report for a cash session.
    /// </summary>
    /// <param name="sessionId">The cash session ID.</param>
    public void GenerateDrawerPullReport(string sessionId)
    {
        EnterText(SessionIdTextBoxId, sessionId);
        ClickButton(GenerateDrawerPullReportButtonId);
    }

    /// <summary>
    /// Views the audit log for a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    public void ViewAuditLog(DateTime startDate, DateTime endDate)
    {
        EnterText(StartDatePickerId, startDate.ToString("yyyy-MM-dd"));
        EnterText(EndDatePickerId, endDate.ToString("yyyy-MM-dd"));
        ClickButton(ViewAuditLogButtonId);
    }

    /// <summary>
    /// Filters report by username.
    /// </summary>
    /// <param name="username">The username to filter by.</param>
    public void FilterByUser(string username)
    {
        EnterText(UsernameFilterTextBoxId, username);
        ClickButton(ApplyFilterButtonId);
    }

    /// <summary>
    /// Filters report by transaction type.
    /// </summary>
    /// <param name="transactionType">The transaction type to filter by.</param>
    public void FilterByTransactionType(string transactionType)
    {
        EnterText(TransactionTypeFilterTextBoxId, transactionType);
        ClickButton(ApplyFilterButtonId);
    }

    /// <summary>
    /// Exports the current report to PDF.
    /// </summary>
    /// <param name="filename">The output filename.</param>
    public void ExportToPdf(string filename)
    {
        EnterText(FilenameTextBoxId, filename);
        ClickButton(ExportToPdfButtonId);
    }

    /// <summary>
    /// Exports the current report to Excel.
    /// </summary>
    /// <param name="filename">The output filename.</param>
    public void ExportToExcel(string filename)
    {
        EnterText(FilenameTextBoxId, filename);
        ClickButton(ExportToExcelButtonId);
    }

    /// <summary>
    /// Gets the report total amount.
    /// </summary>
    /// <returns>The total as a decimal.</returns>
    public decimal GetReportTotal()
    {
        var totalText = GetText(ReportTotalTextBlockId);
        return decimal.Parse(totalText);
    }
}
