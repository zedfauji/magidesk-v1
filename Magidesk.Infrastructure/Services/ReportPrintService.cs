using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service for printing various management reports.
/// Formats reports and sends them to printers using raw print service.
/// </summary>
public class ReportPrintService : IReportPrintService
{
    private readonly IRawPrintService _rawPrintService;
    private readonly ITerminalContext _terminalContext;
    private readonly ILogger<ReportPrintService> _logger;

    public ReportPrintService(
        IRawPrintService rawPrintService,
        ITerminalContext terminalContext,
        ILogger<ReportPrintService> logger)
    {
        _rawPrintService = rawPrintService;
        _terminalContext = terminalContext;
        _logger = logger;
    }

    public async Task<bool> PrintDrawerPullReportAsync(DrawerPullReportDto report, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var reportContent = FormatDrawerPullReport(report);
            var printerName = await GetDefaultPrinterAsync();
            
            if (string.IsNullOrEmpty(printerName))
            {
                _logger.LogWarning("No default printer configured for drawer pull report");
                return false;
            }

            await _rawPrintService.PrintRawStringAsync(printerName, reportContent);
            
            _logger.LogInformation("Drawer pull report printed successfully for session {SessionId} by user {UserId}", 
                report.CashSessionId, userId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print drawer pull report for session {SessionId}", report.CashSessionId);
            return false;
        }
    }

    public async Task<bool> PrintCashReconciliationReportAsync(Guid sessionId, decimal expectedAmount, decimal actualAmount, decimal variance, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var reportContent = FormatCashReconciliationReport(sessionId, expectedAmount, actualAmount, variance);
            var printerName = await GetDefaultPrinterAsync();
            
            if (string.IsNullOrEmpty(printerName))
            {
                _logger.LogWarning("No default printer configured for cash reconciliation report");
                return false;
            }

            await _rawPrintService.PrintRawStringAsync(printerName, reportContent);
            
            _logger.LogInformation("Cash reconciliation report printed successfully for session {SessionId} by user {UserId}", 
                sessionId, userId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print cash reconciliation report for session {SessionId}", sessionId);
            return false;
        }
    }

