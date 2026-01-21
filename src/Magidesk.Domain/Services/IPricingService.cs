using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service for calculating time-based pricing charges.
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calculates the charge for a given billable time based on table type pricing rules.
    /// </summary>
    /// <param name="billableTime">The billable time duration.</param>
    /// <param name="tableType">The table type with pricing configuration.</param>
    /// <returns>The calculated charge as Money.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tableType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when billableTime is negative.</exception>
    Money CalculateTimeCharge(
        TimeSpan billableTime,
        TableType tableType);
}
