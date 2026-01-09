using Magidesk.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances during design-time (migrations) and runtime.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly IDatabaseConfigurationService? _configService;
    private readonly ILogger<ApplicationDbContextFactory>? _logger;

    // Parameterless constructor for design-time (migrations)
    public ApplicationDbContextFactory()
    {
    }

    // Constructor for runtime with DI
    public ApplicationDbContextFactory(
        IDatabaseConfigurationService configService,
        ILogger<ApplicationDbContextFactory> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    // Design-time factory method (for migrations)
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            DatabaseConnection.GetConnectionString(),
            npgsqlOptions => npgsqlOptions.MigrationsAssembly("Magidesk.Migrations"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    // Runtime factory method (async)
    public async Task<ApplicationDbContext> CreateDbContextAsync()
    {
        string connectionString;

        if (_configService != null)
        {
            try
            {
                var config = await _configService.GetConfigurationAsync().ConfigureAwait(false);
                if (config != null && config.IsValid())
                {
                    connectionString = config.ToConnectionString();
                    _logger?.LogInformation("Using connection string from DatabaseConfigurationService");
                }
                else
                {
                    // No valid config - use invalid connection string
                    // This will cause operations to fail, triggering setup flow
                    connectionString = "Host=localhost;Port=5432;Database=__setup_required__;Username=__invalid__;Password=;";
                    _logger?.LogWarning("No valid database configuration found, using placeholder connection string");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting database configuration, using placeholder");
                connectionString = "Host=localhost;Port=5432;Database=__setup_required__;Username=__invalid__;Password=;";
            }
        }
        else
        {
            // Fallback for when service is not available (shouldn't happen at runtime)
            connectionString = DatabaseConnection.GetConnectionString();
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.MigrationsAssembly("Magidesk.Migrations"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    // Runtime factory method (sync)
    public ApplicationDbContext CreateDbContext()
    {
        return CreateDbContextAsync().GetAwaiter().GetResult();
    }
}

