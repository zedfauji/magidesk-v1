namespace Magidesk.Application.DTOs;

/// <summary>
/// Database configuration settings for PostgreSQL connection
/// </summary>
public class DatabaseConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string DatabaseName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Builds a PostgreSQL connection string from the configuration
    /// </summary>
    public string ToConnectionString()
    {
        return $"Host={Host};Port={Port};Database={DatabaseName};Username={Username};Password={Password};";
    }

    /// <summary>
    /// Validates that all required fields are populated
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               Port > 0 &&
               !string.IsNullOrWhiteSpace(DatabaseName) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}
