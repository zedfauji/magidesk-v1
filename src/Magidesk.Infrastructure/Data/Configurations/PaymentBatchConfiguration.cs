using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class PaymentBatchConfiguration : IEntityTypeConfiguration<PaymentBatch>
{
    public void Configure(EntityTypeBuilder<PaymentBatch> builder)
    {
        builder.ToTable("PaymentBatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.GatewayBatchId)
            .HasMaxLength(100);

        builder.Property(x => x.OpenedAt)
            .IsRequired();

        builder.HasIndex(x => x.TerminalId);
        builder.HasIndex(x => x.Status);
    }
}
