using System;
using System.IO;
using System.Text.Json;
using Magidesk.Tests.E2E.Infrastructure.Exceptions;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Manages test configuration from environment variables and appsettings.test.json.
/// Configuration sources are checked in priority order: environment variables, config file, defaults.
/// </summary>
public sealed class ConfigurationManager
{
    private const string EnvDbConnection = "MAGIDESK_TEST_DB_CONNECTION";
    private const string EnvAppPath = "MAGIDESK_APP_PATH";
    private const string EnvTimeoutMultiplier = "MAGIDESK_TEST_TIMEOUT_MULTIPLIER";
    private const string EnvArtifactsDir = "MAGIDESK_TEST_ARTIFACTS_DIR";
    private const string ConfigFileName = "appsettings.test.json";

    public string DatabaseConnectionString { get; private init; }
    public string? ApplicationPath { get; private init; }
    public double TimeoutMultiplier { get; private init; }
    public string ArtifactsDirectory { get; private init; }

    private ConfigurationManager(string databaseConnectionString, string? applicationPath, double timeoutMultiplier, string artifactsDirectory)
    {
        DatabaseConnectionString = databaseConnectionString;
        ApplicationPath = applicationPath;
        TimeoutMultiplier = timeoutMultiplier;
        ArtifactsDirectory = artifactsDirectory;
    }

    /// <summary>
    /// Loads configuration from environment variables and appsettings.test.json.
    /// Priority: environment variables > config file > defaults.
    /// </summary>
    public static ConfigurationManager Load()
    {
        var configFilePath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
        TestConfigurationFile? fileConfig = null;

        if (File.Exists(configFilePath))
        {
            try
            {
                var json = File.ReadAllText(configFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                fileConfig = JsonSerializer.Deserialize<TestConfigurationFile>(json, options);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(
                    $"Failed to parse configuration file '{configFilePath}': {ex.Message}",
                    ConfigFileName);
            }
        }

        // Load configuration with priority: env vars > config file > defaults
        var dbConnection = Environment.GetEnvironmentVariable(EnvDbConnection)
            ?? fileConfig?.TestConfiguration?.DatabaseConnectionString
            ?? string.Empty;

        var appPath = Environment.GetEnvironmentVariable(EnvAppPath)
            ?? fileConfig?.TestConfiguration?.ApplicationPath;

        var timeoutMultiplierStr = Environment.GetEnvironmentVariable(EnvTimeoutMultiplier);
        var timeoutMultiplier = 1.0;
        if (!string.IsNullOrEmpty(timeoutMultiplierStr))
        {
            if (!double.TryParse(timeoutMultiplierStr, out timeoutMultiplier))
            {
                throw new ConfigurationException(
                    $"Environment variable '{EnvTimeoutMultiplier}' has invalid value '{timeoutMultiplierStr}'. Expected a numeric value.",
                    EnvTimeoutMultiplier);
            }
        }
        else if (fileConfig?.TestConfiguration?.TimeoutMultiplier != null)
        {
            timeoutMultiplier = fileConfig.TestConfiguration.TimeoutMultiplier;
        }

        var artifactsDir = Environment.GetEnvironmentVariable(EnvArtifactsDir)
            ?? fileConfig?.TestConfiguration?.ArtifactsDirectory
            ?? "TestResults/";

        var config = new ConfigurationManager(dbConnection, appPath, timeoutMultiplier, artifactsDir);
        config.Validate();
        config.LogConfiguration();

        return config;
    }

    /// <summary>
    /// Validates that all required configuration values are present and valid.
    /// Throws ConfigurationException if validation fails.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DatabaseConnectionString))
        {
            throw new ConfigurationException(
                $"Database connection string is required. Set environment variable '{EnvDbConnection}' or configure in '{ConfigFileName}'.",
                EnvDbConnection);
        }

        if (TimeoutMultiplier <= 0 || TimeoutMultiplier > 10)
        {
            throw new ConfigurationException(
                $"Timeout multiplier must be greater than 0 and less than or equal to 10. Current value: {TimeoutMultiplier}",
                EnvTimeoutMultiplier);
        }

        // Validate artifacts directory path
        try
        {
            var fullPath = Path.GetFullPath(ArtifactsDirectory);
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ConfigurationException(
                    $"Artifacts directory path is invalid: '{ArtifactsDirectory}'",
                    EnvArtifactsDir);
            }
        }
        catch (Exception ex) when (ex is not ConfigurationException)
        {
            throw new ConfigurationException(
                $"Artifacts directory path is invalid: '{ArtifactsDirectory}'. {ex.Message}",
                EnvArtifactsDir);
        }

        // Validate application path if provided
        if (!string.IsNullOrWhiteSpace(ApplicationPath))
        {
            if (!File.Exists(ApplicationPath))
            {
                throw new ConfigurationException(
                    $"Application path '{ApplicationPath}' does not exist.",
                    EnvAppPath);
            }
        }
    }

    /// <summary>
    /// Logs configuration values to console, excluding sensitive data.
    /// </summary>
    private void LogConfiguration()
    {
        Console.WriteLine("=== E2E Test Configuration ===");
        Console.WriteLine($"Database Connection: {MaskConnectionString(DatabaseConnectionString)}");
        Console.WriteLine($"Application Path: {ApplicationPath ?? "(auto-detect)"}");
        Console.WriteLine($"Timeout Multiplier: {TimeoutMultiplier}");
        Console.WriteLine($"Artifacts Directory: {ArtifactsDirectory}");
        Console.WriteLine("==============================");
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

        // Mask password in connection string
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

    // JSON deserialization models
    private sealed class TestConfigurationFile
    {
        public TestConfigurationSection? TestConfiguration { get; set; }
    }

    private sealed class TestConfigurationSection
    {
        public string? DatabaseConnectionString { get; set; }
        public string? ApplicationPath { get; set; }
        public double TimeoutMultiplier { get; set; } = 1.0;
        public string? ArtifactsDirectory { get; set; }
    }
}
