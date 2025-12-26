using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Infrastructure.Data.Configurations;

public class TerminalTransactionConfiguration : IEntityTypeConfiguration<TerminalTransaction>
{
    public void Configure(EntityTypeBuilder<TerminalTransaction> builder)
    {
        builder.ToTable("TerminalTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(255);
            
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount");
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(x => x.Timestamp)
            .IsRequired();
            
        // Index for performance
        builder.HasIndex(x => x.CashSessionId);
    }
}
