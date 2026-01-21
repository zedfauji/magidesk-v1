using System;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to update the note on a ticket.
/// </summary>
public class UpdateTicketNoteCommand
{
    public Guid TicketId { get; set; }
    public string? Note { get; set; }

    public UpdateTicketNoteCommand(Guid ticketId, string? note)
    {
        TicketId = ticketId;
        Note = note;
    }
}
