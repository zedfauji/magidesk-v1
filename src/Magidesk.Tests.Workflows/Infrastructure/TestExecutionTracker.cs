using System.Runtime.InteropServices;
using Npgsql;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Implements test execution tracking with PostgreSQL database operations using Npgsql.
/// Captures test execution history, failure details, and environment metadata for analysis.
/// </summary>
public class TestExecutionTracker : ITestExecutionTracker
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the TestExecutionTracker class.
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string for test execution database.</param>
    /// <exception cref="ArgumentNullException">Thrown when connectionString is null or empty.</exception>
    public TestExecutionTracker(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        }

        _connectionString = connectionString;
    }

    /// <inheritdoc />
    public async Task<Guid> StartTestExecutionAsync(string testName, string category, string priority)
    {
        if (string.IsNullOrWhiteSpace(testName))
        {
            throw new ArgumentNullException(nameof(testName), "Test name cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentNullException(nameof(category), "Category cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentNullException(nameof(priority), "Priority cannot be null or empty.");
        }

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO test_executions (
                    test_name, 
                    test_category, 
                    test_priority, 
                    started_at, 
                    machine_name, 
                    os_version, 
                    framework_version,
                    result
                )
                VALUES (
                    @testName, 
                    @category, 
                    @priority, 
                    @startedAt, 
                    @machineName, 
                    @osVersion, 
                    @frameworkVersion,
                    'Skipped'
                )
                RETURNING execution_id";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@testName", testName);
            command.Parameters.AddWithValue("@category", category);
            command.Parameters.AddWithValue("@priority", priority);
            command.Parameters.AddWithValue("@startedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@machineName", Environment.MachineName);
            command.Parameters.AddWithValue("@osVersion", GetOsVersion());
            command.Parameters.AddWithValue("@frameworkVersion", GetFrameworkVersion());

            var executionId = await command.ExecuteScalarAsync();
            return (Guid)executionId!;
        }
        catch (NpgsqlException ex)
        {
            throw new InvalidOperationException(
                $"Failed to start test execution tracking for test '{testName}'. Database connection error: {ex.Message}", 
                ex);
        }
    }

    /// <inheritdoc />
    public async Task CompleteTestExecutionAsync(Guid executionId, TestResult result, string? failureReason = null)
    {
        if (executionId == Guid.Empty)
        {
            throw new ArgumentException("Execution ID cannot be empty.", nameof(executionId));
        }

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // First, get the started_at timestamp to calculate duration
            const string selectSql = "SELECT started_at FROM test_executions WHERE execution_id = @executionId";
            await using var selectCommand = new NpgsqlCommand(selectSql, connection);
            selectCommand.Parameters.AddWithValue("@executionId", executionId);

            var startedAt = await selectCommand.ExecuteScalarAsync();
            if (startedAt == null)
            {
                throw new InvalidOperationException($"Test execution with ID '{executionId}' not found.");
            }

            var startTime = (DateTime)startedAt;
            var completedAt = DateTime.UtcNow;
            var durationMs = (int)(completedAt - startTime).TotalMilliseconds;

            // Parse failure reason to extract stack trace if present
            string? failureMessage = null;
            string? stackTrace = null;

            if (!string.IsNullOrWhiteSpace(failureReason))
            {
                var parts = failureReason.Split(new[] { "\n---\n" }, StringSplitOptions.None);
                failureMessage = parts[0];
                stackTrace = parts.Length > 1 ? parts[1] : null;
            }

            const string updateSql = @"
                UPDATE test_executions 
                SET 
                    completed_at = @completedAt,
                    duration_ms = @durationMs,
                    result = @result,
                    failure_reason = @failureReason,
                    stack_trace = @stackTrace
                WHERE execution_id = @executionId";

            await using var updateCommand = new NpgsqlCommand(updateSql, connection);
            updateCommand.Parameters.AddWithValue("@executionId", executionId);
            updateCommand.Parameters.AddWithValue("@completedAt", completedAt);
            updateCommand.Parameters.AddWithValue("@durationMs", durationMs);
            updateCommand.Parameters.AddWithValue("@result", result.ToString());
            updateCommand.Parameters.AddWithValue("@failureReason", (object?)failureMessage ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@stackTrace", (object?)stackTrace ?? DBNull.Value);

            var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Failed to update test execution with ID '{executionId}'.");
            }
        }
        catch (NpgsqlException ex)
        {
            throw new InvalidOperationException(
                $"Failed to complete test execution tracking for execution ID '{executionId}'. Database connection error: {ex.Message}", 
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TestExecutionRecord>> GetTestHistoryAsync(string testName, int count = 10)
    {
        if (string.IsNullOrWhiteSpace(testName))
        {
            throw new ArgumentNullException(nameof(testName), "Test name cannot be null or empty.");
        }

        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero.", nameof(count));
        }

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    execution_id,
                    test_name,
                    test_category,
                    test_priority,
                    started_at,
                    completed_at,
                    duration_ms,
                    result,
                    failure_reason,
                    stack_trace,
                    machine_name,
                    os_version,
                    framework_version
                FROM test_executions
                WHERE test_name = @testName
                ORDER BY started_at DESC
                LIMIT @count";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@testName", testName);
            command.Parameters.AddWithValue("@count", count);

            var records = new List<TestExecutionRecord>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                records.Add(new TestExecutionRecord
                {
                    ExecutionId = reader.GetGuid(0),
                    TestName = reader.GetString(1),
                    TestCategory = reader.GetString(2),
                    TestPriority = reader.GetString(3),
                    StartedAt = reader.GetDateTime(4),
                    CompletedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    DurationMs = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    Result = Enum.Parse<TestResult>(reader.GetString(7)),
                    FailureReason = reader.IsDBNull(8) ? null : reader.GetString(8),
                    StackTrace = reader.IsDBNull(9) ? null : reader.GetString(9),
                    MachineName = reader.IsDBNull(10) ? null : reader.GetString(10),
                    OsVersion = reader.IsDBNull(11) ? null : reader.GetString(11),
                    FrameworkVersion = reader.IsDBNull(12) ? null : reader.GetString(12)
                });
            }

            return records;
        }
        catch (NpgsqlException ex)
        {
            throw new InvalidOperationException(
                $"Failed to retrieve test history for test '{testName}'. Database connection error: {ex.Message}", 
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FlakyTestReport>> GetFlakyTestsAsync(int minExecutions = 10, double failureThreshold = 0.1)
    {
        if (minExecutions <= 0)
        {
            throw new ArgumentException("Minimum executions must be greater than zero.", nameof(minExecutions));
        }

        if (failureThreshold < 0 || failureThreshold > 1)
        {
            throw new ArgumentException("Failure threshold must be between 0 and 1.", nameof(failureThreshold));
        }

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    test_name,
                    total_executions,
                    failure_count,
                    failure_rate,
                    last_execution,
                    avg_duration_ms
                FROM flaky_tests
                WHERE total_executions >= @minExecutions
                  AND failure_rate >= @failureThreshold
                ORDER BY failure_rate DESC, total_executions DESC";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@minExecutions", minExecutions);
            command.Parameters.AddWithValue("@failureThreshold", failureThreshold);

            var reports = new List<FlakyTestReport>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reports.Add(new FlakyTestReport
                {
                    TestName = reader.GetString(0),
                    TotalExecutions = reader.GetInt32(1),
                    FailureCount = reader.GetInt32(2),
                    FailureRate = reader.GetDouble(3),
                    LastExecution = reader.GetDateTime(4),
                    AvgDurationMs = reader.GetDouble(5)
                });
            }

            return reports;
        }
        catch (NpgsqlException ex)
        {
            throw new InvalidOperationException(
                $"Failed to retrieve flaky tests. Database connection error: {ex.Message}", 
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<TestExecutionStatistics> GetTestStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be before or equal to end date.", nameof(startDate));
        }

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT 
                    COUNT(*) as total_executions,
                    SUM(CASE WHEN result = 'Passed' THEN 1 ELSE 0 END) as passed_count,
                    SUM(CASE WHEN result = 'Failed' THEN 1 ELSE 0 END) as failed_count,
                    SUM(CASE WHEN result = 'Skipped' THEN 1 ELSE 0 END) as skipped_count,
                    AVG(duration_ms) as avg_duration_ms,
                    SUM(duration_ms) as total_duration_ms,
                    COUNT(DISTINCT test_name) as unique_test_count
                FROM test_executions
                WHERE started_at >= @startDate 
                  AND started_at <= @endDate
                  AND completed_at IS NOT NULL";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var totalExecutions = reader.GetInt32(0);
                var passedCount = reader.GetInt32(1);
                var failedCount = reader.GetInt32(2);
                var skippedCount = reader.GetInt32(3);
                var avgDurationMs = reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4);
                var totalDurationMs = reader.IsDBNull(5) ? 0L : reader.GetInt64(5);
                var uniqueTestCount = reader.GetInt32(6);

                var passRate = totalExecutions > 0 
                    ? (double)passedCount / totalExecutions 
                    : 0.0;

                return new TestExecutionStatistics
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalExecutions = totalExecutions,
                    PassedCount = passedCount,
                    FailedCount = failedCount,
                    SkippedCount = skippedCount,
                    PassRate = passRate,
                    AvgDurationMs = avgDurationMs,
                    TotalDurationMs = totalDurationMs,
                    UniqueTestCount = uniqueTestCount
                };
            }

            // Return empty statistics if no data found
            return new TestExecutionStatistics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalExecutions = 0,
                PassedCount = 0,
                FailedCount = 0,
                SkippedCount = 0,
                PassRate = 0.0,
                AvgDurationMs = 0.0,
                TotalDurationMs = 0,
                UniqueTestCount = 0
            };
        }
        catch (NpgsqlException ex)
        {
            throw new InvalidOperationException(
                $"Failed to retrieve test statistics for date range {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}. Database connection error: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Gets the operating system version string.
    /// </summary>
    private static string GetOsVersion()
    {
        return $"{Environment.OSVersion.Platform} {Environment.OSVersion.Version}";
    }

    /// <summary>
    /// Gets the .NET framework version string.
    /// </summary>
    private static string GetFrameworkVersion()
    {
        return RuntimeInformation.FrameworkDescription;
    }
}
