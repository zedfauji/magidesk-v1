namespace Magidesk.Migrations.Seeding;

public sealed record SeedOptions(
    int RandomSeed,
    int HistoryDays,
    int TicketsPerDayMin,
    int TicketsPerDayMax,
    int OpenTicketsToday,
    int ScheduledTicketsToday)
{
    public static SeedOptions FullDefaults() => new(
        RandomSeed: 42,
        HistoryDays: 60,
        TicketsPerDayMin: 6,
        TicketsPerDayMax: 14,
        OpenTicketsToday: 10,
        ScheduledTicketsToday: 3);
}


