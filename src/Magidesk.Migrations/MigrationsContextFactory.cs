using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Migrations;

/// <summary>
/// Factory for creating DbContext instances during design-time (migrations) for the Migrations assembly.
/// </summary>
public class MigrationsContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(DatabaseConnection.GetConnectionString(),
            x => x.MigrationsAssembly("Magidesk.Migrations"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
