using FluentAssertions;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class RefundTests
{
    private readonly TaxDomainService _taxDomainService;
    private readonly TicketDomainService _ticketDomainService;
    private readonly PaymentDomainService _paymentDomainService;

    public RefundTests()
    {
        _taxDomainService = new TaxDomainService();
        _ticketDomainService = new TicketDomainService(_taxDomainService);
        _paymentDomainService = new PaymentDomainService();
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(obj, value);
    }

    [Fact]
    public void Payment_CreateRefund_ShouldCreateDebitTransaction()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var originalPayment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(100m),
            userId,
            Guid.NewGuid());

        // Act
        var refundPayment = Payment.CreateRefund(
            originalPayment,
            new Money(50m),
            userId,
            Guid.NewGuid(),
            "Customer request");

        // Assert
        refundPayment.TransactionType.Should().Be(TransactionType.Debit);
        refundPayment.Amount.Amount.Should().Be(50m);
        refundPayment.PaymentType.Should().Be(PaymentType.Cash);
        refundPayment.TicketId.Should().Be(originalPayment.TicketId);
    }

    [Fact]
    public void Payment_CreateRefund_WithAmountExceedingOriginal_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var originalPayment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(100m),
            userId,
            Guid.NewGuid());

        // Act
        var act = () => Payment.CreateRefund(
            originalPayment,
            new Money(150m),
            userId,
            Guid.NewGuid());

        // Assert
        act.Should().Throw<Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*cannot exceed original payment amount*");
    }

    [Fact]
    public void Payment_CreateRefund_WithVoidedPayment_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var originalPayment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(100m),
            userId,
            Guid.NewGuid());
        originalPayment.Void();

        // Act
        var act = () => Payment.CreateRefund(
            originalPayment,
            new Money(50m),
            userId,
            Guid.NewGuid());

        // Assert
        act.Should().Throw<Domain.Exceptions.InvalidOperationException>()
            .WithMessage("*Cannot refund a voided payment*");
    }

    [Fact]
    public void Ticket_ProcessRefund_ShouldReducePaidAmount()
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
        
        // Calculate totals first
        _ticketDomainService.CalculateTotals(ticket);
        
        var payment = CashPayment.Create(
            ticket.Id,
            new Money(100m),
            userId,
            Guid.NewGuid());
        ticket.AddPayment(payment);
        
        // Ensure ticket is properly closed for testing
        // Calculate totals first
        ticket.CalculateTotals();
        
        // Manually transition to Closed status for testing
        // (In real scenario, ticket would be Paid first, then Closed)
        SetPrivateProperty(ticket, "Status", TicketStatus.Closed);

        // Create refund
        var refundPayment = Payment.CreateRefund(
            payment,
            new Money(50m),
            userId,
            Guid.NewGuid());

        // Act
        ticket.ProcessRefund(refundPayment);

        // Assert
        ticket.PaidAmount.Amount.Should().Be(50m); // 100 - 50
        ticket.Status.Should().Be(TicketStatus.Closed); // Still closed, not fully refunded
    }

    [Fact]
    public void Ticket_ProcessRefund_FullRefund_ShouldMarkAsRefunded()
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
        
        // Calculate totals first
        _ticketDomainService.CalculateTotals(ticket);
        
        var payment = CashPayment.Create(
            ticket.Id,
            new Money(100m),
            userId,
            Guid.NewGuid());
        ticket.AddPayment(payment);
        
        // Ensure ticket is properly closed for testing
        // Calculate totals first
        ticket.CalculateTotals();
        
        // Manually transition to Closed status for testing
        // (In real scenario, ticket would be Paid first, then Closed)
        SetPrivateProperty(ticket, "Status", TicketStatus.Closed);

        // Create full refund
        var refundPayment = Payment.CreateRefund(
            payment,
            new Money(100m),
            userId,
            Guid.NewGuid());

        // Act
        ticket.ProcessRefund(refundPayment);

        // Assert
        ticket.PaidAmount.Should().Be(Money.Zero());
        ticket.Status.Should().Be(TicketStatus.Refunded);
    }

    [Fact]
    public void Ticket_ProcessRefund_WithNonClosedTicket_ShouldThrowException()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var refundPayment = Payment.CreateRefund(
            CashPayment.Create(ticket.Id, new Money(100m), userId, Guid.NewGuid()),
            new Money(50m),
            userId,
            Guid.NewGuid());

        // Act
        var act = () => ticket.ProcessRefund(refundPayment);

        // Assert
        act.Should().Throw<Domain.Exceptions.InvalidOperationException>()
            .WithMessage("*Cannot refund ticket*");
    }

    [Fact]
    public void PaymentDomainService_CanRefundPayment_WithValidPayment_ShouldReturnTrue()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var payment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(100m),
            userId,
            Guid.NewGuid());

        // Act
        var canRefund = _paymentDomainService.CanRefundPayment(payment, new Money(50m));

        // Assert
        canRefund.Should().BeTrue();
    }

    [Fact]
    public void PaymentDomainService_CanRefundPayment_WithVoidedPayment_ShouldReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var payment = CashPayment.Create(
            Guid.NewGuid(),
            new Money(100m),
            userId,
            Guid.NewGuid());
        payment.Void();

        // Act
        var canRefund = _paymentDomainService.CanRefundPayment(payment, new Money(50m));

        // Assert
        canRefund.Should().BeFalse();
    }

    [Fact]
    public void TicketDomainService_CanRefundTicket_WithClosedTicket_ShouldReturnTrue()
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
        
        // Calculate totals and add payment
        _ticketDomainService.CalculateTotals(ticket);
        var payment = CashPayment.Create(ticket.Id, new Money(100m), userId, Guid.NewGuid());
        ticket.AddPayment(payment);
        
        // Ensure ticket is properly closed for testing
        // Calculate totals first
        ticket.CalculateTotals();
        
        // Manually transition to Closed status for testing
        // (In real scenario, ticket would be Paid first, then Closed)
        SetPrivateProperty(ticket, "Status", TicketStatus.Closed);

        // Act
        var canRefund = _ticketDomainService.CanRefundTicket(ticket, new Money(50m));

        // Assert
        canRefund.Should().BeTrue();
    }

    [Fact]
    public void TicketDomainService_CanRefundTicket_WithOpenTicket_ShouldReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetPrivateProperty(ticket, "Status", TicketStatus.Open);

        // Act
        var canRefund = _ticketDomainService.CanRefundTicket(ticket, new Money(50m));

        // Assert
        canRefund.Should().BeFalse();
    }
}

