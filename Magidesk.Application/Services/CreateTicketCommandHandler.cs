using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for CreateTicketCommand.
/// </summary>
public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, CreateTicketResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITableRepository tableRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _tableRepository = tableRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task<CreateTicketResult> HandleAsync(CreateTicketCommand command, CancellationToken cancellationToken = default)
    {
        // Get next ticket number
        var ticketNumber = await _ticketRepository.GetNextTicketNumberAsync(cancellationToken);

        // Create ticket
        var ticket = Ticket.Create(
            ticketNumber,
            command.CreatedBy,
            command.TerminalId,
            command.ShiftId,
            command.OrderTypeId,
            command.GlobalId);

        // Set Guest Count (F-0023)
        if (command.NumberOfGuests > 0)
        {
            ticket.SetNumberOfGuests(command.NumberOfGuests);
        }

        // Set optional properties
        if (command.CustomerId.HasValue)
        {
            // Note: CustomerId would need a setter or constructor parameter in Ticket entity
            // For now, we'll handle this in the entity if needed
        }

        if (command.TableNumbers != null)
        {
            foreach (var tableNumber in command.TableNumbers)
            {
                ticket.AddTableNumber(tableNumber);
                
                // Phase 1 Core Integrity: Ensure table status is updated
                var table = await _tableRepository.GetByTableNumberAsync(tableNumber, cancellationToken);
                if (table != null)
                {
                    if (table.Status != Domain.Enumerations.TableStatus.Available)
                    {
                        // Double-seat prevention: Blocking if table is already occupied
                        throw new Domain.Exceptions.BusinessRuleViolationException($"Table {tableNumber} is already occupied (Status: {table.Status}).");
                    }

                    // Assign table to this ticket (Updates Table Entity Status)
                    table.AssignTicket(ticket.Id);
                    await _tableRepository.UpdateAsync(table, cancellationToken);
                }
            }
        }

        // Add to repository
        await _ticketRepository.AddAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            Domain.Enumerations.AuditEventType.Created,
            nameof(Ticket),
            ticket.Id,
            command.CreatedBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { ticket.TicketNumber, ticket.Status }),
            $"Ticket {ticketNumber} created",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        return new CreateTicketResult
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber
        };
    }
}

