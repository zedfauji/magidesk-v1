using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

/// <summary>
/// Core analytics engine implementation for calculating business metrics and insights.
/// Provides comprehensive analytics capabilities for billiard club operations.
/// </summary>
public class AnalyticsEngine : IAnalyticsEngine
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<AnalyticsEngine> _logger;

    public AnalyticsEngine(
        IAnalyticsRepository repository,
        ILogger<AnalyticsEngine> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculates table utilization metrics for the specified date range.
    /// </summary>
    public async Task<TableUtilizationMetrics> CalculateTableUtilizationAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating table utilization metrics from {StartDate} to {EndDate}", 
            startDate, endDate);

        try
        {
            // Get table session data
            var sessionData = await _repository.GetTableSessionDataAsync(startDate, endDate, cancellationToken);
            var sessions = sessionData.ToList();

            if (!sessions.Any())
            {
                _logger.LogWarning("No table session data found for the specified date range");
                return new TableUtilizationMetrics(
                    OccupancyPercent: 0m,
                    AverageSessionDuration: TimeSpan.Zero,
                    TotalSessions: 0,
                    TotalOperatingHours: TimeSpan.Zero,
                    TotalOccupiedTime: TimeSpan.Zero
                );
            }

            // Calculate total occupied time
            var totalOccupiedTime = sessions.Aggregate(TimeSpan.Zero, (sum, session) => sum + session.BillableTime);

            // Calculate total operating hours for the date range
            var totalOperatingHours = TimeSpan.Zero;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dailyOperatingHours = await _repository.GetOperatingHoursAsync(date, cancellationToken);
                totalOperatingHours = totalOperatingHours.Add(dailyOperatingHours);
            }

            // Prevent division by zero
            if (totalOperatingHours == TimeSpan.Zero)
            {
                _logger.LogWarning("Total operating hours is zero for the specified date range");
                return new TableUtilizationMetrics(
                    OccupancyPercent: 0m,
                    AverageSessionDuration: TimeSpan.Zero,
                    TotalSessions: sessions.Count,
                    TotalOperatingHours: TimeSpan.Zero,
                    TotalOccupiedTime: totalOccupiedTime
                );
            }

            // Calculate occupancy percentage
            var occupancyPercent = (decimal)(totalOccupiedTime.TotalHours / totalOperatingHours.TotalHours) * 100m;

            // Ensure occupancy doesn't exceed 100%
            occupancyPercent = Math.Min(occupancyPercent, 100m);

            // Calculate average session duration
            var averageSessionDuration = new TimeSpan(sessions.Sum(s => s.BillableTime.Ticks) / sessions.Count);

            var result = new TableUtilizationMetrics(
                OccupancyPercent: Math.Round(occupancyPercent, 2),
                AverageSessionDuration: averageSessionDuration,
                TotalSessions: sessions.Count,
                TotalOperatingHours: totalOperatingHours,
                TotalOccupiedTime: totalOccupiedTime
            );

            _logger.LogInformation("Calculated table utilization: {OccupancyPercent}% occupancy, {TotalSessions} sessions", 
                result.OccupancyPercent, result.TotalSessions);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating table utilization metrics");
            throw;
        }
    }

    /// <summary>
    /// Calculates revenue metrics for the specified date range.
    /// </summary>
    public async Task<RevenueMetrics> CalculateRevenueMetricsAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating revenue metrics from {StartDate} to {EndDate}", 
            startDate, endDate);

        try
        {
            // Get time-based revenue data
            var timeRevenueData = await _repository.GetTimeRevenueDataAsync(startDate, endDate, cancellationToken);
            var timeRevenue = timeRevenueData.Aggregate(Money.Zero(), (sum, data) => sum + data.TimeRevenue);

            // Get product revenue data
            var productRevenueData = await _repository.GetProductRevenueDataAsync(startDate, endDate, cancellationToken);
            var productRevenue = productRevenueData.Aggregate(Money.Zero(), (sum, data) => sum + data.ProductRevenue);

            // Calculate total revenue
            var totalRevenue = timeRevenue + productRevenue;

            // Calculate time revenue percentage
            var timeRevenuePercent = totalRevenue.Amount > 0 
                ? (timeRevenue.Amount / totalRevenue.Amount) * 100m 
                : 0m;

            // Calculate average transaction value
            var totalTransactions = productRevenueData.Count();
            var averageTransactionValue = totalTransactions > 0 
                ? totalRevenue.Amount / totalTransactions 
                : 0m;

            // Calculate growth rate (simplified - comparing to previous period)
            var periodLength = endDate - startDate;
            var previousStartDate = startDate - periodLength;
            var previousEndDate = startDate;

            var previousTimeRevenue = await _repository.GetTimeRevenueDataAsync(previousStartDate, previousEndDate, cancellationToken);
            var previousProductRevenue = await _repository.GetProductRevenueDataAsync(previousStartDate, previousEndDate, cancellationToken);
            
            var previousTotalRevenue = previousTimeRevenue.Aggregate(Money.Zero(), (sum, data) => sum + data.TimeRevenue) +
                                     previousProductRevenue.Aggregate(Money.Zero(), (sum, data) => sum + data.ProductRevenue);

            var growthRate = previousTotalRevenue.Amount > 0 
                ? ((totalRevenue.Amount - previousTotalRevenue.Amount) / previousTotalRevenue.Amount) * 100m 
                : 0m;

            var result = new RevenueMetrics(
                TotalRevenue: totalRevenue,
                TimeRevenue: timeRevenue,
                ProductRevenue: productRevenue,
                TimeRevenuePercent: Math.Round(timeRevenuePercent, 2),
                GrowthRate: Math.Round(growthRate, 2),
                AverageTransactionValue: Math.Round(averageTransactionValue, 2)
            );

            _logger.LogInformation("Calculated revenue metrics: Total {TotalRevenue}, Time {TimeRevenue}, Product {ProductRevenue}", 
                result.TotalRevenue.Amount, result.TimeRevenue.Amount, result.ProductRevenue.Amount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating revenue metrics");
            throw;
        }
    }

    /// <summary>
    /// Calculates member activity metrics for the specified date range.
    /// </summary>
    public async Task<MemberActivityMetrics> CalculateMemberActivityAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating member activity metrics from {StartDate} to {EndDate}", 
            startDate, endDate);

        try
        {
            // Get member activity data
            var memberData = await _repository.GetMemberActivityDataAsync(startDate, endDate, cancellationToken);
            var members = memberData.ToList();

            if (!members.Any())
            {
                _logger.LogWarning("No member activity data found for the specified date range");
                return new MemberActivityMetrics(
                    ActiveMembers: 0,
                    NewMembers: 0,
                    ChurnedMembers: 0,
                    RetentionRate: 0m,
                    AverageVisitFrequency: 0m,
                    AverageMemberValue: Money.Zero()
                );
            }

            // Calculate active members (visited during period)
            var activeMembers = members.Count(m => m.LastVisitDate >= startDate && m.LastVisitDate <= endDate);

            // Calculate new members (joined during period)
            var newMembers = members.Count(m => m.JoinDate >= startDate && m.JoinDate <= endDate);

            // Calculate churned members (haven't visited in 30+ days before end date)
            var churnThreshold = endDate.AddDays(-30);
            var churnedMembers = members.Count(m => m.LastVisitDate < churnThreshold && m.IsActive);

            // Calculate retention rate
            var totalMembersAtStart = members.Count(m => m.JoinDate < startDate);
            var retentionRate = totalMembersAtStart > 0 
                ? ((decimal)(totalMembersAtStart - churnedMembers) / totalMembersAtStart) * 100m 
                : 100m;

            // Calculate average visit frequency (visits per member during period)
            var totalVisits = members.Sum(m => m.VisitCount);
            var averageVisitFrequency = activeMembers > 0 
                ? (decimal)totalVisits / activeMembers 
                : 0m;

            // Calculate average member value
            var totalMemberSpending = members.Aggregate(Money.Zero(), (sum, m) => sum + m.TotalSpent);
            var averageMemberValue = activeMembers > 0 
                ? new Money(totalMemberSpending.Amount / activeMembers, totalMemberSpending.Currency) 
                : Money.Zero();

            var result = new MemberActivityMetrics(
                ActiveMembers: activeMembers,
                NewMembers: newMembers,
                ChurnedMembers: churnedMembers,
                RetentionRate: Math.Round(retentionRate, 2),
                AverageVisitFrequency: Math.Round(averageVisitFrequency, 2),
                AverageMemberValue: averageMemberValue
            );

            _logger.LogInformation("Calculated member activity metrics: {ActiveMembers} active, {NewMembers} new, {ChurnedMembers} churned", 
                result.ActiveMembers, result.NewMembers, result.ChurnedMembers);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating member activity metrics");
            throw;
        }
    }

    /// <summary>
    /// Analyzes trends for the specified metric type and date range.
    /// </summary>
    public async Task<TrendAnalysis> AnalyzeTrendsAsync(
        string metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing trends for {MetricType} from {StartDate} to {EndDate}", 
            metricType, startDate, endDate);

        try
        {
            // Get trend data points
            var trendData = await _repository.GetTrendDataAsync(metricType, startDate, endDate, cancellationToken);
            var dataPoints = trendData.ToList();

            if (!dataPoints.Any())
            {
                _logger.LogWarning("No trend data found for metric type {MetricType}", metricType);
                return new TrendAnalysis(
                    MetricType: metricType,
                    StartDate: startDate,
                    EndDate: endDate,
                    TrendDirection: 0m,
                    ChangePercent: 0m,
                    DataPoints: Enumerable.Empty<TrendDataPoint>(),
                    SeasonalPattern: null,
                    ForecastValue: null
                );
            }

            // Sort data points by date
            var sortedPoints = dataPoints.OrderBy(p => p.Date).ToList();

            // Calculate trend direction using linear regression (simplified)
            var firstValue = sortedPoints.First().Value;
            var lastValue = sortedPoints.Last().Value;
            
            var trendDirection = lastValue > firstValue ? 1m : lastValue < firstValue ? -1m : 0m;
            var changePercent = firstValue > 0 ? ((lastValue - firstValue) / firstValue) * 100m : 0m;

            // Simple seasonal pattern detection (look for weekly patterns)
            var seasonalPattern = DetectSeasonalPattern(sortedPoints);

            // Simple forecast (extend trend)
            var forecastValue = CalculateSimpleForecast(sortedPoints);

            var result = new TrendAnalysis(
                MetricType: metricType,
                StartDate: startDate,
                EndDate: endDate,
                TrendDirection: trendDirection,
                ChangePercent: Math.Round(changePercent, 2),
                DataPoints: sortedPoints,
                SeasonalPattern: seasonalPattern,
                ForecastValue: forecastValue
            );

            _logger.LogInformation("Analyzed trends for {MetricType}: {TrendDirection} direction, {ChangePercent}% change", 
                metricType, result.TrendDirection, result.ChangePercent);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing trends for metric type {MetricType}", metricType);
            throw;
        }
    }

    /// <summary>
    /// Detects seasonal patterns in the data points.
    /// </summary>
    private static string? DetectSeasonalPattern(List<TrendDataPoint> dataPoints)
    {
        if (dataPoints.Count < 14) // Need at least 2 weeks of data
            return null;

        // Group by day of week and calculate averages
        var weekdayAverages = dataPoints
            .GroupBy(p => p.Date.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Average(p => (double)p.Value));

        // Find the day with highest and lowest averages
        var maxDay = weekdayAverages.OrderByDescending(kvp => kvp.Value).First();
        var minDay = weekdayAverages.OrderBy(kvp => kvp.Value).First();

        // If there's a significant difference (>20%), report pattern
        var difference = (maxDay.Value - minDay.Value) / maxDay.Value;
        if (difference > 0.2)
        {
            return $"Peak on {maxDay.Key}, Low on {minDay.Key}";
        }

        return null;
    }

    /// <summary>
    /// Calculates a simple forecast based on trend.
    /// </summary>
    private static decimal? CalculateSimpleForecast(List<TrendDataPoint> dataPoints)
    {
        if (dataPoints.Count < 3)
            return null;

        // Simple linear trend extrapolation
        var recentPoints = dataPoints.TakeLast(3).ToList();
        var avgChange = recentPoints.Zip(recentPoints.Skip(1), (prev, curr) => curr.Value - prev.Value).Average();
        
        return recentPoints.Last().Value + avgChange;
    }
}