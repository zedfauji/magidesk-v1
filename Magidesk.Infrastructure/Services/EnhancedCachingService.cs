using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Enhanced caching service for active sessions and pricing rules.
/// </summary>
public interface IEnhancedCachingService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task ClearAsync();
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;
}

/// <summary>
/// Implementation of enhanced caching service using in-memory cache.
/// </summary>
public class EnhancedCachingService : IEnhancedCachingService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<EnhancedCachingService> _logger;
    private readonly ConcurrentDictionary<string, DateTime> _cacheKeys;

    public EnhancedCachingService(IMemoryCache memoryCache, ILogger<EnhancedCachingService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheKeys = new ConcurrentDictionary<string, DateTime>();
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            return Task.FromResult<T?>(null);
        }

        var value = _memoryCache.Get<T>(key);
        
        if (value != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
        }
        else
        {
            _logger.LogDebug("Cache miss for key: {Key}", key);
        }

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        if (string.IsNullOrEmpty(key) || value == null)
        {
            return Task.CompletedTask;
        }

        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            // Default expiration based on cache type
            options.AbsoluteExpirationRelativeToNow = GetDefaultExpiration(key);
        }

        // Set up eviction callback to track cache keys
        options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
        {
            _cacheKeys.TryRemove(evictedKey.ToString()!, out _);
            _logger.LogDebug("Cache entry evicted: {Key}, Reason: {Reason}", evictedKey, reason);
        });

        _memoryCache.Set(key, value, options);
        _cacheKeys.TryAdd(key, DateTime.UtcNow);

        _logger.LogDebug("Cache entry set: {Key}, Expiration: {Expiration}", key, options.AbsoluteExpirationRelativeToNow);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return Task.CompletedTask;
        }

        _memoryCache.Remove(key);
        _cacheKeys.TryRemove(key, out _);

        _logger.LogDebug("Cache entry removed: {Key}", key);

        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return Task.CompletedTask;
        }

        var keysToRemove = new List<string>();

        foreach (var key in _cacheKeys.Keys)
        {
            if (IsPatternMatch(key, pattern))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
        }

        _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);

        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        // Note: IMemoryCache doesn't have a clear method, so we remove individual keys
        var keysToRemove = new List<string>(_cacheKeys.Keys);

        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
        }

        _logger.LogInformation("Cleared {Count} cache entries", keysToRemove.Count);

        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        var cachedValue = await GetAsync<T>(key);
        
        if (cachedValue != null)
        {
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);

        var value = await factory();
        
        if (value != null)
        {
            await SetAsync(key, value, expiration);
        }

        return value;
    }

    private TimeSpan GetDefaultExpiration(string key)
    {
        // Set different default expirations based on cache key patterns
        if (key.StartsWith("session:"))
        {
            return TimeSpan.FromSeconds(30); // Active sessions - short TTL for real-time updates
        }
        
        if (key.StartsWith("pricing:"))
        {
            return TimeSpan.FromHours(1); // Pricing rules - longer TTL as they change less frequently
        }
        
        if (key.StartsWith("equipment:"))
        {
            return TimeSpan.FromMinutes(5); // Equipment status - medium TTL
        }
        
        if (key.StartsWith("performance:"))
        {
            return TimeSpan.FromMinutes(1); // Performance metrics - very short TTL
        }

        // Default expiration
        return TimeSpan.FromMinutes(15);
    }

    private static bool IsPatternMatch(string key, string pattern)
    {
        // Simple pattern matching - supports wildcards (*)
        if (pattern == "*")
        {
            return true;
        }

        if (pattern.EndsWith("*"))
        {
            var prefix = pattern[..^1];
            return key.StartsWith(prefix);
        }

        if (pattern.StartsWith("*"))
        {
            var suffix = pattern[1..];
            return key.EndsWith(suffix);
        }

        return key == pattern;
    }
}

/// <summary>
/// Cache key constants for consistent cache key generation.
/// </summary>
public static class CacheKeys
{
    public const string ActiveSessions = "sessions:active";
    public const string SessionPrefix = "session:";
    public const string PricingRulesPrefix = "pricing:";
    public const string EquipmentPrefix = "equipment:";
    public const string PerformancePrefix = "performance:";
    public const string AlertsActive = "alerts:active";

    public static string Session(Guid sessionId) => $"{SessionPrefix}{sessionId}";
    public static string PricingRules(Guid tableTypeId) => $"{PricingRulesPrefix}tabletype:{tableTypeId}";
    public static string Equipment(Guid equipmentId) => $"{EquipmentPrefix}{equipmentId}";
    public static string EquipmentByTable(Guid tableId) => $"{EquipmentPrefix}table:{tableId}";
    public static string Performance(string metricName) => $"{PerformancePrefix}{metricName}";
}