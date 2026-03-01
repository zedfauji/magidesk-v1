namespace Magidesk.Tests.E2E.Infrastructure.Models;

/// <summary>
/// Represents machine and runtime information for test execution context.
/// </summary>
public sealed record MachineInfo
{
    public required string OperatingSystem { get; init; }
    public required string DotNetVersion { get; init; }
    public required string Architecture { get; init; }
}
