using System;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

public class MemberTests
{
    [Fact]
    public void Create_WithValidData_ReturnsMember()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var tierId = Guid.NewGuid();
        var memberNumber = "M1001";

        // Act
        var member = Member.Create(customerId, tierId, memberNumber);

        // Assert
        Assert.NotNull(member);
        Assert.Equal(customerId, member.CustomerId);
        Assert.Equal(tierId, member.TierId);
        Assert.Equal(memberNumber, member.MemberNumber);
        Assert.Equal(MembershipStatus.Active, member.Status);
        Assert.Equal(Money.Zero(), member.PrepaidBalance);
        Assert.True(member.IsActive);
    }

    [Fact]
    public void Renew_UpdatesExpirationDateAndStatus()
    {
        // Arrange
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), "M1001");
        var expiration = DateTime.UtcNow.AddYears(1);

        // Act
        member.Renew(expiration);

        // Assert
        Assert.Equal(expiration, member.ExpirationDate);
        Assert.True(member.IsActive);
    }

    [Fact]
    public void Renew_WithPastDate_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), "M1001");
        var expiration = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => member.Renew(expiration));
    }

    [Fact]
    public void Suspend_SetsStatusToSuspended()
    {
        // Arrange
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), "M1001");

        // Act
        member.Suspend("Non-payment");

        // Assert
        Assert.Equal(MembershipStatus.Suspended, member.Status);
        Assert.False(member.IsActive);
    }

    [Fact]
    public void PrepaidBalance_AddAndDeductWorkingCorrectly()
    {
        // Arrange
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), "M1001");
        var credit = new Money(100m);

        // Act
        member.AddPrepaidCredit(credit);
        var success = member.TryDeductCredit(new Money(40m));

        // Assert
        Assert.True(success);
        Assert.Equal(new Money(60m), member.PrepaidBalance);
    }

    [Fact]
    public void TryDeductCredit_InsufficientBalance_ReturnsFalse()
    {
        // Arrange
        var member = Member.Create(Guid.NewGuid(), Guid.NewGuid(), "M1001");
        member.AddPrepaidCredit(new Money(10m));

        // Act
        var success = member.TryDeductCredit(new Money(20m));

        // Assert
        Assert.False(success);
        Assert.Equal(new Money(10m), member.PrepaidBalance);
    }
}
