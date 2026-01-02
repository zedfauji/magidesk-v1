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
    }
}
