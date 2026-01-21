using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get a cash session by ID.
/// </summary>
public class GetCashSessionQuery
{
    public Guid CashSessionId { get; set; }
}

/// <summary>
/// Result of getting a cash session.
/// </summary>
public class GetCashSessionResult
{
    public CashSessionDto? CashSession { get; set; }
}

