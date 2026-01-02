namespace Magidesk.Migrations.Seeding;

public sealed record SeedResult(
    Dictionary<string, int> Counts,
    Dictionary<string, string> KeyFacts);


