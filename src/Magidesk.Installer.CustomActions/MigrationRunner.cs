using Npgsql;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for executing EF Core migrations.
/// Implements IMigrationRunner interface as defined in design document.
/// </summary>
public class MigrationRunner : IMigrationRunner
{
    private readonly Action<string>? _logAction;

    /// <summary>
    /// Initializes a new instance of the MigrationRunner class
    /// </summary>
    /// <param name="logAction">Optional action to log messages during migration execution</param>
    public MigrationRunner(Action<string>? logAction = null)
    {
        _logAction = logAction;
    }

    /// <summary>
    /// Executes EF Core migrations using efbundle.exe
    /// </summary>
    /// <param name="bundlePath">Path to efbundle.exe</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Migration execution result</returns>
    public async Task<MigrationResult> ExecuteMigrationsAsync(string bundlePath, string connectionString)
    {
        var migrationLogs = new List<MigrationLogEntry>();

        try
        {
            Log("Starting migration execution...");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(bundlePath))
            {
                var errorMsg = "Bundle path cannot be null or empty.";
                Log($"ERROR: {errorMsg}");
                return new MigrationResult(
                    false,
                    string.Empty,
                    0,
                    errorMsg,
                    migrationLogs);
            }

            if (!File.Exists(bundlePath))
            {
                var errorMsg = $"Migration bundle not found at path: {bundlePath}";
                Log($"ERROR: {errorMsg}");
                return new MigrationResult(
                    false,
                    string.Empty,
                    0,
                    errorMsg,
                    migrationLogs);
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var errorMsg = "Connection string cannot be null or empty.";
                Log($"ERROR: {errorMsg}");
                return new MigrationResult(
                    false,
                    string.Empty,
                    0,
                    errorMsg,
                    migrationLogs);
            }

            Log($"Migration bundle path: {bundlePath}");
            Log("Executing efbundle.exe...");

            // Configure process to execute efbundle.exe
            var startInfo = new ProcessStartInfo
            {
                FileName = bundlePath,
                Arguments = $"--connection \"{connectionString}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Execute the migration bundle
            using var process = Process.Start(startInfo);
            if (process == null)
            {
                var errorMsg = "Failed to start migration bundle process.";
                Log($"ERROR: {errorMsg}");
                return new MigrationResult(
                    false,
                    string.Empty,
                    0,
                    errorMsg,
                    migrationLogs);
            }

            // Capture and parse output line by line for real-time logging
            var outputLines = new List<string>();
            var errorLines = new List<string>();

            // Read output asynchronously
            string? outputLine;
            while ((outputLine = await process.StandardOutput.ReadLineAsync()) != null)
            {
                outputLines.Add(outputLine);
                ProcessOutputLine(outputLine, migrationLogs);
            }

            // Read error output asynchronously
            string? errorLine;
            while ((errorLine = await process.StandardError.ReadLineAsync()) != null)
            {
                errorLines.Add(errorLine);
                Log($"STDERR: {errorLine}");
            }

            await process.WaitForExitAsync();

            var output = string.Join(Environment.NewLine, outputLines);
            var error = string.Join(Environment.NewLine, errorLines);

            // Parse exit code (0 = success, non-zero = failure)
            if (process.ExitCode == 0)
            {
                Log("Migration execution completed successfully.");

                // Get the current schema version
                Log("Retrieving current schema version...");
                var schemaVersion = await GetSchemaVersionAsync(connectionString);
                Log($"Current schema version: {schemaVersion}");

                // Count successful migrations
                var migrationsApplied = migrationLogs.Count(m => m.Status == MigrationStatus.Applied);
                Log($"Total migrations applied: {migrationsApplied}");

                return new MigrationResult(true, schemaVersion, migrationsApplied, null, migrationLogs);
            }
            else
            {
                // Migration failed
                var errorMessage = string.IsNullOrWhiteSpace(error) ? output : error;
                Log($"ERROR: Migration failed with exit code {process.ExitCode}");
                Log($"ERROR: {errorMessage}");

                return new MigrationResult(
                    false,
                    string.Empty,
                    0,
                    $"Migration failed with exit code {process.ExitCode}: {errorMessage}",
                    migrationLogs);
            }
        }
        catch (Exception ex)
        {
            var errorMsg = $"Exception during migration execution: {ex.Message}";
            Log($"ERROR: {errorMsg}");
            Log($"Stack trace: {ex.StackTrace}");

            return new MigrationResult(
                false,
                string.Empty,
                0,
                errorMsg,
                migrationLogs);
        }
    }

