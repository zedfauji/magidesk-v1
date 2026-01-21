using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class ChangeTicketOrderTypeCommandHandler : ICommandHandler<ChangeTicketOrderTypeCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ChangeTicketOrderTypeCommandHandler(
        ITicketRepository ticketRepository,
        IOrderTypeRepository orderTypeRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _orderTypeRepository = orderTypeRepository ?? throw new ArgumentNullException(nameof(orderTypeRepository));
        _auditEventRepository = auditEventRepository ?? throw new ArgumentNullException(nameof(auditEventRepository));
    }

    public async Task HandleAsync(ChangeTicketOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null) throw new NotFoundException($"Ticket {command.TicketId} not found.");

        var orderType = await _orderTypeRepository.GetByIdAsync(command.OrderTypeId, cancellationToken);
        if (orderType == null) throw new NotFoundException($"Order Type {command.OrderTypeId} not found.");

        ticket.ChangeOrderType(orderType);
        
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.ChangedBy?.Value ?? Guid.Empty,
            "Order Type Changed",
            $"Ticket order type changed to '{orderType.Name}'",
            correlationId: Guid.NewGuid());
        
        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}

public class SetTicketCustomerCommandHandler : ICommandHandler<SetTicketCustomerCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public SetTicketCustomerCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _auditEventRepository = auditEventRepository ?? throw new ArgumentNullException(nameof(auditEventRepository));
    }

    public async Task HandleAsync(SetTicketCustomerCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null) throw new NotFoundException($"Ticket {command.TicketId} not found.");

        ticket.SetCustomer(command.CustomerId, command.DeliveryAddress, command.ExtraDeliveryInfo);
        
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.ChangedBy?.Value ?? Guid.Empty,
            "Customer Info Updated",
            $"Customer info updated for ticket {ticket.TicketNumber}",
            correlationId: Guid.NewGuid());
        
        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}
