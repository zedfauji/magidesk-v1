using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to close a cash session.
/// </summary>
public class CloseCashSessionCommand
{
    public Guid CashSessionId { get; set; }
    public UserId ClosedBy { get; set; } = null!;
    public Money ActualCash { get; set; } = null!;
}

/// <summary>
/// Result of closing a cash session.
/// </summary>
public class CloseCashSessionResult
{
    public Guid CashSessionId { get; set; }
    public Money ExpectedCash { get; set; } = null!;
    public Money ActualCash { get; set; } = null!;
    public Money Difference { get; set; } = null!;
    public DateTime ClosedAt { get; set; }
}

