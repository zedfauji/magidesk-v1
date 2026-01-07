using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for managing database configuration with secure storage
/// </summary>
public interface IDatabaseConfigurationService
{
    /// <summary>
    /// Checks if database configuration exists in secure storage
    /// </summary>
    Task<bool> HasConfigurationAsync();

    /// <summary>
    /// Retrieves database configuration from secure storage
    /// </summary>
    Task<DatabaseConfig?> GetConfigurationAsync();

    /// <summary>
    /// Saves database configuration to secure storage (encrypted)
    /// </summary>
    Task SaveConfigurationAsync(DatabaseConfig config);

    /// <summary>
    /// Tests database connection without side effects
    /// </summary>
    Task<ConnectionTestResult> TestConnectionAsync(DatabaseConfig config);

    /// <summary>
    /// Removes database configuration from secure storage
    /// </summary>
    Task ClearConfigurationAsync();
}
