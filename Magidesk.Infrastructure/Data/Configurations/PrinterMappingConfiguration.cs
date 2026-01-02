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
    }
}
