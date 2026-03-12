using Npgsql;
using System.Diagnostics;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for database connectivity smoke test.
/// Implements ISmokeTestRunner interface as defined in design document.
/// </summary>
public class SmokeTestRunner : ISmokeTestRunner
{
    private readonly Action<string>? _logAction;

    /// <summary>
    /// Initializes a new instance of the SmokeTestRunner class
    /// </summary>
    /// <param name="logAction">Optional action to log messages during smoke test execution</param>
    public SmokeTestRunner(Action<string>? logAction = null)
    {
        _logAction = logAction;
    }

    /// <summary>
    /// Executes connectivity and schema validation test
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="timeoutSeconds">Test timeout in seconds (default: 10)</param>
    /// <returns>Smoke test result</returns>
    public async Task<SmokeTestResult> ExecuteSmokeTestAsync(
        string connectionString, 
        int timeoutSeconds = 10)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Log("Starting database connectivity smoke test...");
            Log($"Timeout: {timeoutSeconds} seconds");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var errorMsg = "Connection string cannot be null or empty.";
                Log($"ERROR: {errorMsg}");
                return new SmokeTestResult(false, stopwatch.Elapsed, errorMsg);
            }

            // Create cancellation token for timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                // Execute the smoke test query with timeout
                await ExecuteTicketsTableQueryAsync(connectionString, cts.Token);

                stopwatch.Stop();
                Log($"Smoke test completed successfully in {stopwatch.Elapsed.TotalSeconds:F2} seconds");

                return new SmokeTestResult(true, stopwatch.Elapsed, null);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                var errorMsg = $"Smoke test timed out after {timeoutSeconds} seconds";
                Log($"ERROR: {errorMsg}");
                return new SmokeTestResult(false, stopwatch.Elapsed, errorMsg);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var errorMsg = $"Smoke test failed: {ex.Message}";
            Log($"ERROR: {errorMsg}");
            Log($"Stack trace: {ex.StackTrace}");
            return new SmokeTestResult(false, stopwatch.Elapsed, errorMsg);
        }
    }

    /// <summary>
    /// Executes a query against the Tickets table to verify schema and connectivity
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="cancellationToken">Cancellation token for timeout</param>
    private async Task ExecuteTicketsTableQueryAsync(
        string connectionString, 
        CancellationToken cancellationToken)
    {
        Log("Connecting to database...");

        await using var connection = new NpgsqlConnection(connectionString);
        
        // Open connection with timeout support
        await connection.OpenAsync(cancellationToken);
        Log("Database connection established");

        // Query the Tickets table to verify it exists and is accessible
        Log("Verifying Tickets table exists...");
        var query = @"SELECT COUNT(*) FROM ""Tickets""";

        await using var command = new NpgsqlCommand(query, connection);
        command.CommandTimeout = 10; // Additional command-level timeout

        var result = await command.ExecuteScalarAsync(cancellationToken);
        var ticketCount = Convert.ToInt32(result);

        Log($"Tickets table verified. Current record count: {ticketCount}");
    }

    /// <summary>
    /// Logs a message using the configured log action
    /// </summary>
    /// <param name="message">Message to log</param>
    private void Log(string message)
    {
        _logAction?.Invoke($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] [SmokeTest] {message}");
    }
}

/// <summary>
/// Interface for smoke test operations
/// </summary>
public interface ISmokeTestRunner
{
    /// <summary>
    /// Executes connectivity and schema validation test
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="timeoutSeconds">Test timeout in seconds</param>
    /// <returns>Smoke test result</returns>
    Task<SmokeTestResult> ExecuteSmokeTestAsync(
        string connectionString, 
        int timeoutSeconds = 10);
}

/// <summary>
/// Result of smoke test execution
/// </summary>
/// <param name="Success">Whether the smoke test succeeded</param>
/// <param name="ExecutionTime">Time taken to execute the smoke test</param>
/// <param name="ErrorMessage">Error message if smoke test failed</param>
public record SmokeTestResult(
    bool Success,
    TimeSpan ExecutionTime,
    string? ErrorMessage = null);
