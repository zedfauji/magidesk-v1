using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to release a held ticket for payment processing.
/// </summary>
public record ReleaseHeldTicketCommand(
    Guid TicketId,
    UserId UserId
);