    /// <summary>
    /// Processes a single output line from efbundle.exe and extracts migration information
    /// </summary>
    /// <param name="line">Output line from efbundle.exe</param>
    /// <param name="migrationLogs">List to add migration log entries to</param>
    private void ProcessOutputLine(string line, List<MigrationLogEntry> migrationLogs)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        // Log the raw output
        Log($"STDOUT: {line}");

        // Parse migration-related lines
        // EF Core bundle outputs lines like:
        // "Applying migration '20260129_MigrationName'."
        // "Applied migration '20260129_MigrationName'."
        // "Reverting migration '20260129_MigrationName'."
        // "No migrations were applied. The database is already up to date."

        var applyingMatch = Regex.Match(line, @"Applying migration '([^']+)'", RegexOptions.IgnoreCase);
        if (applyingMatch.Success)
        {
            var migrationName = applyingMatch.Groups[1].Value;
            var entry = new MigrationLogEntry(
                DateTime.UtcNow,
                migrationName,
                MigrationStatus.Applying);
            migrationLogs.Add(entry);
            Log($"[MIGRATION] Applying: {migrationName}");
            return;
        }

        var appliedMatch = Regex.Match(line, @"Applied migration '([^']+)'", RegexOptions.IgnoreCase);
        if (appliedMatch.Success)
        {
            var migrationName = appliedMatch.Groups[1].Value;
            
            // Update the existing entry if it exists, otherwise create a new one
            var existingEntry = migrationLogs.FirstOrDefault(m => 
                m.MigrationName == migrationName && m.Status == MigrationStatus.Applying);
            
            if (existingEntry != null)
            {
                migrationLogs.Remove(existingEntry);
            }

            var entry = new MigrationLogEntry(
                DateTime.UtcNow,
                migrationName,
                MigrationStatus.Applied);
            migrationLogs.Add(entry);
            Log($"[MIGRATION] Applied: {migrationName}");
            return;
        }

        var revertingMatch = Regex.Match(line, @"Reverting migration '([^']+)'", RegexOptions.IgnoreCase);
        if (revertingMatch.Success)
        {
            var migrationName = revertingMatch.Groups[1].Value;
            var entry = new MigrationLogEntry(
                DateTime.UtcNow,
                migrationName,
                MigrationStatus.Reverting);
            migrationLogs.Add(entry);
            Log($"[MIGRATION] Reverting: {migrationName}");
            return;
        }

        // Check for "no migrations" message
        if (line.Contains("No migrations were applied", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("already up to date", StringComparison.OrdinalIgnoreCase))
        {
            Log("[MIGRATION] Database is already up to date. No migrations to apply.");
        }
    }

    /// <summary>
    /// Logs a message using the configured log action
    /// </summary>
    /// <param name="message">Message to log</param>
    private void Log(string message)
    {
        _logAction?.Invoke($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}");
    }

