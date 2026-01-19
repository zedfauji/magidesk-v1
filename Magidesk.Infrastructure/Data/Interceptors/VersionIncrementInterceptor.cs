using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Data.Interceptors;

/// <summary>
/// Intercepts SaveChanges to automatically increment Version property
/// for concurrency-tracked entities like Ticket.
/// 
/// This eliminates the need for manual IncrementVersion() calls throughout the codebase.
/// EF Core's .IsConcurrencyToken() uses Version in the WHERE clause for optimistic concurrency,
/// but does NOT auto-increment integer Version fields (only byte[] RowVersion).
/// This interceptor provides the auto-increment behavior.
/// </summary>
public class VersionIncrementInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        IncrementVersionForModifiedEntities(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        IncrementVersionForModifiedEntities(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void IncrementVersionForModifiedEntities(DbContextEventData eventData)
    {
        if (eventData.Context == null) return;

        // Get all Ticket entries
        var ticketEntries = eventData.Context.ChangeTracker.Entries<Ticket>()
            .ToList();

        foreach (var ticketEntry in ticketEntries)
        {
            var ticket = ticketEntry.Entity;
            var shouldIncrementVersion = false;

            // Case 1: Ticket itself is Modified
            if (ticketEntry.State == EntityState.Modified)
            {
                shouldIncrementVersion = true;
                Console.WriteLine($"[VERSION-INTERCEPTOR] Ticket {ticket.Id} is Modified");
            }
            // Case 2: Ticket is Unchanged but has Added/Deleted children (e.g., OrderLines, Payments)
            else if (ticketEntry.State == EntityState.Unchanged)
            {
                // Check if any navigation properties have Added or Deleted entities
                var hasAddedOrDeletedChildren = eventData.Context.ChangeTracker.Entries()
                    .Any(e => (e.State == EntityState.Added || e.State == EntityState.Deleted) &&
                              IsChildOfTicket(e, ticket.Id));

                if (hasAddedOrDeletedChildren)
                {
                    shouldIncrementVersion = true;
                    // Mark the ticket as Modified so EF Core updates it
                    ticketEntry.State = EntityState.Modified;
                    Console.WriteLine($"[VERSION-INTERCEPTOR] Ticket {ticket.Id} has Added/Deleted children, marking as Modified");
                }
            }

            if (shouldIncrementVersion)
            {
                var oldVersion = ticket.Version;
                ticket.Version++;
                Console.WriteLine($"[VERSION-INTERCEPTOR] Incremented ticket {ticket.Id} version: {oldVersion} → {ticket.Version}");
            }
        }
    }

    private static bool IsChildOfTicket(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, Guid ticketId)
    {
        // Check if the entity is a child of the specified ticket
        var entity = entry.Entity;

        // Check OrderLine
        if (entity is Domain.Entities.OrderLine orderLine && orderLine.TicketId == ticketId)
            return true;

        // Check Payment
        if (entity is Domain.Entities.Payment payment && payment.TicketId == ticketId)
            return true;

        // Check TicketDiscount
        if (entity is Domain.Entities.TicketDiscount discount && discount.TicketId == ticketId)
            return true;

        // Check Gratuity
        if (entity is Domain.Entities.Gratuity gratuity && gratuity.TicketId == ticketId)
            return true;

        return false;
    }
}
