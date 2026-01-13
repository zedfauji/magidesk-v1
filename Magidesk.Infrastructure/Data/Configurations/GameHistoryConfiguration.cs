using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using System.Text.Json;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for GameHistory entity.
/// </summary>
public class GameHistoryConfiguration : IEntityTypeConfiguration<GameHistory>
{
    public void Configure(EntityTypeBuilder<GameHistory> builder)
    {
        builder.ToTable("GameHistory", "magidesk");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .IsRequired();

        builder.Property(g => g.SessionId)
            .IsRequired();

        builder.Property(g => g.TableId)
            .IsRequired();

        builder.Property(g => g.GameType)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (GameType)Enum.Parse(typeof(GameType), v));

        builder.Property(g => g.StartTime)
            .IsRequired();

        builder.Property(g => g.EndTime)
            .IsRequired();

        builder.Property(g => g.Duration)
            .IsRequired();

        builder.Property(g => g.PlayerCount)
            .IsRequired();

        builder.Property(g => g.Winner)
            .HasMaxLength(100);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        // Configure Money value object
        builder.OwnsOne(g => g.TotalCharge, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalChargeAmount")
                .HasPrecision(10, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("TotalChargeCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Configure GameData as JSON
        builder.Property(g => g.GameData)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(g => g.SessionId)
            .IsUnique(); // One game history per session

        builder.HasIndex(g => g.TableId);
        builder.HasIndex(g => g.GameType);
        builder.HasIndex(g => g.StartTime);
        builder.HasIndex(g => g.EndTime);
        builder.HasIndex(g => g.PlayerCount);
    }
}