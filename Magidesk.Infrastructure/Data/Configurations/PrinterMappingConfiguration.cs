using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class PrinterMappingConfiguration : IEntityTypeConfiguration<PrinterMapping>
{
    public void Configure(EntityTypeBuilder<PrinterMapping> builder)
    {
        builder.ToTable("PrinterMappings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TerminalId)
            .IsRequired();

        builder.Property(x => x.PrinterGroupId)
            .IsRequired();

        builder.Property(x => x.PhysicalPrinterName)
            .IsRequired()
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(x => new { x.TerminalId, x.PrinterGroupId }).IsUnique();

        builder.Property(x => x.Format)
            .IsRequired()
            .HasConversion<int>(); // Store as enum integer

        builder.Property(x => x.CutEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.PaperWidthMm)
            .IsRequired()
            .HasDefaultValue(80);

        builder.Property(x => x.PrintableWidthChars)
            .IsRequired()
            .HasDefaultValue(48);

        builder.Property(x => x.Dpi)
            .IsRequired()
            .HasDefaultValue(203);

        builder.Property(x => x.SupportsCashDrawer)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.SupportsImages)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.SupportsQr)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
