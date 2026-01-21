using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AssignTableToTicketCommand.
/// </summary>
public class AssignTableToTicketCommandHandler : ICommandHandler<AssignTableToTicketCommand, AssignTableToTicketResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public AssignTableToTicketCommandHandler(
        ITableRepository tableRepository,
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _tableRepository = tableRepository;
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<AssignTableToTicketResult> HandleAsync(AssignTableToTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get table
        var table = await _tableRepository.GetByIdAsync(command.TableId, cancellationToken);
        if (table == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Table {command.TableId} not found.");
        }

        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Assign table to ticket
        table.AssignTicket(command.TicketId);
        ticket.AddTableNumber(table.TableNumber);

        // Update table and ticket
        await _tableRepository.UpdateAsync(table, cancellationToken);
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Table),
            table.Id,
            Guid.Empty, // System operation
            System.Text.Json.JsonSerializer.Serialize(new
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                TicketId = command.TicketId,
                Status = table.Status
            }),
            $"Table {table.TableNumber} assigned to ticket {command.TicketId}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new AssignTableToTicketResult
        {
            TableId = table.Id,
            TicketId = command.TicketId
        };
    }
}

