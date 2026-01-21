using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class ProcessPaymentCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CashPayment_ShouldAddPaymentAndUpdateTicket()
    {
        var tickets = new InMemoryTicketRepository();
        var payments = new InMemoryPaymentRepository();
        var cashSessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var paymentDomain = new PaymentDomainService();

        var handler = new ProcessPaymentCommandHandler(tickets, payments, cashSessions, audits, paymentDomain);

        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, terminalId, Guid.NewGuid(), Guid.NewGuid());
        ticket.AddOrderLine(OrderLine.Create(ticket.Id, Guid.NewGuid(), "Item", 1m, new Money(10m), taxRate: 0m));
        await tickets.AddAsync(ticket);

        var cmd = new ProcessPaymentCommand
        {
            TicketId = ticket.Id,
            PaymentType = PaymentType.Cash,
            Amount = new Money(10m),
            TenderAmount = new Money(10m),
            TipsAmount = null,
            ProcessedBy = userId,
            TerminalId = terminalId,
            CashSessionId = null
        };

        var result = await handler.HandleAsync(cmd);

        result.PaymentId.Should().NotBe(Guid.Empty);
        result.ChangeAmount.Amount.Should().Be(0m);

        var updatedTicket = await tickets.GetByIdAsync(ticket.Id);
        updatedTicket!.Payments.Should().ContainSingle();
        updatedTicket.Payments.First().PaymentType.Should().Be(PaymentType.Cash);

        var savedPayment = await payments.GetByIdAsync(result.PaymentId);
        savedPayment.Should().NotBeNull();

        audits.Events.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_NonCashPayment_ShouldThrow()
    {
        var tickets = new InMemoryTicketRepository();
        var payments = new InMemoryPaymentRepository();
        var cashSessions = new InMemoryCashSessionRepository();
        var audits = new InMemoryAuditEventRepository();
        var paymentDomain = new PaymentDomainService();

        var handler = new ProcessPaymentCommandHandler(tickets, payments, cashSessions, audits, paymentDomain);

        var cmd = new ProcessPaymentCommand
        {
            TicketId = Guid.NewGuid(),
            PaymentType = PaymentType.CreditCard,
            Amount = new Money(1m),
            ProcessedBy = new UserId(Guid.NewGuid()),
            TerminalId = Guid.NewGuid()
        };

        var act = async () => await handler.HandleAsync(cmd);

        await act.Should().ThrowAsync<Magidesk.Domain.Exceptions.BusinessRuleViolationException>();
    }
}
