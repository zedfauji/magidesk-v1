using FluentAssertions;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class ServiceChargeTests
{
    private readonly ServiceChargeDomainService _serviceChargeDomainService;
    private readonly TaxDomainService _taxDomainService;
    private readonly TicketDomainService _ticketDomainService;

    public ServiceChargeTests()
    {
        _serviceChargeDomainService = new ServiceChargeDomainService();
        _taxDomainService = new TaxDomainService();
        _ticketDomainService = new TicketDomainService(_taxDomainService);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(obj, value);
    }

    [Fact]
    public void CalculateServiceCharge_WithPercentage_ShouldCalculateCorrectly()
    {
        // Arrange
        var subtotal = new Money(100m);
        var serviceChargeRate = 0.15m; // 15%

        // Act
        var serviceCharge = _serviceChargeDomainService.CalculateServiceCharge(subtotal, serviceChargeRate);

        // Assert
        serviceCharge.Amount.Should().Be(15m); // 100 * 0.15
    }

    [Fact]
    public void CalculateServiceCharge_WithZeroRate_ShouldReturnZero()
    {
        // Arrange
        var subtotal = new Money(100m);
        var serviceChargeRate = 0m;

        // Act
        var serviceCharge = _serviceChargeDomainService.CalculateServiceCharge(subtotal, serviceChargeRate);

        // Assert
        serviceCharge.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateServiceCharge_WithZeroSubtotal_ShouldReturnZero()
    {
        // Arrange
        var subtotal = Money.Zero();
        var serviceChargeRate = 0.15m;

        // Act
        var serviceCharge = _serviceChargeDomainService.CalculateServiceCharge(subtotal, serviceChargeRate);

        // Assert
        serviceCharge.Should().Be(Money.Zero());
    }

    [Fact]
    public void CalculateServiceCharge_WithInvalidRate_ShouldThrowException()
    {
        // Arrange
        var subtotal = new Money(100m);
        var serviceChargeRate = 1.5m; // > 100%

        // Act
        var act = () => _serviceChargeDomainService.CalculateServiceCharge(subtotal, serviceChargeRate);

        // Assert
        act.Should().Throw<Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*Service charge rate must be between 0 and 1*");
    }

    [Fact]
    public void CalculateServiceChargeForTicket_ShouldUseSubtotalAfterDiscounts()
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
        
        // Note: Discounts are typically added through the Application layer
        // For this test, we'll manually set the discount amount
        // In a real scenario, discounts would be applied through ApplyDiscountCommand
        _ticketDomainService.CalculateTotals(ticket);
        
        // Manually set discount amount for testing
        SetPrivateProperty(ticket, "DiscountAmount", new Money(10m));
        
        // Subtotal = 100, Discount = 10, so subtotal after discounts = 90
        var serviceChargeRate = 0.10m; // 10%

        // Act
        var serviceCharge = _serviceChargeDomainService.CalculateServiceChargeForTicket(ticket, serviceChargeRate);

        // Assert
        // Service charge should be calculated on (100 - 10) = 90
        serviceCharge.Amount.Should().Be(9m); // 90 * 0.10
    }

    [Fact]
    public void CalculateServiceChargePerGuest_ShouldMultiplyCorrectly()
    {
        // Arrange
        var numberOfGuests = 4;
        var chargePerGuest = new Money(2.50m);

        // Act
        var totalCharge = _serviceChargeDomainService.CalculateServiceChargePerGuest(numberOfGuests, chargePerGuest);

        // Assert
        totalCharge.Amount.Should().Be(10m); // 4 * 2.50
    }

    [Fact]
    public void CalculateServiceChargePerGuest_WithZeroGuests_ShouldThrowException()
    {
        // Arrange
        var numberOfGuests = 0;
        var chargePerGuest = new Money(2.50m);

        // Act
        var act = () => _serviceChargeDomainService.CalculateServiceChargePerGuest(numberOfGuests, chargePerGuest);

        // Assert
        act.Should().Throw<Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*Number of guests must be greater than zero*");
    }

    [Fact]
    public void Ticket_SetServiceCharge_ShouldUpdateTotal()
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
        
        _ticketDomainService.CalculateTotals(ticket);
        var originalTotal = ticket.TotalAmount;

        // Act
        ticket.SetServiceCharge(new Money(10m));

        // Assert
        ticket.ServiceChargeAmount.Amount.Should().Be(10m);
        ticket.TotalAmount.Amount.Should().Be(originalTotal.Amount + 10m);
    }

    [Fact]
    public void Ticket_SetServiceCharge_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        var act = () => ticket.SetServiceCharge(new Money(-10m));

        // Assert
        act.Should().Throw<Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*Service charge cannot be negative*");
    }

    [Fact]
    public void Ticket_SetServiceCharge_WithClosedTicket_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetPrivateProperty(ticket, "Status", Enumerations.TicketStatus.Closed);

        // Act
        var act = () => ticket.SetServiceCharge(new Money(10m));

        // Assert
        act.Should().Throw<Domain.Exceptions.InvalidOperationException>()
            .WithMessage("*Cannot modify service charge on ticket*");
    }

    [Fact]
    public void Ticket_SetDeliveryCharge_ShouldUpdateTotal()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id, Guid.NewGuid(), "Test Item", 1m, new Money(100m));
        ticket.AddOrderLine(orderLine);
        
        _ticketDomainService.CalculateTotals(ticket);
        var originalTotal = ticket.TotalAmount;

        // Act
        ticket.SetDeliveryCharge(new Money(5m));

        // Assert
        ticket.DeliveryChargeAmount.Amount.Should().Be(5m);
        ticket.TotalAmount.Amount.Should().Be(originalTotal.Amount + 5m);
    }

    [Fact]
    public void Ticket_SetAdjustment_WithPositiveAmount_ShouldIncreaseTotal()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id, Guid.NewGuid(), "Test Item", 1m, new Money(100m));
        ticket.AddOrderLine(orderLine);
        
        _ticketDomainService.CalculateTotals(ticket);
        var originalTotal = ticket.TotalAmount;

        // Act
        ticket.SetAdjustment(new Money(5m));

        // Assert
        ticket.AdjustmentAmount.Amount.Should().Be(5m);
        ticket.TotalAmount.Amount.Should().Be(originalTotal.Amount + 5m);
    }

    [Fact]
    public void Ticket_SetAdjustment_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id, Guid.NewGuid(), "Test Item", 1m, new Money(100m));
        ticket.AddOrderLine(orderLine);
        
        _ticketDomainService.CalculateTotals(ticket);

        // Act
        // Note: Money doesn't allow negative values, so this will throw ArgumentException from Money constructor
        var act = () => ticket.SetAdjustment(new Money(-5m));

        // Assert
        // The Money constructor will throw ArgumentException before SetAdjustment is called
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Money amount cannot be negative*");
    }

    [Fact]
    public void Ticket_SetAdvancePayment_ShouldUpdateAdvanceAmount()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        ticket.SetAdvancePayment(new Money(20m));

        // Assert
        ticket.AdvanceAmount.Amount.Should().Be(20m);
    }

    [Fact]
    public void Ticket_SetAdvancePayment_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        var act = () => ticket.SetAdvancePayment(new Money(-10m));

        // Assert
        act.Should().Throw<Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*Advance payment cannot be negative*");
    }
}

