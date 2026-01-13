using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries.Handlers;

/// <summary>
/// Handler for GetTableUtilizationReportQuery.
/// Generates comprehensive table utilization reports with occupancy analysis.
/// </summary>
public class GetTableUtilizationReportQueryHandler : IQueryHandler<GetTableUtilizationReportQuery, TableUtilizationReportDto>
{
    private readonly IAnalyticsRepository _repository;
    private readonly IAnalyticsEngine _analyticsEngine;
    private readonly ILogger<GetTableUtilizationReportQueryHandler> _logger;

    public GetTableUtilizationReportQueryHandler(
        IAnalyticsRepository repository,
        IAnalyticsEngine analyticsEngine,
        ILogger<GetTableUtilizationReportQueryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _analyticsEngine = analyticsEngine ?? throw new ArgumentNullException(nameof(analyticsEngine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the table utilization report query.
    /// </summary>
    public async Task<TableUtilizationReportDto> HandleAsync(
        GetTableUtilizationReportQuery query, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating table utilization report from {StartDate} to {EndDate}", 
            query.StartDate, query.EndDate);

        try
        {
            // Get overall utilization metrics
            var utilizationMetrics = await _analyticsEngine.CalculateTableUtilizationAsync(
                query.StartDate, query.EndDate, cancellationToken);

            // Get detailed breakdown data in parallel for performance
            var detailedUtilizationTask = _repository.GetDetailedTableUtilizationDataAsync(
                query.StartDate, query.EndDate, cancellationToken);
            var hourlyOccupancyTask = _repository.GetHourlyOccupancyDataAsync(
                query.StartDate, query.EndDate, cancellationToken);
            var dayOfWeekOccupancyTask = _repository.GetDayOfWeekOccupancyDataAsync(
                query.StartDate, query.EndDate, cancellationToken);
            var timeRevenueTask = _repository.GetTimeRevenueDataAsync(
                query.StartDate, query.EndDate, cancellationToken);

            await Task.WhenAll(detailedUtilizationTask, hourlyOccupancyTask, 
                dayOfWeekOccupancyTask, timeRevenueTask);

            var detailedUtilization = detailedUtilizationTask.Result.ToList();
            var hourlyOccupancy = hourlyOccupancyTask.Result.ToList();
            var dayOfWeekOccupancy = dayOfWeekOccupancyTask.Result.ToList();
            var timeRevenue = timeRevenueTask.Result.ToList();

            // Calculate total time revenue
            var totalTimeRevenue = timeRevenue.Aggregate(Money.Zero(), (sum, data) => sum + data.TimeRevenue);

            // Create table breakdown
            var tableBreakdown = CreateTableBreakdown(detailedUtilization, timeRevenue);

            // Create hourly occupancy breakdown
            var hourlyBreakdown = CreateHourlyBreakdown(hourlyOccupancy);

            // Create weekly pattern breakdown
            var weeklyPattern = CreateWeeklyPattern(dayOfWeekOccupancy);

            // Create the table utilization report DTO
            var report = new TableUtilizationReportDto(
                StartDate: query.StartDate,
                EndDate: query.EndDate,
                OverallOccupancyPercent: utilizationMetrics.OccupancyPercent,
                AverageSessionDuration: utilizationMetrics.AverageSessionDuration,
                TotalTimeRevenue: totalTimeRevenue,
                TableBreakdown: tableBreakdown,
                HourlyOccupancy: hourlyBreakdown,
                WeeklyPattern: weeklyPattern
            );

            _logger.LogInformation("Generated table utilization report: {OccupancyPercent}% occupancy, {TotalRevenue} revenue", 
                report.OverallOccupancyPercent, report.TotalTimeRevenue.Amount);

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating table utilization report from {StartDate} to {EndDate}", 
                query.StartDate, query.EndDate);
            throw new InvalidOperationException($"Failed to generate table utilization report for {query.StartDate:yyyy-MM-dd} to {query.EndDate:yyyy-MM-dd}", ex);
        }
    }

    /// <summary>
    /// Creates table-by-table breakdown from detailed utilization data.
    /// </summary>
    private static IEnumerable<TableUtilizationDto> CreateTableBreakdown(
        IList<DetailedTableUtilizationData> detailedData,
        IList<TimeRevenueData> revenueData)
    {
        // Group by table and calculate metrics
        var tableGroups = detailedData.GroupBy(d => new { d.TableId, d.TableNumber, d.TableType });
        var revenueByTable = revenueData.GroupBy(r => r.TableId)
            .ToDictionary(g => g.Key, g => g.Aggregate(Money.Zero(), (sum, r) => sum + r.TimeRevenue));

        return tableGroups.Select(group =>
        {
            var totalOccupiedTime = group.Sum(d => d.OccupiedTime.Ticks);
            var totalOperatingHours = group.Sum(d => d.OperatingHours.Ticks);
            var totalSessions = group.Sum(d => d.SessionCount);
            var totalSessionDuration = group.Sum(d => d.TotalSessionDuration.Ticks);

            var occupancyPercent = totalOperatingHours > 0 
                ? Math.Min((decimal)(totalOccupiedTime * 100.0 / totalOperatingHours), 100m)
                : 0m;

            var averageSessionDuration = totalSessions > 0 
                ? new TimeSpan(totalSessionDuration / totalSessions)
                : TimeSpan.Zero;

            var revenuePerTable = revenueByTable.TryGetValue(group.Key.TableId, out var revenue) 
                ? revenue 
                : Money.Zero();

            return new TableUtilizationDto(
                TableNumber: group.Key.TableNumber,
                TableType: group.Key.TableType,
                OccupancyPercent: Math.Round(occupancyPercent, 2),
                AverageSessionDuration: averageSessionDuration,
                RevenuePerTable: revenuePerTable,
                TotalSessions: totalSessions,
                TotalOccupiedTime: new TimeSpan(totalOccupiedTime)
            );
        }).OrderBy(t => t.TableNumber);
    }

    /// <summary>
    /// Creates hourly occupancy breakdown for peak hours identification.
    /// </summary>
    private static IEnumerable<HourlyOccupancyDto> CreateHourlyBreakdown(
        IList<HourlyOccupancyData> hourlyData)
    {
        // Group by hour and calculate averages
        return hourlyData.GroupBy(h => h.Hour)
            .Select(group =>
            {
                var averageOccupancy = group.Average(h => (double)h.OccupancyPercent);
                var averageActiveTables = (int)Math.Round(group.Average(h => h.ActiveTables));
                var totalRevenue = group.Aggregate(Money.Zero(), (sum, h) => sum + h.HourlyRevenue);

                return new HourlyOccupancyDto(
                    Hour: group.Key,
                    OccupancyPercent: Math.Round((decimal)averageOccupancy, 2),
                    ActiveTables: averageActiveTables,
                    HourlyRevenue: totalRevenue
                );
            })
            .OrderBy(h => h.Hour);
    }

    /// <summary>
    /// Creates weekly pattern breakdown for day-of-week analysis.
    /// </summary>
    private static IEnumerable<DayOfWeekOccupancyDto> CreateWeeklyPattern(
        IList<DayOfWeekOccupancyData> weeklyData)
    {
        return weeklyData.Select(w => new DayOfWeekOccupancyDto(
            DayOfWeek: w.DayOfWeek,
            AverageOccupancyPercent: w.AverageOccupancyPercent,
            AverageSessionDuration: w.AverageSessionDuration,
            AverageRevenue: w.TotalSessions > 0 
                ? new Money(w.TotalRevenue.Amount / w.TotalSessions, w.TotalRevenue.Currency)
                : Money.Zero()
        )).OrderBy(w => (int)w.DayOfWeek);
    }
}