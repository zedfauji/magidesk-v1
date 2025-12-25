using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;
using DomainInvalidOperationException = Magidesk.Domain.Exceptions.InvalidOperationException;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class CashSessionTests
{
    private readonly UserId _userId = new UserId(Guid.NewGuid());
    private readonly Guid _terminalId = Guid.NewGuid();
    private readonly Guid _shiftId = Guid.NewGuid();

    [Fact]
    public void CashSession_AddPayout_ShouldAddPayoutAndRecalculateExpectedCash()
    {
        // Arrange
        var openingBalance = new Money(100m);
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, openingBalance);
        var payoutAmount = new Money(20m);
        var payout = Payout.Create(cashSession.Id, payoutAmount, _userId, "Test payout");

        // Act
        cashSession.AddPayout(payout);

        // Assert
        cashSession.Payouts.Should().Contain(payout);
        cashSession.ExpectedCash.Should().Be(new Money(80m)); // 100 - 20
    }

    [Fact]
    public void CashSession_AddPayout_WithClosedSession_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));
        cashSession.Close(_userId, new Money(100m));
        var payout = Payout.Create(cashSession.Id, new Money(10m), _userId);

        // Act
        var act = () => cashSession.AddPayout(payout);

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*closed session*");
    }

    [Fact]
    public void CashSession_AddPayout_WithWrongCashSessionId_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));
        var wrongSessionId = Guid.NewGuid();
        var payout = Payout.Create(wrongSessionId, new Money(10m), _userId);

        // Act
        var act = () => cashSession.AddPayout(payout);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*does not belong*");
    }

    [Fact]
    public void CashSession_AddCashDrop_ShouldAddCashDropAndRecalculateExpectedCash()
    {
        // Arrange
        var openingBalance = new Money(100m);
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, openingBalance);
        var cashDropAmount = new Money(30m);
        var cashDrop = CashDrop.Create(cashSession.Id, cashDropAmount, _userId, "Test drop");

        // Act
        cashSession.AddCashDrop(cashDrop);

        // Assert
        cashSession.CashDrops.Should().Contain(cashDrop);
        cashSession.ExpectedCash.Should().Be(new Money(70m)); // 100 - 30
    }

    [Fact]
    public void CashSession_AddCashDrop_WithClosedSession_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));
        cashSession.Close(_userId, new Money(100m));
        var cashDrop = CashDrop.Create(cashSession.Id, new Money(10m), _userId);

        // Act
        var act = () => cashSession.AddCashDrop(cashDrop);

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*closed session*");
    }

    [Fact]
    public void CashSession_AddDrawerBleed_ShouldAddDrawerBleedAndRecalculateExpectedCash()
    {
        // Arrange
        var openingBalance = new Money(100m);
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, openingBalance);
        var bleedAmount = new Money(15m);
        var drawerBleed = DrawerBleed.Create(cashSession.Id, bleedAmount, _userId, "Test bleed");

        // Act
        cashSession.AddDrawerBleed(drawerBleed);

        // Assert
        cashSession.DrawerBleeds.Should().Contain(drawerBleed);
        cashSession.ExpectedCash.Should().Be(new Money(85m)); // 100 - 15
    }

    [Fact]
    public void CashSession_AddDrawerBleed_WithClosedSession_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));
        cashSession.Close(_userId, new Money(100m));
        var drawerBleed = DrawerBleed.Create(cashSession.Id, new Money(10m), _userId);

        // Act
        var act = () => cashSession.AddDrawerBleed(drawerBleed);

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*closed session*");
    }

    [Fact]
    public void CashSession_CalculateExpectedCash_WithMultipleTransactions_ShouldCalculateCorrectly()
    {
        // Arrange
        var openingBalance = new Money(100m);
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, openingBalance);
        
        // Add cash payment (receipt)
        var payment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(50m),
            _userId,
            _terminalId);
        // Use reflection to set CashSessionId since it's a protected property
        var cashSessionIdProperty = typeof(Payment).GetProperty("CashSessionId", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        cashSessionIdProperty?.SetValue(payment, cashSession.Id);
        cashSession.AddPayment(payment);

        // Add payout
        var payout = Payout.Create(cashSession.Id, new Money(20m), _userId);
        cashSession.AddPayout(payout);

        // Add cash drop
        var cashDrop = CashDrop.Create(cashSession.Id, new Money(10m), _userId);
        cashSession.AddCashDrop(cashDrop);

        // Add drawer bleed
        var drawerBleed = DrawerBleed.Create(cashSession.Id, new Money(5m), _userId);
        cashSession.AddDrawerBleed(drawerBleed);

        // Act
        cashSession.CalculateExpectedCash();

        // Assert
        // Expected = 100 (opening) + 50 (receipt) - 20 (payout) - 10 (drop) - 5 (bleed) = 115
        cashSession.ExpectedCash.Should().Be(new Money(115m));
    }

    [Fact]
    public void CashSession_AddPayout_WithNullPayout_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));

        // Act
        var act = () => cashSession.AddPayout(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CashSession_AddCashDrop_WithNullCashDrop_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));

        // Act
        var act = () => cashSession.AddCashDrop(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CashSession_AddDrawerBleed_WithNullDrawerBleed_ShouldThrowException()
    {
        // Arrange
        var cashSession = CashSession.Open(_userId, _terminalId, _shiftId, new Money(100m));

        // Act
        var act = () => cashSession.AddDrawerBleed(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}

