using System.Threading;
using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces;

public interface ITaxConfigurationService
{
    /// <summary>
    /// Returns the current effective tax rate for the given country code (e.g. "MX").
    /// Returns 0 if no configuration exists. Never throws.
    /// </summary>
    Task<decimal> GetCurrentRateAsync(string countryCode = "MX", CancellationToken cancellationToken = default);
}
