using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class ComboDefinitionConfiguration : IEntityTypeConfiguration<ComboDefinition>
{
    public void Configure(EntityTypeBuilder<ComboDefinition> builder)
    {
        builder.ToTable("ComboDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Price");
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.HasMany(x => x.Groups)
            .WithOne()
            .HasForeignKey(x => x.ComboDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
