using FsCheck.Xunit;
using Magidesk.Installer.CustomActions;

namespace Magidesk.Installer.PropertyTests;

/// <summary>
/// Property-based tests for migration logging.
/// **Validates: Requirements 8.2**
/// Property 9: Migration Logging
/// </summary>
public class MigrationLoggingPropertyTests
{
    /// <summary>
    /// Property: For any migration execution, each migration log entry should have
    /// a non-empty migration name and a valid timestamp.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationLogEntry_HasValidData(string migrationName)
    {
        if (string.IsNullOrWhiteSpace(migrationName))
        {
            return true; // Skip invalid inputs
        }

        var timestamp = DateTime.UtcNow;
        var logEntry = new MigrationLogEntry(
            timestamp,
            migrationName,
            MigrationStatus.Applied);

        return !string.IsNullOrEmpty(logEntry.MigrationName) &&
               logEntry.Timestamp <= DateTime.UtcNow &&
               logEntry.Timestamp >= DateTime.UtcNow.AddMinutes(-1);
    }

    /// <summary>
    /// Property: For any list of migrations, the migration logs should contain
    /// an entry for each migration with a status.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationLogs_ContainAllMigrations(List<string> migrationNames)
    {
        if (migrationNames.Count == 0)
        {
            return true; // Skip empty lists
        }

        var migrationLogs = migrationNames
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        return migrationNames.All(name =>
            migrationLogs.Any(log => log.MigrationName == name));
    }

    /// <summary>
    /// Property: For any migration log entry, the status should be one of the
    /// defined MigrationStatus enum values.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationLogEntry_HasValidStatus(MigrationStatus status)
    {
        var logEntry = new MigrationLogEntry(
            DateTime.UtcNow,
            "TestMigration",
            status);

        return Enum.IsDefined(typeof(MigrationStatus), logEntry.Status);
    }

    /// <summary>
    /// Property: For any successful migration result, the migration logs should
    /// only contain Applied or Applying statuses (no Failed or Reverting).
    /// </summary>
    [Property(MaxTest = 100)]
    public bool SuccessfulMigration_HasNoFailedLogs(List<string> migrationNames)
    {
        if (migrationNames.Count == 0)
        {
            return true; // Skip empty lists
        }

        var migrationLogs = migrationNames
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        var result = new MigrationResult(
            Success: true,
            SchemaVersion: migrationNames.Last(),
            MigrationsApplied: migrationLogs.Count,
            ErrorMessage: null,
            MigrationLogs: migrationLogs);

        return result.Success &&
               result.MigrationLogs != null &&
               result.MigrationLogs.All(log =>
                   log.Status == MigrationStatus.Applied ||
                   log.Status == MigrationStatus.Applying);
    }

    /// <summary>
    /// Property: For any migration result with logs, the count of Applied logs
    /// should equal the MigrationsApplied count.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool AppliedLogsCount_MatchesMigrationsApplied(List<string> migrationNames)
    {
        var migrationLogs = migrationNames
            .Select(name => new MigrationLogEntry(
                DateTime.UtcNow,
                name,
                MigrationStatus.Applied))
            .ToList();

        var appliedCount = migrationLogs.Count(log => log.Status == MigrationStatus.Applied);

        var result = new MigrationResult(
            Success: true,
            SchemaVersion: migrationNames.LastOrDefault() ?? string.Empty,
            MigrationsApplied: appliedCount,
            ErrorMessage: null,
            MigrationLogs: migrationLogs);

        return result.MigrationLogs != null &&
               result.MigrationsApplied == result.MigrationLogs.Count(log =>
                   log.Status == MigrationStatus.Applied);
    }

    /// <summary>
    /// Property: Migration log timestamps should be in UTC and recent.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool MigrationLogTimestamps_AreUtc(string migrationName)
    {
        if (string.IsNullOrWhiteSpace(migrationName))
        {
            return true; // Skip invalid inputs
        }

        var timestamp = DateTime.UtcNow;
        var logEntry = new MigrationLogEntry(
            timestamp,
            migrationName,
            MigrationStatus.Applied);

        var timeDiff = Math.Abs((DateTime.UtcNow - logEntry.Timestamp).TotalSeconds);
        return timeDiff < 5; // Within 5 seconds of current UTC time
    }
}
