namespace Magidesk.Infrastructure.Data;

/// <summary>
/// Database connection configuration.
/// </summary>
public static class DatabaseConnection
{
    /// <summary>
    /// Gets the connection string for the Magidesk POS database.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This is a NEW database (magidesk_new) created specifically for the rebuild.
    /// DO NOT use the legacy database (magidesk_pos).
    /// </remarks>
    public static string GetConnectionString()
    {
        // For local development - passwordless PostgreSQL
        // In production, this should come from configuration/secret management
        // Database: magidesk_new (NEW database, separate from legacy magidesk_pos)
        // Credentials: postgres/postgres
        return "Host=localhost;Port=5432;Database=magidesk_new;Username=postgres;Password=postgres;";
    }
}

