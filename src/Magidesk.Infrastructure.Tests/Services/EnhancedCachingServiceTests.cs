using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Magidesk.Infrastructure.Services;

namespace Magidesk.Infrastructure.Tests.Services;

/// <summary>
/// Unit tests for EnhancedCachingService.
/// </summary>
public class EnhancedCachingServiceTests : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<EnhancedCachingService>> _mockLogger;
    private readonly EnhancedCachingService _cachingService;

    public EnhancedCachingServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockLogger = new Mock<ILogger<EnhancedCachingService>>();
        _cachingService = new EnhancedCachingService(_memoryCache, _mockLogger.Object);
    }

    [Fact]
    public async Task SetAsync_And_GetAsync_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        var key = "test-key";
        var value = new TestCacheObject { Id = Guid.NewGuid(), Name = "Test Object" };

        // Act
        await _cachingService.SetAsync(key, value);
        var retrieved = await _cachingService.GetAsync<TestCacheObject>(key);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(value.Id, retrieved.Id);
        Assert.Equal(value.Name, retrieved.Name);
    }

    [Fact]
    public async Task GetAsync_WithNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var key = "non-existent-key";

        // Act
        var result = await _cachingService.GetAsync<TestCacheObject>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_ShouldExpireAfterTime()
    {
        // Arrange
        var key = "expiring-key";
        var value = new TestCacheObject { Id = Guid.NewGuid(), Name = "Expiring Object" };
        var expiration = TimeSpan.FromMilliseconds(100);

        // Act
        await _cachingService.SetAsync(key, value, expiration);
        
        // Verify it's there initially
        var immediate = await _cachingService.GetAsync<TestCacheObject>(key);
        Assert.NotNull(immediate);

        // Wait for expiration
        await Task.Delay(150);

        // Verify it's expired
        var expired = await _cachingService.GetAsync<TestCacheObject>(key);
        Assert.Null(expired);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveValue()
    {
        // Arrange
        var key = "remove-key";
        var value = new TestCacheObject { Id = Guid.NewGuid(), Name = "To Remove" };

        await _cachingService.SetAsync(key, value);
        
        // Verify it's there
        var beforeRemove = await _cachingService.GetAsync<TestCacheObject>(key);
        Assert.NotNull(beforeRemove);

        // Act
        await _cachingService.RemoveAsync(key);

        // Assert
        var afterRemove = await _cachingService.GetAsync<TestCacheObject>(key);
        Assert.Null(afterRemove);
    }

    [Fact]
    public async Task RemoveByPatternAsync_ShouldRemoveMatchingKeys()
    {
        // Arrange
        var value1 = new TestCacheObject { Id = Guid.NewGuid(), Name = "Object 1" };
        var value2 = new TestCacheObject { Id = Guid.NewGuid(), Name = "Object 2" };
        var value3 = new TestCacheObject { Id = Guid.NewGuid(), Name = "Object 3" };

        await _cachingService.SetAsync("session:123", value1);
        await _cachingService.SetAsync("session:456", value2);
        await _cachingService.SetAsync("pricing:789", value3);

        // Act
        await _cachingService.RemoveByPatternAsync("session:*");

        // Assert
        var session1 = await _cachingService.GetAsync<TestCacheObject>("session:123");
        var session2 = await _cachingService.GetAsync<TestCacheObject>("session:456");
        var pricing = await _cachingService.GetAsync<TestCacheObject>("pricing:789");

        Assert.Null(session1);
        Assert.Null(session2);
        Assert.NotNull(pricing); // Should not be removed
    }

    [Fact]
    public async Task GetOrSetAsync_WithCacheMiss_ShouldExecuteFactoryAndCache()
    {
        // Arrange
        var key = "factory-key";
        var factoryExecuted = false;
        var expectedValue = new TestCacheObject { Id = Guid.NewGuid(), Name = "Factory Object" };

        Task<TestCacheObject> Factory()
        {
            factoryExecuted = true;
            return Task.FromResult(expectedValue);
        }

        // Act
        var result = await _cachingService.GetOrSetAsync(key, Factory);

        // Assert
        Assert.True(factoryExecuted);
        Assert.NotNull(result);
        Assert.Equal(expectedValue.Id, result.Id);

        // Verify it's cached
        var cached = await _cachingService.GetAsync<TestCacheObject>(key);
        Assert.NotNull(cached);
        Assert.Equal(expectedValue.Id, cached.Id);
    }

    [Fact]
    public async Task GetOrSetAsync_WithCacheHit_ShouldNotExecuteFactory()
    {
        // Arrange
        var key = "cached-key";
        var cachedValue = new TestCacheObject { Id = Guid.NewGuid(), Name = "Cached Object" };
        var factoryExecuted = false;

        await _cachingService.SetAsync(key, cachedValue);

        Task<TestCacheObject> Factory()
        {
            factoryExecuted = true;
            return Task.FromResult(new TestCacheObject { Id = Guid.NewGuid(), Name = "Factory Object" });
        }

        // Act
        var result = await _cachingService.GetOrSetAsync(key, Factory);

        // Assert
        Assert.False(factoryExecuted);
        Assert.NotNull(result);
        Assert.Equal(cachedValue.Id, result.Id);
    }

    [Fact]
    public async Task ClearAsync_ShouldRemoveAllCachedItems()
    {
        // Arrange
        var value1 = new TestCacheObject { Id = Guid.NewGuid(), Name = "Object 1" };
        var value2 = new TestCacheObject { Id = Guid.NewGuid(), Name = "Object 2" };

        await _cachingService.SetAsync("key1", value1);
        await _cachingService.SetAsync("key2", value2);

        // Verify they're cached
        Assert.NotNull(await _cachingService.GetAsync<TestCacheObject>("key1"));
        Assert.NotNull(await _cachingService.GetAsync<TestCacheObject>("key2"));

        // Act
        await _cachingService.ClearAsync();

        // Assert
        Assert.Null(await _cachingService.GetAsync<TestCacheObject>("key1"));
        Assert.Null(await _cachingService.GetAsync<TestCacheObject>("key2"));
    }

    [Theory]
    [InlineData("session:123", "session:*", true)]
    [InlineData("session:456", "session:*", true)]
    [InlineData("pricing:789", "session:*", false)]
    [InlineData("equipment:abc", "*equipment*", true)]
    [InlineData("performance:cpu", "*performance*", true)]
    [InlineData("other:xyz", "*performance*", false)]
    public async Task RemoveByPatternAsync_ShouldMatchPatternsCorrectly(string key, string pattern, bool shouldBeRemoved)
    {
        // Arrange
        var value = new TestCacheObject { Id = Guid.NewGuid(), Name = "Test Object" };
        await _cachingService.SetAsync(key, value);

        // Act
        await _cachingService.RemoveByPatternAsync(pattern);

        // Assert
        var result = await _cachingService.GetAsync<TestCacheObject>(key);
        
        if (shouldBeRemoved)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.NotNull(result);
        }
    }

    [Fact]
    public void CacheKeys_ShouldGenerateCorrectKeys()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableTypeId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var tableId = Guid.NewGuid();

        // Act & Assert
        Assert.Equal($"session:{sessionId}", CacheKeys.Session(sessionId));
        Assert.Equal($"pricing:tabletype:{tableTypeId}", CacheKeys.PricingRules(tableTypeId));
        Assert.Equal($"equipment:{equipmentId}", CacheKeys.Equipment(equipmentId));
        Assert.Equal($"equipment:table:{tableId}", CacheKeys.EquipmentByTable(tableId));
        Assert.Equal("performance:cpu_usage", CacheKeys.Performance("cpu_usage"));
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    private class TestCacheObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}