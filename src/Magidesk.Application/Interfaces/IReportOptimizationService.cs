namespace Magidesk.Application.Interfaces;

/// <summary>
/// Interface for managing report performance optimizations including materialized views and indexes.
/// </summary>
public interface IReportOptimizationService
{
    /// <summary>
    /// Refreshes all reporting materialized views to ensure data freshness.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshMaterializedViewsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes reporting tables to update statistics for query optimization.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AnalyzeReportingTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about materialized view usage and performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Materialized view statistics</returns>
    Task<IEnumerable<MaterializedViewStats>> GetMaterializedViewStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets index usage statistics for monitoring performance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Index usage statistics</returns>
    Task<IEnumerable<IndexUsageStats>> GetIndexUsageStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the reporting optimizations (views and indexes) are properly installed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if optimizations are installed, false otherwise</returns>
    Task<bool> AreOptimizationsInstalledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Installs or updates the reporting optimizations (views and indexes).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InstallOptimizationsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Statistics for materialized views.
/// </summary>
public record MaterializedViewStats(
    string SchemaName,
    string ViewName,
    string Owner,
    bool HasIndexes,
    bool IsPopulated,
    long? EstimatedRows,
    string? LastRefreshTime
);

/// <summary>
/// Statistics for index usage.
/// </summary>
public record IndexUsageStats(
    string SchemaName,
    string TableName,
    string IndexName,
    long TupleReads,
    long TupleFetches,
    long Scans,
    decimal UsageRatio
);