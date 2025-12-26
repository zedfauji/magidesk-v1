using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for Payment entities.
/// </summary>
public interface IPaymentRepository
{
    /// <summary>
    /// Gets a payment by ID.
    /// </summary>
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all payments for a ticket.
    /// </summary>
    Task<IEnumerable<Payment>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all payments for a cash session.
    /// </summary>
    Task<IEnumerable<Payment>> GetByCashSessionIdAsync(Guid cashSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new payment.
    /// </summary>
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing payment.
    /// </summary>
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all captured payments that are not yet part of a batch.
    /// </summary>
    Task<IEnumerable<Payment>> GetUnbatchedCapturedPaymentsAsync(Guid terminalId, CancellationToken cancellationToken = default);
}

