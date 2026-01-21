using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class FractionalModifierConfiguration : IEntityTypeConfiguration<FractionalModifier>
{
    public void Configure(EntityTypeBuilder<FractionalModifier> builder)
    {
        // TPH: Mapped to same table 'MenuModifiers' via discriminator.
        // EF Core 8 handles inheritance automatically, but we can explicit configure columns if needed.
        
        builder.Property(x => x.Portion)
            .HasConversion<string>();

        builder.Property(x => x.PriceStrategy)
            .HasConversion<string>();
    }
}
