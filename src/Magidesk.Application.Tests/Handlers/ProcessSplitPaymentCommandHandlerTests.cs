using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

/// <summary>
/// Unit tests for ProcessSplitPaymentCommandHandler.
/// Tests validation logic for split payment scenarios.
/// </summary>
public class ProcessSplitPaymentCommandHandlerTests
{
    private readonly InMemoryTicketRepository _tickets;
    private readonly InMemoryPaymentRepository _payments;
    private readonly InMemoryCashSessionRepository _cashSessions;
    private readonly InMemoryAuditEventRepository _audits;
    private readonly ProcessSplitPaymentCommandHandler _handler;

    public ProcessSplitPaymentCommandHandlerTests()
    {
        _tickets = new InMemoryTicketRepository();
        _payments = new InMemoryPaymentRepository();
        _cashSessions = new InMemoryCashSessionRepository();
        _audits = new InMemoryAuditEventRepository();
        _handler = new ProcessSplitPaymentCommandHandler(_tickets, _payments, _cashSessions, _audits);
    }

    /// <summary>
    /// Test: Sum equals total (valid)
    /// Requirements: REQ-2.2
    /// Validates: Property 8
    /// </summary>
    [Fact]
    public async Task HandleAsync_SumEqualsTotal_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await _tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(100m), taxRate: 0m));
        await _tickets.AddAsync(ticket);

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(50m)),
            new SplitPaymentEntry(PaymentType.CreditCard, new Money(50m))
        };

        var command = new ProcessSplitPaymentCommand(ticket.Id, splitPayments, userId);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsUnderpayment.Should().BeFalse();
        result.TicketIsPaid.Should().BeTrue();
        result.PaymentIds.Should().HaveCount(2);
        result.ChangeAmount.Amount.Should().Be(0m);
        result.RemainingAmount.Amount.Should().Be(0m);

        var updatedTicket = await _tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Payments.Should().HaveCount(2);
        updatedTicket.Status.Should().Be(TicketStatus.Paid);

        _audits.Events.Should().NotBeEmpty();
    }

    /// <summary>
    /// Test: Sum exceeds total (calculate change)
    /// Requirements: REQ-2.3
    /// Validates: Property 9
    /// </summary>
    [Fact]
    public async Task HandleAsync_SumExceedsTotal_ShouldCalculateChange()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await _tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(100m), taxRate: 0m));
        await _tickets.AddAsync(ticket);

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(60m)),
            new SplitPaymentEntry(PaymentType.CreditCard, new Money(50m))
        };

        var command = new ProcessSplitPaymentCommand(ticket.Id, splitPayments, userId);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsUnderpayment.Should().BeFalse();
        result.TicketIsPaid.Should().BeTrue();
        result.PaymentIds.Should().HaveCount(2);
        result.ChangeAmount.Amount.Should().Be(10m); // 110 - 100 = 10
        result.RemainingAmount.Amount.Should().Be(0m);

        var updatedTicket = await _tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Status.Should().Be(TicketStatus.Paid);
    }

    /// <summary>
    /// Test: Sum less than total (reject with remaining)
    /// Requirements: REQ-2.4
    /// Validates: Property 10
    /// </summary>
    [Fact]
    public async Task HandleAsync_SumLessThanTotal_ShouldRejectWithRemaining()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await _tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(100m), taxRate: 0m));
        await _tickets.AddAsync(ticket);

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(30m)),
            new SplitPaymentEntry(PaymentType.CreditCard, new Money(40m))
        };

        var command = new ProcessSplitPaymentCommand(ticket.Id, splitPayments, userId);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsUnderpayment.Should().BeTrue();
        result.TicketIsPaid.Should().BeFalse();
        result.PaymentIds.Should().BeEmpty();
        result.RemainingAmount.Amount.Should().Be(30m); // 100 - 70 = 30
        result.ChangeAmount.Amount.Should().Be(0m);

        var updatedTicket = await _tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Payments.Should().BeEmpty(); // No payments should be added
        updatedTicket.Status.Should().Be(TicketStatus.Open);
    }

    /// <summary>
    /// Test: Empty payments list (reject)
    /// Requirements: REQ-2.1
    /// </summary>
    [Fact]
    public void Constructor_EmptyPaymentsList_ShouldThrow()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketId = Guid.NewGuid();
        var emptyPayments = new List<SplitPaymentEntry>();

        // Act
        var act = () => new ProcessSplitPaymentCommand(ticketId, emptyPayments, userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Payments list cannot be null or empty*");
    }

    /// <summary>
    /// Test: Negative payment amount (reject)
    /// Requirements: REQ-2.1
    /// </summary>
    [Fact]
    public void SplitPaymentEntry_NegativeAmount_ShouldThrow()
    {
        // Arrange & Act
        var act = () => new SplitPaymentEntry(PaymentType.Cash, new Money(-10m));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Payment amount must be positive*");
    }

    /// <summary>
    /// Test: Zero payment amount (reject)
    /// Requirements: REQ-2.1
    /// </summary>
    [Fact]
    public void SplitPaymentEntry_ZeroAmount_ShouldThrow()
    {
        // Arrange & Act
        var act = () => new SplitPaymentEntry(PaymentType.Cash, Money.Zero());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Payment amount must be positive*");
    }

    /// <summary>
    /// Test: Split payment creates correct number of payment records
    /// Requirements: REQ-2.8
    /// Validates: Property 11
    /// </summary>
    [Fact]
    public async Task HandleAsync_ThreePayments_ShouldCreateThreeRecords()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await _tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(150m), taxRate: 0m));
        await _tickets.AddAsync(ticket);

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(50m)),
            new SplitPaymentEntry(PaymentType.CreditCard, new Money(50m)),
            new SplitPaymentEntry(PaymentType.DebitCard, new Money(50m))
        };

        var command = new ProcessSplitPaymentCommand(ticket.Id, splitPayments, userId);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.PaymentIds.Should().HaveCount(3);

        var updatedTicket = await _tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Payments.Should().HaveCount(3);

        // Verify all payments have the same SplitGroupId
        var splitGroupIds = updatedTicket.Payments.Select(p => p.SplitGroupId).Distinct();
        splitGroupIds.Should().ContainSingle();
        splitGroupIds.First().Should().NotBeNull();

        // Verify sequence numbers are correct
        var sequences = updatedTicket.Payments.Select(p => p.SplitSequence).OrderBy(s => s).ToList();
        sequences.Should().Equal(1, 2, 3);
    }

    /// <summary>
    /// Test: Ticket not found
    /// Requirements: REQ-2.1
    /// </summary>
    [Fact]
    public async Task HandleAsync_TicketNotFound_ShouldThrow()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var nonExistentTicketId = Guid.NewGuid();

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(50m))
        };

        var command = new ProcessSplitPaymentCommand(nonExistentTicketId, splitPayments, userId);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*not found*");
    }

    /// <summary>
    /// Test: Partial payment followed by split payment
    /// Requirements: REQ-2.2, REQ-2.4
    /// </summary>
    [Fact]
    public async Task HandleAsync_PartiallyPaidTicket_ShouldCalculateRemainingCorrectly()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await _tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(100m), taxRate: 0m));
        
        // Add initial partial payment
        var initialPayment = CashPayment.Create(ticket.Id, new Money(30m), userId, terminalId);
        ticket.AddPayment(initialPayment);
        await _tickets.AddAsync(ticket);

        var splitPayments = new List<SplitPaymentEntry>
        {
            new SplitPaymentEntry(PaymentType.Cash, new Money(40m)),
            new SplitPaymentEntry(PaymentType.CreditCard, new Money(30m))
        };

        var command = new ProcessSplitPaymentCommand(ticket.Id, splitPayments, userId);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.IsUnderpayment.Should().BeFalse();
        result.TicketIsPaid.Should().BeTrue();
        result.PaymentIds.Should().HaveCount(2);
        result.ChangeAmount.Amount.Should().Be(0m); // 30 (initial) + 70 (split) = 100
        result.RemainingAmount.Amount.Should().Be(0m);

        var updatedTicket = await _tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Payments.Should().HaveCount(3); // 1 initial + 2 split
        updatedTicket.Status.Should().Be(TicketStatus.Paid);
    }
}
