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
    private readonly IKitchenRoutingService _kitchenRoutingService;
    private readonly IAuditEventRepository _auditEventRepository;

    public PrintToKitchenCommandHandler(
        ITicketRepository ticketRepository,
        IKitchenPrintService kitchenPrintService,
        IKitchenRoutingService kitchenRoutingService,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _kitchenPrintService = kitchenPrintService;
        _kitchenRoutingService = kitchenRoutingService;
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
        bool printSuccess = true;
        List<string> errors = new();
        string message = "Printed successfully";

        // 1. Route to KDS (Database)
        // This ensures orders appear on Kitchen Screens even if printers fail.
        try
        {
            var ticketDto = MapToDto(ticket);
            await _kitchenRoutingService.RouteToKitchenAsync(ticketDto, command.OrderLineId.HasValue ? new List<Guid> { command.OrderLineId.Value } : null);
        }
        catch (Exception ex)
        {
            errors.Add($"KDS Routing Failed: {ex.Message}");
            // Continue to printing? Yes, redundancy.
        }

        // 2. Physical Printing
        if (command.OrderLineId.HasValue)
        {
            var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId.Value);
            if (orderLine == null) throw new Domain.Exceptions.BusinessRuleViolationException("Order line not found.");

            var result = await _kitchenPrintService.PrintOrderLineAsync(orderLine, ticket, cancellationToken);
            printSuccess = result.Success;
            orderLinesPrinted = result.PrintedCount;
            message = result.Message;
            if (result.Errors != null) errors.AddRange(result.Errors);
        }
        else
        {
            var result = await _kitchenPrintService.PrintTicketAsync(ticket, cancellationToken);
            printSuccess = result.Success;
            orderLinesPrinted = result.PrintedCount;
            message = result.Message;
            if (result.Errors != null) errors.AddRange(result.Errors);
        }

        // 3. Audit
        if (orderLinesPrinted > 0)
        {
            var auditEvent = AuditEvent.Create(
                Domain.Enumerations.AuditEventType.Modified,
                nameof(Ticket),
                ticket.Id,
                Guid.Empty,
                System.Text.Json.JsonSerializer.Serialize(new { OrderLinesPrinted = orderLinesPrinted }),
                $"Printed {orderLinesPrinted} lines to kitchen. KDS: Sent.",
                correlationId: Guid.NewGuid());
            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
        }

        return new PrintToKitchenResult
        {
            OrderLinesPrinted = orderLinesPrinted,
            Success = printSuccess,
            Message = message,
            Errors = errors
        };
    }

    private Magidesk.Application.DTOs.TicketDto MapToDto(Ticket ticket)
    {
        return new Magidesk.Application.DTOs.TicketDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            GlobalId = ticket.GlobalId,
            TableName = string.Join(", ", ticket.TableNumbers),
            OwnerName = "Unknown",
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ActiveDate = ticket.ActiveDate,
            OrderLines = ticket.OrderLines.Select(ol => new Magidesk.Application.DTOs.OrderLineDto
            {
                Id = ol.Id,
                TicketId = ol.TicketId,
                MenuItemId = ol.MenuItemId,
                MenuItemName = ol.MenuItemName,
                Quantity = ol.Quantity,
                ShouldPrintToKitchen = ol.ShouldPrintToKitchen,
                PrinterGroupId = ol.PrinterGroupId,
                Instructions = ol.Instructions,
                Modifiers = ol.Modifiers.Select(m => new Magidesk.Application.DTOs.OrderLineModifierDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    ShouldPrintToKitchen = m.ShouldPrintToKitchen
                }).ToList()
            }).ToList()
        };
    }
}

