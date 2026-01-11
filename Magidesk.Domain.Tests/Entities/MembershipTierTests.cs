using System;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

public class MembershipTierTests
{
    [Fact]
    public void Create_WithValidData_ReturnsTier()
    {
        // Arrange
        var name = "Gold";
        var discount = 15m;
        var fee = new Money(50m);
        var description = "Gold membership";

        // Act
        var tier = MembershipTier.Create(name, discount, fee, description);

        // Assert
        Assert.NotNull(tier);
        Assert.Equal(name, tier.Name);
        Assert.Equal(discount, tier.DiscountPercent);
        Assert.Equal(fee, tier.MonthlyFee);
        Assert.Equal(fee * 12, tier.AnnualFee);
        Assert.Equal(description, tier.Description);
        Assert.True(tier.IsActive);
    }

    [Theory]
    [InlineData("", 10, 20)]
    [InlineData("Tier", -1, 20)]
    [InlineData("Tier", 101, 20)]
    public void Create_WithInvalidData_ThrowsBusinessRuleViolationException(string name, decimal discount, decimal feeAmount)
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => 
            MembershipTier.Create(name, discount, new Money(feeAmount)));
    }

    [Fact]
    public void CalculateMemberPrice_AppliesDiscount()
    {
        // Arrange
        var tier = MembershipTier.Create("Gold", 20m, new Money(50m));
        var regularPrice = new Money(100m);

        // Act
        var memberPrice = tier.CalculateMemberPrice(regularPrice);

        // Assert
        Assert.Equal(new Money(80m), memberPrice);
    }

    [Fact]
    public void GetEffectiveHourlyRate_AppliesDiscount()
    {
        // Arrange
        var tier = MembershipTier.Create("Gold", 0m, new Money(50m), "", 5m);
        var baseRate = 20m;

        // Act
        var effectiveRate = tier.GetEffectiveHourlyRate(baseRate);

        // Assert
        Assert.Equal(15m, effectiveRate);
    }

    [Fact]
    public void UpdateBenefits_UpdatesProperties()
    {
        // Arrange
        var tier = MembershipTier.Create("Gold", 10m, new Money(50m));

        // Act
        tier.UpdateBenefits(20m, 10m);

        // Assert
        Assert.Equal(20m, tier.DiscountPercent);
        Assert.Equal(10m, tier.HourlyRateDiscount);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var tier = MembershipTier.Create("Gold", 10m, new Money(50m));

        // Act
        tier.Deactivate();

        // Assert
        Assert.False(tier.IsActive);
    }
}