    /// <summary>
    /// Verifies the current schema version by querying __EFMigrationsHistory table
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Current schema version (latest migration ID)</returns>
    public async Task<string> GetSchemaVersionAsync(string connectionString)
    {
        try
        {
            Log("Querying __EFMigrationsHistory table for schema version...");
            
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Query the __EFMigrationsHistory table for the latest migration
            var query = @"
                SELECT ""MigrationId"" 
                FROM ""__EFMigrationsHistory"" 
                ORDER BY ""MigrationId"" DESC 
                LIMIT 1";

            await using var command = new NpgsqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();

            var version = result?.ToString() ?? string.Empty;
            Log($"Schema version retrieved: {version}");
            
            return version;
        }
        catch (Exception ex)
        {
            // If the table doesn't exist or query fails, return empty string
            Log($"WARNING: Failed to retrieve schema version: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Retrieves all applied migrations from the __EFMigrationsHistory table
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>List of applied migration IDs</returns>
    public async Task<List<string>> GetAppliedMigrationsAsync(string connectionString)
    {
        var appliedMigrations = new List<string>();

        try
        {
            Log("Querying __EFMigrationsHistory table for all applied migrations...");
            
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Query all migrations from the history table
            var query = @"
                SELECT ""MigrationId"" 
                FROM ""__EFMigrationsHistory"" 
                ORDER BY ""MigrationId"" ASC";

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var migrationId = reader.GetString(0);
                appliedMigrations.Add(migrationId);
            }

            Log($"Found {appliedMigrations.Count} applied migrations in database");
            
            return appliedMigrations;
        }
        catch (Exception ex)
        {
            Log($"WARNING: Failed to retrieve applied migrations: {ex.Message}");
            return appliedMigrations;
        }
    }

    /// <summary>
    /// Verifies that all expected migrations have been applied to the database
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="expectedMigrations">List of expected migration IDs (optional, if null will verify count only)</param>
    /// <returns>Verification result with details</returns>
    public async Task<MigrationVerificationResult> VerifyMigrationsAsync(
        string connectionString, 
        List<string>? expectedMigrations = null)
    {
        try
        {
            Log("Starting migration verification...");

            // Get all applied migrations from the database
            var appliedMigrations = await GetAppliedMigrationsAsync(connectionString);

            if (appliedMigrations.Count == 0)
            {
                var errorMsg = "No migrations found in __EFMigrationsHistory table";
                Log($"ERROR: {errorMsg}");
                return new MigrationVerificationResult(
                    false,
                    errorMsg,
                    appliedMigrations,
                    new List<string>());
            }

            // Get the latest migration (schema version)
            var schemaVersion = appliedMigrations.Last();
            Log($"Current schema version: {schemaVersion}");

            // If expected migrations are provided, verify them
            if (expectedMigrations != null && expectedMigrations.Count > 0)
            {
                Log($"Verifying {expectedMigrations.Count} expected migrations...");

                // Find missing migrations
                var missingMigrations = expectedMigrations
                    .Except(appliedMigrations)
                    .ToList();

                if (missingMigrations.Count > 0)
                {
                    var errorMsg = $"Missing {missingMigrations.Count} expected migration(s)";
                    Log($"ERROR: {errorMsg}");
                    foreach (var missing in missingMigrations)
                    {
                        Log($"  - Missing: {missing}");
                    }

                    return new MigrationVerificationResult(
                        false,
                        errorMsg,
                        appliedMigrations,
                        missingMigrations);
                }

                // Verify the latest migration matches the expected latest
                var expectedLatest = expectedMigrations.Last();
                if (schemaVersion != expectedLatest)
                {
                    var errorMsg = $"Schema version mismatch. Expected: {expectedLatest}, Actual: {schemaVersion}";
                    Log($"ERROR: {errorMsg}");
                    return new MigrationVerificationResult(
                        false,
                        errorMsg,
                        appliedMigrations,
                        new List<string>());
                }

                Log($"All {expectedMigrations.Count} expected migrations verified successfully");
            }
            else
            {
                Log($"No expected migrations provided. Verified {appliedMigrations.Count} migrations are present.");
            }

            Log("Migration verification completed successfully");
            return new MigrationVerificationResult(
                true,
                "All migrations verified successfully",
                appliedMigrations,
                new List<string>());
        }
        catch (Exception ex)
        {
            var errorMsg = $"Exception during migration verification: {ex.Message}";
            Log($"ERROR: {errorMsg}");
            return new MigrationVerificationResult(
                false,
                errorMsg,
                new List<string>(),
                new List<string>());
        }
    }

    /// <summary>
    /// Performs rollback by dropping the database
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="databaseName">Database name to drop</param>
    /// <returns>Rollback result</returns>
    public async Task<RollbackResult> RollbackDatabaseAsync(string connectionString, string databaseName)
    {
        try
        {
            Log($"Starting database rollback for '{databaseName}'...");

            // Connect to postgres database (not the one we're dropping)
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = "postgres"
            };

            await using var connection = new NpgsqlConnection(builder.ToString());
            await connection.OpenAsync();

            // Terminate all connections to the target database
            Log($"Terminating active connections to '{databaseName}'...");
            var terminateQuery = @"
                SELECT pg_terminate_backend(pid) 
                FROM pg_stat_activity 
                WHERE datname = @databaseName AND pid <> pg_backend_pid()";

            await using var terminateCmd = new NpgsqlCommand(terminateQuery, connection);
            terminateCmd.Parameters.AddWithValue("databaseName", databaseName);
            await terminateCmd.ExecuteNonQueryAsync();

            Log("Active connections terminated");

            // Drop the database
            Log($"Dropping database '{databaseName}'...");
            var dropQuery = $"DROP DATABASE IF EXISTS \"{databaseName}\"";
            await using var dropCmd = new NpgsqlCommand(dropQuery, connection);
            await dropCmd.ExecuteNonQueryAsync();

            // Verify database was dropped
            var verifyQuery = "SELECT 1 FROM pg_database WHERE datname = @databaseName";
            await using var verifyCmd = new NpgsqlCommand(verifyQuery, connection);
            verifyCmd.Parameters.AddWithValue("databaseName", databaseName);
            var exists = await verifyCmd.ExecuteScalarAsync();

            if (exists != null)
            {
                var errorMsg = $"Database '{databaseName}' still exists after drop command";
                Log($"ERROR: {errorMsg}");
                return new RollbackResult(false, errorMsg);
            }

            Log($"Database '{databaseName}' successfully dropped");
            return new RollbackResult(true, $"Database '{databaseName}' successfully dropped");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Failed to rollback database: {ex.Message}";
            Log($"ERROR: {errorMsg}");
            return new RollbackResult(false, errorMsg);
        }
    }
}

/// <summary>
/// Interface for migration execution operations
/// </summary>
public interface IMigrationRunner
{
    /// <summary>
    /// Executes EF Core migrations using efbundle.exe
    /// </summary>
    /// <param name="bundlePath">Path to efbundle.exe</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Migration execution result</returns>
    Task<MigrationResult> ExecuteMigrationsAsync(string bundlePath, string connectionString);

