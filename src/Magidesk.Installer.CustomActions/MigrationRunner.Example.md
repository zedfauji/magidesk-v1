# MigrationRunner Usage Example

This document demonstrates how to use the `MigrationRunner` class with comprehensive migration logging.

## Basic Usage

```csharp
// Create a migration runner with logging
var logMessages = new List<string>();
var migrationRunner = new MigrationRunner(message => logMessages.Add(message));

// Execute migrations
var result = await migrationRunner.ExecuteMigrationsAsync(
    bundlePath: @"C:\Program Files\Magidesk\tools\efbundle.exe",
    connectionString: "Host=127.0.0.1;Port=5432;Database=magidesk_pos;Username=postgres;Password=SecurePassword123"
);

// Check result
if (result.Success)
{
    Console.WriteLine($"Migrations applied successfully!");
    Console.WriteLine($"Schema version: {result.SchemaVersion}");
    Console.WriteLine($"Migrations applied: {result.MigrationsApplied}");
    
    // Display detailed migration logs
    if (result.MigrationLogs != null)
    {
        foreach (var log in result.MigrationLogs)
        {
            Console.WriteLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] {log.MigrationName} - {log.Status}");
        }
    }
}
else
{
    Console.WriteLine($"Migration failed: {result.ErrorMessage}");
}

// Review all log messages
foreach (var message in logMessages)
{
    Console.WriteLine(message);
}
```

## Usage with File Logging

```csharp
// Create a log file
var logFilePath = Path.Combine(Path.GetTempPath(), "MagideskInstall", $"migration_{DateTime.Now:yyyyMMdd_HHmmss}.log");
Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);

// Create migration runner with file logging
var migrationRunner = new MigrationRunner(message =>
{
    File.AppendAllText(logFilePath, message + Environment.NewLine);
    Console.WriteLine(message); // Also log to console
});

// Execute migrations
var result = await migrationRunner.ExecuteMigrationsAsync(
    bundlePath: @"C:\Program Files\Magidesk\tools\efbundle.exe",
    connectionString: connectionString
);

Console.WriteLine($"Migration log written to: {logFilePath}");
```

## Migration Log Entry Structure

Each migration step is logged with the following information:

- **Timestamp**: UTC timestamp when the migration step occurred
- **MigrationName**: Name of the migration (e.g., "20260129_InitialCreate")
- **Status**: Current status of the migration

### Migration Status Values

- `Applying`: Migration is currently being applied
- `Applied`: Migration was successfully applied
- `Reverting`: Migration is being reverted
- `Reverted`: Migration was reverted
- `Failed`: Migration failed

## Example Log Output

```
[2026-01-29 14:30:15.123] Starting migration execution...
[2026-01-29 14:30:15.125] Migration bundle path: C:\Program Files\Magidesk\tools\efbundle.exe
[2026-01-29 14:30:15.126] Executing efbundle.exe...
[2026-01-29 14:30:16.234] STDOUT: Applying migration '20251201_InitialCreate'.
[2026-01-29 14:30:16.234] [MIGRATION] Applying: 20251201_InitialCreate
[2026-01-29 14:30:17.456] STDOUT: Applied migration '20251201_InitialCreate'.
[2026-01-29 14:30:17.456] [MIGRATION] Applied: 20251201_InitialCreate
[2026-01-29 14:30:17.567] STDOUT: Applying migration '20260104_AddPaymentTables'.
[2026-01-29 14:30:17.567] [MIGRATION] Applying: 20260104_AddPaymentTables
[2026-01-29 14:30:18.789] STDOUT: Applied migration '20260104_AddPaymentTables'.
[2026-01-29 14:30:18.789] [MIGRATION] Applied: 20260104_AddPaymentTables
[2026-01-29 14:30:18.890] Migration execution completed successfully.
[2026-01-29 14:30:18.891] Retrieving current schema version...
[2026-01-29 14:30:18.992] Querying __EFMigrationsHistory table for schema version...
[2026-01-29 14:30:19.123] Schema version retrieved: 20260104_AddPaymentTables
[2026-01-29 14:30:19.124] Current schema version: 20260104_AddPaymentTables
[2026-01-29 14:30:19.125] Total migrations applied: 2
```

## Error Handling

The MigrationRunner includes comprehensive error handling:

```csharp
var result = await migrationRunner.ExecuteMigrationsAsync(bundlePath, connectionString);

if (!result.Success)
{
    // Log the error
    Console.WriteLine($"ERROR: {result.ErrorMessage}");
    
    // Check if any migrations were partially applied
    if (result.MigrationLogs != null && result.MigrationLogs.Any())
    {
        Console.WriteLine("Partially applied migrations:");
        foreach (var log in result.MigrationLogs)
        {
            Console.WriteLine($"  - {log.MigrationName}: {log.Status}");
        }
    }
    
    // Decide on rollback or retry
    // ...
}
```

## Integration with WiX Custom Actions

When using in a WiX custom action, you can integrate with the MSI logging:

```csharp
[CustomAction]
public static ActionResult RunMigrations(Session session)
{
    try
    {
        // Create migration runner with MSI logging
        var migrationRunner = new MigrationRunner(message =>
        {
            session.Log(message);
        });
        
        var bundlePath = session["INSTALLFOLDER"] + @"tools\efbundle.exe";
        var connectionString = session["DB_CONNECTION_STRING"];
        
        var result = migrationRunner.ExecuteMigrationsAsync(bundlePath, connectionString).Result;
        
        if (result.Success)
        {
            session["SCHEMA_VERSION"] = result.SchemaVersion;
            session["MIGRATIONS_APPLIED"] = result.MigrationsApplied.ToString();
            return ActionResult.Success;
        }
        else
        {
            session.Log($"Migration failed: {result.ErrorMessage}");
            return ActionResult.Failure;
        }
    }
    catch (Exception ex)
    {
        session.Log($"Exception in RunMigrations: {ex.Message}");
        return ActionResult.Failure;
    }
}
```

## Notes

- All timestamps are in UTC to ensure consistency across time zones
- The log action is optional; if not provided, no logging will occur
- Migration logs are included in the result even if the migration fails
- The parser handles various EF Core output formats for maximum compatibility
