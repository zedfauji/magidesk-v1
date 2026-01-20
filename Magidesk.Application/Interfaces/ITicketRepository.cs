using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for Ticket aggregate root.
/// </summary>
public interface ITicketRepository
{
    /// <summary>
    /// Gets a ticket by ID.
    /// </summary>
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a ticket by ticket number.
    /// </summary>
    Task<Ticket?> GetByTicketNumberAsync(int ticketNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tickets for a shift.
    /// </summary>
    Task<IEnumerable<Ticket>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all open tickets.
    /// </summary>
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an open ticket for a specific table number (not Paid, Voided, Refunded, or Held).
    /// </summary>
    Task<Ticket?> GetOpenTicketByTableNumberAsync(int tableNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all held tickets (tickets with Held status).
    /// </summary>
    Task<IEnumerable<Ticket>> GetHeldTicketsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all manageable tickets (Draft, Open, Closed, Refunded) for Ticket Management UI.
    /// </summary>
    Task<IEnumerable<Ticket>> GetManageableTicketsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets scheduled tickets that are due for firing (DeliveryDate <= dueBy).
    /// </summary>
    Task<IEnumerable<Ticket>> GetScheduledTicketsDueAsync(DateTime dueBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new ticket.
    /// </summary>
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    Task UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// gets the next ticket number.
    /// </summary>
    Task<int> GetNextTicketNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction for atomic operations.
    /// </summary>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the Entity Framework change tracker. 
    /// Useful for retry policies to ensure fresh entity fetching.
    /// </summary>
    void ClearChangeTracker();
}

/// <summary>
/// Represents a database transaction.
/// </summary>
public interface ITransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

