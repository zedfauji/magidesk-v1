using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services.Reports;

/// <summary>
/// In-memory implementation of report caching service with thread-safe operations.
/// Provides performance optimization for report generation through intelligent caching.
/// </summary>
public class ReportCacheService : IReportCacheService
{
    private readonly ILogger<ReportCacheService> _logger;
    private readonly ConcurrentDictionary<string, CacheEntry> _cache;
    private readonly Timer _cleanupTimer;
    private readonly object _statsLock = new();
    private int _hitCount;
    private int _missCount;
    private DateTime _lastAccessed;

    public ReportCacheService(ILogger<ReportCacheService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = new ConcurrentDictionary<string, CacheEntry>();
        _lastAccessed = DateTime.UtcNow;
        
        // Setup cleanup timer to run every 5 minutes
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        
        _logger.LogInformation("ReportCacheService initialized with automatic cleanup every 5 minutes");
    }

    /// <summary>
    /// Retrieves a cached report by cache key.
    /// </summary>
    public async Task<T?> GetCachedReportAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(cacheKey));
        }

        try
        {
            lock (_statsLock)
            {
                _lastAccessed = DateTime.UtcNow;
            }

            if (_cache.TryGetValue(cacheKey, out var entry))
            {
                if (entry.ExpiresAt > DateTime.UtcNow)
                {
                    lock (_statsLock)
                    {
                        _hitCount++;
                    }

                    _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    
                    // Deserialize the cached data
                    var jsonData = Encoding.UTF8.GetString(entry.Data);
                    var result = JsonSerializer.Deserialize<T>(jsonData);
                    
                    await Task.CompletedTask; // Make method async
                    return result;
                }
                else
                {
                    // Entry expired, remove it
                    _cache.TryRemove(cacheKey, out _);
                    _logger.LogDebug("Cache entry expired and removed for key: {CacheKey}", cacheKey);
                }
            }

            lock (_statsLock)
            {
                _missCount++;
            }

            _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
            await Task.CompletedTask; // Make method async
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached report for key: {CacheKey}", cacheKey);
            
            lock (_statsLock)
            {
                _missCount++;
            }
            
            return null;
        }
    }

    /// <summary>
    /// Stores a report in the cache with the specified expiration time.
    /// </summary>
    public async Task SetCachedReportAsync<T>(string cacheKey, T report, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            throw new ArgumentException("Cache key cannot be null or empty", nameof(cacheKey));
        }

        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }

        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expiration must be positive", nameof(expiration));
        }

        try
        {
            // Serialize the report data
            var jsonData = JsonSerializer.Serialize(report);
            var data = Encoding.UTF8.GetBytes(jsonData);
            
            var entry = new CacheEntry(
                data,
                DateTime.UtcNow.Add(expiration),
                DateTime.UtcNow,
                typeof(T).Name
            );

            _cache.AddOrUpdate(cacheKey, entry, (key, oldEntry) => entry);
            
            _logger.LogDebug("Cached report for key: {CacheKey}, expires at: {ExpiresAt}, size: {Size} bytes", 
                cacheKey, entry.ExpiresAt, data.Length);
            
            await Task.CompletedTask; // Make method async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching report for key: {CacheKey}", cacheKey);
            throw;
        }
    }

    /// <summary>
    /// Invalidates cached reports matching the specified pattern.
    /// </summary>
    public async Task InvalidateCacheAsync(string pattern, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));
        }

        try
        {
            var keysToRemove = new List<string>();
            
            // Convert simple wildcard pattern to regex-like matching
            var regexPattern = pattern.Replace("*", ".*").Replace("?", ".");
            var regex = new System.Text.RegularExpressions.Regex($"^{regexPattern}$", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (var key in _cache.Keys)
            {
                if (regex.IsMatch(key))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }

            _logger.LogInformation("Invalidated {Count} cache entries matching pattern: {Pattern}", 
                keysToRemove.Count, pattern);
            
            await Task.CompletedTask; // Make method async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache with pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Clears all cached reports.
    /// </summary>
    public async Task ClearAllCacheAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = _cache.Count;
            _cache.Clear();
            
            lock (_statsLock)
            {
                _hitCount = 0;
                _missCount = 0;
            }
            
            _logger.LogInformation("Cleared all cache entries. Removed {Count} entries", count);
            await Task.CompletedTask; // Make method async
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            throw;
        }
    }

    /// <summary>
    /// Gets cache statistics for monitoring and diagnostics.
    /// </summary>
    public async Task<CacheStatistics> GetCacheStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalEntries = _cache.Count;
            var totalMemoryUsage = _cache.Values.Sum(entry => entry.Data.Length);
            
            int hitCount, missCount;
            DateTime lastAccessed;
            
            lock (_statsLock)
            {
                hitCount = _hitCount;
                missCount = _missCount;
                lastAccessed = _lastAccessed;
            }
            
            var totalRequests = hitCount + missCount;
            var hitRatio = totalRequests > 0 ? (decimal)hitCount / totalRequests * 100 : 0;

            await Task.CompletedTask; // Make method async
            
            return new CacheStatistics(
                TotalEntries: totalEntries,
                TotalMemoryUsage: totalMemoryUsage,
                HitCount: hitCount,
                MissCount: missCount,
                HitRatio: Math.Round(hitRatio, 2),
                LastAccessed: lastAccessed
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache statistics");
            throw;
        }
    }

    /// <summary>
    /// Generates a standardized cache key for reports.
    /// </summary>
    public string GenerateCacheKey(string reportType, params object[] parameters)
    {
        if (string.IsNullOrWhiteSpace(reportType))
        {
            throw new ArgumentException("Report type cannot be null or empty", nameof(reportType));
        }

        try
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"report:{reportType.ToLowerInvariant()}");

            if (parameters != null && parameters.Length > 0)
            {
                // Create a hash of the parameters for consistent key generation
                var paramJson = JsonSerializer.Serialize(parameters);
                var paramHash = ComputeHash(paramJson);
                keyBuilder.Append($":{paramHash}");
            }

            var cacheKey = keyBuilder.ToString();
            _logger.LogDebug("Generated cache key: {CacheKey} for report type: {ReportType}", cacheKey, reportType);
            
            return cacheKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cache key for report type: {ReportType}", reportType);
            throw;
        }
    }

    /// <summary>
    /// Computes a hash of the input string for cache key generation.
    /// </summary>
    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashBytes)[..16]; // Take first 16 characters for shorter keys
    }

    /// <summary>
    /// Cleanup timer callback to remove expired entries.
    /// </summary>
    private void CleanupExpiredEntries(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in _cache)
            {
                if (kvp.Value.ExpiresAt <= now)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogDebug("Cleanup removed {Count} expired cache entries", expiredKeys.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache cleanup");
        }
    }

    /// <summary>
    /// Disposes the cache service and cleanup timer.
    /// </summary>
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _cache.Clear();
        _logger.LogInformation("ReportCacheService disposed");
    }

    /// <summary>
    /// Internal cache entry structure.
    /// </summary>
    private record CacheEntry(
        byte[] Data,
        DateTime ExpiresAt,
        DateTime CreatedAt,
        string DataType
    );
}