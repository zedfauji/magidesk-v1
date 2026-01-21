using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class PrinterGroupConfiguration : IEntityTypeConfiguration<PrinterGroup>
{
    public void Configure(EntityTypeBuilder<PrinterGroup> builder)
    {
        builder.ToTable("PrinterGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.CutBehavior)
            .IsRequired()
            .HasDefaultValue(Domain.Enumerations.CutBehavior.Auto);

        builder.Property(x => x.ShowPrices)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.RetryDelayMs)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.AllowReprint)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.FallbackPrinterGroupId)
            .IsRequired(false);
    }
}
