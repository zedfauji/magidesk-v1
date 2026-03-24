using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

public class TaxConfigurationEntityConfig : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfigurations");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Rate).HasColumnType("decimal(5,4)").IsRequired();
        builder.Property(t => t.EffectiveFrom).IsRequired();
        builder.Property(t => t.Country).HasColumnType("varchar(2)").IsRequired();
        builder.Property(t => t.Version).IsConcurrencyToken();
    }
}
