using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.Handlers;

/// <summary>
/// Handler for GetTimeRevenueReportQuery.
/// Generates comprehensive time-based revenue analytics with breakdowns.
/// </summary>
public class GetTimeRevenueReportQueryHandler : IQueryHandler<GetTimeRevenueReportQuery, TimeRevenueReportDto>
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<GetTimeRevenueReportQueryHandler> _logger;

    public GetTimeRevenueReportQueryHandler(
        IAnalyticsRepository repository,
        ILogger<GetTimeRevenueReportQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the time revenue report query.
    /// </summary>
    public async Task<TimeRevenueReportDto> HandleAsync(
        GetTimeRevenueReportQuery query, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating time revenue report for {StartDate} to {EndDate}", 
            query.StartDate, query.EndDate);

        try
        {
            // Get time revenue data for the date range
            var timeRevenueData = await _repository.GetTimeRevenueDataAsync(
                query.StartDate, query.EndDate, cancellationToken);

            var timeRevenueList = timeRevenueData.ToList();

            if (!timeRevenueList.Any())
            {
                _logger.LogInformation("No time revenue data found for date range {StartDate} to {EndDate}", 
                    query.StartDate, query.EndDate);
                
                return CreateEmptyReport(query.StartDate, query.EndDate);
            }

            // Calculate totals
            var totalTimeRevenue = new Money(timeRevenueList.Sum(t => t.TimeRevenue.Amount));
            var totalBilledTime = TimeSpan.FromTicks(timeRevenueList.Sum(t => t.BilledTime.Ticks));
            
            // Calculate average hourly rate and revenue per hour
            var averageHourlyRate = timeRevenueList.Any() 
                ? timeRevenueList.Average(t => t.HourlyRate) 
                : 0m;
            
            var revenuePerHour = totalBilledTime.TotalHours > 0 
                ? (decimal)(totalTimeRevenue.Amount / (decimal)totalBilledTime.TotalHours)
                : 0m;

            // Calculate breakdowns
            var tableTypeBreakdown = CalculateTableTypeBreakdown(timeRevenueList, totalTimeRevenue.Amount);
            var dayOfWeekBreakdown = CalculateDayOfWeekBreakdown(timeRevenueList);
            var hourlyBreakdown = CalculateHourlyBreakdown(timeRevenueList);

            var report = new TimeRevenueReportDto(
                StartDate: query.StartDate,
                EndDate: query.EndDate,
                TotalTimeRevenue: totalTimeRevenue,
                TotalBilledTime: totalBilledTime,
                AverageHourlyRate: Math.Round(averageHourlyRate, 2),
                RevenuePerHour: Math.Round(revenuePerHour, 2),
                ByTableType: tableTypeBreakdown,
                ByDayOfWeek: dayOfWeekBreakdown,
                ByHourOfDay: hourlyBreakdown
            );

            _logger.LogInformation("Generated time revenue report for {StartDate} to {EndDate}: " +
                "Total Revenue {TotalRevenue}, Total Time {TotalHours} hours", 
                query.StartDate, query.EndDate, report.TotalTimeRevenue.Amount, 
                Math.Round(report.TotalBilledTime.TotalHours, 2));

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating time revenue report for {StartDate} to {EndDate}", 
                query.StartDate, query.EndDate);
            throw new InvalidOperationException(
                $"Failed to generate time revenue report for {query.StartDate:yyyy-MM-dd} to {query.EndDate:yyyy-MM-dd}", ex);
        }
    }

    /// <summary>
    /// Creates an empty report when no data is available.
    /// </summary>
    private static TimeRevenueReportDto CreateEmptyReport(DateTime startDate, DateTime endDate)
    {
        return new TimeRevenueReportDto(
            StartDate: startDate,
            EndDate: endDate,
            TotalTimeRevenue: new Money(0),
            TotalBilledTime: TimeSpan.Zero,
            AverageHourlyRate: 0m,
            RevenuePerHour: 0m,
            ByTableType: Enumerable.Empty<TableTypeRevenueDto>(),
            ByDayOfWeek: Enumerable.Empty<DayOfWeekRevenueDto>(),
            ByHourOfDay: Enumerable.Empty<HourlyRevenueDto>()
        );
    }

    /// <summary>
    /// Calculates table type revenue breakdown.
    /// </summary>
    private static IEnumerable<TableTypeRevenueDto> CalculateTableTypeBreakdown(
        List<TimeRevenueData> data, decimal totalRevenue)
    {
        return data
            .GroupBy(t => t.TableTypeName)
            .Select(g =>
            {
                var revenue = new Money(g.Sum(t => t.TimeRevenue.Amount));
                var billedTime = TimeSpan.FromTicks(g.Sum(t => t.BilledTime.Ticks));
                var averageRate = g.Average(t => t.HourlyRate);
                var sessionCount = g.Count();
                var percentOfTotal = totalRevenue > 0 ? (revenue.Amount / totalRevenue) * 100 : 0;

                return new TableTypeRevenueDto(
                    TableType: g.Key,
                    Revenue: revenue,
                    BilledTime: billedTime,
                    AverageRate: Math.Round(averageRate, 2),
                    SessionCount: sessionCount,
                    PercentOfTotal: Math.Round(percentOfTotal, 2)
                );
            })
            .OrderByDescending(t => t.Revenue.Amount);
    }

    /// <summary>
    /// Calculates day of week revenue breakdown.
    /// </summary>
    private static IEnumerable<DayOfWeekRevenueDto> CalculateDayOfWeekBreakdown(
        List<TimeRevenueData> data)
    {
        return data
            .GroupBy(t => t.Date.DayOfWeek)
            .Select(g =>
            {
                var revenue = new Money(g.Sum(t => t.TimeRevenue.Amount));
                var billedTime = TimeSpan.FromTicks(g.Sum(t => t.BilledTime.Ticks));
                var averageRate = g.Average(t => t.HourlyRate);
                var sessionCount = g.Count();
                var isWeekend = g.Key == DayOfWeek.Saturday || g.Key == DayOfWeek.Sunday;

                return new DayOfWeekRevenueDto(
                    DayOfWeek: g.Key,
                    Revenue: revenue,
                    BilledTime: billedTime,
                    AverageRate: Math.Round(averageRate, 2),
                    SessionCount: sessionCount,
                    IsWeekend: isWeekend
                );
            })
            .OrderBy(t => (int)t.DayOfWeek);
    }

    /// <summary>
    /// Calculates hourly revenue breakdown.
    /// </summary>
    private static IEnumerable<HourlyRevenueDto> CalculateHourlyBreakdown(
        List<TimeRevenueData> data)
    {
        return data
            .GroupBy(t => t.Date.Hour)
            .Select(g =>
            {
                var revenue = new Money(g.Sum(t => t.TimeRevenue.Amount));
                var billedTime = TimeSpan.FromTicks(g.Sum(t => t.BilledTime.Ticks));
                var averageRate = g.Average(t => t.HourlyRate);
                var sessionCount = g.Count();

                return new HourlyRevenueDto(
                    Hour: g.Key,
                    Revenue: revenue,
                    BilledTime: billedTime,
                    AverageRate: Math.Round(averageRate, 2),
                    SessionCount: sessionCount
                );
            })
            .OrderBy(t => t.Hour);
    }
}