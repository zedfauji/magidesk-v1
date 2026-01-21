using System;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Unit tests for Ticket void and refund operations.
/// Task 2.3.6: Tests REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.4, REQ-5.5, REQ-5.9
/// </summary>
public class TicketVoidRefundTests
{
    private readonly UserId _testUserId = new UserId(Guid.NewGuid());
    private readonly UserId _managerId = new UserId(Guid.NewGuid());
    private readonly Guid _terminalId = Guid.NewGuid();
    private readonly Guid _shiftId = Guid.NewGuid();
    private readonly Guid _orderTypeId = Guid.NewGuid();

    /// <summary>
    /// Test: Void open ticket (success)
    /// REQ-5.1: Open tickets can be voided with reason and authorization
    /// </summary>
    [Fact]
    public void Void_OpenTicket_Success()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        var reason = "Customer cancelled order";

        // Act
        ticket.Void(reason, _managerId);

        // Assert
        Assert.Equal(TicketStatus.Voided, ticket.Status);
        Assert.Equal(_managerId, ticket.VoidedBy);
        Assert.Equal(reason, ticket.Properties["VoidReason"]);
    }

    /// <summary>
    /// Test: Void draft ticket (success)
    /// REQ-5.1: Draft tickets can be voided
    /// </summary>
    [Fact]
    public void Void_DraftTicket_Success()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        var reason = "Order entry error";

        // Act
        ticket.Void(reason, _managerId);

        // Assert
        Assert.Equal(TicketStatus.Voided, ticket.Status);
        Assert.Equal(_managerId, ticket.VoidedBy);
    }

    /// <summary>
    /// Test: Void held ticket (success)
    /// REQ-5.1: Held tickets can be voided
    /// </summary>
    [Fact]
    public void Void_HeldTicket_Success()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        ticket.Hold("Customer tab", _testUserId);
        var reason = "Customer left without paying";

        // Act
        ticket.Void(reason, _managerId);

        // Assert
        Assert.Equal(TicketStatus.Voided, ticket.Status);
        Assert.Equal(_managerId, ticket.VoidedBy);
    }

    /// <summary>
    /// Test: Void paid ticket (reject, suggest refund)
    /// REQ-5.3: Cannot void paid tickets, must use refund instead
    /// </summary>
    [Fact]
    public void Void_PaidTicket_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        // Add payment to make it paid
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: new Money(100m, "USD"),
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        // Act & Assert
        var exception = Assert.Throws<Magidesk.Domain.Exceptions.InvalidOperationException>(() => 
            ticket.Void("Test reason", _managerId));
        
        Assert.Contains("Cannot void a paid ticket", exception.Message);
        Assert.Contains("Use refund instead", exception.Message);
    }

    /// <summary>
    /// Test: Void requires non-empty reason
    /// REQ-5.2: Void requires a reason
    /// </summary>
    [Fact]
    public void Void_EmptyReason_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ticket.Void("", _managerId));
        
        Assert.Throws<ArgumentException>(() => 
            ticket.Void("   ", _managerId));
    }

    /// <summary>
    /// Test: Void requires authorization (user ID)
    /// REQ-5.2: Void requires manager authorization
    /// </summary>
    [Fact]
    public void Void_NullUserId_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ticket.Void("Test reason", null!));
    }

    /// <summary>
    /// Test: Full refund (status changes to Refunded)
    /// REQ-5.4: Full refund changes status to Refunded
    /// </summary>
    [Fact]
    public void Refund_FullAmount_ChangesStatusToRefunded()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var paymentAmount = new Money(100m, "USD");
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: paymentAmount,
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        var reason = "Customer returned all items";

        // Act
        ticket.Refund(paymentAmount, reason, _managerId);

        // Assert
        Assert.Equal(TicketStatus.Refunded, ticket.Status);
        Assert.Equal(Money.Zero("USD"), ticket.PaidAmount);
        Assert.Equal(reason, ticket.Properties["RefundReason"]);
        Assert.Equal(_managerId.Value.ToString(), ticket.Properties["RefundedBy"]);
    }

    /// <summary>
    /// Test: Partial refund (status remains Paid)
    /// REQ-5.5: Partial refund keeps status as Paid
    /// </summary>
    [Fact]
    public void Refund_PartialAmount_StatusRemainsPaid()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var paymentAmount = new Money(100m, "USD");
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: paymentAmount,
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        var refundAmount = new Money(30m, "USD");
        var reason = "Customer returned one item";

        // Act
        ticket.Refund(refundAmount, reason, _managerId);

        // Assert
        Assert.Equal(TicketStatus.Paid, ticket.Status);
        Assert.Equal(new Money(70m, "USD"), ticket.PaidAmount);
        Assert.Equal(refundAmount, payment.RefundedAmount);
    }

    /// <summary>
    /// Test: Refund amount exceeds paid (reject)
    /// REQ-5.9: Refund amount cannot exceed paid amount
    /// </summary>
    [Fact]
    public void Refund_AmountExceedsPaid_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var paymentAmount = new Money(100m, "USD");
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: paymentAmount,
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        var refundAmount = new Money(150m, "USD");

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() => 
            ticket.Refund(refundAmount, "Test reason", _managerId));
        
        Assert.Contains("exceeds paid amount", exception.Message);
    }

    /// <summary>
    /// Test: Refund requires non-empty reason
    /// REQ-5.6: Refund requires a reason
    /// </summary>
    [Fact]
    public void Refund_EmptyReason_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: new Money(100m, "USD"),
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ticket.Refund(new Money(50m, "USD"), "", _managerId));
    }

    /// <summary>
    /// Test: Refund requires authorization (user ID)
    /// REQ-5.6: Refund requires manager authorization
    /// </summary>
    [Fact]
    public void Refund_NullUserId_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var payment = CashPayment.Create(
            ticketId: ticket.Id,
            amount: new Money(100m, "USD"),
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ticket.Refund(new Money(50m, "USD"), "Test reason", null!));
    }

    /// <summary>
    /// Test: Refund distributes across multiple payments proportionally
    /// REQ-5.5: Refund updates payment records
    /// </summary>
    [Fact]
    public void Refund_MultiplePayments_DistributesProportionally()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        
        var payment1 = CashPayment.Create(
            ticketId: ticket.Id,
            amount: new Money(60m, "USD"),
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment1);

        var payment2 = CashPayment.Create(
            ticketId: ticket.Id,
            amount: new Money(40m, "USD"),
            processedBy: _testUserId,
            terminalId: _terminalId);
        ticket.AddPayment(payment2);

        var refundAmount = new Money(50m, "USD");

        // Act
        ticket.Refund(refundAmount, "Partial refund", _managerId);

        // Assert
        Assert.Equal(new Money(50m, "USD"), ticket.PaidAmount);
        // First payment should be fully refunded (60 - 50 = 10 remaining)
        Assert.True(payment1.RefundedAmount > Money.Zero("USD"));
    }

    /// <summary>
    /// Test: Cannot refund open ticket
    /// REQ-5.4: Only Paid or Closed tickets can be refunded
    /// </summary>
    [Fact]
    public void Refund_OpenTicket_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();

        // Act & Assert
        var exception = Assert.Throws<Magidesk.Domain.Exceptions.InvalidOperationException>(() => 
            ticket.Refund(new Money(10m, "USD"), "Test reason", _managerId));
        
        Assert.Contains("Only Paid or Closed tickets can be refunded", exception.Message);
    }

    /// <summary>
    /// Test: Cannot refund voided ticket
    /// REQ-5.4: Only Paid or Closed tickets can be refunded
    /// </summary>
    [Fact]
    public void Refund_VoidedTicket_ThrowsException()
    {
        // Arrange
        var ticket = Ticket.Create(1, _testUserId, _terminalId, _shiftId, _orderTypeId);
        ticket.Open();
        ticket.Void("Test void", _managerId);

        // Act & Assert
        var exception = Assert.Throws<Magidesk.Domain.Exceptions.InvalidOperationException>(() => 
            ticket.Refund(new Money(10m, "USD"), "Test reason", _managerId));
        
        Assert.Contains("Only Paid or Closed tickets can be refunded", exception.Message);
    }
}
