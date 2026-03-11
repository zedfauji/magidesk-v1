namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Represents aggregated test execution statistics for a date range.
/// </summary>
public class TestExecutionStatistics
{
    /// <summary>
    /// The start date of the reporting period.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the reporting period.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The total number of test executions in the period.
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// The number of tests that passed.
    /// </summary>
    public int PassedCount { get; set; }

    /// <summary>
    /// The number of tests that failed.
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// The number of tests that were skipped.
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// The overall pass rate as a decimal (e.g., 0.95 = 95% pass rate).
    /// </summary>
    public double PassRate { get; set; }

    /// <summary>
    /// The average test execution duration in milliseconds.
    /// </summary>
    public double AvgDurationMs { get; set; }

    /// <summary>
    /// The total execution time for all tests in milliseconds.
    /// </summary>
    public long TotalDurationMs { get; set; }

    /// <summary>
    /// The number of unique tests executed in the period.
    /// </summary>
    public int UniqueTestCount { get; set; }
}
