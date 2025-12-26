using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class MerchantGatewayConfigurationConfiguration : IEntityTypeConfiguration<MerchantGatewayConfiguration>
{
    public void Configure(EntityTypeBuilder<MerchantGatewayConfiguration> builder)
    {
        builder.ToTable("MerchantGatewayConfigurations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TerminalId).IsRequired();
        builder.Property(c => c.ProviderName).HasMaxLength(50).IsRequired();
        builder.Property(c => c.MerchantId).HasMaxLength(100).IsRequired();
        builder.Property(c => c.EncryptedApiKey).HasMaxLength(1000).IsRequired(); // Allow space for base64 encrypted string
        builder.Property(c => c.GatewayUrl).HasMaxLength(500).IsRequired();
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(c => c.Version).IsConcurrencyToken().IsRequired().HasDefaultValue(1);

        builder.HasIndex(c => c.TerminalId).IsUnique();
    }
}
