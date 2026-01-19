using System;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Tests.Properties;

/// <summary>
/// Property-based tests for Ticket void and refund operations.
/// Task 2.3.7: Tests REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.4, REQ-5.6, REQ-5.8, REQ-5.9
/// </summary>
public class VoidRefundPropertiesTests
{
    private readonly Guid _terminalId = Guid.NewGuid();
    private readonly Guid _shiftId = Guid.NewGuid();
    private readonly Guid _orderTypeId = Guid.NewGuid();

    /// <summary>
    /// Property 22: Void ticket state transition
    /// REQ-5.1: Voiding an Open/Draft/Held ticket changes status to Voided
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property22_VoidTicketStateTransition()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                // Arrange: Create ticket in Open status
                var ticket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                ticket.Open();

                // Act: Void the ticket
                ticket.Void(reason, managerId);

                // Assert: Status should be Voided
                return (ticket.Status == TicketStatus.Voided)
                    .Label($"Ticket status should be Voided, but was {ticket.Status}")
                    .And(ticket.VoidedBy == managerId)
                    .Label("VoidedBy should be set to manager ID")
                    .And(ticket.Properties.ContainsKey("VoidReason"))
                    .Label("VoidReason should be recorded in properties");
            });
    }

    /// <summary>
    /// Property 23: Void paid ticket rejection
    /// REQ-5.3: Attempting to void a paid ticket should throw exception
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property23_VoidPaidTicketRejection()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                return Prop.ForAll(
                    Generators.PositiveDecimal(),
                    paymentAmount =>
                    {
                        // Arrange: Create ticket with payment
                        var ticket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                        ticket.Open();
                        
                        var payment = CashPayment.Create(
                            ticketId: ticket.Id,
                            amount: new Money(paymentAmount, "USD"),
                            processedBy: userId,
                            terminalId: _terminalId);
                        ticket.AddPayment(payment);

                        // Act & Assert: Void should throw exception
                        try
                        {
                            ticket.Void(reason, managerId);
                            return false.Label("Expected InvalidOperationException when voiding paid ticket");
                        }
                        catch (Magidesk.Domain.Exceptions.InvalidOperationException ex)
                        {
                            return ex.Message.Contains("Cannot void a paid ticket")
                                .Label($"Exception message should mention paid ticket, but was: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            return false.Label($"Expected InvalidOperationException, but got {ex.GetType().Name}");
                        }
                    });
            });
    }

    /// <summary>
    /// Property 24: Full refund processing
    /// REQ-5.4: Full refund should change status to Refunded and zero out PaidAmount
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property24_FullRefundProcessing()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                return Prop.ForAll(
                    Generators.PositiveDecimal(),
                    paymentAmount =>
                    {
                        // Arrange: Create ticket with payment
                        var ticket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                        ticket.Open();
                        
                        var amount = new Money(paymentAmount, "USD");
                        var payment = CashPayment.Create(
                            ticketId: ticket.Id,
                            amount: amount,
                            processedBy: userId,
                            terminalId: _terminalId);
                        ticket.AddPayment(payment);

                        // Act: Full refund
                        ticket.Refund(amount, reason, managerId);

                        // Assert: Status should be Refunded and PaidAmount should be zero
                        return (ticket.Status == TicketStatus.Refunded)
                            .Label($"Ticket status should be Refunded after full refund, but was {ticket.Status}")
                            .And(ticket.PaidAmount == Money.Zero("USD"))
                            .Label($"PaidAmount should be zero after full refund, but was {ticket.PaidAmount}")
                            .And(ticket.Properties.ContainsKey("RefundReason"))
                            .Label("RefundReason should be recorded in properties");
                    });
            });
    }

    /// <summary>
    /// Property 25: Refund amount constraint
    /// REQ-5.9: Refund amount must not exceed paid amount
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property25_RefundAmountConstraint()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                return Prop.ForAll(
                    Generators.PositiveDecimal(),
                    paymentAmount =>
                    {
                        // Ensure payment amount is at least 1 to allow excess
                        if (paymentAmount < 1m)
                            paymentAmount = 1m;

                        // Arrange: Create ticket with payment
                        var ticket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                        ticket.Open();
                        
                        var amount = new Money(paymentAmount, "USD");
                        var payment = CashPayment.Create(
                            ticketId: ticket.Id,
                            amount: amount,
                            processedBy: userId,
                            terminalId: _terminalId);
                        ticket.AddPayment(payment);

                        // Calculate refund amount that exceeds paid amount
                        var refundAmount = new Money(paymentAmount + 10m, "USD");

                        // Act & Assert: Refund should throw exception
                        try
                        {
                            ticket.Refund(refundAmount, reason, managerId);
                            return false.Label("Expected BusinessRuleViolationException when refund exceeds paid amount");
                        }
                        catch (BusinessRuleViolationException ex)
                        {
                            return ex.Message.Contains("exceeds paid amount")
                                .Label($"Exception message should mention exceeds paid amount, but was: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            return false.Label($"Expected BusinessRuleViolationException, but got {ex.GetType().Name}");
                        }
                    });
            });
    }

    /// <summary>
    /// Property 26: Void/refund authorization required
    /// REQ-5.2, REQ-5.6: Void and refund operations require authorization (non-null user ID)
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property26_VoidRefundAuthorizationRequired()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            (reason, userId) =>
            {
                // Test void authorization
                var voidTicket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                voidTicket.Open();

                bool voidThrowsException = false;
                try
                {
                    voidTicket.Void(reason, null!);
                }
                catch (ArgumentNullException)
                {
                    voidThrowsException = true;
                }

                // Test refund authorization
                var refundTicket = Ticket.Create(2, userId, _terminalId, _shiftId, _orderTypeId);
                refundTicket.Open();
                
                var payment = CashPayment.Create(
                    ticketId: refundTicket.Id,
                    amount: new Money(100m, "USD"),
                    processedBy: userId,
                    terminalId: _terminalId);
                refundTicket.AddPayment(payment);

                bool refundThrowsException = false;
                try
                {
                    refundTicket.Refund(new Money(50m, "USD"), reason, null!);
                }
                catch (ArgumentNullException)
                {
                    refundThrowsException = true;
                }

                return voidThrowsException
                    .Label("Void should throw ArgumentNullException when userId is null")
                    .And(refundThrowsException)
                    .Label("Refund should throw ArgumentNullException when userId is null");
            });
    }

    /// <summary>
    /// Property 27: Void/refund audit trail
    /// REQ-5.8: Void and refund operations should record audit information
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property Property27_VoidRefundAuditTrail()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                return Prop.ForAll(
                    Generators.PositiveDecimal(),
                    paymentAmount =>
                    {
                        // Test void audit trail
                        var voidTicket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                        voidTicket.Open();
                        voidTicket.Void(reason, managerId);

                        var voidAuditRecorded = voidTicket.Properties.ContainsKey("VoidReason")
                            && voidTicket.Properties["VoidReason"] == reason
                            && voidTicket.VoidedBy == managerId;

                        // Test refund audit trail
                        var refundTicket = Ticket.Create(2, userId, _terminalId, _shiftId, _orderTypeId);
                        refundTicket.Open();
                        
                        var amount = new Money(paymentAmount, "USD");
                        var payment = CashPayment.Create(
                            ticketId: refundTicket.Id,
                            amount: amount,
                            processedBy: userId,
                            terminalId: _terminalId);
                        refundTicket.AddPayment(payment);

                        refundTicket.Refund(amount, reason, managerId);

                        var refundAuditRecorded = refundTicket.Properties.ContainsKey("RefundReason")
                            && refundTicket.Properties["RefundReason"] == reason
                            && refundTicket.Properties.ContainsKey("RefundedBy")
                            && refundTicket.Properties["RefundedBy"] == managerId.Value.ToString()
                            && refundTicket.Properties.ContainsKey("RefundedAt");

                        return voidAuditRecorded
                            .Label("Void operation should record reason and voided by user")
                            .And(refundAuditRecorded)
                            .Label("Refund operation should record reason, refunded by user, and timestamp");
                    });
            });
    }

    /// <summary>
    /// Property: Partial refund maintains Paid status
    /// REQ-5.5: Partial refund should keep ticket in Paid status
    /// </summary>
    [Property(Arbitrary = new[] { typeof(Generators) })]
    public Property PartialRefundMaintainsPaidStatus()
    {
        return Prop.ForAll(
            Generators.NonEmptyString(),
            Generators.ValidUserId(),
            Generators.ValidUserId(),
            (reason, userId, managerId) =>
            {
                return Prop.ForAll(
                    Generators.PositiveDecimal(),
                    paymentAmount =>
                    {
                        // Ensure payment amount is at least 2 to allow partial refund
                        if (paymentAmount < 2m)
                            paymentAmount = 2m;

                        // Arrange: Create ticket with payment
                        var ticket = Ticket.Create(1, userId, _terminalId, _shiftId, _orderTypeId);
                        ticket.Open();
                        
                        var amount = new Money(paymentAmount, "USD");
                        var payment = CashPayment.Create(
                            ticketId: ticket.Id,
                            amount: amount,
                            processedBy: userId,
                            terminalId: _terminalId);
                        ticket.AddPayment(payment);

                        // Act: Partial refund (half of payment)
                        var refundAmount = new Money(paymentAmount / 2, "USD");
                        ticket.Refund(refundAmount, reason, managerId);

                        // Assert: Status should remain Paid
                        return (ticket.Status == TicketStatus.Paid)
                            .Label($"Ticket status should remain Paid after partial refund, but was {ticket.Status}")
                            .And(ticket.PaidAmount > Money.Zero("USD"))
                            .Label($"PaidAmount should be greater than zero after partial refund, but was {ticket.PaidAmount}")
                            .And(ticket.PaidAmount < amount)
                            .Label($"PaidAmount should be less than original amount after partial refund");
                    });
            });
    }
}

/// <summary>
/// Custom generators for property-based tests
/// </summary>
public static class Generators
{
    /// <summary>
    /// Generates non-empty strings for reasons
    /// </summary>
    public static Arbitrary<string> NonEmptyString()
    {
        return Arb.Default.String()
            .Generator
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToArbitrary();
    }

    /// <summary>
    /// Generates valid UserId instances
    /// </summary>
    public static Arbitrary<UserId> ValidUserId()
    {
        return Gen.Fresh(() => new UserId(Guid.NewGuid()))
            .ToArbitrary();
    }

    /// <summary>
    /// Generates positive decimal values for amounts
    /// </summary>
    public static Arbitrary<decimal> PositiveDecimal()
    {
        return Arb.Default.Decimal()
            .Generator
            .Where(d => d > 0 && d < 10000) // Reasonable range for testing
            .Select(Math.Abs)
            .ToArbitrary();
    }
}
