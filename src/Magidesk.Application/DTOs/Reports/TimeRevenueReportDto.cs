using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.DTOs.Reports;

/// <summary>
/// Time Revenue Report DTO containing comprehensive time-based revenue analysis.
/// </summary>
public record TimeRevenueReportDto(
    DateTime StartDate,
    DateTime EndDate,
    Money TotalTimeRevenue,
    TimeSpan TotalBilledTime,
    decimal AverageHourlyRate,
    decimal RevenuePerHour,
    IEnumerable<TableTypeRevenueDto> ByTableType,
    IEnumerable<DayOfWeekRevenueDto> ByDayOfWeek,
    IEnumerable<HourlyRevenueDto> ByHourOfDay
);

/// <summary>
/// Table type revenue breakdown for time revenue reports.
/// </summary>
public record TableTypeRevenueDto(
    string TableType,
    Money Revenue,
    TimeSpan BilledTime,
    decimal AverageRate,
    int SessionCount,
    decimal PercentOfTotal
);

/// <summary>
/// Day of week revenue breakdown for time revenue reports.
/// </summary>
public record DayOfWeekRevenueDto(
    DayOfWeek DayOfWeek,
    Money Revenue,
    TimeSpan BilledTime,
    decimal AverageRate,
    int SessionCount,
    bool IsWeekend
);

/// <summary>
/// Hourly revenue breakdown for time revenue reports.
/// </summary>
public record HourlyRevenueDto(
    int Hour,
    Money Revenue,
    TimeSpan BilledTime,
    decimal AverageRate,
    int SessionCount
);

