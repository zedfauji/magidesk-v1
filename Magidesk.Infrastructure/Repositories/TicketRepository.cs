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
            // Modifiers are loaded through the Modifiers navigation property (which uses _modifiers backing field)
            foreach (var orderLine in ticket.OrderLines)
            {
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Modifiers)
                    .LoadAsync(cancellationToken);
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Discounts)
                    .LoadAsync(cancellationToken);
            }

            // Split modifiers after loading (EF Core loads all into Modifiers, we split into Modifiers/AddOns)
            SplitModifiersForOrderLines(ticket.OrderLines);
        }

        return ticket;
    }

    private static void SplitModifiersForOrderLines(IEnumerable<OrderLine> orderLines)
    {
        var method = typeof(OrderLine).GetMethod("SplitModifiersAfterLoad", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (var orderLine in orderLines)
        {
            method?.Invoke(orderLine, null);
        }
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
                    .Collection("_modifiers")
                    .LoadAsync(cancellationToken);
                await _context.Entry(orderLine)
                    .Collection(ol => ol.Discounts)
                    .LoadAsync(cancellationToken);
            }

            SplitModifiersForOrderLines(ticket.OrderLines);
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
            SplitModifiersForOrderLines(ticket.OrderLines);
        }

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
            SplitModifiersForOrderLines(ticket.OrderLines);
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
                    }
                    else if (entry.State == EntityState.Detached)
                    {
                        _context.OrderLines.Attach(orderLine);
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

