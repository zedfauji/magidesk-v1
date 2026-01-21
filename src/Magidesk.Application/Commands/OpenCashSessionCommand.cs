using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to open a new cash session.
/// </summary>
public class OpenCashSessionCommand
{
    public UserId UserId { get; set; } = null!;
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public Money OpeningBalance { get; set; } = null!;
}

/// <summary>
/// Result of opening a cash session.
/// </summary>
public class OpenCashSessionResult
{
    public Guid CashSessionId { get; set; }
    public DateTime OpenedAt { get; set; }
}

