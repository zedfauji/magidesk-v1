using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service for gratuity/tip calculations and operations.
/// </summary>
public interface IGratuityService
{
    /// <summary>
    /// Calculates suggested gratuity amounts based on subtotal.
    /// </summary>
    /// <param name="subtotal">The ticket subtotal to calculate tips on.</param>
    /// <returns>Suggested tip amounts at standard percentages.</returns>
    GratuitySuggestions GetSuggestions(Money subtotal);

    /// <summary>
    /// Applies gratuity to a ticket.
    /// </summary>
    /// <param name="ticket">The ticket to add gratuity to.</param>
    /// <param name="amount">The gratuity amount.</param>
    /// <param name="serverId">Optional server/owner ID for tip assignment.</param>
    void ApplyGratuity(Ticket ticket, Money amount, UserId? serverId = null);
}

/// <summary>
/// Suggested gratuity amounts at standard percentages.
/// </summary>
/// <param name="Percent15">15% gratuity amount.</param>
/// <param name="Percent18">18% gratuity amount.</param>
/// <param name="Percent20">20% gratuity amount.</param>
/// <param name="Percent25">25% gratuity amount.</param>
public record GratuitySuggestions(
    Money Percent15,
    Money Percent18,
    Money Percent20,
    Money Percent25
);
