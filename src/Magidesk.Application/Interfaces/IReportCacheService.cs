namespace Magidesk.Application.Interfaces;

/// <summary>
/// Interface for report caching service to improve performance of report generation.
/// Provides caching capabilities with configurable expiration and cache invalidation.
/// </summary>
public interface IReportCacheService
{
    /// <summary>
    /// Retrieves a cached report by cache key.
    /// </summary>
    /// <typeparam name="T">The type of the cached report</typeparam>
    /// <param name="cacheKey">The unique cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached report or null if not found</returns>
    Task<T?> GetCachedReportAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Stores a report in the cache with the specified expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the report to cache</typeparam>
    /// <param name="cacheKey">The unique cache key</param>
    /// <param name="report">The report data to cache</param>
    /// <param name="expiration">The cache expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetCachedReportAsync<T>(string cacheKey, T report, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Invalidates cached reports matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The cache key pattern to match (supports wildcards)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InvalidateCacheAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached reports.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearAllCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics for monitoring and diagnostics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache statistics</returns>
    Task<CacheStatistics> GetCacheStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a standardized cache key for reports.
    /// </summary>
    /// <param name="reportType">The type of report</param>
    /// <param name="parameters">The report parameters</param>
    /// <returns>A standardized cache key</returns>
    string GenerateCacheKey(string reportType, params object[] parameters);
}

/// <summary>
/// Cache statistics for monitoring cache performance.
/// </summary>
public record CacheStatistics(
    int TotalEntries,
    long TotalMemoryUsage,
    int HitCount,
    int MissCount,
    decimal HitRatio,
    DateTime LastAccessed
);