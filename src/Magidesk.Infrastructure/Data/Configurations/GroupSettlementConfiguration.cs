using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class GroupSettlementConfiguration : IEntityTypeConfiguration<GroupSettlement>
{
    public void Configure(EntityTypeBuilder<GroupSettlement> builder)
    {
        builder.ToTable("GroupSettlements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MasterPaymentId)
            .IsRequired();
            
        builder.Property(x => x.Strategy)
            .HasMaxLength(50);

        // Store ticket IDs as a primitive array in Postgres
        builder.PrimitiveCollection(x => x.ChildTicketIds);

        builder.HasIndex(x => x.MasterPaymentId);
    }
}
