namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Represents a report of a flaky test with failure statistics.
/// A flaky test is one that fails intermittently without code changes.
/// </summary>
public class FlakyTestReport
{
    /// <summary>
    /// The name of the flaky test.
    /// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// The total number of executions for this test in the analysis period.
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// The number of times this test failed in the analysis period.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// The failure rate as a decimal (e.g., 0.15 = 15% failure rate).
    /// </summary>
    public double FailureRate { get; set; }

    /// <summary>
    /// The timestamp of the most recent execution of this test.
    /// </summary>
    public DateTime LastExecution { get; set; }

    /// <summary>
    /// The average execution duration in milliseconds across all runs.
    /// </summary>
    public double AvgDurationMs { get; set; }
}
