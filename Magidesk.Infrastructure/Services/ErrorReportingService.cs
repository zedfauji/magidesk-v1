using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service for reporting and tracking errors for management dashboard.
/// Implements requirement 10.5 for error reporting to management dashboard.
/// </summary>
public class ErrorReportingService : IErrorReportingService
{
    private readonly ILogger<ErrorReportingService> _logger;
    private readonly ConcurrentDictionary<Guid, ErrorReportDto> _errorCache = new();
    private readonly ConcurrentQueue<ErrorReportDto> _recentErrors = new();
    private const int MaxRecentErrors = 1000;

    public ErrorReportingService(ILogger<ErrorReportingService> logger)
    {
        _logger = logger;
    }

    public async Task ReportErrorAsync(ErrorReportDto errorReport)
    {
        try
        {
            // Set ID if not provided
            if (errorReport.Id == Guid.Empty)
            {
                errorReport.Id = Guid.NewGuid();
            }

            // Check for duplicate errors (same title and message within 5 minutes)
            var duplicateError = _errorCache.Values
                .FirstOrDefault(e => e.Title == errorReport.Title && 
                               e.Message == errorReport.Message &&
                               DateTime.UtcNow - e.OccurredAt < TimeSpan.FromMinutes(5));

            if (duplicateError != null)
            {
                // Increment occurrence count instead of creating new error
                duplicateError.OccurrenceCount++;
                _logger.LogInformation("Duplicate error detected, incrementing count for: {Title}", errorReport.Title);
                return;
            }

            // Add to cache and recent errors queue
            _errorCache.TryAdd(errorReport.Id, errorReport);
            _recentErrors.Enqueue(errorReport);

            // Maintain recent errors queue size
            while (_recentErrors.Count > MaxRecentErrors)
            {
                _recentErrors.TryDequeue(out _);
            }

            // Log the error for persistence (in a real implementation, this would go to database)
            _logger.LogError("Error Report: {Category} - {Severity} - {Title}: {Message} | Terminal: {TerminalId} | User: {UserContext}",
                errorReport.Category,
                errorReport.Severity,
                errorReport.Title,
                errorReport.Message,
                errorReport.TerminalId ?? "Unknown",
                errorReport.UserContext ?? "Unknown");

            // For critical errors, also log technical details
            if (errorReport.Severity == ErrorSeverity.Critical && !string.IsNullOrEmpty(errorReport.TechnicalDetails))
            {
                _logger.LogError("Critical Error Details: {TechnicalDetails}", errorReport.TechnicalDetails);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report error: {Title}", errorReport.Title);
        }
    }

    public async Task<IEnumerable<ErrorReportDto>> GetRecentErrorsAsync(int count = 50)
    {
        try
        {
            var errors = _recentErrors
                .TakeLast(count)
                .OrderByDescending(e => e.OccurredAt)
                .ToList();

            return await Task.FromResult(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent errors");
            return Enumerable.Empty<ErrorReportDto>();
        }
    }

    public async Task<IEnumerable<ErrorReportDto>> GetErrorsByCategoryAsync(ErrorCategory category, DateTime? since = null)
    {
        try
        {
            var sinceDate = since ?? DateTime.UtcNow.AddDays(-7); // Default to last 7 days
            
            var errors = _errorCache.Values
                .Where(e => e.Category == category && e.OccurredAt >= sinceDate)
                .OrderByDescending(e => e.OccurredAt)
                .ToList();

            return await Task.FromResult(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get errors by category: {Category}", category);
            return Enumerable.Empty<ErrorReportDto>();
        }
    }

    public async Task ResolveErrorAsync(Guid errorId, string resolvedBy, string? resolution = null)
    {
        try
        {
            if (_errorCache.TryGetValue(errorId, out var error))
            {
                error.IsResolved = true;
                error.ResolvedAt = DateTime.UtcNow;
                error.ResolvedBy = resolvedBy;
                error.RecoveryAction = resolution ?? error.RecoveryAction;

                _logger.LogInformation("Error resolved: {Title} by {ResolvedBy}", error.Title, resolvedBy);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve error: {ErrorId}", errorId);
        }
    }

    public async Task<ErrorStatisticsDto> GetErrorStatisticsAsync(DateTime? since = null)
    {
        try
        {
            var sinceDate = since ?? DateTime.UtcNow.AddDays(-7); // Default to last 7 days
            var errors = _errorCache.Values.Where(e => e.OccurredAt >= sinceDate).ToList();

            var statistics = new ErrorStatisticsDto
            {
                TotalErrors = errors.Sum(e => e.OccurrenceCount),
                CriticalErrors = errors.Where(e => e.Severity == ErrorSeverity.Critical).Sum(e => e.OccurrenceCount),
                UnresolvedErrors = errors.Where(e => !e.IsResolved).Sum(e => e.OccurrenceCount),
                ErrorsByCategory = errors
                    .GroupBy(e => e.Category)
                    .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount)),
                ErrorsByTerminal = errors
                    .Where(e => !string.IsNullOrEmpty(e.TerminalId))
                    .GroupBy(e => e.TerminalId!)
                    .ToDictionary(g => g.Key, g => g.Sum(e => e.OccurrenceCount))
            };

            // Calculate average resolution time for resolved errors
            var resolvedErrors = errors.Where(e => e.IsResolved && e.ResolvedAt.HasValue).ToList();
            if (resolvedErrors.Any())
            {
                var totalResolutionTime = resolvedErrors
                    .Sum(e => (e.ResolvedAt!.Value - e.OccurredAt).TotalMinutes);
                statistics.AverageResolutionTime = TimeSpan.FromMinutes(totalResolutionTime / resolvedErrors.Count);
            }

            return await Task.FromResult(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error statistics");
            return new ErrorStatisticsDto();
        }
    }

    /// <summary>
    /// Gets error trends for dashboard charts.
    /// </summary>
    public async Task<Dictionary<DateTime, int>> GetErrorTrendsAsync(int days = 7)
    {
        try
        {
            var since = DateTime.UtcNow.AddDays(-days);
            var errors = _errorCache.Values.Where(e => e.OccurredAt >= since).ToList();

            var trends = new Dictionary<DateTime, int>();
            
            for (int i = 0; i < days; i++)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                var dayErrors = errors
                    .Where(e => e.OccurredAt.Date == date)
                    .Sum(e => e.OccurrenceCount);
                trends[date] = dayErrors;
            }

            return await Task.FromResult(trends.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get error trends");
            return new Dictionary<DateTime, int>();
        }
    }

    /// <summary>
    /// Clears old errors from cache to prevent memory issues.
    /// </summary>
    public async Task CleanupOldErrorsAsync(TimeSpan maxAge)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow - maxAge;
            var oldErrors = _errorCache.Values
                .Where(e => e.OccurredAt < cutoffDate)
                .ToList();

            foreach (var error in oldErrors)
            {
                _errorCache.TryRemove(error.Id, out _);
            }

            _logger.LogInformation("Cleaned up {Count} old errors older than {MaxAge}", oldErrors.Count, maxAge);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old errors");
        }
    }
}