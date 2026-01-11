using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MemberNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(m => m.MemberNumber)
            .IsUnique();

        builder.OwnsOne(m => m.PrepaidBalance, money =>
        {
            money.Property(p => p.Amount).HasColumnName("PrepaidBalanceAmount");
            money.Property(p => p.Currency).HasColumnName("PrepaidBalanceCurrency");
        });

        builder.HasOne(m => m.Customer)
            .WithOne()
            .HasForeignKey<Member>(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Tier)
            .WithMany()
            .HasForeignKey(m => m.TierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MembershipTierConfiguration : IEntityTypeConfiguration<MembershipTier>
{
    public void Configure(EntityTypeBuilder<MembershipTier> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(t => t.MonthlyFee, money =>
        {
            money.Property(p => p.Amount).HasColumnName("MonthlyFeeAmount");
            money.Property(p => p.Currency).HasColumnName("MonthlyFeeCurrency");
        });

        builder.OwnsOne(t => t.AnnualFee, money =>
        {
            money.Property(p => p.Amount).HasColumnName("AnnualFeeAmount");
            money.Property(p => p.Currency).HasColumnName("AnnualFeeCurrency");
        });
    }
}
