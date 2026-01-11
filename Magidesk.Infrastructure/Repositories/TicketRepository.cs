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
            // IMPORTANT:
            // DbSet.Update() marks the entire graph as Modified, which can incorrectly mark newly-added
            // child entities (OrderLines/Payments) as Modified and cause "0 rows affected" concurrency errors.
            // In our unit-of-work pattern, aggregates are typically loaded/tracked before mutation, so
            // SaveChanges will correctly insert new children and update existing rows.
            if (_context.Entry(ticket).State == EntityState.Detached)
            {
                _context.Tickets.Attach(ticket);
                _context.Entry(ticket).State = EntityState.Modified;
            }

            // Ensure newly-added children are inserted (not updated).
            // IDs are generated in the domain layer, so we must check DB existence to decide Added vs Modified.
            var orderLines = ticket.OrderLines.ToList();
            if (orderLines.Count > 0)
            {
                var ids = orderLines.Select(ol => ol.Id).ToList();
                var existingIds = await _context.OrderLines
                    .Where(ol => ids.Contains(ol.Id))
                    .Select(ol => ol.Id)
                    .ToListAsync(cancellationToken);

                var existing = existingIds.ToHashSet();
                foreach (var orderLine in orderLines)
                {
                    var entry = _context.Entry(orderLine);
                    if (!existing.Contains(orderLine.Id))
                    {
                        entry.State = EntityState.Added;
                        
                        // IMPORTANT: Explicitly mark new Modifiers and Discounts as Added too
                        // If parent is newly added, EF usually handles this, but recursive logic here ensures safety
                        foreach (var mod in orderLine.Modifiers)
                        {
                            _context.Entry(mod).State = EntityState.Added;
                        }
                        // Also AddOns! OrderLine.Modifiers now reads from _modifiers which contains AddOns too if we merged them?
                        // Wait, my previous Edit merged them into _modifiers, so orderLine.Modifiers (public prop) does NOT contain AddOns (it filters).
                        // I need to iterate ALL modifiers.
                        // Since I made _modifiers private, I can't access it directly.
                        // I should iterate Modifiers AND AddOns.
                        // Or better, assume AddOns are also saved via _modifiers mapping if EF accesses the field.
                        
                        // Wait, if I iterate Modifiers AND AddOns, I cover everything.
                        foreach (var addon in orderLine.AddOns)
                        {
                            _context.Entry(addon).State = EntityState.Added;
                        }

                        foreach (var discount in orderLine.Discounts)
                        {
                            _context.Entry(discount).State = EntityState.Added;
                        }
                    }
                    else 
                    {
                        // Existing OrderLine
                        if (entry.State == EntityState.Detached)
                        {
                             _context.OrderLines.Attach(orderLine);
                        }
                        
                         // Check for NEW modifiers on an EXISTING order line
                        foreach (var mod in orderLine.Modifiers)
                        {
                            if (_context.Entry(mod).State == EntityState.Detached) 
                            { 
                                // Ideally check existence but GUIDs are new likely
                                _context.Entry(mod).State = EntityState.Added; 
                            }
                        }
                         foreach (var addon in orderLine.AddOns)
                        {
                            if (_context.Entry(addon).State == EntityState.Detached) 
                            { 
                                _context.Entry(addon).State = EntityState.Added; 
                            }
                        }
                    }
                }
            }

            var payments = ticket.Payments.ToList();
            if (payments.Count > 0)
            {
                var ids = payments.Select(p => p.Id).ToList();
                var existingIds = await _context.Payments
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync(cancellationToken);

                var existing = existingIds.ToHashSet();
                foreach (var payment in payments)
                {
                    var entry = _context.Entry(payment);
                    if (!existing.Contains(payment.Id))
                    {
                        entry.State = EntityState.Added;
                    }
                    else if (entry.State == EntityState.Detached)
                    {
                        _context.Payments.Attach(payment);
                    }
                }
            }

            // Handle Gratuity Persistence
            if (ticket.Gratuity != null)
            {
                // Check if it exists in DB to decide between Add and Update
                // We do this regardless of current state because pre-generated GUIDs can confuse EF
                var exists = await _context.Gratuities.AnyAsync(g => g.Id == ticket.Gratuity.Id, cancellationToken);
                
                var gratuityEntry = _context.Entry(ticket.Gratuity);
                if (!exists) 
                {
                    gratuityEntry.State = EntityState.Added;
                }
                else 
                {
                    if (gratuityEntry.State == EntityState.Detached)
                    {
                        _context.Gratuities.Attach(ticket.Gratuity);
                    }
                    gratuityEntry.State = EntityState.Modified;
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

