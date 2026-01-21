using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

/// <summary>
/// Shift Summary Report DTO containing comprehensive shift performance data.
/// </summary>
public record ShiftSummaryReportDto(
    Guid ShiftId,
    string ShiftName,
    DateTime StartDate,
    DateTime EndDate,
    Money TotalSales,
    Money CashSales,
    Money CardSales,
    Money OtherPaymentSales,
    int TransactionCount,
    decimal AverageTicketSize,
    CashReconciliationDto CashReconciliation,
    IEnumerable<ServerSalesDto> ServerBreakdown,
    IEnumerable<ExceptionSummaryDto> Exceptions,
    IEnumerable<VoidSummaryDto> Voids,
    ShiftMetricsDto Metrics
);

/// <summary>
/// Cash drawer reconciliation data for shift summary.
/// </summary>
public record CashReconciliationDto(
    Money OpeningBalance,
    Money ExpectedCash,
    Money ActualCash,
    Money Difference,
    Money CashDrops,
    Money Payouts,
    Money DrawerBleeds,
    bool IsReconciled
);

/// <summary>
/// Server sales breakdown for shift summary.
/// </summary>
public record ServerSalesDto(
    Guid ServerId,
    string ServerName,
    Money TotalSales,
    int TransactionCount,
    decimal AverageTicketSize,
    Money TotalTips,
    decimal TipPercentage
);

/// <summary>
/// Exception summary for shift reporting.
/// </summary>
public record ExceptionSummaryDto(
    string ExceptionType,
    int Count,
    Money TotalAmount,
    string Description
);

/// <summary>
/// Void summary for shift reporting.
/// </summary>
public record VoidSummaryDto(
    string VoidType,
    int Count,
    Money TotalAmount,
    string Reason
);

/// <summary>
/// Additional shift metrics and KPIs.
/// </summary>
public record ShiftMetricsDto(
    int CustomerCount,
    decimal SalesPerHour,
    decimal TransactionsPerHour,
    TimeSpan AverageServiceTime,
    decimal TableTurnoverRate,
    Money PeakHourSales,
    int PeakHour
);

/// <summary>
/// Raw data for shift summary calculations.
/// </summary>
public record ShiftSummaryData(
    Guid ShiftId,
    string ShiftName,
    DateTime StartDate,
    DateTime EndDate,
    Money TotalSales,
    int TransactionCount,
    int CustomerCount
);

/// <summary>
/// Raw data for server sales breakdown.
/// </summary>
public record ServerSalesData(
    Guid ServerId,
    string ServerName,
    Money TotalSales,
    int TransactionCount,
    Money TotalTips
);

/// <summary>
/// Raw data for cash reconciliation.
/// </summary>
public record CashReconciliationData(
    Guid CashSessionId,
    Money OpeningBalance,
    Money ExpectedCash,
    Money ActualCash,
    Money CashDrops,
    Money Payouts,
    Money DrawerBleeds,
    bool IsClosed
);

/// <summary>
/// Raw data for exception tracking.
/// </summary>
public record ExceptionData(
    string ExceptionType,
    Money Amount,
    string Description,
    DateTime Timestamp
);

/// <summary>
/// Raw data for void tracking.
/// </summary>
public record VoidData(
    string VoidType,
    Money Amount,
    string Reason,
    DateTime Timestamp
);