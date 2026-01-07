using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Manages database configuration with Windows DPAPI encryption
/// </summary>
public class DatabaseConfigurationService : IDatabaseConfigurationService
{
    private readonly ILogger<DatabaseConfigurationService> _logger;
    private readonly string _configFilePath;

    public DatabaseConfigurationService(ILogger<DatabaseConfigurationService> logger)
    {
        _logger = logger;
        
        // Store in LocalAppData (user-specific, survives app updates)
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var magideskFolder = Path.Combine(appDataPath, "Magidesk");
        Directory.CreateDirectory(magideskFolder);
        
        _configFilePath = Path.Combine(magideskFolder, "db.config");
    }

    public Task<bool> HasConfigurationAsync()
    {
        try
        {
            return Task.FromResult(File.Exists(_configFilePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if configuration exists");
            return Task.FromResult(false);
        }
    }

    public async Task<DatabaseConfig?> GetConfigurationAsync()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.LogWarning("Configuration file not found at {Path}", _configFilePath);
                return null;
            }

            // Read encrypted data
            var encryptedData = await File.ReadAllBytesAsync(_configFilePath);

            // Decrypt using Windows DPAPI (machine-specific)
            var decryptedData = ProtectedData.Unprotect(
                encryptedData,
                null,
                DataProtectionScope.CurrentUser
            );

            // Deserialize
            var json = Encoding.UTF8.GetString(decryptedData);
            var config = JsonSerializer.Deserialize<DatabaseConfig>(json);

            _logger.LogInformation("Database configuration loaded successfully");
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading database configuration");
            return null;
        }
    }

    public async Task SaveConfigurationAsync(DatabaseConfig config)
    {
        try
        {
            if (!config.IsValid())
            {
                throw new ArgumentException("Invalid database configuration");
            }

            // Serialize
            var json = JsonSerializer.Serialize(config);
            var dataToEncrypt = Encoding.UTF8.GetBytes(json);

            // Encrypt using Windows DPAPI (machine-specific)
            var encryptedData = ProtectedData.Protect(
                dataToEncrypt,
                null,
                DataProtectionScope.CurrentUser
            );

            // Write to file
            await File.WriteAllBytesAsync(_configFilePath, encryptedData);

            _logger.LogInformation("Database configuration saved successfully to {Path}", _configFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving database configuration");
            throw;
        }
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(DatabaseConfig config)
    {
        try
        {
            if (!config.IsValid())
            {
                return ConnectionTestResult.Failed("Invalid configuration. Please fill in all fields.");
            }

            var connectionString = config.ToConnectionString();

            // Attempt to open connection
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Test with a simple query
            await using var command = new NpgsqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();

            _logger.LogInformation("Database connection test successful for {Host}:{Port}/{Database}",
                config.Host, config.Port, config.DatabaseName);

            return ConnectionTestResult.Successful();
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("password"))
        {
            _logger.LogWarning("Database connection failed: Authentication error");
            return ConnectionTestResult.Failed(
                "Authentication failed. Please check your username and password.",
                ex.Message
            );
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("does not exist"))
        {
            _logger.LogWarning("Database connection failed: Database not found");
            return ConnectionTestResult.Failed(
                $"Database '{config.DatabaseName}' does not exist on the server.",
                ex.Message
            );
        }
        catch (NpgsqlException ex) when (ex.Message.Contains("Connection refused") || ex.Message.Contains("No such host"))
        {
            _logger.LogWarning("Database connection failed: Host unreachable");
            return ConnectionTestResult.Failed(
                $"Cannot reach database server at {config.Host}:{config.Port}. Please check the host and port.",
                ex.Message
            );
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning("Database connection failed: Timeout");
            return ConnectionTestResult.Failed(
                "Connection timeout. The database server is not responding.",
                ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during connection test");
            return ConnectionTestResult.Failed(
                "An unexpected error occurred. Please check the logs for details.",
                ex.Message
            );
        }
    }

    public Task ClearConfigurationAsync()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                File.Delete(_configFilePath);
                _logger.LogInformation("Database configuration cleared");
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing database configuration");
            throw;
        }
    }
}
