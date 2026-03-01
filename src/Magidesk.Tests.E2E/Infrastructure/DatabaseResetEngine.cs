using System;
using System.Diagnostics;
using System.IO;
using Npgsql;
using Magidesk.Tests.E2E.Infrastructure.Exceptions;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Restores database to clean baseline state before each test.
/// Executes SQL scripts to delete transactional data and seed minimum required configuration.
/// </summary>
public sealed class DatabaseResetEngine
{
    private const int PerformanceWarningThresholdSeconds = 5;
    private readonly string _connectionString;
    private readonly string _resetScriptPath;
    private readonly string _seedScriptPath;

    /// <summary>
    /// Initializes a new instance of DatabaseResetEngine.
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string from ConfigurationManager.</param>
    public DatabaseResetEngine(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;

        // Locate SQL scripts relative to assembly location
        var assemblyDir = AppContext.BaseDirectory;
        var scriptsDir = Path.Combine(assemblyDir, "Scripts");
        _resetScriptPath = Path.Combine(scriptsDir, "reset-database.sql");
        _seedScriptPath = Path.Combine(scriptsDir, "seed-test-data.sql");

        ValidateScriptFiles();
    }

    /// <summary>
    /// Resets the database to clean state by deleting transactional data and seeding baseline configuration.
    /// Executes within a transaction to ensure atomicity.
    /// Logs warning if reset exceeds 5 seconds.
    /// </summary>
    /// <exception cref="DatabaseResetException">Thrown when database reset fails.</exception>
    public void ResetDatabase()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Execute reset script (delete transactional data)
                ExecuteScript(connection, transaction, _resetScriptPath);

                // Execute seed script (insert baseline configuration)
                ExecuteScript(connection, transaction, _seedScriptPath);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            stopwatch.Stop();

            // Log performance warning if reset exceeds threshold
            if (stopwatch.Elapsed.TotalSeconds > PerformanceWarningThresholdSeconds)
            {
                Console.WriteLine(
                    $"WARNING: Database reset took {stopwatch.Elapsed.TotalSeconds:F2} seconds, " +
                    $"exceeds {PerformanceWarningThresholdSeconds}s target. " +
                    "Consider optimizing reset scripts or database performance.");
            }
        }
        catch (Exception ex)
        {
            throw new DatabaseResetException(
                $"Failed to reset database. Connection: {MaskConnectionString(_connectionString)}. " +
                $"Error: {ex.Message}",
                _connectionString,
                ex);
        }
    }

    /// <summary>
    /// Validates that required SQL script files exist.
    /// </summary>
    private void ValidateScriptFiles()
    {
        if (!File.Exists(_resetScriptPath))
        {
            throw new FileNotFoundException(
                $"Database reset script not found at '{_resetScriptPath}'. " +
                "Ensure Scripts/reset-database.sql is included in the test project.",
                _resetScriptPath);
        }

        if (!File.Exists(_seedScriptPath))
        {
            throw new FileNotFoundException(
                $"Database seed script not found at '{_seedScriptPath}'. " +
                "Ensure Scripts/seed-test-data.sql is included in the test project.",
                _seedScriptPath);
        }
    }

    /// <summary>
    /// Executes a SQL script file within the provided transaction.
    /// </summary>
    private static void ExecuteScript(NpgsqlConnection connection, NpgsqlTransaction transaction, string scriptPath)
    {
        var sql = File.ReadAllText(scriptPath);

        using var command = new NpgsqlCommand(sql, connection, transaction);
        command.CommandTimeout = 30; // 30 second timeout for script execution
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Masks sensitive information in connection string for logging.
    /// </summary>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return "(empty)";
        }

        var parts = connectionString.Split(';');
        var masked = new System.Collections.Generic.List<string>();

        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("Pwd=", StringComparison.OrdinalIgnoreCase))
            {
                var keyValue = trimmed.Split('=', 2);
                masked.Add($"{keyValue[0]}=***");
            }
            else
            {
                masked.Add(trimmed);
            }
        }

        return string.Join("; ", masked);
    }
}
