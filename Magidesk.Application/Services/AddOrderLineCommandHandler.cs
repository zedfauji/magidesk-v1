using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddOrderLineCommand.
/// </summary>
public class AddOrderLineCommandHandler : ICommandHandler<AddOrderLineCommand, AddOrderLineResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AddOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AddOrderLineResult> HandleAsync(AddOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Create order line
        var orderLine = OrderLine.Create(
            command.TicketId,
            command.MenuItemId,
            command.MenuItemName,
            command.Quantity,
            command.UnitPrice,
            command.TaxRate,
            command.CategoryName,
            command.GroupName);

        // Add to ticket
        ticket.AddOrderLine(orderLine);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            Guid.Empty, // Would need to get from context
            System.Text.Json.JsonSerializer.Serialize(new { OrderLineId = orderLine.Id, Action = "Added" }),
            $"Order line added to ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AddOrderLineResult
        {
            OrderLineId = orderLine.Id
        };
    }
}