    public async Task<bool> PrintSalesSummaryReportAsync(DateTime startDate, DateTime endDate, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var reportContent = FormatSalesSummaryReport(startDate, endDate);
            var printerName = await GetDefaultPrinterAsync();
            
            if (string.IsNullOrEmpty(printerName))
            {
                _logger.LogWarning("No default printer configured for sales summary report");
                return false;
            }

            await _rawPrintService.PrintRawStringAsync(printerName, reportContent);
            
            _logger.LogInformation("Sales summary report printed successfully for period {StartDate} to {EndDate} by user {UserId}", 
                startDate, endDate, userId);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print sales summary report for period {StartDate} to {EndDate}", startDate, endDate);
            return false;
        }
    }

    private string FormatDrawerPullReport(DrawerPullReportDto report)
    {
        var sb = new StringBuilder();
        
        // ESC/POS commands for formatting
        sb.AppendLine("\x1B\x40"); // Initialize printer
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("DRAWER PULL REPORT");
        sb.AppendLine("==================");
        sb.AppendLine();
        
        sb.AppendLine("\x1B\x61\x00"); // Left align
        sb.AppendLine($"Session ID: {report.CashSessionId}");
        sb.AppendLine($"Terminal: {_terminalContext.TerminalId}");
        sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        
        sb.AppendLine("CASH SUMMARY");
        sb.AppendLine("------------");
        sb.AppendLine($"Opening Balance:     {report.OpeningBalance:C}");
        sb.AppendLine($"Cash Receipts:       {report.TotalCashReceipts:C}");
        sb.AppendLine($"Cash Drops:          {report.TotalCashDrops:C}");
        sb.AppendLine($"Drawer Bleeds:       {report.TotalDrawerBleeds:C}");
        sb.AppendLine($"Expected Cash:       {report.ExpectedCash:C}");
        sb.AppendLine();
        
        if (report.ActualCash.HasValue)
        {
            sb.AppendLine($"Actual Cash:         {report.ActualCash.Value:C}");
            sb.AppendLine($"Difference:          {report.Difference ?? 0:C}");
            sb.AppendLine();
        }
        
        sb.AppendLine("SALES SUMMARY");
        sb.AppendLine("-------------");
        sb.AppendLine($"Net Sales:           {report.NetSales:C}");
        sb.AppendLine($"Tax Collected:       {report.Tax:C}");
        sb.AppendLine($"Tips:                {report.TotalTips:C}");
        sb.AppendLine($"Cash Refunds:        {report.TotalCashRefunds:C}");
        sb.AppendLine($"Payouts:             {report.TotalPayouts:C}");
        sb.AppendLine();
        
        if (report.CashDrops?.Any() == true)
        {
            sb.AppendLine("CASH DROP DETAILS");
            sb.AppendLine("-----------------");
            foreach (var drop in report.CashDrops)
            {
                sb.AppendLine($"{drop.ProcessedAt:HH:mm} - {drop.Amount:C} - {drop.Reason}");
            }
            sb.AppendLine();
        }
        
        if (report.DrawerBleeds?.Any() == true)
        {
            sb.AppendLine("DRAWER BLEED DETAILS");
            sb.AppendLine("--------------------");
            foreach (var bleed in report.DrawerBleeds)
            {
                sb.AppendLine($"{bleed.ProcessedAt:HH:mm} - {bleed.Amount:C} - {bleed.Reason}");
            }
            sb.AppendLine();
        }
        
        if (report.Payouts?.Any() == true)
        {
            sb.AppendLine("PAYOUT DETAILS");
            sb.AppendLine("--------------");
            foreach (var payout in report.Payouts)
            {
                sb.AppendLine($"{payout.ProcessedAt:HH:mm} - {payout.Amount:C} - {payout.Reason}");
            }
            sb.AppendLine();
        }
        
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("END OF REPORT");
        sb.AppendLine();
        sb.AppendLine("\x1D\x56\x42\x00"); // Cut paper
        
        return sb.ToString();
    }

    private string FormatCashReconciliationReport(Guid sessionId, decimal expectedAmount, decimal actualAmount, decimal variance)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("\x1B\x40"); // Initialize printer
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("CASH RECONCILIATION");
        sb.AppendLine("===================");
        sb.AppendLine();
        
        sb.AppendLine("\x1B\x61\x00"); // Left align
        sb.AppendLine($"Session ID: {sessionId}");
        sb.AppendLine($"Terminal: {_terminalContext.TerminalId}");
        sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        
        sb.AppendLine("RECONCILIATION DETAILS");
        sb.AppendLine("----------------------");
        sb.AppendLine($"Expected Amount:     {expectedAmount:C}");
        sb.AppendLine($"Actual Amount:       {actualAmount:C}");
        sb.AppendLine($"Variance:            {variance:C}");
        sb.AppendLine();
        
        if (variance != 0)
        {
            sb.AppendLine(variance > 0 ? "STATUS: OVERAGE" : "STATUS: SHORTAGE");
        }
        else
        {
            sb.AppendLine("STATUS: BALANCED");
        }
        
        sb.AppendLine();
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("END OF REPORT");
        sb.AppendLine();
        sb.AppendLine("\x1D\x56\x42\x00"); // Cut paper
        
        return sb.ToString();
    }

    private string FormatSalesSummaryReport(DateTime startDate, DateTime endDate)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("\x1B\x40"); // Initialize printer
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("SALES SUMMARY REPORT");
        sb.AppendLine("====================");
        sb.AppendLine();
        
        sb.AppendLine("\x1B\x61\x00"); // Left align
        sb.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        sb.AppendLine($"Terminal: {_terminalContext.TerminalId}");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        
        // Note: This is a placeholder implementation
        // In a real implementation, you would query the database for sales data
        sb.AppendLine("SALES BREAKDOWN");
        sb.AppendLine("---------------");
        sb.AppendLine("(Sales data would be populated from database)");
        sb.AppendLine();
        
        sb.AppendLine("\x1B\x61\x01"); // Center align
        sb.AppendLine("END OF REPORT");
        sb.AppendLine();
        sb.AppendLine("\x1D\x56\x42\x00"); // Cut paper
        
        return sb.ToString();
    }

    private async Task<string?> GetDefaultPrinterAsync()
    {
        try
        {
            var printers = await _rawPrintService.GetInstalledPrintersAsync();
            return printers.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get default printer");
            return null;
        }
    }
}