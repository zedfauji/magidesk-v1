using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class PrintTemplateConfiguration : IEntityTypeConfiguration<PrintTemplate>
{
    public void Configure(EntityTypeBuilder<PrintTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Content)
            .IsRequired();

        builder.Property(t => t.Type)
            .IsRequired();
            
        builder.Property(t => t.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        // Versioning for concurrency
        builder.Property(t => t.Version)
            .IsConcurrencyToken();
    }
}
