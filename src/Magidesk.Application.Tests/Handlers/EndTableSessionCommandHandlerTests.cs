using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands.TableSessions;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class EndTableSessionCommandHandlerTests
{
    private readonly Mock<ITableSessionRepository> _sessionRepositoryMock;
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<ILogger<EndTableSessionCommandHandler>> _loggerMock;
    private readonly EndTableSessionCommandHandler _handler;

    public EndTableSessionCommandHandlerTests()
    {
        _sessionRepositoryMock = new Mock<ITableSessionRepository>();
        _tableRepositoryMock = new Mock<ITableRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _loggerMock = new Mock<ILogger<EndTableSessionCommandHandler>>();

        _handler = new EndTableSessionCommandHandler(
            _sessionRepositoryMock.Object,
            _tableRepositoryMock.Object,
            _ticketRepositoryMock.Object,
            _pricingServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateTicketWithTimeCharge_WhenCreateTicketIsTrue()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();
        var shiftId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();

        var session = TableSession.Start(tableId, Guid.NewGuid(), 10m, 2);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        if (sessionProp != null) sessionProp.SetValue(session, sessionId);

        var table = Table.Create(1, 4);
        var tableType = typeof(Table);
        var tableProp = tableType.GetProperty("Id");
        if (tableProp != null) tableProp.SetValue(table, tableId);
        
        var command = new EndTableSessionCommand(sessionId, true, userId, terminalId, shiftId, orderTypeId);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
        _tableRepositoryMock.Setup(r => r.GetByIdAsync(tableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _pricingServiceMock.Setup(s => s.CalculateTimeCharge(It.IsAny<TimeSpan>(), It.IsAny<decimal>()))
            .Returns(new Money(15m, "USD"));
        _ticketRepositoryMock.Setup(r => r.GetNextTicketNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(101);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.TicketId);
        _ticketRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()), Times.Once);
        _sessionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TableSession>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLinkToExistingTicket_WhenSessionAlreadyHasTicketId()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var ticketId = Guid.NewGuid();
        
        var session = TableSession.Start(tableId, Guid.NewGuid(), 10m, 2, null, ticketId);
        var sessionType = typeof(TableSession);
        var sessionProp = sessionType.GetProperty("Id");
        if (sessionProp != null) sessionProp.SetValue(session, sessionId);

        var table = Table.Create(1, 4);
        var tableType = typeof(Table);
        var tableProp = tableType.GetProperty("Id");
        if (tableProp != null) tableProp.SetValue(table, tableId);

        var ticket = Ticket.Create(101, new UserId(Guid.NewGuid()), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var ticketType = typeof(Ticket);
        var ticketProp = ticketType.GetProperty("Id");
        if (ticketProp != null) ticketProp.SetValue(ticket, ticketId);

        var command = new EndTableSessionCommand(sessionId, false);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);
        _tableRepositoryMock.Setup(r => r.GetByIdAsync(tableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(table);
        _pricingServiceMock.Setup(s => s.CalculateTimeCharge(It.IsAny<TimeSpan>(), It.IsAny<decimal>()))
            .Returns(new Money(20m, "USD"));
        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(ticketId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticketId, result.TicketId);
        _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
