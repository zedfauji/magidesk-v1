using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class TransferTicketToTableCommandHandler : ICommandHandler<TransferTicketToTableCommand, TransferTicketToTableResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public TransferTicketToTableCommandHandler(
        ITableRepository tableRepository,
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _tableRepository = tableRepository;
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<TransferTicketToTableResult> HandleAsync(TransferTicketToTableCommand command, CancellationToken cancellationToken = default)
    {
        // Load entities
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new BusinessRuleViolationException($"Ticket {command.TicketId} not found.");

        var sourceTable = await _tableRepository.GetByIdAsync(command.SourceTableId, cancellationToken);
        if (sourceTable == null)
            throw new BusinessRuleViolationException($"Source Table {command.SourceTableId} not found.");

        var targetTable = await _tableRepository.GetByIdAsync(command.TargetTableId, cancellationToken);
        if (targetTable == null)
            throw new BusinessRuleViolationException($"Target Table {command.TargetTableId} not found.");

        // Validations
        if (sourceTable.CurrentTicketId != ticket.Id)
            throw new BusinessRuleViolationException($"Source Table {sourceTable.TableNumber} does not hold Ticket {ticket.TicketNumber}.");

        if (targetTable.Status != TableStatus.Available)  // Assuming simple transfer (no merge) for now
            throw new BusinessRuleViolationException($"Target Table {targetTable.TableNumber} is occupied.");

        // Execute Transfer
        // 1. Release Source
        sourceTable.ReleaseTicket();

        // 2. Assign Target
        targetTable.AssignTicket(ticket.Id);

        // 3. Update Ticket
        ticket.RemoveTableNumber(sourceTable.TableNumber);
        ticket.AddTableNumber(targetTable.TableNumber);

        // 4. Persistence
        await _tableRepository.UpdateAsync(sourceTable, cancellationToken);
        await _tableRepository.UpdateAsync(targetTable, cancellationToken);
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // 5. Audit
        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.UserId,
            System.Text.Json.JsonSerializer.Serialize(new { From = sourceTable.TableNumber, To = targetTable.TableNumber }),
            $"Transferred Ticket {ticket.TicketNumber} from Table {sourceTable.TableNumber} to {targetTable.TableNumber}",
            null, // beforeState
            Guid.NewGuid() // correlationId
        ), cancellationToken);

        return new TransferTicketToTableResult
        {
            Success = true,
            TicketId = ticket.Id,
            SourceTableId = sourceTable.Id,
            TargetTableId = targetTable.Id
        };
    }
}
