using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("Terminals");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TerminalKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.TerminalKey)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Location)
            .HasMaxLength(100);

        builder.Property(t => t.OpeningBalance)
            .HasPrecision(18, 2);

        builder.Property(t => t.CurrentBalance)
            .HasPrecision(18, 2);

        builder.Property(t => t.DefaultFontSize)
            .HasMaxLength(10);

        builder.Property(t => t.DefaultFontFamily)
            .HasMaxLength(50);
    }
}
