using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a cash session is opened.
/// </summary>
public sealed class CashSessionOpened : DomainEventBase
{
    public Guid CashSessionId { get; }
    public UserId UserId { get; }
    public Money OpeningBalance { get; }

    public CashSessionOpened(Guid cashSessionId, UserId userId, Money openingBalance, Guid? correlationId = null)
        : base(correlationId)
    {
        CashSessionId = cashSessionId;
        UserId = userId;
        OpeningBalance = openingBalance;
    }
}

/// <summary>
/// Domain event raised when a cash session is closed.
/// </summary>
public sealed class CashSessionClosed : DomainEventBase
{
    public Guid CashSessionId { get; }
    public UserId ClosedBy { get; }
    public Money ExpectedCash { get; }
    public Money ActualCash { get; }
    public Money Difference { get; }

    public CashSessionClosed(
        Guid cashSessionId,
        UserId closedBy,
        Money expectedCash,
        Money actualCash,
        Money difference,
        Guid? correlationId = null)
        : base(correlationId)
    {
        CashSessionId = cashSessionId;
        ClosedBy = closedBy;
        ExpectedCash = expectedCash;
        ActualCash = actualCash;
        Difference = difference;
    }
}

