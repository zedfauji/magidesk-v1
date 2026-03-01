namespace Magidesk.Tests.E2E.Infrastructure.Models;

/// <summary>
/// Represents metadata about a test failure for artifact capture.
/// </summary>
public sealed record FailureArtifact
{
    public required string TestName { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string ExceptionType { get; init; }
    public required string ExceptionMessage { get; init; }
    public required string StackTrace { get; init; }
    public required MachineInfo MachineInfo { get; init; }
}
