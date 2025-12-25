namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get a ticket by ID.
/// </summary>
public class GetTicketQuery
{
    public Guid TicketId { get; set; }
}

/// <summary>
/// Query to get a ticket by ticket number.
/// </summary>
public class GetTicketByNumberQuery
{
    public int TicketNumber { get; set; }
}

/// <summary>
/// Query to get all open tickets.
/// </summary>
public class GetOpenTicketsQuery
{
    // No parameters - gets all open tickets
}

