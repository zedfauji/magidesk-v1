using Microsoft.EntityFrameworkCore;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Migrations.Seeding;

internal static class SeedGuards
{
    // Conservative: if any of these have data, we're not in a clean seed environment.
    public static async Task<bool> HasAnyPosDataAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken)) return true;
        if (await db.Roles.AnyAsync(cancellationToken)) return true;
        if (await db.MenuItems.AnyAsync(cancellationToken)) return true;
        if (await db.Tickets.AnyAsync(cancellationToken)) return true;
        if (await db.Payments.AnyAsync(cancellationToken)) return true;
        if (await db.CashSessions.AnyAsync(cancellationToken)) return true;
        if (await db.Tables.AnyAsync(cancellationToken)) return true;
        return false;
    }
}


