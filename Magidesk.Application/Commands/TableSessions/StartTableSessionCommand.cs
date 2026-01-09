using System;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to start a new table session.
/// </summary>
public record StartTableSessionCommand(
    Guid TableId,
    Guid TableTypeId,
    Guid? CustomerId,
    int GuestCount,
    Guid? TicketId = null
);

/// <summary>
/// Result of starting a table session.
/// </summary>
public record StartTableSessionResult(
    Guid SessionId,
    DateTime StartTime,
    decimal HourlyRate,
    string TableTypeName
);
