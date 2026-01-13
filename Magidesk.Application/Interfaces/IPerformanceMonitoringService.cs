using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service interface for performance monitoring and metrics collection.
/// </summary>
public interface IPerformanceMonitoringService
{
    /// <summary>
    /// Records a performance metric.
    /// </summary>
    /// <param name="metricName">Name of the metric</param>
    /// <param name="value">Metric value</param>
    /// <param name="tags">Optional tags for categorization</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task RecordMetricAsync(
        string metricName, 
        double value, 
        Dictionary<string, string>? tags = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records response time for an operation.
    /// </summary>
    /// <param name="operationName">Name of the operation</param>
    /// <param name="responseTime">Response time in milliseconds</param>
    /// <param name="success">Whether the operation was successful</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task RecordResponseTimeAsync(
        string operationName, 
        double responseTime, 
        bool success = true, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records system resource usage.
    /// </summary>
    /// <param name="cpuUsage">CPU usage percentage</param>
    /// <param name="memoryUsage">Memory usage in MB</param>
    /// <param name="diskUsage">Disk usage percentage</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task RecordSystemResourcesAsync(
        double cpuUsage, 
        double memoryUsage, 
        double diskUsage, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records database performance metrics.
    /// </summary>
    /// <param name="queryType">Type of database query</param>
    /// <param name="executionTime">Query execution time in milliseconds</param>
    /// <param name="recordCount">Number of records affected/returned</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task RecordDatabasePerformanceAsync(
        string queryType, 
        double executionTime, 
        int recordCount, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance metrics for a specific time range.
    /// </summary>
    /// <param name="metricName">Name of the metric</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of performance metrics</returns>
    Task<IEnumerable<PerformanceMetric>> GetMetricsAsync(
        string metricName, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets system health status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System health status</returns>
    Task<SystemHealthStatus> GetSystemHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance summary for a time range.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Performance summary</returns>
    Task<PerformanceSummary> GetPerformanceSummaryAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if performance thresholds are being exceeded.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of threshold violations</returns>
    Task<IEnumerable<ThresholdViolation>> CheckThresholdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts monitoring system performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the monitoring operation</returns>
    Task StartMonitoringAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops monitoring system performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task StopMonitoringAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a performance metric data point.
/// </summary>
public record PerformanceMetric(
    string Name,
    double Value,
    DateTime Timestamp,
    Dictionary<string, string> Tags
);

/// <summary>
/// Represents system health status.
/// </summary>
public record SystemHealthStatus(
    HealthStatus OverallStatus,
    double CpuUsage,
    double MemoryUsage,
    double DiskUsage,
    int ActiveSessions,
    double AverageResponseTime,
    DateTime LastChecked,
    IEnumerable<HealthCheck> HealthChecks
);

/// <summary>
/// Individual health check result.
/// </summary>
public record HealthCheck(
    string Name,
    HealthStatus Status,
    string Message,
    DateTime CheckedAt
);

/// <summary>
/// Performance summary for a time period.
/// </summary>
public record PerformanceSummary(
    DateTime FromDate,
    DateTime ToDate,
    double AverageResponseTime,
    double MaxResponseTime,
    double MinResponseTime,
    int TotalRequests,
    int SuccessfulRequests,
    int FailedRequests,
    double AverageCpuUsage,
    double AverageMemoryUsage,
    double PeakCpuUsage,
    double PeakMemoryUsage
);

/// <summary>
/// Represents a performance threshold violation.
/// </summary>
public record ThresholdViolation(
    string MetricName,
    double CurrentValue,
    double ThresholdValue,
    string Severity,
    DateTime DetectedAt,
    string Description
);

/// <summary>
/// Health status enumeration.
/// </summary>
public enum HealthStatus
{
    Healthy,
    Warning,
    Critical,
    Unknown
}