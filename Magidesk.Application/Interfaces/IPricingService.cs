using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for calculating time-based pricing charges.
/// TODO: Replace with full implementation in BE-A.9-01 (PricingService)
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calculates the charge for a given duration at the specified hourly rate.
    /// </summary>
    /// <param name="duration">Billable duration</param>
    /// <param name="hourlyRate">Hourly rate</param>
    /// <returns>Calculated charge</returns>
    Money CalculateTimeCharge(TimeSpan duration, decimal hourlyRate);
}
