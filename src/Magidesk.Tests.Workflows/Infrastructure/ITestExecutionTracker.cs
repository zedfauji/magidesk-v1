namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Tracks test execution history in PostgreSQL database for historical analysis and flaky test detection.
/// </summary>
public interface ITestExecutionTracker
{
    /// <summary>
    /// Records the start of a test execution and returns a unique execution ID.
    /// </summary>
    /// <param name="testName">The name of the test being executed.</param>
    /// <param name="category">The test category (e.g., FinancialSafety, OperationalIntegrity, Stability).</param>
    /// <param name="priority">The test priority (e.g., P0, P1, P2).</param>
    /// <returns>A unique execution ID for tracking this test run.</returns>
    Task<Guid> StartTestExecutionAsync(string testName, string category, string priority);

    /// <summary>
    /// Records the completion of a test execution with result and optional failure details.
    /// </summary>
    /// <param name="executionId">The execution ID returned from StartTestExecutionAsync.</param>
    /// <param name="result">The test result (Passed, Failed, or Skipped).</param>
    /// <param name="failureReason">Optional failure reason and stack trace if the test failed.</param>
    Task CompleteTestExecutionAsync(Guid executionId, TestResult result, string? failureReason = null);

    /// <summary>
    /// Retrieves the execution history for a specific test.
    /// </summary>
    /// <param name="testName">The name of the test to query.</param>
    /// <param name="count">The maximum number of recent executions to return (default: 10).</param>
    /// <returns>A collection of test execution records ordered by most recent first.</returns>
    Task<IEnumerable<TestExecutionRecord>> GetTestHistoryAsync(string testName, int count = 10);

    /// <summary>
    /// Identifies flaky tests based on failure rate thresholds.
    /// </summary>
    /// <param name="minExecutions">Minimum number of executions required to consider a test (default: 10).</param>
    /// <param name="failureThreshold">Minimum failure rate to classify as flaky (default: 0.1 = 10%).</param>
    /// <returns>A collection of flaky test reports with failure statistics.</returns>
    Task<IEnumerable<FlakyTestReport>> GetFlakyTestsAsync(int minExecutions = 10, double failureThreshold = 0.1);

    /// <summary>
    /// Retrieves aggregated test execution statistics for a date range.
    /// </summary>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <returns>Aggregated statistics including pass/fail counts and average duration.</returns>
    Task<TestExecutionStatistics> GetTestStatisticsAsync(DateTime startDate, DateTime endDate);
}
