using System.Data;
using Microsoft.EntityFrameworkCore;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Migrations.Seeding;

internal static class DbResetter
{
    public static async Task ResetAsync(
        ApplicationDbContext db,
        string expectedDatabaseName,
        bool force,
        CancellationToken cancellationToken = default)
    {
        await db.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            var currentDb = await ExecuteScalarStringAsync(db, "select current_database();", cancellationToken);
            if (!force && !string.Equals(currentDb, expectedDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Refusing to reset database '{currentDb}'. Expected '{expectedDatabaseName}'. " +
                    "Pass --force to override (DANGEROUS).");
            }

            // Collect all user tables across non-system schemas (e.g., 'magidesk', 'public'),
            // excluding EF migrations history.
            var tables = await QueryQualifiedTablesAsync(
                db,
                cancellationToken);

            if (tables.Count == 0)
            {
                return;
            }

            // TRUNCATE ... CASCADE to handle FK dependencies safely.
            var sql = $"TRUNCATE TABLE {string.Join(", ", tables)} RESTART IDENTITY CASCADE;";
            await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }

    private static async Task<string> ExecuteScalarStringAsync(
        ApplicationDbContext db,
        string sql,
        CancellationToken cancellationToken)
    {
        await using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return Convert.ToString(result) ?? string.Empty;
    }

    private static async Task<List<string>> QueryQualifiedTablesAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        await using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText =
            "select schemaname, tablename " +
            "from pg_tables " +
            "where schemaname not in ('pg_catalog','information_schema') " +
            "and tablename <> '__EFMigrationsHistory';";
        cmd.CommandType = CommandType.Text;

        var results = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var schema = reader.GetString(0);
            var table = reader.GetString(1);
            results.Add($"\"{schema.Replace("\"", "\"\"")}\".\"{table.Replace("\"", "\"\"")}\"");
        }

        return results;
    }
}


