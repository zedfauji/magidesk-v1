using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;


namespace Magidesk.Application.Services;

public class UpdateTicketNoteCommandHandler : ICommandHandler<UpdateTicketNoteCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public UpdateTicketNoteCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task HandleAsync(UpdateTicketNoteCommand command, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId);
        
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {command.TicketId} not found.");
        }

        ticket.SetNote(command.Note);
        
        await _ticketRepository.UpdateAsync(ticket);
    }
}
