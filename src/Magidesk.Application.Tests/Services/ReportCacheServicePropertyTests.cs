using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.DTOs.Reports;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services.Reports;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Services;

/// <summary>
/// Property-based tests for ReportCacheService.
/// Feature: reporting-export, Property 16: Concurrent Report Generation
/// Validates: Cache integrity and concurrent access
/// </summary>
public class ReportCacheServicePropertyTests : IDisposable
{
    private readonly Mock<ILogger<ReportCacheService>> _mockLogger;
    private readonly ReportCacheService _cacheService;

    public ReportCacheServicePropertyTests()
    {
        _mockLogger = new Mock<ILogger<ReportCacheService>>();
        _cacheService = new ReportCacheService(_mockLogger.Object);
    }

    /// <summary>
    /// Unit test: Cache key generation should be consistent for same inputs.
    /// </summary>
    [Fact]
    public void GenerateCacheKey_WithSameInputs_ReturnsConsistentKey()
    {
        // Arrange
        var reportType = "daily-sales";
        var parameters = new object[] { DateTime.Today, "USD" };

        // Act
        var key1 = _cacheService.GenerateCacheKey(reportType, parameters);
        var key2 = _cacheService.GenerateCacheKey(reportType, parameters);

        // Assert
        Assert.Equal(key1, key2);
        Assert.StartsWith("report:daily-sales:", key1);
    }

    /// <summary>
    /// Unit test: Cache key generation should produce different keys for different inputs.
    /// </summary>
    [Fact]
    public void GenerateCacheKey_WithDifferentInputs_ReturnsDifferentKeys()
    {
        // Arrange
        var reportType1 = "daily-sales";
        var reportType2 = "time-revenue";
        var parameters = new object[] { DateTime.Today };

        // Act
        var key1 = _cacheService.GenerateCacheKey(reportType1, parameters);
        var key2 = _cacheService.GenerateCacheKey(reportType2, parameters);

        // Assert
        Assert.NotEqual(key1, key2);
    }

    /// <summary>
    /// Unit test: Setting and getting cached report should work correctly.
    /// </summary>
    [Fact]
    public async Task SetAndGetCachedReport_WithValidData_ReturnsCorrectData()
    {
        // Arrange
        var cacheKey = "test-key";
        var reportData = CreateSampleDailySalesReport();
        var expiration = TimeSpan.FromMinutes(30);

        // Act
        await _cacheService.SetCachedReportAsync(cacheKey, reportData, expiration);
        var retrievedData = await _cacheService.GetCachedReportAsync<DailySalesReportDto>(cacheKey);

        // Assert
        Assert.NotNull(retrievedData);
        Assert.Equal(reportData.Date, retrievedData.Date);
        Assert.Equal(reportData.TotalSales.Amount, retrievedData.TotalSales.Amount);
    }

    /// <summary>
    /// Unit test: Getting non-existent cached report should return null.
    /// </summary>
    [Fact]
    public async Task GetCachedReport_WithNonExistentKey_ReturnsNull()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";

        // Act
        var result = await _cacheService.GetCachedReportAsync<DailySalesReportDto>(nonExistentKey);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Unit test: Cache invalidation should remove matching entries.
    /// </summary>
    [Fact]
    public async Task InvalidateCache_WithPattern_RemovesMatchingEntries()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var expiration = TimeSpan.FromMinutes(30);
        
        await _cacheService.SetCachedReportAsync("report:daily-sales:key1", reportData, expiration);
        await _cacheService.SetCachedReportAsync("report:daily-sales:key2", reportData, expiration);
        await _cacheService.SetCachedReportAsync("report:time-revenue:key3", reportData, expiration);

        // Act
        await _cacheService.InvalidateCacheAsync("report:daily-sales:*");

        // Assert
        var result1 = await _cacheService.GetCachedReportAsync<DailySalesReportDto>("report:daily-sales:key1");
        var result2 = await _cacheService.GetCachedReportAsync<DailySalesReportDto>("report:daily-sales:key2");
        var result3 = await _cacheService.GetCachedReportAsync<DailySalesReportDto>("report:time-revenue:key3");

