using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for tax calculations.
/// Handles complex tax scenarios including multiple rates, tax groups, tax-exempt, and price-includes-tax mode.
/// </summary>
public class TaxDomainService
{
    /// <summary>
    /// Calculates tax for a subtotal amount using a tax group.
    /// </summary>
    public Money CalculateTax(Money subtotal, TaxGroup? taxGroup, bool isTaxExempt = false)
    {
        if (isTaxExempt)
        {
            return Money.Zero();
        }

        if (taxGroup == null)
        {
            return Money.Zero();
        }

        return taxGroup.CalculateTotalTax(subtotal);
    }

    /// <summary>
    /// Calculates tax for a subtotal amount using a single tax rate.
    /// </summary>
    public Money CalculateTax(Money subtotal, TaxRate? taxRate, bool isTaxExempt = false)
    {
        if (isTaxExempt)
        {
            return Money.Zero();
        }

        if (taxRate == null)
        {
            return Money.Zero();
        }

        return taxRate.CalculateTax(subtotal);
    }

    /// <summary>
    /// Calculates the base amount (excluding tax) when price includes tax.
    /// Formula: baseAmount = totalAmount / (1 + taxRate)
    /// </summary>
    public Money CalculateBaseAmountFromInclusivePrice(Money totalAmount, TaxGroup? taxGroup)
    {
        if (taxGroup == null || !taxGroup.TaxRates.Any())
        {
            return totalAmount;
        }

        // For inclusive pricing, we need to reverse-calculate the base
        // If we have a simple tax rate, use: base = total / (1 + rate)
        // For multiple rates or compound taxes, this is more complex
        // For now, we'll use the combined rate approximation
        decimal combinedRate = taxGroup.CombinedRate;
        if (combinedRate == 0)
        {
            return totalAmount;
        }

        decimal baseAmount = totalAmount.Amount / (1 + combinedRate);
        return new Money(baseAmount);
    }

    /// <summary>
    /// Calculates the base amount (excluding tax) when price includes tax, using a single tax rate.
    /// </summary>
    public Money CalculateBaseAmountFromInclusivePrice(Money totalAmount, TaxRate? taxRate)
    {
        if (taxRate == null || taxRate.Rate == 0)
        {
            return totalAmount;
        }

        decimal baseAmount = totalAmount.Amount / (1 + taxRate.Rate);
        return new Money(baseAmount);
    }

    /// <summary>
    /// Calculates the total amount (including tax) when price excludes tax.
    /// </summary>
    public Money CalculateTotalAmountWithTax(Money baseAmount, TaxGroup? taxGroup, bool isTaxExempt = false)
    {
        Money tax = CalculateTax(baseAmount, taxGroup, isTaxExempt);
        return baseAmount + tax;
    }

    /// <summary>
    /// Calculates the total amount (including tax) when price excludes tax, using a single tax rate.
    /// </summary>
    public Money CalculateTotalAmountWithTax(Money baseAmount, TaxRate? taxRate, bool isTaxExempt = false)
    {
        Money tax = CalculateTax(baseAmount, taxRate, isTaxExempt);
        return baseAmount + tax;
    }

    /// <summary>
    /// Breaks down tax calculation into individual tax components.
    /// Returns a dictionary of tax name to tax amount.
    /// </summary>
    public Dictionary<string, Money> CalculateTaxBreakdown(Money subtotal, TaxGroup? taxGroup, bool isTaxExempt = false)
    {
        var breakdown = new Dictionary<string, Money>();

        if (isTaxExempt || taxGroup == null)
        {
            return breakdown;
        }

        Money totalTax = Money.Zero();
        Money currentBase = subtotal;

        foreach (var rate in taxGroup.TaxRates)
        {
            Money taxAmount = rate.CalculateTax(currentBase, totalTax);
            breakdown[rate.Name] = taxAmount;
            totalTax += taxAmount;

            if (rate.IsCompound)
            {
                currentBase = subtotal + totalTax;
            }
        }

        return breakdown;
    }
}

