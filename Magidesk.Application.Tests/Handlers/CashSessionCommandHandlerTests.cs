using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class CashSessionCommandHandlerTests
{
    [Fact]
    public async Task OpenCashSession_ShouldCreateSession_AndAuditEvent()
    {
        var sessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var domain = new CashSessionDomainService();

        var handler = new OpenCashSessionCommandHandler(sessions, audits, domain);

        var userId = new UserId(Guid.NewGuid());
        var cmd = new OpenCashSessionCommand
        {
            UserId = userId,
            TerminalId = Guid.NewGuid(),
            ShiftId = Guid.NewGuid(),
            OpeningBalance = new Money(100m)
        };

        var result = await handler.HandleAsync(cmd);

        result.CashSessionId.Should().NotBe(Guid.Empty);
        audits.Events.Should().HaveCount(1);

        var saved = await sessions.GetByIdAsync(result.CashSessionId);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(userId);
        saved.OpeningBalance.Amount.Should().Be(100m);
    }

    [Fact]
    public async Task OpenCashSession_WhenUserAlreadyHasOpenSession_ShouldThrow()
    {
        var sessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var domain = new CashSessionDomainService();

        var handler = new OpenCashSessionCommandHandler(sessions, audits, domain);

        var userId = new UserId(Guid.NewGuid());
        var existing = Magidesk.Domain.Entities.CashSession.Open(userId, Guid.NewGuid(), Guid.NewGuid(), new Money(10m));
        await sessions.AddAsync(existing);

        var cmd = new OpenCashSessionCommand
        {
            UserId = userId,
            TerminalId = Guid.NewGuid(),
            ShiftId = Guid.NewGuid(),
            OpeningBalance = new Money(50m)
        };

        var act = async () => await handler.HandleAsync(cmd);
        await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>();
    }

    [Fact]
    public async Task CloseCashSession_ShouldCloseSession_AndReturnTotals()
    {
        var sessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var tickets = new InMemoryTicketRepository();
        var domain = new CashSessionDomainService();

        var handler = new CloseCashSessionCommandHandler(sessions, audits, tickets, domain);

        var userId = new UserId(Guid.NewGuid());
        var session = Magidesk.Domain.Entities.CashSession.Open(userId, Guid.NewGuid(), Guid.NewGuid(), new Money(100m));
        await sessions.AddAsync(session);

        var cmd = new CloseCashSessionCommand
        {
            CashSessionId = session.Id,
            ClosedBy = userId,
            ActualCash = new Money(100m)
        };

        var result = await handler.HandleAsync(cmd);

        result.CashSessionId.Should().Be(session.Id);
        result.Difference.Amount.Should().Be(0m);

        var saved = await sessions.GetByIdAsync(session.Id);
        saved!.ClosedAt.Should().NotBeNull();
        saved.Status.Should().Be(Magidesk.Domain.Enumerations.CashSessionStatus.Closed);

        audits.Events.Should().HaveCount(1);
    }

    [Fact]
    public async Task CloseCashSession_WhenOpenTicketsExist_ShouldThrow()
    {
        var sessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var tickets = new InMemoryTicketRepository();
        var domain = new CashSessionDomainService();

        var handler = new CloseCashSessionCommandHandler(sessions, audits, tickets, domain);

        // Setup Open Ticket
        var openTicket = Magidesk.Domain.Entities.Ticket.Create(
            1, 
            new UserId(Guid.NewGuid()), 
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            Guid.NewGuid());
        await tickets.AddAsync(openTicket);

        var userId = new UserId(Guid.NewGuid());
        var session = Magidesk.Domain.Entities.CashSession.Open(userId, Guid.NewGuid(), Guid.NewGuid(), new Money(100m));
        await sessions.AddAsync(session);

        var cmd = new CloseCashSessionCommand
        {
            CashSessionId = session.Id,
            ClosedBy = userId,
            ActualCash = new Money(100m)
        };

        var act = async () => await handler.HandleAsync(cmd);

        await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>()
            .WithMessage("*tickets are still OPEN*");
    }
}
