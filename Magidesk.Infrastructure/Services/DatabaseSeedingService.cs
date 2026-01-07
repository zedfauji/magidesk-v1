using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Orchestrates database seeding with progress reporting
/// </summary>
public class DatabaseSeedingService : IDatabaseSeedingService
{
    private readonly ILogger<DatabaseSeedingService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ISystemInitializationService _initService;

    public event EventHandler<SeedingProgressEventArgs>? ProgressChanged;

    public DatabaseSeedingService(
        ILogger<DatabaseSeedingService> logger,
        ApplicationDbContext dbContext,
        ISystemInitializationService initService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _initService = initService;
    }

    public async Task<bool> IsDatabaseSeededAsync()
    {
        try
        {
            // Check if database exists and has data
            var canConnect = await _dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                return false;
            }

            // Check if essential tables have data
            var hasTerminals = await _dbContext.Terminals.AnyAsync();
            var hasRoles = await _dbContext.Roles.AnyAsync();

            return hasTerminals && hasRoles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if database is seeded");
            return false;
        }
    }

    public async Task<SeedingResult> SeedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");
            ReportProgress("Starting database setup...", 0);

            // Step 1: Ensure database exists and apply migrations
            ReportProgress("Creating database and applying migrations...", 10);
            await _dbContext.Database.MigrateAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return SeedingResult.Failed("Seeding cancelled by user");
            }

            // Step 2: Check if already seeded
            ReportProgress("Checking existing data...", 30);
            var isSeeded = await IsDatabaseSeededAsync();
            
            if (isSeeded)
            {
                _logger.LogWarning("Database is already seeded");
                ReportProgress("Database already contains data", 100);
                return SeedingResult.Successful("Database is already seeded");
            }

            // Step 3: Seed reference data using existing initialization service
            ReportProgress("Seeding reference data...", 50);
            await _initService.SeedReferenceDataAsync();

            if (cancellationToken.IsCancellationRequested)
            {
                return SeedingResult.Failed("Seeding cancelled by user");
            }

            // Step 4: Verify seeding
            ReportProgress("Verifying data...", 90);
            var verifySeeded = await IsDatabaseSeededAsync();
            
            if (!verifySeeded)
            {
                return SeedingResult.Failed("Seeding completed but verification failed");
            }

            ReportProgress("Database setup complete!", 100);
            _logger.LogInformation("Database seeding completed successfully");

            return SeedingResult.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database seeding");
            ReportProgress($"Error: {ex.Message}", 0);
            
            return SeedingResult.Failed(
                "Database seeding failed. Please check the logs for details.",
                ex.ToString()
            );
        }
    }

    private void ReportProgress(string message, int percentComplete)
    {
        _logger.LogInformation("Seeding progress: {Percent}% - {Message}", percentComplete, message);
        ProgressChanged?.Invoke(this, new SeedingProgressEventArgs
        {
            Message = message,
            PercentComplete = percentComplete
        });
    }
}
