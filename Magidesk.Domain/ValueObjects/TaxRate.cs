using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a tax rate value object.
/// </summary>
public record TaxRate
{
    public decimal Rate { get; init; } // As decimal (e.g., 0.10 for 10%)
    public string Name { get; init; } = string.Empty; // e.g., "State Tax", "City Tax"
    public bool IsCompound { get; init; } // If true, tax is calculated on subtotal + previous taxes

    private TaxRate()
    {
    }

    public TaxRate(decimal rate, string name, bool isCompound = false)
    {
        if (rate < 0 || rate > 1)
        {
            throw new Exceptions.BusinessRuleViolationException("Tax rate must be between 0 and 1 (0% to 100%).");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tax rate name cannot be null or empty.", nameof(name));
        }

        Rate = rate;
        Name = name;
        IsCompound = isCompound;
    }

    /// <summary>
    /// Calculates tax amount for a given base amount.
    /// </summary>
    public Money CalculateTax(Money baseAmount)
    {
        return baseAmount * Rate;
    }

    /// <summary>
    /// Calculates tax amount for a given base amount, including compound tax calculation.
    /// </summary>
    public Money CalculateTax(Money baseAmount, Money previousTaxes)
    {
        if (IsCompound)
        {
            return (baseAmount + previousTaxes) * Rate;
        }
        return baseAmount * Rate;
    }

    public static TaxRate Zero() => new TaxRate(0m, "No Tax");
}

