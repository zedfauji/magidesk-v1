using FluentAssertions;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Exceptions;
using Xunit;

namespace Magidesk.Domain.Tests.ValueObjects;

public class TaxGroupTests
{
    [Fact]
    public void Constructor_WithValidTaxRates_ShouldCreateTaxGroup()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax"),
            new TaxRate(0.05m, "City Tax")
        };

        // Act
        var taxGroup = new TaxGroup("Standard", taxRates);

        // Assert
        taxGroup.Name.Should().Be("Standard");
        taxGroup.TaxRates.Should().HaveCount(2);
        taxGroup.TaxRates.Should().Contain(r => r.Name == "State Tax");
        taxGroup.TaxRates.Should().Contain(r => r.Name == "City Tax");
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentException()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };

        // Act
        var act = () => new TaxGroup(null!, taxRates);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tax group name cannot be null or empty*");
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };

        // Act
        var act = () => new TaxGroup("", taxRates);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tax group name cannot be null or empty*");
    }

    [Fact]
    public void Constructor_WithNullTaxRates_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new TaxGroup("Standard", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*taxRates*");
    }

    [Fact]
    public void Constructor_WithEmptyTaxRates_ShouldThrowBusinessRuleViolationException()
    {
        // Arrange
        var taxRates = Array.Empty<TaxRate>();

        // Act
        var act = () => new TaxGroup("Standard", taxRates);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Tax group must contain at least one tax rate*");
    }

    [Fact]
    public void CalculateTotalTax_WithMultipleRates_ShouldSumAllTaxes()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax"),
            new TaxRate(0.05m, "City Tax")
        };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var baseAmount = new Money(100m);

        // Act
        var totalTax = taxGroup.CalculateTotalTax(baseAmount);

        // Assert
        // State Tax: 100 * 0.10 = 10
        // City Tax: 100 * 0.05 = 5
        // Total: 15
        totalTax.Amount.Should().Be(15m);
    }

    [Fact]
    public void CalculateTotalTax_WithCompoundTax_ShouldCalculateCorrectly()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax", isCompound: false),
            new TaxRate(0.05m, "City Tax", isCompound: true) // Compound on base + state tax
        };
        var taxGroup = new TaxGroup("Compound", taxRates);
        var baseAmount = new Money(100m);

        // Act
        var totalTax = taxGroup.CalculateTotalTax(baseAmount);

        // Assert
        // State Tax: 100 * 0.10 = 10
        // City Tax (compound): (100 + 10) * 0.05 = 5.50
        // Total: 15.50
        totalTax.Amount.Should().Be(15.50m);
    }

    [Fact]
    public void CalculateTotalTax_WithZeroBaseAmount_ShouldReturnZero()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var baseAmount = Money.Zero();

        // Act
        var totalTax = taxGroup.CalculateTotalTax(baseAmount);

        // Assert
        totalTax.Should().Be(Money.Zero());
    }

    [Fact]
    public void CombinedRate_ShouldSumAllRates()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax"),
            new TaxRate(0.05m, "City Tax"),
            new TaxRate(0.02m, "County Tax")
        };
        var taxGroup = new TaxGroup("Standard", taxRates);

        // Act
        var combinedRate = taxGroup.CombinedRate;

        // Assert
        combinedRate.Should().Be(0.17m); // 0.10 + 0.05 + 0.02
    }

    [Fact]
    public void NoTax_ShouldReturnZeroTaxGroup()
    {
        // Arrange & Act
        var taxGroup = TaxGroup.NoTax();

        // Assert
        taxGroup.Name.Should().Be("No Tax");
        taxGroup.TaxRates.Should().HaveCount(1);
        taxGroup.TaxRates.First().Rate.Should().Be(0m);
    }
}

