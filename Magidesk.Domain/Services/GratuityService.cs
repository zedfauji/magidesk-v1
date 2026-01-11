using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service for gratuity/tip calculations and operations.
/// Implements business rules for tip suggestions and application.
/// </summary>
public class GratuityService : IGratuityService
{
    /// <summary>
    /// Calculates suggested gratuity amounts based on subtotal.
    /// Standard percentages: 15%, 18%, 20%, 25%
    /// </summary>
    public GratuitySuggestions GetSuggestions(Money subtotal)
    {
        if (subtotal == null)
        {
            throw new ArgumentNullException(nameof(subtotal));
        }

        if (subtotal < Money.Zero())
        {
            throw new BusinessRuleViolationException("Cannot calculate gratuity on negative subtotal.");
        }

        // Calculate standard tip percentages
        var percent15 = subtotal * 0.15m;
        var percent18 = subtotal * 0.18m;
        var percent20 = subtotal * 0.20m;
        var percent25 = subtotal * 0.25m;

        return new GratuitySuggestions(
            Percent15: percent15,
            Percent18: percent18,
            Percent20: percent20,
            Percent25: percent25
        );
    }

    /// <summary>
    /// Applies gratuity to a ticket.
    /// Creates a new Gratuity entity and adds it to the ticket.
    /// </summary>
    public void ApplyGratuity(Ticket ticket, Money amount, UserId? serverId = null)
    {
        if (ticket == null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount));
        }

        if (amount < Money.Zero())
        {
            throw new BusinessRuleViolationException("Gratuity amount cannot be negative.");
        }

        // If no server specified, use ticket creator as default
        var ownerId = serverId ?? ticket.CreatedBy;

        // Create gratuity entity
        var gratuity = Gratuity.Create(
            ticketId: ticket.Id,
            amount: amount,
            terminalId: ticket.TerminalId,
            ownerId: ownerId
        );

        // Add to ticket (this will recalculate totals)
        ticket.AddGratuity(gratuity);
    }
}
