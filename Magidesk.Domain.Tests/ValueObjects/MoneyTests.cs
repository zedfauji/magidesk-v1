using FluentAssertions;
using Magidesk.Domain.ValueObjects;
using Xunit;
using System;

namespace Magidesk.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(100.50m);

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_WithCustomCurrency_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(100.50m, "EUR");

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Money(-10m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Money amount cannot be negative.*");
    }

    [Fact]
    public void Constructor_WithNullCurrency_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Money(100m, null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency cannot be null or empty.*");
    }

    [Fact]
    public void Constructor_WithEmptyCurrency_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new Money(100m, "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Currency cannot be null or empty.*");
    }

    [Fact]
    public void Constructor_WithMoreThanTwoDecimals_ShouldRoundToTwoDecimals()
    {
        // Arrange & Act
        var money = new Money(100.555m);

        // Assert
        money.Amount.Should().Be(100.56m);
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        // Arrange & Act
        var money = Money.Zero();

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Zero_WithCustomCurrency_ShouldReturnZeroAmount()
    {
        // Arrange & Act
        var money = Money.Zero("EUR");

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Addition_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(100.50m);
        var money2 = new Money(50.25m);

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150.75m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        // Act
        var act = () => money1 + money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtraction_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = new Money(100.50m);
        var money2 = new Money(50.25m);

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(50.25m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtraction_WithResultNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(50m);
        var money2 = new Money(100m);

        // Act
        var act = () => money1 - money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Money subtraction result cannot be negative.*");
    }

    [Fact]
    public void Subtraction_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        // Act
        var act = () => money1 - money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Multiplication_WithPositiveFactor_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(100m);
        var factor = 1.5m;

        // Act
        var result = money * factor;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Multiplication_WithNegativeFactor_ShouldThrowArgumentException()
    {
        // Arrange
        var money = new Money(100m);
        var factor = -1m;

        // Act
        var act = () => money * factor;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Multiplication factor cannot be negative.*");
    }

    [Fact]
    public void Division_WithPositiveDivisor_ShouldDivideAmount()
    {
        // Arrange
        var money = new Money(100m);
        var divisor = 2m;

        // Act
        var result = money / divisor;

        // Assert
        result.Amount.Should().Be(50m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Division_WithZeroDivisor_ShouldThrowArgumentException()
    {
        // Arrange
        var money = new Money(100m);
        var divisor = 0m;

        // Act
        var act = () => money / divisor;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Divisor must be greater than zero.*");
    }

    [Fact]
    public void Division_WithNegativeDivisor_ShouldThrowArgumentException()
    {
        // Arrange
        var money = new Money(100m);
        var divisor = -1m;

        // Act
        var act = () => money / divisor;

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Divisor must be greater than zero.*");
    }

    [Fact]
    public void Comparison_LessThan_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(50m);
        var money2 = new Money(100m);

        // Act & Assert
        (money1 < money2).Should().BeTrue();
        (money2 < money1).Should().BeFalse();
    }

    [Fact]
    public void Comparison_GreaterThan_ShouldCompareCorrectly()
    {
        // Arrange
        var money1 = new Money(100m);
        var money2 = new Money(50m);

        // Act & Assert
        (money1 > money2).Should().BeTrue();
        (money2 > money1).Should().BeFalse();
    }

    [Fact]
    public void Comparison_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");

        // Act
        var act = () => { var _ = money1 < money2; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Equality_WithSameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        (money1 == money2).Should().BeTrue();
        money1.Equals(money2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentAmount_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");

        // Act & Assert
        (money1 == money2).Should().BeFalse();
        money1.Equals(money2).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithDifferentCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = new Money(100m, "USD");
        var money2 = new Money(100m, "EUR");

        // Act & Assert
        (money1 == money2).Should().BeFalse();
        money1.Equals(money2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(100.50m, "USD");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("USD 100.50");
    }
}

