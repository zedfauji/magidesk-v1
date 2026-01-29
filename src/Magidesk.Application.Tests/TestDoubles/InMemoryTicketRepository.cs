using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.TestDoubles;

internal sealed class InMemoryTicketRepository : ITicketRepository
{
    private readonly Dictionary<Guid, Ticket> _tickets = new();
    private int _nextTicketNumber = 1;

    private sealed class NoOpTransaction : ITransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _tickets[ticket.Id] = ticket;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        _tickets[ticket.Id] = ticket;
        return Task.CompletedTask;
    }

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _tickets.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    public Task<Ticket?> GetByTicketNumberAsync(int ticketNumber, CancellationToken cancellationToken = default)
    {
        var ticket = _tickets.Values.FirstOrDefault(t => t.TicketNumber == ticketNumber);
        return Task.FromResult(ticket);
    }

    public Task<IEnumerable<Ticket>> GetOpenTicketsAsync(CancellationToken cancellationToken = default)
    {
        var open = _tickets.Values.Where(t => t.Status != Magidesk.Domain.Enumerations.TicketStatus.Closed
                                           && t.Status != Magidesk.Domain.Enumerations.TicketStatus.Voided)
                                  .ToList();
        return Task.FromResult<IEnumerable<Ticket>>(open);
    }

    public Task<Ticket?> GetOpenTicketByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default)
    {
        var ticket = _tickets.Values.FirstOrDefault(t => 
            t.TableNumbers.Contains(tableNumber) &&
            t.Status != Magidesk.Domain.Enumerations.TicketStatus.Paid &&
            t.Status != Magidesk.Domain.Enumerations.TicketStatus.Voided &&
            t.Status != Magidesk.Domain.Enumerations.TicketStatus.Refunded &&
            t.Status != Magidesk.Domain.Enumerations.TicketStatus.Held);
        return Task.FromResult(ticket);
    }

    public Task<IEnumerable<Ticket>> GetScheduledTicketsDueAsync(DateTime dueBy, CancellationToken cancellationToken = default)
    {
        // Simplistic implementation for in-memory stub
        var due = _tickets.Values.Where(t => t.DeliveryDate != null && t.DeliveryDate <= dueBy).ToList();
        return Task.FromResult<IEnumerable<Ticket>>(due);
    }

    public Task<IEnumerable<Ticket>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default)
    {
        var result = _tickets.Values.Where(t => t.ShiftId == shiftId).ToList();
        return Task.FromResult<IEnumerable<Ticket>>(result);
    }

    public Task<IEnumerable<Ticket>> GetManageableTicketsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Ticket>>(_tickets.Values.ToList());
    }

    public Task<IEnumerable<Ticket>> GetHeldTicketsAsync(CancellationToken cancellationToken = default)
    {
        var held = _tickets.Values.Where(t => t.Status == Magidesk.Domain.Enumerations.TicketStatus.Held).ToList();
        return Task.FromResult<IEnumerable<Ticket>>(held);
    }

    public Task<int> GetNextTicketNumberAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_nextTicketNumber++);
    }

    public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ITransaction>(new NoOpTransaction());
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory
    }

    public void MarkOrderLineAsAdded(OrderLine orderLine)
    {
        // No-op for in-memory
    }
}
