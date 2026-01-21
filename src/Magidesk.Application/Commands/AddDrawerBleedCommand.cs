using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Commands;

/// <summary>
/// Command to add a drawer bleed to a cash session.
/// </summary>
public class AddDrawerBleedCommand
{
    public Guid CashSessionId { get; set; }
    public Money Amount { get; set; } = null!;
    public UserId ProcessedBy { get; set; } = null!;
    public string? Reason { get; set; }
}

/// <summary>
/// Result of adding a drawer bleed.
/// </summary>
public class AddDrawerBleedResult
{
    public Guid DrawerBleedId { get; set; }
    public Guid CashSessionId { get; set; }
    public Money Amount { get; set; } = null!;
    public Money UpdatedExpectedCash { get; set; } = null!;
}

