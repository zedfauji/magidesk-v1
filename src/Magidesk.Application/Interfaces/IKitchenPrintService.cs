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
    Task<KitchenPrintResult> PrintOrderLineAsync(OrderLine orderLine, Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prints all unprinted order lines for a ticket to the kitchen.
    /// </summary>
    Task<KitchenPrintResult> PrintTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an order line as printed to kitchen.
    /// </summary>
    Task MarkOrderLinePrintedAsync(OrderLine orderLine, CancellationToken cancellationToken = default);
}

public record KitchenPrintResult(bool Success, string Message, int PrintedCount = 0, List<string>? Errors = null)
{
    public static KitchenPrintResult Failure(string message, List<string>? errors = null) => new(false, message, 0, errors);
    public static KitchenPrintResult SuccessResult(int count) => new(true, "Printed successfully", count);
}

