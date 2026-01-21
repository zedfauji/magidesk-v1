using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

/// <summary>
/// Core metrics for table utilization calculations.
/// </summary>
public record TableUtilizationMetrics(
    decimal OccupancyPercent,
    TimeSpan AverageSessionDuration,
    int TotalSessions,
    TimeSpan TotalOperatingHours,
    TimeSpan TotalOccupiedTime
);

/// <summary>
/// Core metrics for revenue calculations.
/// </summary>
public record RevenueMetrics(
    Money TotalRevenue,
    Money TimeRevenue,
    Money ProductRevenue,
    decimal TimeRevenuePercent,
    decimal GrowthRate,
    decimal AverageTransactionValue
);

/// <summary>
/// Core metrics for member activity calculations.
/// </summary>
public record MemberActivityMetrics(
    int ActiveMembers,
    int NewMembers,
    int ChurnedMembers,
    decimal RetentionRate,
    decimal AverageVisitFrequency,
    Money AverageMemberValue
);

/// <summary>
/// Trend analysis results for various metrics.
/// </summary>
public record TrendAnalysis(
    string MetricType,
    DateTime StartDate,
    DateTime EndDate,
    decimal TrendDirection, // Positive for upward, negative for downward
    decimal ChangePercent,
    IEnumerable<TrendDataPoint> DataPoints,
    string? SeasonalPattern,
    decimal? ForecastValue
);

/// <summary>
/// Individual data point in trend analysis.
/// </summary>
public record TrendDataPoint(
    DateTime Date,
    decimal Value,
    string? Label = null
);

/// <summary>
/// Raw data for table session analytics.
/// </summary>
public record TableSessionData(
    Guid SessionId,
    Guid TableId,
    DateTime StartTime,
    DateTime? EndTime,
    TimeSpan BillableTime,
    Money TotalCharge,
    int GuestCount,
    Guid? CustomerId
);

/// <summary>
/// Raw data for table occupancy analytics.
/// </summary>
public record TableOccupancyData(
    Guid TableId,
    int TableNumber,
    string TableTypeName,
    DateTime Date,
    TimeSpan OccupiedTime,
    TimeSpan OperatingHours,
    int SessionCount
);

/// <summary>
/// Raw data for time-based revenue analytics.
/// </summary>
public record TimeRevenueData(
    DateTime Date,
    Guid TableId,
    string TableTypeName,
    Money TimeRevenue,
    TimeSpan BilledTime,
    decimal HourlyRate
);

/// <summary>
/// Raw data for product revenue analytics.
/// </summary>
public record ProductRevenueData(
    DateTime Date,
    Guid TicketId,
    Money ProductRevenue,
    Money TaxAmount,
    Money TotalAmount,
    int ItemCount
);

/// <summary>
/// Raw data for member activity analytics.
/// </summary>
public record MemberActivityData(
    Guid MemberId,
    Guid CustomerId,
    DateTime LastVisitDate,
    int VisitCount,
    Money TotalSpent,
    DateTime JoinDate,
    bool IsActive
);

/// <summary>
/// Raw data for member visit tracking.
/// </summary>
public record MemberVisitData(
    Guid MemberId,
    DateTime VisitDate,
    Money AmountSpent,
    TimeSpan SessionDuration
);

/// <summary>
/// Table Utilization Report DTO containing comprehensive utilization analysis.
/// </summary>
public record TableUtilizationReportDto(
    DateTime StartDate,
    DateTime EndDate,
    decimal OverallOccupancyPercent,
    TimeSpan AverageSessionDuration,
    Money TotalTimeRevenue,
    IEnumerable<TableUtilizationDto> TableBreakdown,
    IEnumerable<HourlyOccupancyDto> HourlyOccupancy,
    IEnumerable<DayOfWeekOccupancyDto> WeeklyPattern
);

/// <summary>
/// Individual table utilization breakdown.
/// </summary>
public record TableUtilizationDto(
    int TableNumber,
    string TableType,
    decimal OccupancyPercent,
    TimeSpan AverageSessionDuration,
    Money RevenuePerTable,
    int TotalSessions,
    TimeSpan TotalOccupiedTime
);

/// <summary>
/// Hourly occupancy pattern for peak hours identification.
/// </summary>
public record HourlyOccupancyDto(
    int Hour,
    decimal OccupancyPercent,
    int ActiveTables,
    Money HourlyRevenue
);

/// <summary>
/// Day of week occupancy pattern for weekly analysis.
/// </summary>
public record DayOfWeekOccupancyDto(
    DayOfWeek DayOfWeek,
    decimal AverageOccupancyPercent,
    TimeSpan AverageSessionDuration,
    Money AverageRevenue
);

/// <summary>
/// Detailed table utilization data for individual table analysis.
/// </summary>
public record DetailedTableUtilizationData(
    Guid TableId,
    int TableNumber,
    string TableType,
    DateTime Date,
    TimeSpan OccupiedTime,
    TimeSpan OperatingHours,
    int SessionCount,
    Money TotalRevenue,
    TimeSpan TotalSessionDuration
);

/// <summary>
/// Hourly occupancy data for peak hours identification.
/// </summary>
public record HourlyOccupancyData(
    DateTime Date,
    int Hour,
    int ActiveTables,
    int TotalTables,
    decimal OccupancyPercent,
    Money HourlyRevenue
);

/// <summary>
/// Day of week occupancy data for weekly pattern analysis.
/// </summary>
public record DayOfWeekOccupancyData(
    DayOfWeek DayOfWeek,
    decimal AverageOccupancyPercent,
    TimeSpan AverageSessionDuration,
    Money TotalRevenue,
    int TotalSessions
);