    /// <summary>
    /// Verifies the current schema version
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Current schema version</returns>
    Task<string> GetSchemaVersionAsync(string connectionString);

    /// <summary>
    /// Retrieves all applied migrations from the __EFMigrationsHistory table
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>List of applied migration IDs</returns>
    Task<List<string>> GetAppliedMigrationsAsync(string connectionString);

    /// <summary>
    /// Verifies that all expected migrations have been applied to the database
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="expectedMigrations">List of expected migration IDs (optional, if null will verify count only)</param>
    /// <returns>Verification result with details</returns>
    Task<MigrationVerificationResult> VerifyMigrationsAsync(
        string connectionString, 
        List<string>? expectedMigrations = null);

    /// <summary>
    /// Performs rollback by dropping the database
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="databaseName">Database name to drop</param>
    /// <returns>Rollback result</returns>
    Task<RollbackResult> RollbackDatabaseAsync(string connectionString, string databaseName);
}

/// <summary>
/// Result of migration execution operation
/// </summary>
/// <param name="Success">Whether the migration succeeded</param>
/// <param name="SchemaVersion">Current schema version after migration</param>
/// <param name="MigrationsApplied">Number of migrations applied</param>
/// <param name="ErrorMessage">Error message if migration failed</param>
/// <param name="MigrationLogs">Detailed log entries for each migration step</param>
public record MigrationResult(
    bool Success,
    string SchemaVersion,
    int MigrationsApplied,
    string? ErrorMessage = null,
    List<MigrationLogEntry>? MigrationLogs = null);

/// <summary>
/// Log entry for a single migration step
/// </summary>
/// <param name="Timestamp">UTC timestamp when the migration step occurred</param>
/// <param name="MigrationName">Name of the migration (e.g., "20260129_InitialCreate")</param>
/// <param name="Status">Status of the migration step</param>
public record MigrationLogEntry(
    DateTime Timestamp,
    string MigrationName,
    MigrationStatus Status);

/// <summary>
/// Status of a migration step
/// </summary>
public enum MigrationStatus
{
    /// <summary>
    /// Migration is currently being applied
    /// </summary>
    Applying,

    /// <summary>
    /// Migration was successfully applied
    /// </summary>
    Applied,

    /// <summary>
    /// Migration is being reverted
    /// </summary>
    Reverting,

    /// <summary>
    /// Migration was reverted
    /// </summary>
    Reverted,

    /// <summary>
    /// Migration failed
    /// </summary>
    Failed
}

/// <summary>
/// Result of migration verification operation
/// </summary>
/// <param name="Success">Whether the verification succeeded</param>
/// <param name="Message">Verification message (error or success)</param>
/// <param name="AppliedMigrations">List of all migrations found in the database</param>
/// <param name="MissingMigrations">List of expected migrations that were not found</param>
public record MigrationVerificationResult(
    bool Success,
    string Message,
    List<string> AppliedMigrations,
    List<string> MissingMigrations);

/// <summary>
/// Result of database rollback operation
/// </summary>
/// <param name="Success">Whether the rollback succeeded</param>
/// <param name="Message">Rollback message (error or success)</param>
public record RollbackResult(
    bool Success,
    string Message);
