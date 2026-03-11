using FluentAssertions;
using Magidesk.Tests.Workflows.Infrastructure;
using Npgsql;

namespace Magidesk.Tests.Workflows.Infrastructure;

/// <summary>
/// Unit tests for TestExecutionTracker database operations.
/// Uses test database for isolation and verifies all CRUD operations.
/// 
/// PREREQUISITES:
/// - PostgreSQL database 'magidesk_test' must exist
/// - test_executions and test_artifacts tables must be created (see task 1 SQL migration)
/// - Connection string can be overridden via MAGIDESK_TEST_DB_CONNECTION environment variable
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "Database")]
public class TestExecutionTrackerTests : IAsyncLifetime
{
    private static readonly string TestConnectionString = 
        Environment.GetEnvironmentVariable("MAGIDESK_TEST_DB_CONNECTION") 
        ?? "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres";
    
    private TestExecutionTracker _tracker = null!;
    private NpgsqlConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _tracker = new TestExecutionTracker(TestConnectionString);
        _connection = new NpgsqlConnection(TestConnectionString);
        await _connection.OpenAsync();
        
        // Clean test data before each test
        await CleanTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanTestDataAsync();
        await _connection.DisposeAsync();
    }

    private async Task CleanTestDataAsync()
    {
        const string sql = "DELETE FROM test_executions WHERE test_name LIKE 'UnitTest_%'";
        await using var command = new NpgsqlCommand(sql, _connection);
        await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TestExecutionTracker(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TestExecutionTracker(string.Empty);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("connectionString");
    }

    [Fact]
    public async Task StartTestExecutionAsync_WithValidInputs_CreatesRecordWithCorrectFields()
    {
        // Arrange
        const string testName = "UnitTest_ValidTest";
        const string category = "FinancialSafety";
        const string priority = "P0";

        // Act
        var executionId = await _tracker.StartTestExecutionAsync(testName, category, priority);

        // Assert
        executionId.Should().NotBeEmpty();

        // Verify record in database
        const string sql = @"
            SELECT test_name, test_category, test_priority, started_at, 
                   machine_name, os_version, framework_version, result
            FROM test_executions 
            WHERE execution_id = @executionId";

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@executionId", executionId);

        await using var reader = await command.ExecuteReaderAsync();
        reader.Read().Should().BeTrue();

        reader.GetString(0).Should().Be(testName);
        reader.GetString(1).Should().Be(category);
        reader.GetString(2).Should().Be(priority);
        reader.GetDateTime(3).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        reader.GetString(4).Should().Be(Environment.MachineName);
        reader.GetString(5).Should().NotBeNullOrEmpty();
        reader.GetString(6).Should().NotBeNullOrEmpty();
        reader.GetString(7).Should().Be("Skipped");
    }

    [Fact]
    public async Task StartTestExecutionAsync_WithNullTestName_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _tracker.StartTestExecutionAsync(null!, "Category", "P0");

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("testName");
    }

    [Fact]
    public async Task StartTestExecutionAsync_WithNullCategory_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _tracker.StartTestExecutionAsync("TestName", null!, "P0");

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("category");
    }

    [Fact]
    public async Task StartTestExecutionAsync_WithNullPriority_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _tracker.StartTestExecutionAsync("TestName", "Category", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("priority");
    }

    [Fact]
    public async Task CompleteTestExecutionAsync_WithPassedResult_UpdatesRecordCorrectly()
    {
        // Arrange
        var executionId = await _tracker.StartTestExecutionAsync(
            "UnitTest_PassedTest", "OperationalIntegrity", "P1");

        // Wait a bit to ensure duration is measurable
        await Task.Delay(100);

        // Act
        await _tracker.CompleteTestExecutionAsync(executionId, TestResult.Passed);

        // Assert
        const string sql = @"
            SELECT result, completed_at, duration_ms, failure_reason, stack_trace
            FROM test_executions 
            WHERE execution_id = @executionId";

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@executionId", executionId);

        await using var reader = await command.ExecuteReaderAsync();
        reader.Read().Should().BeTrue();

        reader.GetString(0).Should().Be("Passed");
        reader.GetDateTime(1).Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        reader.GetInt32(2).Should().BeGreaterThan(0);
        reader.IsDBNull(3).Should().BeTrue();
        reader.IsDBNull(4).Should().BeTrue();
    }

    [Fact]
    public async Task CompleteTestExecutionAsync_WithFailedResultAndReason_UpdatesRecordCorrectly()
    {
        // Arrange
        var executionId = await _tracker.StartTestExecutionAsync(
            "UnitTest_FailedTest", "Stability", "P2");

        const string failureReason = "Expected value to be 42 but found 0";
        const string stackTrace = "at TestClass.TestMethod() in TestFile.cs:line 123";
        var failureMessage = $"{failureReason}\n---\n{stackTrace}";

        // Act
        await _tracker.CompleteTestExecutionAsync(executionId, TestResult.Failed, failureMessage);

        // Assert
        const string sql = @"
            SELECT result, failure_reason, stack_trace
            FROM test_executions 
            WHERE execution_id = @executionId";

        await using var command = new NpgsqlCommand(sql, _connection);
        command.Parameters.AddWithValue("@executionId", executionId);

        await using var reader = await command.ExecuteReaderAsync();
        reader.Read().Should().BeTrue();

        reader.GetString(0).Should().Be("Failed");
        reader.GetString(1).Should().Be(failureReason);
        reader.GetString(2).Should().Be(stackTrace);
    }

    [Fact]
    public async Task CompleteTestExecutionAsync_WithEmptyExecutionId_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _tracker.CompleteTestExecutionAsync(
            Guid.Empty, TestResult.Passed);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("executionId");
    }

    [Fact]
    public async Task CompleteTestExecutionAsync_WithNonExistentExecutionId_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var act = async () => await _tracker.CompleteTestExecutionAsync(
            nonExistentId, TestResult.Passed);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Test execution with ID '{nonExistentId}' not found.");
    }

    [Fact]
    public async Task GetTestHistoryAsync_ReturnsCorrectNumberOfRecords()
    {
        // Arrange
        const string testName = "UnitTest_HistoryTest";
        
        // Create 5 test executions
        for (int i = 0; i < 5; i++)
        {
            var executionId = await _tracker.StartTestExecutionAsync(
                testName, "FinancialSafety", "P0");
            await Task.Delay(10); // Ensure different timestamps
            await _tracker.CompleteTestExecutionAsync(
                executionId, i % 2 == 0 ? TestResult.Passed : TestResult.Failed);
        }

        // Act
        var history = await _tracker.GetTestHistoryAsync(testName, count: 3);

        // Assert
        var records = history.ToList();
        records.Should().HaveCount(3);
        records.Should().BeInDescendingOrder(r => r.StartedAt);
    }

    [Fact]
    public async Task GetTestHistoryAsync_ReturnsRecordsWithAllFields()
    {
        // Arrange
        const string testName = "UnitTest_CompleteRecord";
        var executionId = await _tracker.StartTestExecutionAsync(
            testName, "OperationalIntegrity", "P1");
        await _tracker.CompleteTestExecutionAsync(
            executionId, TestResult.Passed);

        // Act
        var history = await _tracker.GetTestHistoryAsync(testName, count: 1);

        // Assert
        var record = history.Single();
        record.ExecutionId.Should().Be(executionId);
        record.TestName.Should().Be(testName);
        record.TestCategory.Should().Be("OperationalIntegrity");
        record.TestPriority.Should().Be("P1");
        record.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        record.CompletedAt.Should().NotBeNull();
        record.DurationMs.Should().BeGreaterThanOrEqualTo(0);
        record.Result.Should().Be(TestResult.Passed);
        record.MachineName.Should().Be(Environment.MachineName);
        record.OsVersion.Should().NotBeNullOrEmpty();
        record.FrameworkVersion.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetTestHistoryAsync_WithNullTestName_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _tracker.GetTestHistoryAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("testName");
    }

    [Fact]
    public async Task GetTestHistoryAsync_WithZeroCount_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _tracker.GetTestHistoryAsync("TestName", count: 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("count");
    }

    [Fact]
    public async Task GetFlakyTestsAsync_IdentifiesFlakyTestsCorrectly()
    {
        // Arrange
        const string flakyTestName = "UnitTest_FlakyTest";
        const string stableTestName = "UnitTest_StableTest";

        // Create flaky test: 10 executions with 20% failure rate
        for (int i = 0; i < 10; i++)
        {
            var executionId = await _tracker.StartTestExecutionAsync(
                flakyTestName, "Stability", "P2");
            await Task.Delay(10);
            var result = i < 2 ? TestResult.Failed : TestResult.Passed;
            await _tracker.CompleteTestExecutionAsync(executionId, result);
        }

        // Create stable test: 10 executions with 0% failure rate
        for (int i = 0; i < 10; i++)
        {
            var executionId = await _tracker.StartTestExecutionAsync(
                stableTestName, "Stability", "P2");
            await Task.Delay(10);
            await _tracker.CompleteTestExecutionAsync(executionId, TestResult.Passed);
        }

        // Act
        var flakyTests = await _tracker.GetFlakyTestsAsync(
            minExecutions: 10, failureThreshold: 0.1);

        // Assert
        var reports = flakyTests.ToList();
        reports.Should().ContainSingle(r => r.TestName == flakyTestName);
        reports.Should().NotContain(r => r.TestName == stableTestName);

        var flakyReport = reports.Single(r => r.TestName == flakyTestName);
        flakyReport.TotalExecutions.Should().Be(10);
        flakyReport.FailureCount.Should().Be(2);
        flakyReport.FailureRate.Should().BeApproximately(0.2, 0.01);
        flakyReport.LastExecution.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        flakyReport.AvgDurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetFlakyTestsAsync_WithInvalidMinExecutions_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _tracker.GetFlakyTestsAsync(minExecutions: 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("minExecutions");
    }

    [Fact]
    public async Task GetFlakyTestsAsync_WithInvalidFailureThreshold_ThrowsArgumentException()
    {
        // Act
        var act = async () => await _tracker.GetFlakyTestsAsync(failureThreshold: 1.5);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("failureThreshold");
    }

    [Fact]
    public async Task GetTestStatisticsAsync_CalculatesStatisticsCorrectly()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(1);

        // Create test executions with known results
        for (int i = 0; i < 10; i++)
        {
            var executionId = await _tracker.StartTestExecutionAsync(
                $"UnitTest_Stats_{i}", "FinancialSafety", "P0");
            await Task.Delay(10);
            
            TestResult result = i switch
            {
                < 7 => TestResult.Passed,
                < 9 => TestResult.Failed,
                _ => TestResult.Skipped
            };
            
            await _tracker.CompleteTestExecutionAsync(executionId, result);
        }

        // Act
        var statistics = await _tracker.GetTestStatisticsAsync(startDate, endDate);

        // Assert
        statistics.StartDate.Should().Be(startDate);
        statistics.EndDate.Should().Be(endDate);
        statistics.TotalExecutions.Should().Be(10);
        statistics.PassedCount.Should().Be(7);
        statistics.FailedCount.Should().Be(2);
        statistics.SkippedCount.Should().Be(1);
        statistics.PassRate.Should().BeApproximately(0.7, 0.01);
        statistics.AvgDurationMs.Should().BeGreaterThanOrEqualTo(0);
        statistics.TotalDurationMs.Should().BeGreaterThanOrEqualTo(0);
        statistics.UniqueTestCount.Should().Be(10);
    }

    [Fact]
    public async Task GetTestStatisticsAsync_WithInvalidDateRange_ThrowsArgumentException()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act
        var act = async () => await _tracker.GetTestStatisticsAsync(startDate, endDate);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("startDate");
    }

    [Fact]
    public async Task GetTestStatisticsAsync_WithNoData_ReturnsEmptyStatistics()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddYears(-10);
        var endDate = startDate.AddDays(1);

        // Act
        var statistics = await _tracker.GetTestStatisticsAsync(startDate, endDate);

        // Assert
        statistics.TotalExecutions.Should().Be(0);
        statistics.PassedCount.Should().Be(0);
        statistics.FailedCount.Should().Be(0);
        statistics.SkippedCount.Should().Be(0);
        statistics.PassRate.Should().Be(0.0);
        statistics.AvgDurationMs.Should().Be(0.0);
        statistics.TotalDurationMs.Should().Be(0);
        statistics.UniqueTestCount.Should().Be(0);
    }
}
