namespace Magidesk.Domain.Entities;

/// <summary>
/// Status of a club membership.
/// </summary>
public enum MembershipStatus
{
    Active = 0,
    Expired = 1,
    Suspended = 2,
    Cancelled = 3
}
