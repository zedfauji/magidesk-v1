using FluentAssertions;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Exceptions;
using Xunit;

namespace Magidesk.Domain.Tests.ValueObjects;

public class TaxRateTests
{
    [Fact]
    public void Constructor_WithValidRate_ShouldCreateTaxRate()
    {
        // Arrange & Act
        var taxRate = new TaxRate(0.10m, "State Tax");

        // Assert
        taxRate.Rate.Should().Be(0.10m);
        taxRate.Name.Should().Be("State Tax");
        taxRate.IsCompound.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithCompoundTax_ShouldCreateTaxRate()
    {
        // Arrange & Act
        var taxRate = new TaxRate(0.05m, "City Tax", isCompound: true);

        // Assert
        taxRate.Rate.Should().Be(0.05m);
        taxRate.Name.Should().Be("City Tax");
        taxRate.IsCompound.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNegativeRate_ShouldThrowBusinessRuleViolationException()
    {
        // Arrange & Act
        var act = () => new TaxRate(-0.01m, "Invalid Tax");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Tax rate must be between 0 and 1*");
    }

    [Fact]
    public void Constructor_WithRateGreaterThanOne_ShouldThrowBusinessRuleViolationException()
    {
        // Arrange & Act
        var act = () => new TaxRate(1.01m, "Invalid Tax");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Tax rate must be between 0 and 1*");
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new TaxRate(0.10m, null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tax rate name cannot be null or empty*");
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new TaxRate(0.10m, "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tax rate name cannot be null or empty*");
    }

    [Fact]
    public void Zero_ShouldReturnZeroTaxRate()
    {
        // Arrange & Act
        var taxRate = TaxRate.Zero();

        // Assert
        taxRate.Rate.Should().Be(0m);
        taxRate.Name.Should().Be("No Tax");
        taxRate.IsCompound.Should().BeFalse();
    }

    [Fact]
    public void CalculateTax_WithSimpleTax_ShouldCalculateCorrectly()
    {
        // Arrange
        var taxRate = new TaxRate(0.10m, "State Tax");
        var baseAmount = new Money(100m);

        // Act
        var taxAmount = taxRate.CalculateTax(baseAmount);

        // Assert
        taxAmount.Amount.Should().Be(10m);
    }

    [Fact]
    public void CalculateTax_WithZeroRate_ShouldReturnZero()
    {
        // Arrange
        var taxRate = TaxRate.Zero();
        var baseAmount = new Money(100m);

        // Act
        var taxAmount = taxRate.CalculateTax(baseAmount);

        // Assert
        taxAmount.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateTax_WithCompoundTax_ShouldCalculateOnBasePlusPreviousTaxes()
    {
        // Arrange
        var taxRate = new TaxRate(0.10m, "Compound Tax", isCompound: true);
        var baseAmount = new Money(100m);
        var previousTaxes = new Money(5m); // Previous tax of $5

        // Act
        var taxAmount = taxRate.CalculateTax(baseAmount, previousTaxes);

        // Assert
        // Compound tax: (100 + 5) * 0.10 = 10.50
        taxAmount.Amount.Should().Be(10.50m);
    }

    [Fact]
    public void CalculateTax_WithNonCompoundTax_ShouldIgnorePreviousTaxes()
    {
        // Arrange
        var taxRate = new TaxRate(0.10m, "Simple Tax", isCompound: false);
        var baseAmount = new Money(100m);
        var previousTaxes = new Money(5m);

        // Act
        var taxAmount = taxRate.CalculateTax(baseAmount, previousTaxes);

        // Assert
        // Non-compound tax: 100 * 0.10 = 10 (ignores previous taxes)
        taxAmount.Amount.Should().Be(10m);
    }

    [Fact]
    public void CalculateTax_WithZeroBaseAmount_ShouldReturnZero()
    {
        // Arrange
        var taxRate = new TaxRate(0.10m, "State Tax");
        var baseAmount = Money.Zero();

        // Act
        var taxAmount = taxRate.CalculateTax(baseAmount);

        // Assert
        taxAmount.Should().Be(Money.Zero());
    }
}

