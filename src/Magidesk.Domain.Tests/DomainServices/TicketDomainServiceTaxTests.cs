using FluentAssertions;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class TicketDomainServiceTaxTests
{
    private readonly TaxDomainService _taxDomainService;
    private readonly TicketDomainService _ticketDomainService;

    public TicketDomainServiceTaxTests()
    {
        _taxDomainService = new TaxDomainService();
        _ticketDomainService = new TicketDomainService(_taxDomainService);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(obj, value);
    }

    [Fact]
    public void CalculateTotals_WithTaxGroup_ShouldCalculateTaxCorrectly()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(100m),
            taxRate: 0m); // Order line tax rate not used when using tax group
        
        ticket.AddOrderLine(orderLine);

        // Act
        _ticketDomainService.CalculateTotals(ticket, taxGroup);

        // Assert
        ticket.SubtotalAmount.Amount.Should().Be(100m);
        ticket.TaxAmount.Amount.Should().Be(10m); // 100 * 0.10
        ticket.TotalAmount.Amount.Should().Be(110m); // 100 + 10
    }

    [Fact]
    public void CalculateTotals_WithTaxExempt_ShouldReturnZeroTax()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(100m));
        
        ticket.AddOrderLine(orderLine);
        
        // Set tax exempt using reflection
        SetPrivateProperty(ticket, "IsTaxExempt", true);

        // Act
        _ticketDomainService.CalculateTotals(ticket, taxGroup);

        // Assert
        ticket.SubtotalAmount.Amount.Should().Be(100m);
        ticket.TaxAmount.Should().Be(Money.Zero());
        ticket.TotalAmount.Amount.Should().Be(100m);
    }

    [Fact]
    public void CalculateTotals_WithPriceIncludesTax_ShouldExtractTaxFromSubtotal()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        // When price includes tax, order line total already includes tax
        // So if item price is $110, that includes $10 tax
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(110m)); // Price includes tax
        
        ticket.AddOrderLine(orderLine);
        
        // Set PriceIncludesTax using reflection
        SetPrivateProperty(ticket, "PriceIncludesTax", true);

        // Act
        _ticketDomainService.CalculateTotals(ticket, taxGroup);

        // Assert
        ticket.SubtotalAmount.Amount.Should().Be(110m); // Total including tax
        // Tax should be extracted: 110 - (110 / 1.10) = 110 - 100 = 10
        ticket.TaxAmount.Amount.Should().BeApproximately(10m, 0.01m);
        // Total should be subtotal (tax already included) = 110
        ticket.TotalAmount.Amount.Should().BeApproximately(110m, 0.01m);
    }

    [Fact]
    public void CalculateTotals_WithMultipleOrderLines_ShouldSumCorrectly()
    {
        // Arrange
        var taxRates = new[] { new TaxRate(0.10m, "State Tax") };
        var taxGroup = new TaxGroup("Standard", taxRates);
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine1 = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Item 1",
            1m,
            new Money(50m));
        
        var orderLine2 = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Item 2",
            1m,
            new Money(50m));
        
        ticket.AddOrderLine(orderLine1);
        ticket.AddOrderLine(orderLine2);

        // Act
        _ticketDomainService.CalculateTotals(ticket, taxGroup);

        // Assert
        ticket.SubtotalAmount.Amount.Should().Be(100m); // 50 + 50
        ticket.TaxAmount.Amount.Should().Be(10m); // 100 * 0.10
        ticket.TotalAmount.Amount.Should().Be(110m); // 100 + 10
    }

    [Fact]
    public void CalculateTotals_WithNullTaxGroup_ShouldReturnZeroTax()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(100m));
        
        ticket.AddOrderLine(orderLine);

        // Act
        _ticketDomainService.CalculateTotals(ticket, null);

        // Assert
        ticket.SubtotalAmount.Amount.Should().Be(100m);
        ticket.TaxAmount.Should().Be(Money.Zero());
        ticket.TotalAmount.Amount.Should().Be(100m);
    }
}

