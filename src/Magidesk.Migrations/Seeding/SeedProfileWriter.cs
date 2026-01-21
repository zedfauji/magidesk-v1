using System.Text;

namespace Magidesk.Migrations.Seeding;

public static class SeedProfileWriter
{
    public static async Task WriteAsync(SeedResult result, SeedOptions options, CancellationToken cancellationToken = default)
    {
        var repoRoot = Directory.GetCurrentDirectory();
        var outputPath = Path.Combine(repoRoot, "memory", "seed_profile.md");

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        var sb = new StringBuilder();
        sb.AppendLine("# FULL POS Seed Profile");
        sb.AppendLine();
        sb.AppendLine($"- Generated: `{DateTime.UtcNow:O}`");
        sb.AppendLine($"- Target DB: `{result.KeyFacts.GetValueOrDefault("Database", "unknown")}`");
        sb.AppendLine($"- Random seed: `{options.RandomSeed}`");
        sb.AppendLine($"- History days: `{options.HistoryDays}`");
        sb.AppendLine();

        sb.AppendLine("## How to re-run / reset");
        sb.AppendLine();
        sb.AppendLine("- **Reset + full seed**:");
        sb.AppendLine();
        sb.AppendLine("```bash");
        sb.AppendLine("dotnet run --project Magidesk.Migrations -- --reset");
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("- **Safety guard**: reset is blocked unless the database name is `magidesk_new` (use `--force` only if you *really* mean it).");
        sb.AppendLine();

        sb.AppendLine("## Seeded credentials (for end-to-end workflows)");
        sb.AppendLine();
        foreach (var kvp in result.KeyFacts.Where(k => k.Key.StartsWith("Login:", StringComparison.OrdinalIgnoreCase)))
        {
            sb.AppendLine($"- **{kvp.Key.Replace("Login:", "").Trim()}**: {kvp.Value}");
        }
        sb.AppendLine();

        sb.AppendLine("## Seeded data (counts)");
        sb.AppendLine();
        foreach (var kvp in result.Counts.OrderBy(k => k.Key))
        {
            sb.AppendLine($"- **{kvp.Key}**: {kvp.Value}");
        }
        sb.AppendLine();

        sb.AppendLine("## Coverage notes");
        sb.AppendLine();
        sb.AppendLine("- **Master data**: Restaurant config (singleton), terminals, users/roles, printer groups/mappings, order types, shifts, tables, inventory.");
        sb.AppendLine("- **Menu**: ~100+ items across drinks/appetizers/salads/burgers/pizzas/desserts/combos + menu modifier groups and modifiers.");
        sb.AppendLine("- **Tickets**:");
        sb.AppendLine("  - Open dine-in tickets today (with notes/discount snapshots on some).");
        sb.AppendLine("  - Scheduled pickup tickets (future).");
        sb.AppendLine("  - Historical closed tickets across the last N days (cash/card/split tender mix).");
        sb.AppendLine("  - Voided tickets (manager).");
        sb.AppendLine("  - Split tickets (order-line split).");
        sb.AppendLine("  - Partial refunds on a subset of payments.");
        sb.AppendLine("- **Kitchen/KDS**: Kitchen orders are created for a subset of historical tickets with mixed statuses.");
        sb.AppendLine("- **Reports/Labor**: Attendance history for server + cashier across history range.");
        sb.AppendLine();

        sb.AppendLine("## Known limitations (schema / implementation constraints)");
        sb.AppendLine();
        sb.AppendLine("- **Tax rules**: The current schema is primarily item/ticket-level and the Domain ticket totals use a simplified calculation; the seed uses `TaxRate = 0` on order lines to avoid double-taxing. If/when a full tax-rule engine exists, the seed should be updated to match it.");
        sb.AppendLine("- **Discount enforcement**: Manager-only / conditional discount authorization is not represented as a first-class DB relationship; discount snapshots are applied directly to tickets where needed.");
        sb.AppendLine("- **Happy hour / time-based pricing**: Not modeled as first-class entities; not seeded beyond basic items/modifiers.");
        sb.AppendLine();

        await File.WriteAllTextAsync(outputPath, sb.ToString(), cancellationToken);
    }
}


