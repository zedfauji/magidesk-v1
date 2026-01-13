using System.Runtime;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services.Reports;

/// <summary>
/// Service for optimizing memory usage when processing large datasets in reports.
/// </summary>
public class MemoryOptimizationService : IMemoryOptimizationService
{
    private readonly ILogger<MemoryOptimizationService> _logger;
    private const int DefaultChunkSize = 1000;
    private const decimal MemoryPressureThreshold = 85.0m; // 85% memory usage threshold

    public MemoryOptimizationService(ILogger<MemoryOptimizationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes large datasets in chunks to avoid memory issues.
    /// </summary>
    public async Task<TResult> ProcessInChunksAsync<T, TResult>(
        Func<int, int, CancellationToken, Task<IEnumerable<T>>> dataSource,
        Func<IEnumerable<T>, CancellationToken, Task<TResult>> processor,
        Func<IEnumerable<TResult>, TResult> aggregator,
        int chunkSize = DefaultChunkSize,
        CancellationToken cancellationToken = default)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        if (processor == null) throw new ArgumentNullException(nameof(processor));
        if (aggregator == null) throw new ArgumentNullException(nameof(aggregator));
        if (chunkSize <= 0) throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));

        _logger.LogDebug("Starting chunked processing with chunk size: {ChunkSize}", chunkSize);

        var results = new List<TResult>();
        var offset = 0;
        var processedCount = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check memory pressure before processing each chunk
                if (IsUnderMemoryPressure())
                {
                    _logger.LogWarning("System under memory pressure, forcing garbage collection");
                    ForceGarbageCollection();
                    
                    // Wait a bit for GC to complete
                    await Task.Delay(100, cancellationToken);
                }

                // Get the next chunk of data
                var chunk = await dataSource(offset, chunkSize, cancellationToken);
                var chunkList = chunk.ToList();

                if (!chunkList.Any())
                {
                    _logger.LogDebug("No more data to process, stopping at offset: {Offset}", offset);
                    break;
                }

                // Process the chunk
                var chunkResult = await processor(chunkList, cancellationToken);
                results.Add(chunkResult);

                processedCount += chunkList.Count;
                offset += chunkSize;

                _logger.LogDebug("Processed chunk: {ProcessedCount} total items, offset: {Offset}", 
                    processedCount, offset);

                // Clear the chunk from memory
                chunkList.Clear();
            }

            // Aggregate all results
            var finalResult = aggregator(results);
            
            _logger.LogInformation("Completed chunked processing: {ProcessedCount} total items processed", 
                processedCount);
            
            return finalResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chunked processing at offset: {Offset}", offset);
            throw;
        }
        finally
        {
            // Clean up results list
            results.Clear();
            
            // Force garbage collection after processing
            if (processedCount > chunkSize * 10) // Only for large datasets
            {
                ForceGarbageCollection();
            }
        }
    }

    /// <summary>
    /// Streams large datasets without loading everything into memory.
    /// </summary>
    public async IAsyncEnumerable<T> StreamDataAsync<T>(
        Func<int, int, CancellationToken, Task<IEnumerable<T>>> dataSource,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));

        _logger.LogDebug("Starting data streaming");

        var offset = 0;
        const int streamChunkSize = 500; // Smaller chunks for streaming
        var totalStreamed = 0;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check memory pressure periodically
                if (totalStreamed > 0 && totalStreamed % 5000 == 0 && IsUnderMemoryPressure())
                {
                    _logger.LogWarning("Memory pressure detected during streaming, forcing GC");
                    ForceGarbageCollection();
                    await Task.Delay(50, cancellationToken);
                }

                var chunk = await dataSource(offset, streamChunkSize, cancellationToken);
                var chunkList = chunk.ToList();

                if (!chunkList.Any())
                {
                    _logger.LogDebug("Streaming completed, total items: {TotalStreamed}", totalStreamed);
                    break;
                }

                foreach (var item in chunkList)
                {
                    yield return item;
                    totalStreamed++;
                }

                offset += streamChunkSize;
                
                // Clear chunk from memory immediately
                chunkList.Clear();
            }
        }
        finally
        {
            _logger.LogDebug("Data streaming finished, streamed {TotalStreamed} items", totalStreamed);
        }
    }

    /// <summary>
    /// Gets current memory usage statistics.
    /// </summary>
    public MemoryUsageInfo GetMemoryUsage()
    {
        try
        {
            var totalMemory = GC.GetTotalMemory(false);
            
            // Get generation collection counts
            var gen0Collections = GC.CollectionCount(0);
            var gen1Collections = GC.CollectionCount(1);
            var gen2Collections = GC.CollectionCount(2);

            // Estimate available memory (this is approximate)
            var workingSet = Environment.WorkingSet;
            var availableMemory = Math.Max(0, workingSet - totalMemory);
            var usagePercentage = workingSet > 0 ? (decimal)totalMemory / workingSet * 100 : 0;

            var memoryInfo = new MemoryUsageInfo(
                TotalMemoryBytes: workingSet,
                UsedMemoryBytes: totalMemory,
                AvailableMemoryBytes: availableMemory,
                UsagePercentage: Math.Round(usagePercentage, 2),
                Gen0Collections: gen0Collections,
                Gen1Collections: gen1Collections,
                Gen2Collections: gen2Collections
            );

            _logger.LogDebug("Memory usage: {UsedMB} MB used, {UsagePercentage}% of working set", 
                totalMemory / 1024 / 1024, memoryInfo.UsagePercentage);

            return memoryInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting memory usage information");
            
            // Return default values on error
            return new MemoryUsageInfo(0, 0, 0, 0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Forces garbage collection to free up memory.
    /// </summary>
    public void ForceGarbageCollection()
    {
        try
        {
            var beforeMemory = GC.GetTotalMemory(false);
            
            _logger.LogDebug("Forcing garbage collection, memory before: {BeforeMemoryMB} MB", 
                beforeMemory / 1024 / 1024);

            // Force full garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var afterMemory = GC.GetTotalMemory(false);
            var freedMemory = beforeMemory - afterMemory;

            _logger.LogDebug("Garbage collection completed, freed: {FreedMemoryMB} MB, memory after: {AfterMemoryMB} MB", 
                freedMemory / 1024 / 1024, afterMemory / 1024 / 1024);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forced garbage collection");
        }
    }

    /// <summary>
    /// Checks if the system is under memory pressure.
    /// </summary>
    public bool IsUnderMemoryPressure()
    {
        try
        {
            var memoryInfo = GetMemoryUsage();
            var isUnderPressure = memoryInfo.UsagePercentage > MemoryPressureThreshold;

            if (isUnderPressure)
            {
                _logger.LogWarning("System under memory pressure: {UsagePercentage}% usage", 
                    memoryInfo.UsagePercentage);
            }

            return isUnderPressure;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking memory pressure");
            return false; // Assume no pressure on error
        }
    }
}