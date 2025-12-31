using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magidesk.Application.Services;

public class ChangeSeatCommandHandler : ICommandHandler<ChangeSeatCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public ChangeSeatCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
    }

    public async Task HandleAsync(ChangeSeatCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.NotFoundException($"Ticket {command.TicketId} not found");
        }

        var line = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        if (line == null)
        {
            throw new Domain.Exceptions.NotFoundException($"Order line {command.OrderLineId} not found in ticket {command.TicketId}");
        }

        line.SetSeatNumber(command.SeatNumber);

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }
}
