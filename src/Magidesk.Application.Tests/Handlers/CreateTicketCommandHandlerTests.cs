using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Tests.Handlers;

public class CreateTicketCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateTicket_AndWriteAuditEvent()
    {
        var tickets = new InMemoryTicketRepository();
        var tables = new InMemoryTableRepository();
        var sessions = new InMemoryTableSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new CreateTicketCommandHandler(tickets, tables, sessions, audits);

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

    [Fact]
    public async Task HandleAsync_ShouldLinkSession_AndAutoPopulateCustomer()
    {
        // Arrange
        var tickets = new InMemoryTicketRepository();
        var tables = new InMemoryTableRepository();
        var sessions = new InMemoryTableSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new CreateTicketCommandHandler(tickets, tables, sessions, audits);

        var sessionId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), 10m, 2, customerId);
        // Hack: Set session ID via reflection or just reuse Start's return if ID is accessible.
        // TableSession.Start generates random ID. We need to match it or inject it.
        // Actually Start generates new ID. We can add *that* session to repo.
        await sessions.AddAsync(session);

        var cmd = new CreateTicketCommand
        {
            CreatedBy = new UserId(Guid.NewGuid()),
            TerminalId = Guid.NewGuid(),
            ShiftId = Guid.NewGuid(),
            OrderTypeId = Guid.NewGuid(),
            SessionId = session.Id // Use the ID from the created session
        };

        // Act
        var result = await handler.HandleAsync(cmd);

        // Assert
        var ticket = await tickets.GetByIdAsync(result.TicketId);
        ticket.Should().NotBeNull();
        ticket!.SessionId.Should().Be(session.Id);
        ticket.CustomerId.Should().Be(customerId);
    }
}
