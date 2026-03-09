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
            // EF Core change detection pattern for tracked aggregate roots:
            // The ticket entity is already tracked from GetByIdAsync().
            // EF Core's change tracker automatically detects property modifications
            // when SaveChangesAsync() is called. No explicit Update() needed.
            // 
            // IMPORTANT: Do NOT call _context.Tickets.Update(ticket) here!
            // Update() marks the ENTIRE aggregate graph (including Gratuity, OrderLines, etc.)
            // as Modified, causing EF to generate UPDATE statements for unchanged child entities.
            // This breaks optimistic concurrency and causes DbUpdateConcurrencyException.
            
            // CORRECTIVE LOGIC: Ensure Gratuity is Added, not Modified, if it's new
            if (ticket.Gratuity != null)
            {
                var gratuityEntry = _context.Entry(ticket.Gratuity);
                if (gratuityEntry.State == EntityState.Modified)
                {
                    // Check if it actually exists in DB
                    var exists = await _context.Set<Gratuity>().AnyAsync(g => g.Id == ticket.Gratuity.Id, cancellationToken);
                    if (!exists)
                    {
                        gratuityEntry.State = EntityState.Added;
                        
                        // CRITICAL: Must also set the Owned Entity "Amount" to Added
                        var amountEntry = gratuityEntry.Reference(g => g.Amount).TargetEntry;
                        if (amountEntry != null)
                        {
                            amountEntry.State = EntityState.Added;
                        }
                    }
                }
                else if (gratuityEntry.State == EntityState.Detached)
                {
                    gratuityEntry.State = EntityState.Added;
                    
                    // CRITICAL: Must also set the Owned Entity "Amount" to Added
                    var amountEntry = gratuityEntry.Reference(g => g.Amount).TargetEntry;
                    if (amountEntry != null)
                    {
                        amountEntry.State = EntityState.Added;
                    }
                }
            }

            // CORRECTIVE LOGIC: Ensure new OrderLines (like TimeChargeLine) are Added, not Modified
            // When ending a session, a new TimeChargeLine is added to the ticket.
            // EF Core might track this as Modified because the Ticket is Modified.
            // If we attempt to UPDATE non-existent rows, we get a concurrency exception.
            if (ticket.OrderLines != null)
            {
                foreach (var line in ticket.OrderLines)
                {
                    var lineEntry = _context.Entry(line);
                    if (lineEntry.State == EntityState.Modified || lineEntry.State == EntityState.Detached)
                    {
                        // Check if it actually exists in DB
                        // Optimization: Check Local first to avoid DB hit? No, Local reflects Context which is what we are fighting.
                        var lineExists = await _context.Set<OrderLine>().AnyAsync(ol => ol.Id == line.Id, cancellationToken);
                        if (!lineExists)
                        {
                            // It's a new line that EF thinks is an update. Force it to Added.
                            // This includes setting all Owned Entity properties to Added.
                            MarkOrderLineAsAdded(line);
                        }
                    }
                }
            }

            
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
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

    public async Task<bool> HasActiveOrdersWithItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        // Active tickets are those not in terminal states (Paid, Closed, Voided, Refunded)
        // Check if any active ticket has order lines referencing menu items that use this inventory item in their recipes
        var hasActiveOrders = await _context.Tickets
            .Where(t => t.Status != Domain.Enumerations.TicketStatus.Paid
                     && t.Status != Domain.Enumerations.TicketStatus.Closed
                     && t.Status != Domain.Enumerations.TicketStatus.Voided
                     && t.Status != Domain.Enumerations.TicketStatus.Refunded)
            .SelectMany(t => t.OrderLines)
            .Join(_context.Set<MenuItem>(),
                orderLine => orderLine.MenuItemId,
                menuItem => menuItem.Id,
                (orderLine, menuItem) => menuItem)
            .SelectMany(menuItem => menuItem.RecipeLines)
            .AnyAsync(recipeLine => recipeLine.InventoryItemId == itemId, cancellationToken);

        return hasActiveOrders;
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

