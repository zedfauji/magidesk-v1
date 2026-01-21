namespace Magidesk.Application.Commands;

/// <summary>
/// Command to assign a table to a ticket.
/// </summary>
public class AssignTableToTicketCommand
{
    public Guid TableId { get; set; }
    public Guid TicketId { get; set; }
}

/// <summary>
/// Result of assigning a table to a ticket.
/// </summary>
public class AssignTableToTicketResult
{
    public Guid TableId { get; set; }
    public Guid TicketId { get; set; }
}

