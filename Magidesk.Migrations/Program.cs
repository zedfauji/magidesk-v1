using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Magidesk.Application.DependencyInjection;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.DependencyInjection;
using Magidesk.Infrastructure.Security;
using Magidesk.Migrations.Seeding;

// FULL POS DATABASE SEEDING (resettable)
// Usage:
//   dotnet run --project Magidesk.Migrations -- --reset
//
// Safety:
// - --reset will TRUNCATE ALL TABLES (CASCADE) in the target database.
// - By default, reset is only allowed for database name "magidesk_new".
//   Use --force to override the guard (not recommended).

var reset = args.Any(a => a.Equals("--reset", StringComparison.OrdinalIgnoreCase));
var force = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
var help = args.Any(a => a is "--help" or "-h" or "/?");

if (help)
{
    Console.WriteLine("Magidesk.Migrations");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  --reset        Truncate all tables (CASCADE) and reseed a full POS dataset");
    Console.WriteLine("  --force        Allow --reset even if DB name is not 'magidesk_new' (DANGEROUS)");
    Console.WriteLine();
    Console.WriteLine("Environment:");
    Console.WriteLine("  MAGIDESK_CONNECTION_STRING  Optional override. Defaults to magidesk_new.");
    return;
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    Environment.GetEnvironmentVariable("MAGIDESK_CONNECTION_STRING")
    ?? configuration.GetConnectionString("Magidesk")
    ?? "Host=localhost;Port=5432;Database=magidesk_new;Username=postgres;Password=postgres;";

// Minimal host for DI-based handlers/services (no UI)
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddApplication();
        services.AddInfrastructure();

        // Override Infrastructure's DbContext registration to ensure we seed the same DB we migrate/reset.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("Magidesk.Infrastructure")));
    })
    .Build();

await using var scope = host.Services.CreateAsyncScope();
var services = scope.ServiceProvider;

var db = services.GetRequiredService<ApplicationDbContext>();

Console.WriteLine("Applying EF Core migrations...");
await db.Database.MigrateAsync();
Console.WriteLine("Migrations applied.");

if (reset)
{
    Console.WriteLine("RESET requested: truncating all POS tables...");
    await DbResetter.ResetAsync(db, expectedDatabaseName: "magidesk_new", force: force);
    Console.WriteLine("Reset complete.");
}
else
{
    var hasAnyData = await SeedGuards.HasAnyPosDataAsync(db);
    if (hasAnyData)
{
        Console.WriteLine("Database contains existing POS data.");
        Console.WriteLine("Refusing to run FULL seed without --reset (to prevent collisions).");
        Console.WriteLine("Run: dotnet run --project Magidesk.Migrations -- --reset");
        Environment.ExitCode = 2;
        return;
    }
}

// Encryption service for PINs and gateway credentials
    var inMemConfig = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"Security:EncryptionKey", "MagideskDevKey_MustChangeInProd!"}
        })
        .Build();
    var encryptionService = new AesEncryptionService(inMemConfig);

Console.WriteLine("Seeding FULL realistic POS dataset...");
var options = SeedOptions.FullDefaults();
var result = await FullPosSeeder.SeedAsync(services, db, encryptionService, options);

Console.WriteLine("Writing seed profile to /memory/seed_profile.md ...");
await SeedProfileWriter.WriteAsync(result, options);
Console.WriteLine("Done.");
