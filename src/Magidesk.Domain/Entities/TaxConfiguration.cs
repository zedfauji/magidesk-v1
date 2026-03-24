using System;

namespace Magidesk.Domain.Entities;

public class TaxConfiguration
{
    public Guid Id { get; private set; }
    public decimal Rate { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public string Country { get; private set; } = string.Empty;
    public int Version { get; private set; }

    private TaxConfiguration() { } // For EF

    public static TaxConfiguration Create(decimal rate, string country, DateTime effectiveFrom)
    {
        return new TaxConfiguration
        {
            Id = Guid.NewGuid(),
            Rate = rate,
            Country = country,
            EffectiveFrom = effectiveFrom,
            Version = 1
        };
    }
}
