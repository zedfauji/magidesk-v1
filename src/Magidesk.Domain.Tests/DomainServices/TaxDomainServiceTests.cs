using FluentAssertions;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.ValueObjects;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class TaxDomainServiceTests
{
    private readonly TaxDomainService _taxDomainService;

    public TaxDomainServiceTests()
    {
        _taxDomainService = new TaxDomainService();
    }

    [Fact]
    public void CalculateTax_WithTaxGroup_ShouldCalculateCorrectly()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax"),
            new TaxRate(0.05m, "City Tax")
        };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var subtotal = new Money(100m);

        // Act
        var taxAmount = _taxDomainService.CalculateTax(subtotal, taxGroup);

        // Assert
        taxAmount.Amount.Should().Be(15m); // 10 + 5
    }

    [Fact]
    public void CalculateTax_WithTaxExempt_ShouldReturnZero()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var subtotal = new Money(100m);

        // Act
        var taxAmount = _taxDomainService.CalculateTax(subtotal, taxGroup, isTaxExempt: true);

        // Assert
        taxAmount.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateTax_WithNullTaxGroup_ShouldReturnZero()
    {
        // Arrange
        TaxGroup? taxGroup = null;
        var subtotal = new Money(100m);

        // Act
        var taxAmount = _taxDomainService.CalculateTax(subtotal, taxGroup);

        // Assert
        taxAmount.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateTax_WithTaxRate_ShouldCalculateCorrectly()
    {
        // Arrange
        TaxRate? taxRate = new TaxRate(0.10m, "State Tax");
        var subtotal = new Money(100m);

        // Act
        var taxAmount = _taxDomainService.CalculateTax(subtotal, taxRate);

        // Assert
        taxAmount.Amount.Should().Be(10m);
    }

    [Fact]
    public void CalculateTax_WithTaxRateAndTaxExempt_ShouldReturnZero()
    {
        // Arrange
        var taxRate = new TaxRate(0.10m, "State Tax");
        var subtotal = new Money(100m);

        // Act
        var taxAmount = _taxDomainService.CalculateTax(subtotal, taxRate, isTaxExempt: true);

        // Assert
        taxAmount.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateBaseAmountFromInclusivePrice_WithTaxGroup_ShouldExtractBaseAmount()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var totalAmount = new Money(110m); // Price includes 10% tax

        // Act
        var baseAmount = _taxDomainService.CalculateBaseAmountFromInclusivePrice(totalAmount, taxGroup);

        // Assert
        // baseAmount = 110 / (1 + 0.10) = 100
        baseAmount.Amount.Should().BeApproximately(100m, 0.01m);
    }

    [Fact]
    public void CalculateBaseAmountFromInclusivePrice_WithTaxRate_ShouldExtractBaseAmount()
    {
        // Arrange
        TaxRate? taxRate = new TaxRate(0.10m, "State Tax");
        var totalAmount = new Money(110m);

        // Act
        var baseAmount = _taxDomainService.CalculateBaseAmountFromInclusivePrice(totalAmount, taxRate);

        // Assert
        // baseAmount = 110 / (1 + 0.10) = 100
        baseAmount.Amount.Should().BeApproximately(100m, 0.01m);
    }

    [Fact]
    public void CalculateBaseAmountFromInclusivePrice_WithNullTaxGroup_ShouldReturnTotalAmount()
    {
        // Arrange
        TaxGroup? taxGroup = null;
        var totalAmount = new Money(110m);

        // Act
        var baseAmount = _taxDomainService.CalculateBaseAmountFromInclusivePrice(totalAmount, taxGroup);

        // Assert
        baseAmount.Should().Be(totalAmount);
    }

    [Fact]
    public void CalculateTotalAmountWithTax_WithTaxGroup_ShouldAddTax()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var baseAmount = new Money(100m);

        // Act
        var totalAmount = _taxDomainService.CalculateTotalAmountWithTax(baseAmount, taxGroup);

        // Assert
        totalAmount.Amount.Should().Be(110m); // 100 + 10
    }

    [Fact]
    public void CalculateTotalAmountWithTax_WithTaxExempt_ShouldReturnBaseAmount()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var baseAmount = new Money(100m);

        // Act
        var totalAmount = _taxDomainService.CalculateTotalAmountWithTax(baseAmount, taxGroup, isTaxExempt: true);

        // Assert
        totalAmount.Should().Be(baseAmount);
    }

    [Fact]
    public void CalculateTaxBreakdown_WithMultipleRates_ShouldReturnBreakdown()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax"),
            new TaxRate(0.05m, "City Tax")
        };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var subtotal = new Money(100m);

        // Act
        var breakdown = _taxDomainService.CalculateTaxBreakdown(subtotal, taxGroup);

        // Assert
        breakdown.Should().HaveCount(2);
        breakdown["State Tax"].Amount.Should().Be(10m);
        breakdown["City Tax"].Amount.Should().Be(5m);
    }

    [Fact]
    public void CalculateTaxBreakdown_WithTaxExempt_ShouldReturnEmpty()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var subtotal = new Money(100m);

        // Act
        var breakdown = _taxDomainService.CalculateTaxBreakdown(subtotal, taxGroup, isTaxExempt: true);

        // Assert
        breakdown.Should().BeEmpty();
    }

    [Fact]
    public void CalculateTaxBreakdown_WithCompoundTax_ShouldCalculateCorrectly()
    {
        // Arrange
        var taxRates = new[]
        {
            new TaxRate(0.10m, "State Tax", isCompound: false),
            new TaxRate(0.05m, "City Tax", isCompound: true)
        };
        var taxGroup = new TaxGroup("Compound", taxRates);
        var subtotal = new Money(100m);

        // Act
        var breakdown = _taxDomainService.CalculateTaxBreakdown(subtotal, taxGroup);

        // Assert
        breakdown.Should().HaveCount(2);
        breakdown["State Tax"].Amount.Should().Be(10m);
        breakdown["City Tax"].Amount.Should().Be(5.50m); // (100 + 10) * 0.05
    }
}

