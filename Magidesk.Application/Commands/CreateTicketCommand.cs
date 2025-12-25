using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to create a new ticket.
/// </summary>
public class CreateTicketCommand
{
    public UserId CreatedBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid OrderTypeId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<int>? TableNumbers { get; set; }
    public int NumberOfGuests { get; set; } = 1;
    public string? GlobalId { get; set; }
}

/// <summary>
/// Result of creating a ticket.
/// </summary>
public class CreateTicketResult
{
    public Guid TicketId { get; set; }
    public int TicketNumber { get; set; }
}

