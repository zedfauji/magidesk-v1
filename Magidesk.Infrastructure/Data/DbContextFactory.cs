using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Magidesk.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances during design-time (migrations).
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(DatabaseConnection.GetConnectionString());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

