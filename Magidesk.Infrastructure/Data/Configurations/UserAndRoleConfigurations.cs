using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.EncryptedPin)
            .HasMaxLength(256);

        builder.Property(u => u.EncryptedPassword)
            .HasMaxLength(256);

        builder.OwnsOne(u => u.HourlyRate, hr =>
        {
            hr.Property(m => m.Amount)
                .HasColumnName("HourlyRate")
                .HasPrecision(18, 2)
                .IsRequired()
                .HasDefaultValue(0m);

            hr.Property(m => m.Currency)
                .HasColumnName("HourlyRateCurrency")
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("USD");
        });
            
        // Assuming relationship with Role is enforced at app layer for now, OR enforce FK
        // builder.HasOne<Role>().WithMany().HasForeignKey(u => u.RoleId); 
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Permissions)
            .IsRequired();
    }
}
