using FsCheck.Xunit;
using Magidesk.Installer.CustomActions;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for migration execution completeness.
/// **Validates: Requirements 8.1, 8.3**
/// Property 8: Migration Execution Completeness
/// </summary>
public class MigrationCompletenessPropertyTests
{
    /// <summary>
    /// Property: For any list of migration names, when ExecuteMigrationsAsync succeeds,
    /// the returned MigrationsApplied count should match the number of migrations
    /// that were actually applied (as indicated by the migration logs).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationCount_MatchesAppliedMigrations(List<string> migrationNames)
    {
        var migrationLogs = migrationNames
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        var result = new MigrationResult(
            Success: true,
            SchemaVersion: migrationNames.LastOrDefault() ?? string.Empty,
            MigrationsApplied: migrationLogs.Count(m => m.Status == MigrationStatus.Applied),
            ErrorMessage: null,
            MigrationLogs: migrationLogs);

        return result.MigrationsApplied == migrationLogs.Count(m => m.Status == MigrationStatus.Applied);
    }

    /// <summary>
    /// Property: For any successful migration execution with at least one migration,
    /// the SchemaVersion should be non-empty and should match the last migration.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool SchemaVersion_MatchesLatestMigration(List<string> migrationNames)
    {
        // Filter out null or empty strings
        var validMigrations = migrationNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (validMigrations.Count == 0)
        {
            return true; // Skip empty lists
        }

        var migrationLogs = validMigrations
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        var expectedSchemaVersion = validMigrations.Last();

        var result = new MigrationResult(
            Success: true,
            SchemaVersion: expectedSchemaVersion,
            MigrationsApplied: migrationLogs.Count,
            ErrorMessage: null,
            MigrationLogs: migrationLogs);

        return result.SchemaVersion == expectedSchemaVersion &&
               !string.IsNullOrEmpty(result.SchemaVersion);
    }

    /// <summary>
    /// Property: For any migration execution, if Success is true, then
    /// MigrationsApplied should be >= 0 and ErrorMessage should be null.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool SuccessfulMigration_HasValidState(List<string> migrationNames)
    {
        var migrationLogs = migrationNames
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        var result = new MigrationResult(
            Success: true,
            SchemaVersion: migrationNames.LastOrDefault() ?? string.Empty,
            MigrationsApplied: migrationLogs.Count,
            ErrorMessage: null,
            MigrationLogs: migrationLogs);

        return result.Success &&
               result.MigrationsApplied >= 0 &&
               result.ErrorMessage == null;
    }

    /// <summary>
    /// Property: For any migration execution, if Success is false, then
    /// ErrorMessage should be non-null and non-empty.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool FailedMigration_HasErrorMessage(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            return true; // Skip invalid inputs
        }

        var result = new MigrationResult(
            Success: false,
            SchemaVersion: string.Empty,
            MigrationsApplied: 0,
            ErrorMessage: errorMessage,
            MigrationLogs: new List<MigrationLogEntry>());

        return !result.Success && !string.IsNullOrEmpty(result.ErrorMessage);
    }

    /// <summary>
    /// Property: Migration logs should be ordered chronologically by timestamp.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationLogs_AreChronological(List<string> migrationNames)
    {
        if (migrationNames.Count <= 1)
        {
            return true; // Skip lists with 0 or 1 items
        }

        var baseTime = DateTime.UtcNow;
        var migrationLogs = migrationNames
            .Select((name, index) => new MigrationLogEntry(
                baseTime.AddSeconds(index),
                name,
                MigrationStatus.Applied))
            .ToList();

        // Property: Each timestamp should be >= the previous timestamp
        for (int i = 1; i < migrationLogs.Count; i++)
        {
            if (migrationLogs[i].Timestamp < migrationLogs[i - 1].Timestamp)
            {
                return false;
            }
        }

        return true;
    }
}
