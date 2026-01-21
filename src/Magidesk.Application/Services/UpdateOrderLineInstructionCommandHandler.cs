using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;


namespace Magidesk.Application.Services;

public class UpdateOrderLineInstructionCommandHandler : ICommandHandler<UpdateOrderLineInstructionCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public UpdateOrderLineInstructionCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task HandleAsync(UpdateOrderLineInstructionCommand command, CancellationToken cancellationToken = default)
    {
        // We load the ticket aggregate including lines
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId);
        
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {command.TicketId} not found.");
        }

        var orderLine = ticket.OrderLines.FirstOrDefault(ol => ol.Id == command.OrderLineId);
        
        if (orderLine == null)
        {
             throw new KeyNotFoundException($"OrderLine with ID {command.OrderLineId} not found in Ticket {command.TicketId}.");
        }
        
        orderLine.SetInstructions(command.Instruction);
        
        await _ticketRepository.UpdateAsync(ticket);
    }
}
