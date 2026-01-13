namespace Magidesk.Application.Interfaces;

/// <summary>
/// Interface for memory optimization when handling large datasets in reports.
/// </summary>
public interface IMemoryOptimizationService
{
    /// <summary>
    /// Processes large datasets in chunks to avoid memory issues.
    /// </summary>
    /// <typeparam name="T">The type of data being processed</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="dataSource">The data source function</param>
    /// <param name="processor">The processing function for each chunk</param>
    /// <param name="aggregator">The function to aggregate results from chunks</param>
    /// <param name="chunkSize">The size of each chunk</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The aggregated result</returns>
    Task<TResult> ProcessInChunksAsync<T, TResult>(
        Func<int, int, CancellationToken, Task<IEnumerable<T>>> dataSource,
        Func<IEnumerable<T>, CancellationToken, Task<TResult>> processor,
        Func<IEnumerable<TResult>, TResult> aggregator,
        int chunkSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams large datasets without loading everything into memory.
    /// </summary>
    /// <typeparam name="T">The type of data being streamed</typeparam>
    /// <param name="dataSource">The data source function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of data items</returns>
    IAsyncEnumerable<T> StreamDataAsync<T>(
        Func<int, int, CancellationToken, Task<IEnumerable<T>>> dataSource,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current memory usage statistics.
    /// </summary>
    /// <returns>Memory usage information</returns>
    MemoryUsageInfo GetMemoryUsage();

    /// <summary>
    /// Forces garbage collection to free up memory.
    /// </summary>
    void ForceGarbageCollection();

    /// <summary>
    /// Checks if the system is under memory pressure.
    /// </summary>
    /// <returns>True if under memory pressure, false otherwise</returns>
    bool IsUnderMemoryPressure();
}

/// <summary>
/// Information about current memory usage.
/// </summary>
public record MemoryUsageInfo(
    long TotalMemoryBytes,
    long UsedMemoryBytes,
    long AvailableMemoryBytes,
    decimal UsagePercentage,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections
);