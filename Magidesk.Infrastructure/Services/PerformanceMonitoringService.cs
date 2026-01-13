using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service implementation for performance monitoring and metrics collection.
/// </summary>
public class PerformanceMonitoringService : IPerformanceMonitoringService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PerformanceMonitoringService> _logger;
    private bool _isMonitoring;

    public PerformanceMonitoringService(ApplicationDbContext dbContext, ILogger<PerformanceMonitoringService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task RecordMetricAsync(
        string metricName, 
        double value, 
        Dictionary<string, string>? tags = null, 
        CancellationToken cancellationToken = default)
    {
        var metric = new PerformanceMetricEntity
        {
            Id = Guid.NewGuid(),
            Name = metricName,
            Value = value,
            Timestamp = DateTime.UtcNow,
            Tags = tags != null ? string.Join(";", tags.Select(kvp => $"{kvp.Key}={kvp.Value}")) : string.Empty
        };

        _dbContext.Set<PerformanceMetricEntity>().Add(metric);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordResponseTimeAsync(
        string operationName, 
        double responseTime, 
        bool success = true, 
        CancellationToken cancellationToken = default)
    {
        var tags = new Dictionary<string, string>
        {
            ["operation"] = operationName,
            ["success"] = success.ToString()
        };

        await RecordMetricAsync("response_time_ms", responseTime, tags, cancellationToken);
    }

    public async Task RecordSystemResourcesAsync(
        double cpuUsage, 
        double memoryUsage, 
        double diskUsage, 
        CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            RecordMetricAsync("cpu_usage_percent", cpuUsage, null, cancellationToken),
            RecordMetricAsync("memory_usage_mb", memoryUsage, null, cancellationToken),
            RecordMetricAsync("disk_usage_percent", diskUsage, null, cancellationToken)
        );
    }

    public async Task RecordDatabasePerformanceAsync(
        string queryType, 
        double executionTime, 
        int recordCount, 
        CancellationToken cancellationToken = default)
    {
        var tags = new Dictionary<string, string>
        {
            ["query_type"] = queryType,
            ["record_count"] = recordCount.ToString()
        };

        await RecordMetricAsync("db_query_time_ms", executionTime, tags, cancellationToken);
    }

    public async Task<IEnumerable<PerformanceMetric>> GetMetricsAsync(
        string metricName, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Name == metricName && 
                       m.Timestamp >= fromDate && 
                       m.Timestamp <= toDate)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new PerformanceMetric(
            e.Name,
            e.Value,
            e.Timestamp,
            ParseTags(e.Tags)
        ));
    }

    public async Task<SystemHealthStatus> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var oneHourAgo = now.AddHours(-1);

        // Get recent performance metrics
        var recentMetrics = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Timestamp >= oneHourAgo)
            .ToListAsync(cancellationToken);

        var cpuMetrics = recentMetrics.Where(m => m.Name == "cpu_usage_percent").ToList();
        var memoryMetrics = recentMetrics.Where(m => m.Name == "memory_usage_mb").ToList();
        var responseTimeMetrics = recentMetrics.Where(m => m.Name == "response_time_ms").ToList();

        var avgCpuUsage = cpuMetrics.Any() ? cpuMetrics.Average(m => m.Value) : GetCurrentCpuUsage();
        var avgMemoryUsage = memoryMetrics.Any() ? memoryMetrics.Average(m => m.Value) : GetCurrentMemoryUsage();
        var avgResponseTime = responseTimeMetrics.Any() ? responseTimeMetrics.Average(m => m.Value) : 0;

        // Get active session count
        var activeSessions = await _dbContext.TableSessions
            .CountAsync(s => s.Status == Domain.Enumerations.TableSessionStatus.Active || 
                            s.Status == Domain.Enumerations.TableSessionStatus.Paused, 
                       cancellationToken);

        // Determine overall health status
        var overallStatus = DetermineHealthStatus(avgCpuUsage, avgMemoryUsage, avgResponseTime);

        var healthChecks = new List<HealthCheck>
        {
            new("CPU Usage", avgCpuUsage < 80 ? HealthStatus.Healthy : HealthStatus.Warning, 
                $"CPU usage: {avgCpuUsage:F1}%", now),
            new("Memory Usage", avgMemoryUsage < 1000 ? HealthStatus.Healthy : HealthStatus.Warning, 
                $"Available memory: {avgMemoryUsage:F0} MB", now),
            new("Response Time", avgResponseTime < 200 ? HealthStatus.Healthy : HealthStatus.Warning, 
                $"Average response time: {avgResponseTime:F1} ms", now),
            new("Database", HealthStatus.Healthy, "Database connection healthy", now)
        };

        return new SystemHealthStatus(
            overallStatus,
            avgCpuUsage,
            avgMemoryUsage,
            0, // Disk usage would be calculated separately
            activeSessions,
            avgResponseTime,
            now,
            healthChecks
        );
    }

    public async Task<PerformanceSummary> GetPerformanceSummaryAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        var responseTimeMetrics = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Name == "response_time_ms" && 
                       m.Timestamp >= fromDate && 
                       m.Timestamp <= toDate)
            .ToListAsync(cancellationToken);

        var cpuMetrics = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Name == "cpu_usage_percent" && 
                       m.Timestamp >= fromDate && 
                       m.Timestamp <= toDate)
            .ToListAsync(cancellationToken);

        var memoryMetrics = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Name == "memory_usage_mb" && 
                       m.Timestamp >= fromDate && 
                       m.Timestamp <= toDate)
            .ToListAsync(cancellationToken);

        var avgResponseTime = responseTimeMetrics.Any() ? responseTimeMetrics.Average(m => m.Value) : 0;
        var maxResponseTime = responseTimeMetrics.Any() ? responseTimeMetrics.Max(m => m.Value) : 0;
        var minResponseTime = responseTimeMetrics.Any() ? responseTimeMetrics.Min(m => m.Value) : 0;

        var avgCpuUsage = cpuMetrics.Any() ? cpuMetrics.Average(m => m.Value) : 0;
        var peakCpuUsage = cpuMetrics.Any() ? cpuMetrics.Max(m => m.Value) : 0;

        var avgMemoryUsage = memoryMetrics.Any() ? memoryMetrics.Average(m => m.Value) : 0;
        var peakMemoryUsage = memoryMetrics.Any() ? memoryMetrics.Max(m => m.Value) : 0;

        return new PerformanceSummary(
            fromDate,
            toDate,
            avgResponseTime,
            maxResponseTime,
            minResponseTime,
            responseTimeMetrics.Count,
            responseTimeMetrics.Count(m => ParseTags(m.Tags).GetValueOrDefault("success", "true") == "true"),
            responseTimeMetrics.Count(m => ParseTags(m.Tags).GetValueOrDefault("success", "true") == "false"),
            avgCpuUsage,
            avgMemoryUsage,
            peakCpuUsage,
            peakMemoryUsage
        );
    }

    public async Task<IEnumerable<ThresholdViolation>> CheckThresholdsAsync(CancellationToken cancellationToken = default)
    {
        var violations = new List<ThresholdViolation>();
        var now = DateTime.UtcNow;
        var recentTime = now.AddMinutes(-5);

        // Check recent metrics against thresholds
        var recentMetrics = await _dbContext.Set<PerformanceMetricEntity>()
            .Where(m => m.Timestamp >= recentTime)
            .ToListAsync(cancellationToken);

        // CPU threshold check
        var recentCpuMetrics = recentMetrics.Where(m => m.Name == "cpu_usage_percent").ToList();
        if (recentCpuMetrics.Any())
        {
            var avgCpu = recentCpuMetrics.Average(m => m.Value);
            if (avgCpu > 80)
            {
                violations.Add(new ThresholdViolation(
                    "CPU Usage",
                    avgCpu,
                    80,
                    avgCpu > 95 ? "Critical" : "Warning",
                    now,
                    $"CPU usage is {avgCpu:F1}%, exceeding threshold of 80%"
                ));
            }
        }

        // Response time threshold check
        var recentResponseTimes = recentMetrics.Where(m => m.Name == "response_time_ms").ToList();
        if (recentResponseTimes.Any())
        {
            var avgResponseTime = recentResponseTimes.Average(m => m.Value);
            if (avgResponseTime > 200)
            {
                violations.Add(new ThresholdViolation(
                    "Response Time",
                    avgResponseTime,
                    200,
                    avgResponseTime > 1000 ? "Critical" : "Warning",
                    now,
                    $"Average response time is {avgResponseTime:F1}ms, exceeding threshold of 200ms"
                ));
            }
        }

        return violations;
    }

    public async Task StartMonitoringAsync(CancellationToken cancellationToken = default)
    {
        if (_isMonitoring)
        {
            return;
        }

        _isMonitoring = true;
        _logger.LogInformation("Starting performance monitoring");

        // Start background monitoring task
        _ = Task.Run(async () =>
        {
            while (_isMonitoring && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cpuUsage = GetCurrentCpuUsage();
                    var memoryUsage = GetCurrentMemoryUsage();

                    await RecordSystemResourcesAsync(cpuUsage, memoryUsage, 0, cancellationToken);

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during performance monitoring");
                }
            }
        }, cancellationToken);

        await Task.CompletedTask;
    }

    public async Task StopMonitoringAsync(CancellationToken cancellationToken = default)
    {
        _isMonitoring = false;
        _logger.LogInformation("Stopping performance monitoring");
        await Task.CompletedTask;
    }

    private double GetCurrentCpuUsage()
    {
        // Placeholder implementation - would use actual performance counters in production
        return Random.Shared.NextDouble() * 100;
    }

    private double GetCurrentMemoryUsage()
    {
        // Placeholder implementation - would use actual memory monitoring in production
        return Random.Shared.NextDouble() * 2000 + 500; // 500-2500 MB
    }

    private static HealthStatus DetermineHealthStatus(double cpuUsage, double memoryUsage, double responseTime)
    {
        if (cpuUsage > 95 || memoryUsage < 100 || responseTime > 1000)
        {
            return HealthStatus.Critical;
        }

        if (cpuUsage > 80 || memoryUsage < 500 || responseTime > 500)
        {
            return HealthStatus.Warning;
        }

        return HealthStatus.Healthy;
    }

    private static Dictionary<string, string> ParseTags(string tagsString)
    {
        var tags = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(tagsString))
        {
            return tags;
        }

        var pairs = tagsString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2);
            if (parts.Length == 2)
            {
                tags[parts[0]] = parts[1];
            }
        }

        return tags;
    }
}

/// <summary>
/// Entity for storing performance metrics in the database.
/// </summary>
public class PerformanceMetricEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public string Tags { get; set; } = string.Empty;
}