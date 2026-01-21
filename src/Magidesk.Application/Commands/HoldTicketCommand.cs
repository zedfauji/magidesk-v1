using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to hold a ticket for later payment.
/// </summary>
public record HoldTicketCommand(
    Guid TicketId,
    string Reason,
    UserId UserId
);
