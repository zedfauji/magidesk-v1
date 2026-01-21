using System.Collections.Generic;
using System.Linq;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a tax group containing multiple tax rates.
/// </summary>
public record TaxGroup
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<TaxRate> TaxRates { get; init; } = new List<TaxRate>();

    private TaxGroup()
    {
    }

    public TaxGroup(string name, IEnumerable<TaxRate> taxRates)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tax group name cannot be null or empty.", nameof(name));
        }

        if (taxRates == null)
        {
            throw new ArgumentNullException(nameof(taxRates));
        }

        var ratesList = taxRates.ToList();
        if (!ratesList.Any())
        {
            throw new Exceptions.BusinessRuleViolationException("Tax group must contain at least one tax rate.");
        }

        Name = name;
        TaxRates = ratesList.AsReadOnly();
    }

    /// <summary>
    /// Calculates total tax for a given base amount using all rates in the group.
    /// </summary>
    public Money CalculateTotalTax(Money baseAmount)
    {
        Money totalTax = Money.Zero();
        Money currentBase = baseAmount;

        foreach (var rate in TaxRates)
        {
            Money taxAmount = rate.CalculateTax(currentBase, totalTax);
            totalTax += taxAmount;

            // For compound taxes, update the base for next calculation
            if (rate.IsCompound)
            {
                currentBase = baseAmount + totalTax;
            }
        }

        return totalTax;
    }

    /// <summary>
    /// Gets the combined tax rate (sum of all rates).
    /// Note: This is approximate for compound taxes.
    /// </summary>
    public decimal CombinedRate => TaxRates.Sum(r => r.Rate);

    public static TaxGroup NoTax() => new TaxGroup("No Tax", new[] { TaxRate.Zero() });
}

