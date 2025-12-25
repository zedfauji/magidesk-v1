using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for CashPayment.
/// CashPayment doesn't have additional properties beyond Payment base class.
/// </summary>
public class CashPaymentConfiguration : IEntityTypeConfiguration<CashPayment>
{
    public void Configure(EntityTypeBuilder<CashPayment> builder)
    {
        // CashPayment inherits all properties from Payment base class
        // No additional configuration needed for Phase 2
    }
}

