using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Ticket aggregate root.
/// </summary>
public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .Include(t => t.Discounts)
            .Include(t => t.Gratuity)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket != null)
        {
            // Load modifiers and discounts for all order lines
            // Note: With AsNoTracking(), we need to query these separately
            foreach (var orderLine in ticket.OrderLines)
            {
                var modifiers = await _context.OrderLineModifiers
                    .Where(m => m.OrderLineId == orderLine.Id)
                    .ToListAsync(cancellationToken);
                
                var discounts = await _context.OrderLineDiscounts
                    .Where(d => d.OrderLineId == orderLine.Id)
                    .ToListAsync(cancellationToken);
                
                // Use reflection to set the private collections
                var modifiersField = orderLine.GetType().GetField("_modifiers", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var discountsField = orderLine.GetType().GetField("_discounts", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (modifiersField != null)
                {
                    modifiersField.SetValue(orderLine, modifiers.ToList());
                }
                
                if (discountsField != null)
                {
                    discountsField.SetValue(orderLine, discounts.ToList());
                }
            }
        }

        return ticket;
    }

    public async Task<Ticket?> GetByTicketNumberAsync(int ticketNumber, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .Include(t => t.Discounts)
            .Include(t => t.Gratuity)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

        if (ticket != null)
        {
            // Load modifiers and discounts
            foreach (var orderLine in ticket.OrderLines)
            {
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Modifiers)
                    .LoadAsync(cancellationToken);
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Discounts)
                    .LoadAsync(cancellationToken);
            }
        }

        return ticket;
    }

    public async Task<IEnumerable<Ticket>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(t => t.ShiftId == shiftId)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .Where(t => t.Status == Domain.Enumerations.TicketStatus.Open || t.Status == Domain.Enumerations.TicketStatus.Draft)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .ToListAsync(cancellationToken);

        // Load modifiers for all order lines
        foreach (var ticket in tickets)
        {
            foreach (var orderLine in ticket.OrderLines)
            {
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Modifiers)
                    .LoadAsync(cancellationToken);
            }
        }

        return tickets;
    }

    public async Task<Ticket?> GetOpenTicketByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default)
    {
        // Get tickets that are not Closed, Paid, Voided, Refunded, or Held
        // and contain the specified table number
        var ticket = await _context.Tickets
            .Where(t => t.Status != Domain.Enumerations.TicketStatus.Closed
                     && t.Status != Domain.Enumerations.TicketStatus.Paid 
                     && t.Status != Domain.Enumerations.TicketStatus.Voided 
                     && t.Status != Domain.Enumerations.TicketStatus.Refunded
                     && t.Status != Domain.Enumerations.TicketStatus.Held)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .Include(t => t.Discounts)
            .Include(t => t.Gratuity)
            .ToListAsync(cancellationToken);

        // Filter by table number (TableNumbers is a collection)
        var matchingTicket = ticket.FirstOrDefault(t => t.TableNumbers.Contains(tableNumber));

        if (matchingTicket != null)
        {
            // Load modifiers and discounts for all order lines
            foreach (var orderLine in matchingTicket.OrderLines)
            {
                var modifiers = await _context.OrderLineModifiers
                    .Where(m => m.OrderLineId == orderLine.Id)
                    .ToListAsync(cancellationToken);
                
                var discounts = await _context.OrderLineDiscounts
                    .Where(d => d.OrderLineId == orderLine.Id)
                    .ToListAsync(cancellationToken);
                
                // Use reflection to set the private collections
                var modifiersField = orderLine.GetType().GetField("_modifiers", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var discountsField = orderLine.GetType().GetField("_discounts", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (modifiersField != null)
                {
                    modifiersField.SetValue(orderLine, modifiers.ToList());
                }
                
                if (discountsField != null)
                {
                    discountsField.SetValue(orderLine, discounts.ToList());
                }
            }
        }

        return matchingTicket;
    }

    public async Task<IEnumerable<Ticket>> GetHeldTicketsAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .Where(t => t.Status == Domain.Enumerations.TicketStatus.Held)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .OrderByDescending(t => t.HeldAt)
            .ToListAsync(cancellationToken);

        // Load modifiers for all order lines
        foreach (var ticket in tickets)
        {
            foreach (var orderLine in ticket.OrderLines)
            {
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Modifiers)
                    .LoadAsync(cancellationToken);
            }
        }

        return tickets;
    }

    public async Task<IEnumerable<Ticket>> GetManageableTicketsAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .Where(t => t.Status == Domain.Enumerations.TicketStatus.Draft 
                     || t.Status == Domain.Enumerations.TicketStatus.Open
                     || t.Status == Domain.Enumerations.TicketStatus.Closed
                     || t.Status == Domain.Enumerations.TicketStatus.Refunded)
            .Include(t => t.OrderLines)
                .ThenInclude(ol => ol.Modifiers)
            .Include(t => t.Payments)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return tickets;
    }

    public async Task<IEnumerable<Ticket>> GetScheduledTicketsDueAsync(DateTime dueBy, CancellationToken cancellationToken = default)
    {
        var tickets = await _context.Tickets
            .Where(t => t.Status == Domain.Enumerations.TicketStatus.Scheduled && t.DeliveryDate <= dueBy)
            .Include(t => t.OrderLines)
            .Include(t => t.Payments)
            .Include(t => t.Discounts)
            .ToListAsync(cancellationToken);

        // Load modifiers
        foreach (var ticket in tickets)
        {
            foreach (var orderLine in ticket.OrderLines)
            {
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Modifiers)
                    .LoadAsync(cancellationToken);
            }
        }

        return tickets;
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simplified Safe Update Pattern for Tracked Entities
            // Since we removed AsNoTracking, the entity and its graph are likely already tracked.
            // EF Core Change Tracking will automatically detect additions to collections (Payments, OrderLines)
            // without manual state management.
            var entry = _context.Entry(ticket);
            
            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] UpdateAsync called - TicketId: {ticket.Id}, Version: {ticket.Version}, EntryState: {entry.State}");
            
            if (entry.State == EntityState.Detached)
            {
                System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Entity is Detached. Calling Update() to attach.");
                _context.Tickets.Update(ticket);
            }
            else if (entry.State == EntityState.Unchanged)
            {
                System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Entity is Unchanged. Marking as Modified.");
                entry.State = EntityState.Modified;
            }
            else
            {
                 System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Entity state is {entry.State}. Proceeding to SaveChanges.");
            }

            // Log Version Tracking Info
            if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged) // Should be modified by now or naturally
            {
                var origVersion = entry.OriginalValues.GetValue<int>("Version");
                var currVersion = entry.CurrentValues.GetValue<int>("Version");
                var dbVal = await entry.GetDatabaseValuesAsync(cancellationToken);
                var dbVersion = dbVal?.GetValue<int>("Version");

                System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Pre-Save Version Check: Original={origVersion}, Current={currVersion}, DB_Actual={dbVersion}");
            }

            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Calling SaveChangesAsync...");
            await _context.SaveChangesAsync(cancellationToken);
            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] SaveChangesAsync completed successfully!");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] DbUpdateConcurrencyException caught!");
            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Exception Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Affected Entities: {ex.Entries?.Count ?? 0}");
            
            if (ex.Entries != null && ex.Entries.Any())
            {
                foreach (var entry in ex.Entries)
                {
                    System.Diagnostics.Debug.WriteLine($"[DIAGNOSTIC-REPO] Failed Entity Type: {entry.Entity.GetType().Name}, State: {entry.State}");
                }
            }
            
            throw new Domain.Exceptions.ConcurrencyException(
                $"Ticket {ticket.Id} was modified by another process. Please refresh and try again.",
                ex);
        }
    }

    public async Task<int> GetNextTicketNumberAsync(CancellationToken cancellationToken = default)
    {
        var maxTicketNumber = await _context.Tickets
            .MaxAsync(t => (int?)t.TicketNumber, cancellationToken);

        return (maxTicketNumber ?? 0) + 1;
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(transaction);
    }

    public void ClearChangeTracker()
    {
        _context.ChangeTracker.Clear();
    }

    public void MarkOrderLineAsAdded(OrderLine orderLine)
    {
        // 1. Mark the OrderLine itself as Added
        _context.Entry(orderLine).State = EntityState.Added;

        // 2. Explicitly mark Owned Entities (Money types) as Added
        //    This is required because forcing State=Added on the root entity
        //    does NOT automatically propagate to Owned Types when they are treated as separate entries internally.
        
        var entry = _context.Entry(orderLine);
        
        // Mark all Money properties
        if (entry.Reference(o => o.UnitPrice).TargetEntry != null)
            entry.Reference(o => o.UnitPrice).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.SubtotalAmount).TargetEntry != null)
            entry.Reference(o => o.SubtotalAmount).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.SubtotalAmountWithoutModifiers).TargetEntry != null)
            entry.Reference(o => o.SubtotalAmountWithoutModifiers).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.DiscountAmount).TargetEntry != null)
            entry.Reference(o => o.DiscountAmount).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.TaxAmount).TargetEntry != null)
            entry.Reference(o => o.TaxAmount).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.TaxAmountWithoutModifiers).TargetEntry != null)
            entry.Reference(o => o.TaxAmountWithoutModifiers).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.TotalAmount).TargetEntry != null)
            entry.Reference(o => o.TotalAmount).TargetEntry!.State = EntityState.Added;
            
        if (entry.Reference(o => o.TotalAmountWithoutModifiers).TargetEntry != null)
            entry.Reference(o => o.TotalAmountWithoutModifiers).TargetEntry!.State = EntityState.Added;
    }
}

/// <summary>
/// Entity Framework transaction wrapper.
/// </summary>
public class EfTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}