        Assert.Null(result1);
        Assert.Null(result2);
        Assert.NotNull(result3); // Should not be affected by pattern
    }

    /// <summary>
    /// Unit test: Cache statistics should reflect cache operations.
    /// </summary>
    [Fact]
    public async Task GetCacheStatistics_AfterOperations_ReflectsCorrectStats()
    {
        // Arrange
        var reportData = CreateSampleDailySalesReport();
        var expiration = TimeSpan.FromMinutes(30);
        var cacheKey = "stats-test-key";

        // Act
        await _cacheService.SetCachedReportAsync(cacheKey, reportData, expiration);
        await _cacheService.GetCachedReportAsync<DailySalesReportDto>(cacheKey); // Hit
        await _cacheService.GetCachedReportAsync<DailySalesReportDto>("non-existent"); // Miss

        var stats = await _cacheService.GetCacheStatisticsAsync();

        // Assert
        Assert.True(stats.TotalEntries >= 1);
        Assert.True(stats.HitCount >= 1);
        Assert.True(stats.MissCount >= 1);
        Assert.True(stats.TotalMemoryUsage > 0);
    }

    /// <summary>
    /// Property: For any valid cache key and data, storing and retrieving should return identical data.
    /// **Validates: Cache integrity and concurrent access**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property CacheIntegrity_StoreAndRetrieve_ReturnsIdenticalData()
    {
        return Prop.ForAll(
            GenerateValidCacheKey(),
            GenerateValidReportData(),
            GenerateValidExpiration(),
            (cacheKey, reportData, expiration) =>
            {
                try
                {
                    // Store data
                    var storeTask = _cacheService.SetCachedReportAsync(cacheKey, reportData, expiration);
                    storeTask.GetAwaiter().GetResult();

                    // Retrieve data
                    var retrieveTask = _cacheService.GetCachedReportAsync<DailySalesReportDto>(cacheKey);
                    var retrievedData = retrieveTask.GetAwaiter().GetResult();

                    // Property: Retrieved data should be identical to stored data
                    return retrievedData != null &&
                           retrievedData.Date == reportData.Date &&
                           retrievedData.TotalSales.Amount == reportData.TotalSales.Amount &&
                           retrievedData.TransactionCount == reportData.TransactionCount;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any concurrent cache operations, data integrity should be maintained.
    /// **Validates: Cache integrity and concurrent access**
    /// </summary>
    [Property(MaxTest = 50)] // Reduced iterations for concurrent testing
    public Property ConcurrentAccess_MultipleOperations_MaintainsIntegrity()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateValidExpiration(),
            (reportData, expiration) =>
            {
                try
                {
                    var tasks = new List<Task>();
                    var cacheKeys = new List<string>();

                    // Create multiple concurrent cache operations
                    for (int i = 0; i < 10; i++)
                    {
                        var key = $"concurrent-test-{i}-{Guid.NewGuid()}";
                        cacheKeys.Add(key);
                        
                        // Store operation
                        tasks.Add(_cacheService.SetCachedReportAsync(key, reportData, expiration));
                    }

                    // Wait for all store operations to complete
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();

                    // Retrieve all stored data concurrently
                    var retrieveTasks = cacheKeys.Select(key => 
                        _cacheService.GetCachedReportAsync<DailySalesReportDto>(key)).ToArray();

                    Task.WaitAll(retrieveTasks);

                    // Property: All concurrent operations should succeed and return correct data
                    var allSuccessful = retrieveTasks.All(t => 
                        t.Result != null && 
                        t.Result.TotalSales.Amount == reportData.TotalSales.Amount);

                    return allSuccessful;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any cache key generation with same inputs, result should be consistent.
    /// **Validates: Cache integrity and concurrent access**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property CacheKeyConsistency_SameInputs_ProducesSameKey()
    {
        return Prop.ForAll(
            GenerateReportType(),
            GenerateParameters(),
            (reportType, parameters) =>
            {
                try
                {
                    var key1 = _cacheService.GenerateCacheKey(reportType, parameters);
                    var key2 = _cacheService.GenerateCacheKey(reportType, parameters);

                    // Property: Same inputs should always produce the same cache key
                    return key1 == key2 && !string.IsNullOrEmpty(key1);
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any cache invalidation pattern, only matching keys should be removed.
    /// **Validates: Cache integrity and concurrent access**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property CacheInvalidation_PatternMatching_OnlyRemovesMatchingKeys()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateValidExpiration(),
            (reportData, expiration) =>
            {
                try
                {
                    // Store data with different key patterns
                    var matchingKey = "report:daily-sales:test123";
                    var nonMatchingKey = "report:time-revenue:test456";
                    
                    var storeTask1 = _cacheService.SetCachedReportAsync(matchingKey, reportData, expiration);
                    var storeTask2 = _cacheService.SetCachedReportAsync(nonMatchingKey, reportData, expiration);
                    
                    Task.WaitAll(storeTask1, storeTask2);

                    // Invalidate with pattern
                    var invalidateTask = _cacheService.InvalidateCacheAsync("report:daily-sales:*");
                    invalidateTask.GetAwaiter().GetResult();

                    // Check results
                    var matchingResult = _cacheService.GetCachedReportAsync<DailySalesReportDto>(matchingKey)
                        .GetAwaiter().GetResult();
                    var nonMatchingResult = _cacheService.GetCachedReportAsync<DailySalesReportDto>(nonMatchingKey)
                        .GetAwaiter().GetResult();

                    // Property: Matching key should be removed, non-matching should remain
                    return matchingResult == null && nonMatchingResult != null;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    /// <summary>
    /// Property: For any cache statistics request, values should be non-negative and consistent.
    /// **Validates: Cache integrity and concurrent access**
    /// </summary>
    [Property(MaxTest = 100)]
    public Property CacheStatistics_AlwaysValid_NonNegativeValues()
    {
        return Prop.ForAll(
            GenerateValidReportData(),
            GenerateValidExpiration(),
            (reportData, expiration) =>
            {
                try
                {
                    // Perform some cache operations
                    var key = $"stats-test-{Guid.NewGuid()}";
                    var storeTask = _cacheService.SetCachedReportAsync(key, reportData, expiration);
                    storeTask.GetAwaiter().GetResult();

                    var retrieveTask = _cacheService.GetCachedReportAsync<DailySalesReportDto>(key);
                    retrieveTask.GetAwaiter().GetResult();

                    // Get statistics
                    var statsTask = _cacheService.GetCacheStatisticsAsync();
                    var stats = statsTask.GetAwaiter().GetResult();

                    // Property: All statistics should be non-negative and hit ratio should be valid
                    return stats.TotalEntries >= 0 &&
                           stats.TotalMemoryUsage >= 0 &&
                           stats.HitCount >= 0 &&
                           stats.MissCount >= 0 &&
                           stats.HitRatio >= 0 &&
                           stats.HitRatio <= 100;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }

    // Generator methods for property-based testing

    private static Arbitrary<string> GenerateValidCacheKey()
    {
        return Arb.From(Gen.Fresh(() => $"test-key-{Guid.NewGuid()}"));
    }

    private static Arbitrary<DailySalesReportDto> GenerateValidReportData()
    {
        return Arb.From(Gen.Fresh(() => CreateSampleDailySalesReport()));
    }

    private static Arbitrary<TimeSpan> GenerateValidExpiration()
    {
        return Arb.From(Gen.Choose(1, 3600).Select(seconds => TimeSpan.FromSeconds(seconds)));
    }

    private static Arbitrary<string> GenerateReportType()
    {
        var reportTypes = new[] { "daily-sales", "time-revenue", "shift-summary", "member-activity" };
        return Arb.From(Gen.Elements(reportTypes));
    }

    private static Arbitrary<object[]> GenerateParameters()
    {
        return Arb.From(
            Gen.Choose(0, 5).SelectMany(count =>
                Gen.ArrayOf(count, Gen.OneOf<object>(
                    Gen.Fresh(() => (object)DateTime.Today.AddDays(Gen.Choose(-30, 0).Sample(0, 1).First())),
                    Gen.Elements("USD", "EUR", "GBP").Select(x => (object)x),
                    Gen.Choose(1, 1000).Select(x => (object)x)
                ))
            )
        );
    }

    private static DailySalesReportDto CreateSampleDailySalesReport()
    {
        return new DailySalesReportDto(
            Date: DateTime.Today,
            TotalSales: new Money(1000m, "USD"),
            TimeBasedSales: new Money(600m, "USD"),
            ProductSales: new Money(400m, "USD"),
            TotalTax: new Money(80m, "USD"),
            TotalGratuity: new Money(120m, "USD"),
            TransactionCount: 25,
            CustomerCount: 20,
            AverageTicketSize: 40m,
            HourlyBreakdown: new[]
            {
                new HourlySalesDto(9, new Money(100m, "USD"), 3, 3),
                new HourlySalesDto(10, new Money(150m, "USD"), 4, 4)
            },
            CategoryBreakdown: new[]
            {
                new CategorySalesDto("Food", new Money(300m, "USD"), 15, 30m)
            },
            PaymentBreakdown: new[]
            {
                new PaymentMethodSalesDto("Cash", new Money(400m, "USD"), 10, 40m)
            },
            TableBreakdown: new[]
            {
                new TableSalesDto(1, "Pool", new Money(200m, "USD"), new Money(100m, "USD"), 
                    new Money(300m, "USD"), TimeSpan.FromHours(4), 2)
            }
        );
    }

    public void Dispose()
    {
        _cacheService?.Dispose();
    }
}