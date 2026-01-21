using Magidesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magidesk.Infrastructure.Data.Configurations;

public class PromotionScheduleConfiguration : IEntityTypeConfiguration<PromotionSchedule>
{
    public void Configure(EntityTypeBuilder<PromotionSchedule> builder)
    {
        builder.ToTable("PromotionSchedules");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.DayOfWeek)
            .IsRequired();

        builder.Property(ps => ps.StartTime)
            .IsRequired();

        builder.Property(ps => ps.EndTime)
            .IsRequired();

        builder.Property(ps => ps.IsActive)
            .IsRequired();

        builder.HasOne(ps => ps.Discount)
            .WithMany()
            .HasForeignKey(ps => ps.DiscountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for faster lookups by day
        builder.HasIndex(ps => ps.DayOfWeek);
    }
}
