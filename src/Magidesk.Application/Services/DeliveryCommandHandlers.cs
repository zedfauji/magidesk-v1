using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class MarkTicketAsReadyCommandHandler : ICommandHandler<MarkTicketAsReadyCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public MarkTicketAsReadyCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(MarkTicketAsReadyCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new BusinessRuleViolationException($"Ticket {command.TicketId} not found.");

        ticket.MarkAsReady();

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.UserId,
            System.Text.Json.JsonSerializer.Serialize(new { ReadyTime = ticket.ReadyTime }),
            $"Ticket {ticket.TicketNumber} marked as Ready",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}

public class DispatchTicketCommandHandler : ICommandHandler<DispatchTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public DispatchTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(DispatchTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new BusinessRuleViolationException($"Ticket {command.TicketId} not found.");

        ticket.MarkAsDispatched(command.DriverId);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.UserId,
            System.Text.Json.JsonSerializer.Serialize(new { DispatchedTime = ticket.DispatchedTime, DriverId = command.DriverId }),
            $"Ticket {ticket.TicketNumber} Dispatched",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}

public class ScheduleTicketCommandHandler : ICommandHandler<ScheduleTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ScheduleTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ScheduleTicketCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
            throw new BusinessRuleViolationException($"Ticket {command.TicketId} not found.");

        ticket.Schedule(command.DeliveryDate);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        await _auditEventRepository.AddAsync(AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.UserId,
            System.Text.Json.JsonSerializer.Serialize(new { DeliveryDate = ticket.DeliveryDate, Status = ticket.Status }),
            $"Ticket {ticket.TicketNumber} scheduled for {ticket.DeliveryDate}",
            null,
            Guid.NewGuid()
        ), cancellationToken);
    }
}

public class FireScheduledTicketsCommandHandler : ICommandHandler<FireScheduledTicketsCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public FireScheduledTicketsCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(FireScheduledTicketsCommand command, CancellationToken cancellationToken = default)
    {
        var ticketsToFire = await _ticketRepository.GetScheduledTicketsDueAsync(command.DueBy, cancellationToken);
        
        foreach (var ticket in ticketsToFire)
        {
            try
            {
                ticket.Fire();
                await _ticketRepository.UpdateAsync(ticket, cancellationToken);

                await _auditEventRepository.AddAsync(AuditEvent.Create(
                    AuditEventType.Modified,
                    nameof(Ticket),
                    ticket.Id,
                    Guid.Empty, // System Action
                    System.Text.Json.JsonSerializer.Serialize(new { Action = "AutoFire", Time = DateTime.UtcNow }),
                    $"Ticket {ticket.TicketNumber} auto-fired from Scheduled to Open",
                    null,
                    Guid.NewGuid()
                ), cancellationToken);
            }
            catch (Exception ex)
            {
                // Log failure but continue processing others
                // Real logging infrastructure should be used here. 
                // For now, we rely on the fact that if it fails, it stays scheduled and will pick up next time.
            }
        }
    }
}
