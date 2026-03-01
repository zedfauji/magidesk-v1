namespace Magidesk.Tests.E2E.Infrastructure.Models;

/// <summary>
/// Represents the state of a process at a point in time.
/// </summary>
public sealed record ProcessState
{
    public required int ProcessId { get; init; }
    public required long WorkingSetMemoryMB { get; init; }
    public required double CpuUsagePercent { get; init; }
    public required int ThreadCount { get; init; }
    public required TimeSpan TotalProcessorTime { get; init; }
}
