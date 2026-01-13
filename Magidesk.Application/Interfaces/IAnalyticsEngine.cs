using Magidesk.Application.DTOs.Reports;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Core analytics engine for calculating business metrics and insights.
/// Provides comprehensive analytics capabilities for billiard club operations.
/// </summary>
public interface IAnalyticsEngine
{
    /// <summary>
    /// Calculates table utilization metrics for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Table utilization metrics</returns>
    Task<TableUtilizationMetrics> CalculateTableUtilizationAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates revenue metrics for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Revenue metrics</returns>
    Task<RevenueMetrics> CalculateRevenueMetricsAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates member activity metrics for the specified date range.
    /// </summary>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Member activity metrics</returns>
    Task<MemberActivityMetrics> CalculateMemberActivityAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes trends for the specified metric type and date range.
    /// </summary>
    /// <param name="metricType">Type of metric to analyze (e.g., "revenue", "utilization", "member_activity")</param>
    /// <param name="startDate">Start date for analysis</param>
    /// <param name="endDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trend analysis results</returns>
    Task<TrendAnalysis> AnalyzeTrendsAsync(
        string metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
}