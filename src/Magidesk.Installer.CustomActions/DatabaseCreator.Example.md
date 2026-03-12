# DatabaseCreator Usage Example

This document demonstrates how to use the `DatabaseCreator` class in the installer.

## Overview

The `DatabaseCreator` class implements the `IDatabaseCreator` interface and provides:
1. Database creation with existence checking
2. Connection string generation and writing to appsettings.Production.json
3. Secure file permissions on the configuration file

## Usage Example

```csharp
using Magidesk.Installer.CustomActions;

// Create an instance of DatabaseCreator
var creator = new DatabaseCreator();

// Step 1: Create the database
var connectionString = "Host=127.0.0.1;Port=5432;Database=postgres;Username=postgres;Password=your_generated_password";
var databaseName = "magidesk_pos";

var createResult = await creator.CreateDatabaseAsync(connectionString, databaseName);

if (!createResult.Success)
{
    // Handle error - database might already exist or connection failed
    Console.WriteLine($"Database creation failed: {createResult.ErrorMessage}");
    
    // If database exists, you might want to prompt user to drop/recreate
    if (createResult.ErrorMessage.Contains("already exists"))
    {
        // Show dialog to user asking if they want to drop and recreate
    }
    return;
}

Console.WriteLine($"Database '{createResult.DatabaseName}' created successfully");

// Step 2: Generate connection string for the new database
var appConnectionString = $"Host=127.0.0.1;Port=5432;Database={databaseName};Username=postgres;Password=your_generated_password";

// Step 3: Write connection string to configuration file
var configPath = @"C:\Program Files\Magidesk\appsettings.Production.json";
var writeResult = await creator.WriteConnectionStringAsync(configPath, appConnectionString);

if (!writeResult.Success)
{
    Console.WriteLine($"Configuration write failed: {writeResult.ErrorMessage}");
    return;
}

Console.WriteLine($"Configuration written to: {writeResult.ConfigPath}");
Console.WriteLine("File permissions set: Administrators (Full), SYSTEM (Full), NetworkService (Read)");
```

## Security Features

### File Permissions (Task 6.3)

The `WriteConnectionStringAsync` method automatically sets secure file permissions:

- **Administrators**: Full Control
- **SYSTEM**: Full Control  
- **NetworkService**: Read Only (for the application to read the connection string)
- **Inherited permissions**: Removed
- **All other users**: No access

This ensures that the database password stored in the configuration file is protected from unauthorized access.

## Error Handling

The class returns result objects with clear success/failure indicators:

### DatabaseCreationResult
- `Success`: Boolean indicating if the operation succeeded
- `DatabaseName`: The name of the database
- `ErrorMessage`: Detailed error message if operation failed

### ConfigurationWriteResult
- `Success`: Boolean indicating if the operation succeeded
- `ConfigPath`: Path to the configuration file
- `ErrorMessage`: Detailed error message if operation failed

## Validation

The class includes validation for:

1. **Database name validation**: Ensures database names contain only alphanumeric characters and underscores
2. **Database existence checking**: Detects if database already exists before attempting creation
3. **Connection string verification**: Verifies the written configuration can be read back
4. **Directory creation**: Automatically creates the configuration directory if it doesn't exist

## Requirements Satisfied

- **Requirement 7.1**: Creates magidesk_pos database
- **Requirement 7.2**: Handles case where database already exists
- **Requirement 7.3**: Verifies database creation succeeded
- **Requirement 7.5**: Generates and writes connection string to appsettings.Production.json
- **Requirement 7.6**: Sets secure file permissions on configuration file
