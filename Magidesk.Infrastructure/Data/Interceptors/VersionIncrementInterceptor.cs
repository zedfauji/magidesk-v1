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
/// 
/// IMPORTANT: Uses thread-local tracking to prevent double-increment when EF's change detection
/// triggers the interceptor multiple times during a single SaveChanges operation.
/// </summary>
public class VersionIncrementInterceptor : SaveChangesInterceptor
{
    // Thread-local flag to prevent re-entrance when EF's change detection triggers the interceptor recursively
    [ThreadStatic]
    private static bool _isExecuting;




    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        // Prevent re-entrance
        if (_isExecuting)
        {
            return base.SavingChanges(eventData, result);
        }

        try
        {
            _isExecuting = true;
            IncrementVersionForModifiedEntities(eventData);
            return base.SavingChanges(eventData, result);
        }
        finally
        {
            _isExecuting = false;
        }
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {


        // Prevent re-entrance: If we're already executing, skip (EF's change detection triggered us recursively)
        if (_isExecuting)
        {
// System.Diagnostics.Debug.WriteLine("[VERSION-INTERCEPTOR] Re-entrance detected - skipping");
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        try
        {
            _isExecuting = true;
// System.Diagnostics.Debug.WriteLine("[VERSION-INTERCEPTOR] SavingChangesAsync intercepted!");
            IncrementVersionForModifiedEntities(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        finally
        {
            _isExecuting = false;
        }
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

// System.Diagnostics.Debug.WriteLine($"[VERSION-INTERCEPTOR] Examining Ticket {ticket.Id}, State: {ticketEntry.State}, Current Version: {ticket.Version}");

            // Case 1: Ticket itself is Modified
            if (ticketEntry.State == EntityState.Modified)
            {
                shouldIncrementVersion = true;
// System.Diagnostics.Debug.WriteLine($"[VERSION-INTERCEPTOR] Ticket {ticket.Id} is Modified");
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
// System.Diagnostics.Debug.WriteLine($"[VERSION-INTERCEPTOR] Ticket {ticket.Id} has Added/Deleted children, marking as Modified");
                }
            }

            if (shouldIncrementVersion)
            {
                var oldVersion = ticket.Version;
                ticket.Version++;
System.Diagnostics.Debug.WriteLine($"[VERSION-INTERCEPTOR] Incremented ticket {ticket.Id} version: {oldVersion} → {ticket.Version}");
                
                // Verify EF tracking state
                var origVal = ticketEntry.OriginalValues.GetValue<int>("Version");
                var currVal = ticketEntry.CurrentValues.GetValue<int>("Version");
System.Diagnostics.Debug.WriteLine($"[VERSION-INTERCEPTOR] EF Tracking - Original: {origVal}, Current: {currVal}");
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
