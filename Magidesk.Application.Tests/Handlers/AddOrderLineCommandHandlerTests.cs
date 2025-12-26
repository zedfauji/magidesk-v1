using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class AddOrderLineCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAddOrderLine_AndWriteAuditEvent()
    {
        var tickets = new InMemoryTicketRepository();
        var menu = new InMemoryMenuRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new AddOrderLineCommandHandler(tickets, menu, audits);

        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await tickets.AddAsync(ticket);

        var cmd = new AddOrderLineCommand
        {
            TicketId = ticket.Id,
            MenuItemId = Guid.NewGuid(),
            MenuItemName = "Burger",
            Quantity = 2m,
            UnitPrice = new Money(5m),
            TaxRate = 0.1m
        };

        var result = await handler.HandleAsync(cmd);

        result.OrderLineId.Should().NotBe(Guid.Empty);

        var updated = await tickets.GetByIdAsync(ticket.Id);
        updated!.OrderLines.Should().ContainSingle(ol => ol.Id == result.OrderLineId);

        audits.Events.Should().HaveCount(1);
    }

    [Fact]
    public async Task HandleAsync_WithMissingTicket_ShouldThrow()
    {
        var tickets = new InMemoryTicketRepository();
        var menu = new InMemoryMenuRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new AddOrderLineCommandHandler(tickets, menu, audits);

        var cmd = new AddOrderLineCommand
        {
            TicketId = Guid.NewGuid(),
            MenuItemId = Guid.NewGuid(),
            MenuItemName = "Burger",
            Quantity = 1m,
            UnitPrice = new Money(5m),
            TaxRate = 0m
        };

        var act = async () => await handler.HandleAsync(cmd);

        await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>();
    }
}
