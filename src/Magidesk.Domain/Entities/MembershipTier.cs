using System;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a membership tier (e.g., Bronze, Silver, Gold).
/// </summary>
public class MembershipTier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal DiscountPercent { get; private set; }
    public decimal? HourlyRateDiscount { get; private set; }
    public bool IncludesFreeGuests { get; private set; }
    public int FreeGuestsPerVisit { get; private set; }
    public Money MonthlyFee { get; private set; }
    public Money AnnualFee { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private MembershipTier()
    {
        MonthlyFee = Money.Zero();
        AnnualFee = Money.Zero();
    }

    public static MembershipTier Create(
        string name, 
        decimal discountPercent, 
        Money monthlyFee, 
        string description = "", 
        decimal? hourlyRateDiscount = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleViolationException("Tier name is required.");

        if (discountPercent < 0 || discountPercent > 100)
            throw new BusinessRuleViolationException("Discount percent must be between 0 and 100.");

        if (monthlyFee < Money.Zero())
            throw new BusinessRuleViolationException("Monthly fee cannot be negative.");

        return new MembershipTier
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            DiscountPercent = discountPercent,
            HourlyRateDiscount = hourlyRateDiscount,
            MonthlyFee = monthlyFee,
            AnnualFee = monthlyFee * 12, // Default annual
            IsActive = true
        };
    }

    public Money CalculateMemberPrice(Money regularPrice)
    {
        if (DiscountPercent == 0) return regularPrice;
        var discountAmount = regularPrice * (DiscountPercent / 100m);
        return regularPrice - discountAmount;
    }

    public decimal GetEffectiveHourlyRate(decimal baseRate)
    {
        if (!HourlyRateDiscount.HasValue) return baseRate;
        var rate = baseRate - HourlyRateDiscount.Value;
        return rate > 0 ? rate : 0;
    }

    public void UpdateBenefits(decimal discountPercent, decimal? hourlyRateDiscount)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new BusinessRuleViolationException("Discount percent must be between 0 and 100.");

        DiscountPercent = discountPercent;
        HourlyRateDiscount = hourlyRateDiscount;
    }

    public void Deactivate() => IsActive = false;
    public void Reactivate() => IsActive = true;
}
