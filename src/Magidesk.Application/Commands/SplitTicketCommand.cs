using System;
using System.Collections.Generic;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to split a ticket by moving selected order lines to a new ticket.
/// </summary>
public class SplitTicketCommand
{
    public Guid OriginalTicketId { get; set; }
    public List<Guid> OrderLineIdsToSplit { get; set; } = new();
    public UserId SplitBy { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid OrderTypeId { get; set; }
    public string? GlobalId { get; set; }
}

/// <summary>
/// Result of splitting a ticket.
/// </summary>
public class SplitTicketResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid NewTicketId { get; set; }
    public int NewTicketNumber { get; set; }
    public int OrderLinesMoved { get; set; }
}

