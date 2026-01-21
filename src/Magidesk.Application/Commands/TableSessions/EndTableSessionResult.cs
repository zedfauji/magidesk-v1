using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands.TableSessions;

/// <summary>
/// Result of ending a table session.
/// </summary>
public record EndTableSessionResult(
    Guid SessionId,
    Guid? TicketId,
    TimeSpan Duration,
    Money TotalCharge,
    DateTime EndTime
);
