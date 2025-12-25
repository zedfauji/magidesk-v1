using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get the current open cash session for a user.
/// </summary>
public class GetCurrentCashSessionQuery
{
    public Guid UserId { get; set; }
}

/// <summary>
/// Result of getting current cash session.
/// </summary>
public class GetCurrentCashSessionResult
{
    public CashSessionDto? CashSession { get; set; }
}

