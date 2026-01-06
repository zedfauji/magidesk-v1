using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class CreateTicketCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateTicket_AndWriteAuditEvent()
    {
        var tickets = new InMemoryTicketRepository();
        var tables = new InMemoryTableRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new CreateTicketCommandHandler(tickets, tables, audits);

        var createdBy = new UserId(Guid.NewGuid());
        var cmd = new CreateTicketCommand
        {
            CreatedBy = createdBy,
            TerminalId = Guid.NewGuid(),
            ShiftId = Guid.NewGuid(),
            OrderTypeId = Guid.NewGuid()
        };

        var result = await handler.HandleAsync(cmd);

        result.TicketId.Should().NotBe(Guid.Empty);
        result.TicketNumber.Should().BeGreaterThan(0);

        var ticket = await tickets.GetByIdAsync(result.TicketId);
        ticket.Should().NotBeNull();
        ticket!.CreatedBy.Should().Be(createdBy);

        audits.Events.Should().HaveCount(1);
        audits.Events[0].EntityId.Should().Be(ticket.Id);
    }
}
