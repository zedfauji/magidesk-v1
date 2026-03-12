# Migration Verification Feature

## Overview

The `MigrationRunner` class now includes comprehensive schema version verification functionality to ensure all expected migrations have been applied to the database after installation.

## New Methods

### 1. `GetAppliedMigrationsAsync(string connectionString)`

Retrieves all applied migrations from the `__EFMigrationsHistory` table.

**Returns:** `List<string>` - List of migration IDs in chronological order

**Example:**
```csharp
var runner = new MigrationRunner(Console.WriteLine);
var appliedMigrations = await runner.GetAppliedMigrationsAsync(connectionString);

Console.WriteLine($"Found {appliedMigrations.Count} migrations:");
foreach (var migration in appliedMigrations)
{
    Console.WriteLine($"  - {migration}");
}
```

### 2. `VerifyMigrationsAsync(string connectionString, List<string>? expectedMigrations = null)`

Verifies that all expected migrations have been applied to the database.

**Parameters:**
- `connectionString` - Database connection string
- `expectedMigrations` - Optional list of expected migration IDs. If null, only verifies that migrations exist.

**Returns:** `MigrationVerificationResult` with:
- `Success` - Whether verification passed
- `Message` - Success or error message
- `AppliedMigrations` - All migrations found in database
- `MissingMigrations` - Expected migrations that were not found

**Example:**
```csharp
var runner = new MigrationRunner(Console.WriteLine);

// Define expected migrations from both projects
var expectedMigrations = new List<string>
{
    // Magidesk.Infrastructure migrations (Dec 2025 - Jan 2, 2026)
    "20251225181547_InitialCreate",
    "20251225183036_PaymentTypesTPH",
    "20251226051133_AddKitchenOrders",
    // ... more migrations ...
    "20260102025436_AddPrinterConfiguration",
    
    // Magidesk.Migrations migrations (Jan 4 - Jan 29, 2026)
    "20260104154305_AddPrinterSupportColumns",
    "20260104171112_AddPrinterDetailedConfiguration",
    // ... more migrations ...
    "20260304163045_AddInventoryCategory_ExtendInventoryItem"
};

var result = await runner.VerifyMigrationsAsync(connectionString, expectedMigrations);

if (result.Success)
{
    Console.WriteLine($"✓ {result.Message}");
    Console.WriteLine($"  Schema version: {result.AppliedMigrations.Last()}");
}
else
{
    Console.WriteLine($"✗ {result.Message}");
    if (result.MissingMigrations.Count > 0)
    {
        Console.WriteLine("  Missing migrations:");
        foreach (var missing in result.MissingMigrations)
        {
            Console.WriteLine($"    - {missing}");
        }
    }
}
```

## Usage in Installer

After running migrations with `ExecuteMigrationsAsync`, the installer should verify the schema:

```csharp
// Step 1: Execute migrations
var migrationResult = await runner.ExecuteMigrationsAsync(bundlePath, connectionString);

if (!migrationResult.Success)
{
    // Handle migration failure
    return;
}

// Step 2: Verify all migrations were applied
var verificationResult = await runner.VerifyMigrationsAsync(
    connectionString, 
    expectedMigrations);

if (!verificationResult.Success)
{
    // Log verification failure
    session.Log($"Migration verification failed: {verificationResult.Message}");
    
    // Display error to user
    MessageBox.Show(
        $"Database migration verification failed.\n\n{verificationResult.Message}\n\nPlease check the installation log.",
        "Installation Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    
    return ActionResult.Failure;
}

// Step 3: Log success
session.Log($"Migration verification passed. Schema version: {verificationResult.AppliedMigrations.Last()}");
```

## Verification Logic

The verification performs the following checks:

1. **Existence Check**: Verifies that the `__EFMigrationsHistory` table contains at least one migration
2. **Completeness Check**: If expected migrations are provided, verifies all are present in the database
3. **Version Check**: Verifies the latest migration in the database matches the expected latest migration
4. **Detailed Reporting**: Returns lists of applied and missing migrations for troubleshooting

## Error Handling

The verification method handles errors gracefully:
- Returns `Success = false` with descriptive error message
- Logs all errors through the configured log action
- Never throws exceptions (returns error result instead)
- Provides detailed context for troubleshooting

## Requirements Satisfied

This implementation satisfies **Requirement 8.3**:
> "WHEN all migrations complete successfully, THE Migration_Runner SHALL verify the schema version matches the application version"

The verification ensures:
- All migrations from both `Magidesk.Infrastructure` and `Magidesk.Migrations` projects are applied
- The schema version (latest migration) matches the expected version
- Missing migrations are detected and reported
