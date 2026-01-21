using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Infrastructure.Printing;

/// <summary>
/// Mock implementation of kitchen print service for development.
/// Logs print operations instead of actually printing.
/// </summary>
public class MockKitchenPrintService : IKitchenPrintService
{
    private readonly ILogger<MockKitchenPrintService>? _logger;

    public MockKitchenPrintService(ILogger<MockKitchenPrintService>? logger = null)
    {
        _logger = logger;
    }

    public Task<KitchenPrintResult> PrintOrderLineAsync(OrderLine orderLine, Ticket ticket, CancellationToken cancellationToken = default)
    {
        if (orderLine.ShouldPrintToKitchen && !orderLine.PrintedToKitchen)
        {
            _logger?.LogInformation(
                "KITCHEN PRINT: Order Line {OrderLineId} - {MenuItemName} x{Quantity} (Ticket #{TicketNumber})",
                orderLine.Id,
                orderLine.MenuItemName,
                orderLine.Quantity,
                ticket.TicketNumber);

            // Print modifiers if any
            foreach (var modifier in orderLine.Modifiers.Where(m => m.ShouldPrintToKitchen && !m.PrintedToKitchen))
            {
                _logger?.LogInformation(
                    "  - Modifier: {ModifierName} x{ItemCount}",
                    modifier.Name,
                    modifier.ItemCount);
            }

            return Task.FromResult(KitchenPrintResult.SuccessResult(1));
        }

        return Task.FromResult(KitchenPrintResult.Failure("Item not configured to print or already printed."));
    }

    public async Task<KitchenPrintResult> PrintTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        int printedCount = 0;
        List<string> errors = new();

        foreach (var orderLine in ticket.OrderLines.Where(ol => ol.ShouldPrintToKitchen && !ol.PrintedToKitchen))
        {
            var result = await PrintOrderLineAsync(orderLine, ticket, cancellationToken);
            if (result.Success)
            {
                printedCount++;
            }
            else if (result.Errors != null)
            {
                errors.AddRange(result.Errors);
            }
        }

        return new KitchenPrintResult(true, $"Printed {printedCount} lines.", printedCount, errors);
    }

    public Task MarkOrderLinePrintedAsync(OrderLine orderLine, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would update the database
        // For mock, we just log it
        _logger?.LogInformation(
            "Marked Order Line {OrderLineId} as printed to kitchen",
            orderLine.Id);

        return Task.CompletedTask;
    }
}

