using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for PrintToKitchenCommand.
/// </summary>
public class PrintToKitchenCommandHandler : ICommandHandler<PrintToKitchenCommand, PrintToKitchenResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IKitchenPrintService _kitchenPrintService;
    private readonly IAuditEventRepository _auditEventRepository;

    public PrintToKitchenCommandHandler(
        ITicketRepository ticketRepository,
        IKitchenPrintService kitchenPrintService,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _kitchenPrintService = kitchenPrintService;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<PrintToKitchenResult> HandleAsync(PrintToKitchenCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        int orderLinesPrinted = 0;

        if (command.OrderLineId.HasValue)
        {
            // Print specific order line
            var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId.Value);
            if (orderLine == null)
            {
                throw new Domain.Exceptions.BusinessRuleViolationException($"Order line {command.OrderLineId.Value} not found.");
            }

            if (orderLine.ShouldPrintToKitchen && !orderLine.PrintedToKitchen)
            {
                var success = await _kitchenPrintService.PrintOrderLineAsync(orderLine, ticket, cancellationToken);
                if (success)
                {
                    orderLine.MarkPrintedToKitchen();
                    await _kitchenPrintService.MarkOrderLinePrintedAsync(orderLine, cancellationToken);
                    orderLinesPrinted = 1;
                }
            }
        }
        else
        {
            // Print all unprinted order lines
            orderLinesPrinted = await _kitchenPrintService.PrintTicketAsync(ticket, cancellationToken);
            
            // Mark all printed order lines
            foreach (var orderLine in ticket.OrderLines.Where(ol => ol.ShouldPrintToKitchen && !ol.PrintedToKitchen))
            {
                orderLine.MarkPrintedToKitchen();
                await _kitchenPrintService.MarkOrderLinePrintedAsync(orderLine, cancellationToken);
            }
        }

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            Domain.Enumerations.AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                OrderLinesPrinted = orderLinesPrinted,
                OrderLineId = command.OrderLineId
            }),
            $"Printed {orderLinesPrinted} order line(s) to kitchen for ticket #{ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new PrintToKitchenResult
        {
            OrderLinesPrinted = orderLinesPrinted,
            Success = orderLinesPrinted > 0
        };
    }
}

