namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Represents a single test execution record with all captured metadata.
/// </summary>
public class TestExecutionRecord
{
    /// <summary>
    /// Unique identifier for this test execution.
    /// </summary>
    public Guid ExecutionId { get; set; }

    /// <summary>
    /// The name of the test that was executed.
    /// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// The test category (e.g., FinancialSafety, OperationalIntegrity, Stability).
    /// </summary>
    public string TestCategory { get; set; } = string.Empty;

    /// <summary>
    /// The test priority (e.g., P0, P1, P2).
    /// </summary>
    public string TestPriority { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the test execution started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// The timestamp when the test execution completed (null if still running).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// The test execution duration in milliseconds (null if not completed).
    /// </summary>
    public int? DurationMs { get; set; }

    /// <summary>
    /// The test result (Passed, Failed, or Skipped).
    /// </summary>
    public TestResult Result { get; set; }

    /// <summary>
    /// The failure reason if the test failed (null if passed or skipped).
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// The stack trace if the test failed (null if passed or skipped).
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// The machine name where the test was executed.
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// The operating system version where the test was executed.
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// The .NET framework version used for test execution.
    /// </summary>
    public string? FrameworkVersion { get; set; }
}
