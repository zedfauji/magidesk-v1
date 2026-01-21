using System;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a club member, linking a Customer record to a membership plan.
/// </summary>
public class Member
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid TierId { get; private set; }
    public string MemberNumber { get; private set; } = string.Empty;
    public DateTime JoinDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public MembershipStatus Status { get; private set; }
    public Money PrepaidBalance { get; private set; }

    // Navigation
    public Customer Customer { get; private set; } = null!;
    public MembershipTier Tier { get; private set; } = null!;

    // Private constructor for EF Core
    private Member()
    {
        PrepaidBalance = Money.Zero();
    }

    public static Member Create(Guid customerId, Guid tierId, string memberNumber)
    {
        if (string.IsNullOrWhiteSpace(memberNumber))
            throw new BusinessRuleViolationException("Member number is required.");

        return new Member
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TierId = tierId,
            MemberNumber = memberNumber,
            JoinDate = DateTime.UtcNow,
            Status = MembershipStatus.Active,
            PrepaidBalance = Money.Zero()
        };
    }

    public bool IsActive => Status == MembershipStatus.Active &&
                           (ExpirationDate == null || ExpirationDate > DateTime.UtcNow);

    public void Renew(DateTime newExpirationDate)
    {
        if (newExpirationDate <= DateTime.UtcNow)
            throw new BusinessRuleViolationException("New expiration date must be in the future.");

        ExpirationDate = newExpirationDate;
        if (Status == MembershipStatus.Expired)
        {
            Status = MembershipStatus.Active;
        }
    }

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessRuleViolationException("Suspension reason is required.");

        Status = MembershipStatus.Suspended;
    }

    public void Reactivate()
    {
        Status = MembershipStatus.Active;
    }

    public void UpgradeTier(Guid newTierId)
    {
        TierId = newTierId;
    }

    public void AddPrepaidCredit(Money amount)
    {
        if (amount <= Money.Zero())
            throw new BusinessRuleViolationException("Credit amount must be positive.");

        PrepaidBalance += amount;
    }

    public bool TryDeductCredit(Money amount)
    {
        if (amount <= Money.Zero())
            throw new BusinessRuleViolationException("Deduction amount must be positive.");

        if (PrepaidBalance < amount)
            return false;

        PrepaidBalance -= amount;
        return true;
    }
}
