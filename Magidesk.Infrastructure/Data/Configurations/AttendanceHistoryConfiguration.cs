using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class AttendanceHistoryConfiguration : IEntityTypeConfiguration<AttendanceHistory>
{
    public void Configure(EntityTypeBuilder<AttendanceHistory> builder)
    {
        builder.ToTable("AttendanceHistories");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .HasConversion(
                id => id.Value,
                value => new Domain.ValueObjects.UserId(value))
            .IsRequired();

        builder.Property(a => a.ClockInTime)
            .IsRequired();

        builder.Property(a => a.ClockOutTime);

        builder.Property(a => a.ShiftId);
        
        // Optional: Index on UserId for faster lookups
        builder.HasIndex(a => a.UserId);
    }
}
