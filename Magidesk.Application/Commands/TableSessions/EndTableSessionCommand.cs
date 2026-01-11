namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Command to end a table session and calculate charges.
/// </summary>
/// <param name="SessionId">ID of the session to end</param>
/// <param name="CreateTicket">Whether to create a new ticket (true) or add to existing (false)</param>
public record EndTableSessionCommand(
    Guid SessionId,
    bool CreateTicket = true,
    Guid? UserId = null,
    Guid? TerminalId = null,
    Guid? ShiftId = null,
    Guid? OrderTypeId = null
);
