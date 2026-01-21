using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

/// <summary>
/// Server Performance Report DTO containing comprehensive server analytics.
/// </summary>
public record ServerPerformanceReportDto(
    DateTime StartDate,
    DateTime EndDate,
    int TotalServers,
    Money TotalSales,
    Money TotalTips,
    decimal OverallTipPercentage,
    decimal AverageSalesPerServer,
    decimal AverageTransactionsPerServer,
    IEnumerable<ServerPerformanceDto> ServerBreakdown,
    IEnumerable<ServerComparisonDto> TopPerformers,
    IEnumerable<ServerTrendDto> PerformanceTrends
);

/// <summary>
/// Individual server performance metrics.
/// </summary>
public record ServerPerformanceDto(
    Guid ServerId,
    string ServerName,
    Money TotalSales,
    int TransactionCount,
    decimal AverageTicketSize,
    Money TotalTips,
    decimal TipPercentage,
    decimal SalesRank,
    decimal PerformanceScore,
    TimeSpan TotalWorkTime,
    decimal SalesPerHour
);

/// <summary>
/// Server comparison data for rankings.
/// </summary>
public record ServerComparisonDto(
    Guid ServerId,
    string ServerName,
    Money TotalSales,
    decimal TipPercentage,
    int Rank,
    string PerformanceCategory
);

/// <summary>
/// Server performance trends over time.
/// </summary>
public record ServerTrendDto(
    Guid ServerId,
    string ServerName,
    DateTime Date,
    Money DailySales,
    decimal DailyTipPercentage,
    decimal GrowthRate
);

/// <summary>
/// Extended server sales data with additional performance metrics.
/// </summary>
public record ServerPerformanceData(
    Guid ServerId,
    string ServerName,
    Money TotalSales,
    int TransactionCount,
    Money TotalTips,
    TimeSpan WorkTime,
    DateTime FirstTransaction,
    DateTime LastTransaction,
    int ShiftCount
);