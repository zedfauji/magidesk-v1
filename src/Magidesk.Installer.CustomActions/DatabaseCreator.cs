using Npgsql;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;

namespace Magidesk.Installer.CustomActions;

/// <summary>
/// Custom action for database creation and configuration.
/// Implements IDatabaseCreator interface as defined in design document.
/// </summary>
public class DatabaseCreator : IDatabaseCreator
{
    /// <summary>
    /// Creates the Magidesk database
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string (should connect to 'postgres' database)</param>
    /// <param name="databaseName">Database name to create</param>
    /// <returns>Database creation result</returns>
    public async Task<DatabaseCreationResult> CreateDatabaseAsync(string connectionString, string databaseName)
    {
        try
        {
            // Ensure we're connecting to the postgres database, not the target database
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = "postgres"
            };

            await using var connection = new NpgsqlConnection(builder.ToString());
            await connection.OpenAsync();

            // Check if database already exists
            var checkQuery = "SELECT 1 FROM pg_database WHERE datname = @databaseName";
            await using var checkCmd = new NpgsqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("databaseName", databaseName);

            var exists = await checkCmd.ExecuteScalarAsync();

            if (exists != null)
            {
                // Database already exists - this is a warning condition
                // The installer should prompt the user to drop/recreate
                return new DatabaseCreationResult(
                    false,
                    databaseName,
                    $"Database '{databaseName}' already exists. Please drop the existing database or choose a different name.");
            }

            // Create the database
            // Note: Database names cannot be parameterized in PostgreSQL, but we validate the name
            if (!IsValidDatabaseName(databaseName))
            {
                return new DatabaseCreationResult(
                    false,
                    databaseName,
                    $"Invalid database name: '{databaseName}'. Database names must contain only alphanumeric characters and underscores.");
            }

            var createQuery = $"CREATE DATABASE {databaseName}";
            await using var createCmd = new NpgsqlCommand(createQuery, connection);
            await createCmd.ExecuteNonQueryAsync();

            // Verify database was created
            var verifyQuery = "SELECT 1 FROM pg_database WHERE datname = @databaseName";
            await using var verifyCmd = new NpgsqlCommand(verifyQuery, connection);
            verifyCmd.Parameters.AddWithValue("databaseName", databaseName);

            var verified = await verifyCmd.ExecuteScalarAsync();

            if (verified == null)
            {
                return new DatabaseCreationResult(
                    false,
                    databaseName,
                    $"Database '{databaseName}' was not found after creation. Verification failed.");
            }

            return new DatabaseCreationResult(true, databaseName);
        }
        catch (Exception ex)
        {
            return new DatabaseCreationResult(
                false,
                databaseName,
                $"Failed to create database: {ex.Message}");
        }
    }

    /// <summary>
    /// Writes connection string to configuration file with secure permissions
    /// </summary>
    /// <param name="configPath">Path to appsettings.Production.json</param>
    /// <param name="connectionString">Connection string to write</param>
    /// <returns>Configuration write result</returns>
    public async Task<ConfigurationWriteResult> WriteConnectionStringAsync(string configPath, string connectionString)
    {
        try
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(configPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create or update the configuration file
            var config = new
            {
                ConnectionStrings = new
                {
                    DefaultConnection = connectionString
                }
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonContent = JsonSerializer.Serialize(config, jsonOptions);
            await File.WriteAllTextAsync(configPath, jsonContent);

            // Set secure file permissions (Task 6.3)
            SetSecureFilePermissions(configPath);

            // Verify the connection string can be read back
            var verifyContent = await File.ReadAllTextAsync(configPath);
            if (!verifyContent.Contains(connectionString))
            {
                return new ConfigurationWriteResult(
                    false,
                    configPath,
                    "Connection string verification failed. The written content does not match the expected value.");
            }

            return new ConfigurationWriteResult(true, configPath);
        }
        catch (Exception ex)
        {
            return new ConfigurationWriteResult(
                false,
                configPath,
                $"Failed to write configuration file: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets secure file permissions on the configuration file (Task 6.3)
    /// Restricts access to Administrators, SYSTEM, and NetworkService (read-only)
    /// </summary>
    /// <param name="filePath">Path to the configuration file</param>
    private void SetSecureFilePermissions(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var fileSecurity = fileInfo.GetAccessControl();

        // Remove inherited permissions
        fileSecurity.SetAccessRuleProtection(true, false);

        // Remove all existing rules
        foreach (FileSystemAccessRule rule in fileSecurity.GetAccessRules(true, false, typeof(SecurityIdentifier)))
        {
            fileSecurity.RemoveAccessRule(rule);
        }

        // Grant Administrators full control
        var adminRule = new FileSystemAccessRule(
            new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
            FileSystemRights.FullControl,
            AccessControlType.Allow);
        fileSecurity.AddAccessRule(adminRule);

        // Grant SYSTEM full control
        var systemRule = new FileSystemAccessRule(
            new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
            FileSystemRights.FullControl,
            AccessControlType.Allow);
        fileSecurity.AddAccessRule(systemRule);

        // Grant NetworkService read access (for application)
        var networkServiceRule = new FileSystemAccessRule(
            new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null),
            FileSystemRights.Read,
            AccessControlType.Allow);
        fileSecurity.AddAccessRule(networkServiceRule);

        fileInfo.SetAccessControl(fileSecurity);
    }

    /// <summary>
    /// Validates that a database name contains only safe characters
    /// </summary>
    /// <param name="databaseName">Database name to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    private bool IsValidDatabaseName(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return false;
        }

        // PostgreSQL database names should contain only alphanumeric characters and underscores
        // and should not start with a digit
        return databaseName.All(c => char.IsLetterOrDigit(c) || c == '_') &&
               !char.IsDigit(databaseName[0]);
    }
}

/// <summary>
/// Interface for database creation operations
/// </summary>
public interface IDatabaseCreator
{
    /// <summary>
    /// Creates the Magidesk database
    /// </summary>
    /// <param name="connectionString">PostgreSQL connection string</param>
    /// <param name="databaseName">Database name to create</param>
    /// <returns>Database creation result</returns>
    Task<DatabaseCreationResult> CreateDatabaseAsync(string connectionString, string databaseName);

    /// <summary>
    /// Writes connection string to configuration file
    /// </summary>
    /// <param name="configPath">Path to appsettings.Production.json</param>
    /// <param name="connectionString">Connection string to write</param>
    /// <returns>Configuration write result</returns>
    Task<ConfigurationWriteResult> WriteConnectionStringAsync(string configPath, string connectionString);
}

/// <summary>
/// Result of database creation operation
/// </summary>
/// <param name="Success">Whether the operation succeeded</param>
/// <param name="DatabaseName">Name of the database</param>
/// <param name="ErrorMessage">Error message if operation failed</param>
public record DatabaseCreationResult(bool Success, string DatabaseName, string? ErrorMessage = null);

/// <summary>
/// Result of configuration write operation
/// </summary>
/// <param name="Success">Whether the operation succeeded</param>
/// <param name="ConfigPath">Path to the configuration file</param>
/// <param name="ErrorMessage">Error message if operation failed</param>
public record ConfigurationWriteResult(bool Success, string ConfigPath, string? ErrorMessage = null);
