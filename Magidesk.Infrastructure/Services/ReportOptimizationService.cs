using System.Data;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service for managing report performance optimizations including materialized views and indexes.
/// </summary>
public class ReportOptimizationService : IReportOptimizationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportOptimizationService> _logger;

    public ReportOptimizationService(
        ApplicationDbContext context,
        ILogger<ReportOptimizationService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Refreshes all reporting materialized views to ensure data freshness.
    /// </summary>
    public async Task RefreshMaterializedViewsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting refresh of reporting materialized views");

            // Call the database function to refresh all views
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT refresh_reporting_views();", 
                cancellationToken);

            _logger.LogInformation("Successfully refreshed all reporting materialized views");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh materialized views");
            throw;
        }
    }

    /// <summary>
    /// Analyzes reporting tables to update statistics for query optimization.
    /// </summary>
    public async Task AnalyzeReportingTablesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting analysis of reporting tables");

            // Call the database function to analyze tables
            await _context.Database.ExecuteSqlRawAsync(
                "SELECT analyze_reporting_tables();", 
                cancellationToken);

            _logger.LogInformation("Successfully analyzed reporting tables");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze reporting tables");
            throw;
        }
    }

    /// <summary>
    /// Gets statistics about materialized view usage and performance.
    /// </summary>
    public async Task<IEnumerable<MaterializedViewStats>> GetMaterializedViewStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving materialized view statistics");

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    schemaname,
                    matviewname,
                    matviewowner,
                    hasindexes,
                    ispopulated,
                    CASE 
                        WHEN ispopulated THEN (
                            SELECT reltuples::bigint 
                            FROM pg_class 
                            WHERE relname = matviewname
                        )
                        ELSE NULL
                    END as estimated_rows
                FROM pg_matviews 
                WHERE matviewname LIKE '%_summary'
                ORDER BY matviewname;";

            var stats = new List<MaterializedViewStats>();
            
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                stats.Add(new MaterializedViewStats(
                    SchemaName: reader.GetString("schemaname"),
                    ViewName: reader.GetString("matviewname"),
                    Owner: reader.GetString("matviewowner"),
                    HasIndexes: reader.GetBoolean("hasindexes"),
                    IsPopulated: reader.GetBoolean("ispopulated"),
                    EstimatedRows: reader.IsDBNull("estimated_rows") ? null : reader.GetInt64("estimated_rows"),
                    LastRefreshTime: null // PostgreSQL doesn't track this by default
                ));
            }

            _logger.LogDebug("Retrieved {Count} materialized view statistics", stats.Count);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get materialized view statistics");
            throw;
        }
    }

    /// <summary>
    /// Gets index usage statistics for monitoring performance.
    /// </summary>
    public async Task<IEnumerable<IndexUsageStats>> GetIndexUsageStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Retrieving index usage statistics");

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    schemaname,
                    tablename,
                    indexname,
                    idx_tup_read,
                    idx_tup_fetch,
                    idx_scan,
                    CASE 
                        WHEN idx_tup_read + idx_tup_fetch > 0 
                        THEN (idx_scan::decimal / (idx_tup_read + idx_tup_fetch)) * 100
                        ELSE 0
                    END as usage_ratio
                FROM pg_stat_user_indexes 
                WHERE indexname LIKE 'idx_%'
                ORDER BY idx_scan DESC;";

            var stats = new List<IndexUsageStats>();
            
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                stats.Add(new IndexUsageStats(
                    SchemaName: reader.GetString("schemaname"),
                    TableName: reader.GetString("tablename"),
                    IndexName: reader.GetString("indexname"),
                    TupleReads: reader.GetInt64("idx_tup_read"),
                    TupleFetches: reader.GetInt64("idx_tup_fetch"),
                    Scans: reader.GetInt64("idx_scan"),
                    UsageRatio: reader.GetDecimal("usage_ratio")
                ));
            }

            _logger.LogDebug("Retrieved {Count} index usage statistics", stats.Count);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get index usage statistics");
            throw;
        }
    }

    /// <summary>
    /// Checks if the reporting optimizations (views and indexes) are properly installed.
    /// </summary>
    public async Task<bool> AreOptimizationsInstalledAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if reporting optimizations are installed");

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pg_matviews 
                WHERE matviewname IN (
                    'daily_sales_summary',
                    'hourly_sales_summary', 
                    'table_utilization_summary',
                    'member_activity_summary',
                    'server_performance_summary'
                );";

            var viewCount = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
            
            // Check if refresh function exists
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM pg_proc 
                WHERE proname = 'refresh_reporting_views';";

            var functionCount = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

            var isInstalled = viewCount == 5 && functionCount == 1;
            
            _logger.LogDebug("Reporting optimizations installed: {IsInstalled} (Views: {ViewCount}/5, Functions: {FunctionCount}/1)", 
                isInstalled, viewCount, functionCount);
            
            return isInstalled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if optimizations are installed");
            return false;
        }
    }

    /// <summary>
    /// Installs or updates the reporting optimizations (views and indexes).
    /// </summary>
    public async Task InstallOptimizationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Installing reporting optimizations");

            // Read the SQL script
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                "Data", "ReportingOptimizations.sql");
            
            if (!File.Exists(scriptPath))
            {
                // Try alternative path for development
                scriptPath = Path.Combine(Directory.GetCurrentDirectory(), 
                    "Magidesk.Infrastructure", "Data", "ReportingOptimizations.sql");
            }

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"Reporting optimizations script not found at: {scriptPath}");
            }

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            
            // Split script into individual statements and execute them
            var statements = script.Split(new[] { "-- =====================================================" }, 
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var statement in statements)
            {
                var cleanStatement = statement.Trim();
                if (string.IsNullOrEmpty(cleanStatement) || cleanStatement.StartsWith("--"))
                {
                    continue;
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync(cleanStatement, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to execute optimization statement (continuing): {Statement}", 
                        cleanStatement.Substring(0, Math.Min(100, cleanStatement.Length)));
                }
            }

            _logger.LogInformation("Successfully installed reporting optimizations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to install reporting optimizations");
            throw;
        }
    }
}