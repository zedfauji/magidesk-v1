namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for database seeding orchestration
/// </summary>
public interface IDatabaseSeedingService
{
    /// <summary>
    /// Event raised when seeding progress changes
    /// </summary>
    event EventHandler<SeedingProgressEventArgs>? ProgressChanged;

    /// <summary>
    /// Checks if the database has been seeded with initial data
    /// </summary>
    Task<bool> IsDatabaseSeededAsync();

    /// <summary>
    /// Triggers database seeding process
    /// </summary>
    Task<SeedingResult> SeedDatabaseAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Progress information for database seeding
/// </summary>
public class SeedingProgressEventArgs : EventArgs
{
    public string Message { get; set; } = string.Empty;
    public int PercentComplete { get; set; }
}

/// <summary>
/// Result of database seeding operation
/// </summary>
public class SeedingResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorDetails { get; set; }

    public static SeedingResult Successful(string message = "Database seeded successfully!")
    {
        return new SeedingResult { Success = true, Message = message };
    }

    public static SeedingResult Failed(string message, string? errorDetails = null)
    {
        return new SeedingResult
        {
            Success = false,
            Message = message,
            ErrorDetails = errorDetails
        };
    }
}
