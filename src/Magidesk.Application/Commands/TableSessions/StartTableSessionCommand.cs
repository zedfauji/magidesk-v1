using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to start a new table session.
/// </summary>
public record StartTableSessionCommand(
    Guid TableId,
    Guid TableTypeId,
    int GuestCount,
    Guid? CustomerId = null,
    Guid? TicketId = null,
    bool CreateTicket = false,
    Guid? UserId = null,
    Guid? TerminalId = null,
    Guid? ShiftId = null,
    Guid? OrderTypeId = null
);

/// <summary>
/// Result of starting a table session.
/// </summary>
public record StartTableSessionResult(
    Guid SessionId,
    DateTime StartTime,
    decimal HourlyRate,
    string TableTypeName,
    Guid? TicketId = null
);
