using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class ComboGroupConfiguration : IEntityTypeConfiguration<ComboGroup>
{
    public void Configure(EntityTypeBuilder<ComboGroup> builder)
    {
        builder.ToTable("ComboGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.ComboGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
