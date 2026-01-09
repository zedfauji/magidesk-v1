using Magidesk.Application.Interfaces;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Simple pricing service implementation for time-based charges.
/// TODO: Replace with full implementation in BE-A.9-01 with:
/// - First-hour pricing rules
/// - Time rounding rules
/// - Minimum charge rules
/// - Peak/off-peak rates
/// </summary>
public class SimplePricingService : IPricingService
{
    public Money CalculateTimeCharge(TimeSpan duration, decimal hourlyRate)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentException("Duration cannot be negative", nameof(duration));
        }

        if (hourlyRate < 0)
        {
            throw new ArgumentException("Hourly rate cannot be negative", nameof(hourlyRate));
        }

        // Simple calculation: hours × rate
        var hours = (decimal)duration.TotalHours;
        var charge = hours * hourlyRate;

        return new Money(charge);
    }
}
