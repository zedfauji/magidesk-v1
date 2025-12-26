using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service interface for kitchen printing.
/// Handles printing order lines and modifiers to kitchen printers.
/// </summary>
public interface IKitchenPrintService
{
    /// <summary>
    /// Prints an order line to the kitchen.
    /// </summary>
    /// <param name="orderLine">The order line to print.</param>
    /// <param name="ticket">The ticket containing the order line.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if printing was successful, false otherwise.</returns>
    Task<bool> PrintOrderLineAsync(OrderLine orderLine, Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints all unprinted order lines for a ticket to the kitchen.
    /// </summary>
    /// <param name="ticket">The ticket to print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of order lines printed.</returns>
    Task<int> PrintTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an order line as printed to kitchen.
    /// </summary>
    /// <param name="orderLine">The order line to mark as printed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkOrderLinePrintedAsync(OrderLine orderLine, CancellationToken cancellationToken = default);
}

