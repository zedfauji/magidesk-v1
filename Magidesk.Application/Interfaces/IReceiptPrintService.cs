using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service interface for receipt printing.
/// Handles printing receipts for tickets and payments.
/// </summary>
public interface IReceiptPrintService
{
    /// <summary>
    /// Prints a receipt for a closed ticket.
    /// </summary>
    /// <param name="ticket">The ticket to print receipt for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintTicketReceiptAsync(Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints a payment receipt.
    /// </summary>
    /// <param name="payment">The payment to print receipt for.</param>
    /// <param name="ticket">The ticket associated with the payment.</param>
    /// <param name="userId">The user initiating the print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintPaymentReceiptAsync(Payment payment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints a refund receipt.
    /// </summary>
    /// <param name="refundPayment">The refund payment to print receipt for.</param>
    /// <param name="ticket">The ticket associated with the refund.</param>
    /// <param name="userId">The user initiating the print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintRefundReceiptAsync(Payment refundPayment, Ticket ticket, Guid? userId = null, CancellationToken cancellationToken = default);
}

