using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class RefundAndSplitCommandHandlerTests
{
    [Fact]
    public async Task RefundTicket_ShouldCreateRefundPayments_AndUpdateTicketStatus()
    {
        var tickets = new InMemoryTicketRepository();
        var payments = new InMemoryPaymentRepository();
        var gateway = new StubPaymentGateway();
        var audits = new InMemoryAuditEventRepository();
        var paymentDomain = new PaymentDomainService();
        var tax = new TaxDomainService();
        var ticketDomain = new TicketDomainService(tax);

        var handler = new RefundTicketCommandHandler(tickets, payments, gateway, audits, paymentDomain, ticketDomain);

        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();

        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(10m), taxRate: 0m));
        ticketDomain.CalculateTotals(ticket);

        var cashPayment = CashPayment.Create(ticket.Id, ticket.DueAmount, userId, terminalId);
        ticket.AddPayment(cashPayment);
        ticketDomain.CalculateTotals(ticket);
        ticket.Close(userId);

        await tickets.AddAsync(ticket);
        await payments.AddAsync(cashPayment);

        var cmd = new RefundTicketCommand
        {
            TicketId = ticket.Id,
            ProcessedBy = userId,
            TerminalId = terminalId,
            Reason = "Test refund"
        };

        var result = await handler.HandleAsync(cmd);

        result.Success.Should().BeTrue();
        result.RefundPaymentsCreated.Should().Be(1);

        var updated = await tickets.GetByIdAsync(ticket.Id);
        updated!.Status.Should().Be(Magidesk.Domain.Enumerations.TicketStatus.Refunded);
        updated.Payments.Count.Should().Be(2);
        updated.Payments.Count(p => p.TransactionType == Magidesk.Domain.Enumerations.TransactionType.Debit).Should().Be(1);

        audits.Events.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SplitTicket_ShouldCreateNewTicket_AndRemoveLineFromOriginal()
    {
        var tickets = new InMemoryTicketRepository();
        var audits = new InMemoryAuditEventRepository();
        var tax = new TaxDomainService();
        var ticketDomain = new TicketDomainService(tax);

        var handler = new SplitTicketCommandHandler(tickets, audits, ticketDomain);

        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var shiftId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();

        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, shiftId, orderTypeId);

        var line1 = OrderLine.Create(ticket.Id, Guid.NewGuid(), "A", 1m, new Money(5m), taxRate: 0m);
        var line2 = OrderLine.Create(ticket.Id, Guid.NewGuid(), "B", 1m, new Money(6m), taxRate: 0m);
        ticket.AddOrderLine(line1);
        ticket.AddOrderLine(line2);

        await tickets.AddAsync(ticket);

        var cmd = new SplitTicketCommand
        {
            OriginalTicketId = ticket.Id,
            OrderLineIdsToSplit = new List<Guid> { line1.Id },
            SplitBy = userId,
            TerminalId = terminalId,
            ShiftId = shiftId,
            OrderTypeId = orderTypeId
        };

        var result = await handler.HandleAsync(cmd);

        result.Success.Should().BeTrue();
        result.OrderLinesMoved.Should().Be(1);

        var original = await tickets.GetByIdAsync(ticket.Id);
        original!.OrderLines.Should().ContainSingle(ol => ol.MenuItemName == "B");

        var newTicket = await tickets.GetByIdAsync(result.NewTicketId);
        newTicket.Should().NotBeNull();
        newTicket!.OrderLines.Should().ContainSingle();

        audits.Events.Should().HaveCount(2);
    }
}
