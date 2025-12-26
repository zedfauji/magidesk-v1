using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Magidesk.Infrastructure.Data;

// Migrations runner for Magidesk.
// Usage:
//   1) Set MAGIDESK_CONNECTION_STRING (recommended), or
//   2) Put ConnectionStrings:Magidesk in Magidesk.Migrations/appsettings.json
// Then run:
//   dotnet run --project Magidesk.Migrations

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    Environment.GetEnvironmentVariable("MAGIDESK_CONNECTION_STRING")
    ?? configuration.GetConnectionString("Magidesk")
    ?? throw new InvalidOperationException(
        "Missing connection string. Set MAGIDESK_CONNECTION_STRING or configure ConnectionStrings:Magidesk in Magidesk.Migrations/appsettings.json.");

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseNpgsql(connectionString);

await using var db = new ApplicationDbContext(optionsBuilder.Options);

Console.WriteLine("Applying EF Core migrations...");
await db.Database.MigrateAsync();
Console.WriteLine("Done.");
