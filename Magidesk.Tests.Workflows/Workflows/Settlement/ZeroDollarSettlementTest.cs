using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Magidesk.Tests.Workflows.Settlement;

public class ZeroDollarSettlementTest
{
    [Fact]
    public async Task Settle_ZeroDollar_DraftTicket_Should_BePaid()
    {
        // Arrange
        var ticketRepository = new Mock<ITicketRepository>();
        var paymentRepository = new Mock<IPaymentRepository>();
        var cashSessionRepository = new Mock<ICashSessionRepository>();
        var auditEventRepository = new Mock<IAuditEventRepository>();
        
        var paymentDomainService = new Magidesk.Domain.DomainServices.PaymentDomainService(); 

        var handler = new ProcessPaymentCommandHandler(
            ticketRepository.Object,
            paymentRepository.Object,
            cashSessionRepository.Object,
            auditEventRepository.Object,
            paymentDomainService
        );

        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        var shiftId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();

        // Create a Draft ticket ($0.00)
        var ticket = Ticket.Create(1, userId, terminalId, shiftId, orderTypeId);
        
        // Setup Repository to return this ticket
        ticketRepository.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
            
        // Cash Session setup
        var cashSession = CashSession.Open(userId, terminalId, shiftId, new Money(100));
        var sessionId = cashSession.Id;

        cashSessionRepository.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cashSession);

        var command = new ProcessPaymentCommand
        {
            TicketId = ticket.Id,
            PaymentType = PaymentType.Cash,
            Amount = Money.Zero(), // $0.00 payment
            TenderAmount = Money.Zero(),
            ProcessedBy = userId,
            TerminalId = terminalId,
            CashSessionId = sessionId,
            GlobalId = Guid.NewGuid().ToString()
        };

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.TicketIsPaid.Should().BeTrue("Ticket should be marked as paid");
        ticket.Status.Should().Be(TicketStatus.Paid, "Ticket status should be Paid");
        ticket.PaidAmount.Amount.Should().Be(0);
        ticket.TotalAmount.Amount.Should().Be(0);
    }
}
