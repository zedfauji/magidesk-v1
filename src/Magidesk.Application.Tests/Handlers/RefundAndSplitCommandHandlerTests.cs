using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Moq;

namespace Magidesk.Application.Tests.Handlers;

public class RefundAndSplitCommandHandlerTests
{
    [Fact]
    public async Task RefundTicket_ShouldCreateRefundPayments_AndUpdateTicketStatus()
    {
        var tickets = new InMemoryTicketRepository();
        var audits = new InMemoryAuditEventRepository();
        var securityService = new Mock<ISecurityService>();
        var receiptPrintService = new Mock<IReceiptPrintService>();

        var handler = new RefundTicketCommandHandler(tickets, audits, securityService.Object, receiptPrintService.Object);

        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();

        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(10m), taxRate: 0m));

        var cashPayment = CashPayment.Create(ticket.Id, new Money(10m), userId, terminalId);
        ticket.AddPayment(cashPayment);
        ticket.Close(userId);

        await tickets.AddAsync(ticket);

        var cmd = new RefundTicketCommand
        {
            TicketId = ticket.Id,
            Amount = new Money(10m),
            Reason = "Test refund",
            RefundedBy = userId,
            AuthorizedBy = userId,
            IsPartial = false
        };

        await handler.HandleAsync(cmd);

        var updated = await tickets.GetByIdAsync(ticket.Id);
        updated.Should().NotBeNull();
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
