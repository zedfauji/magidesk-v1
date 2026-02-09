using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Services;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Magidesk.Application.Tests.Handlers;

/// <summary>
/// Property-based tests for PrintToKitchenCommandHandler notification integration.
/// Feature: kds-realtime-notifications
/// </summary>
public class PrintToKitchenCommandHandlerPropertyTests
{
    private readonly Mock<ITicketRepository> _mockTicketRepository;
    private readonly Mock<IKitchenPrintService> _mockPrintService;
    private readonly Mock<IKitchenRoutingService> _mockRoutingService;
    private readonly Mock<IAuditEventRepository> _mockAuditRepository;
    private readonly Mock<IOrderNotificationService> _mockNotificationService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<PrintToKitchenCommandHandler>> _mockLogger;

    public PrintToKitchenCommandHandlerPropertyTests()
    {
        _mockTicketRepository = new Mock<ITicketRepository>();
        _mockPrintService = new Mock<IKitchenPrintService>();
        _mockRoutingService = new Mock<IKitchenRoutingService>();
        _mockAuditRepository = new Mock<IAuditEventRepository>();
        _mockNotificationService = new Mock<IOrderNotificationService>();
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<PrintToKitchenCommandHandler>>();
    }

    /// <summary>
    /// Property 1: Order Creation Triggers Notification
    /// For any valid ticket with kitchen-routable items, when PrintToKitchenCommandHandler.HandleAsync 
    /// successfully routes the order to the kitchen, then NotifyOrderCreatedAsync must be called 
    /// exactly once per kitchen order ID returned.
    /// Validates: Requirements US-001.1, REQ-002.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property1_OrderCreationTriggersNotification_ForAnyValidTicket_NotificationCalledPerKitchenOrderId()
    {
        return Prop.ForAll(
            ValidTicketWithKitchenItemsGenerator(),
            KitchenOrderIdsGenerator(),
            (ticket, kitchenOrderIds) =>
            {
                // Arrange
                var notificationCallCount = 0;
                var capturedKitchenOrderIds = new List<Guid>();

                _mockTicketRepository.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ticket);

                _mockRoutingService.Setup(r => r.RouteToKitchenAsync(It.IsAny<TicketDto>(), It.IsAny<List<Guid>?>()))
                    .ReturnsAsync(kitchenOrderIds);

                _mockPrintService.Setup(p => p.PrintTicketAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new KitchenPrintResult(
                        Success: true,
                        Message: "Success",
                        PrintedCount: 1,
                        Errors: null));

                _mockNotificationService.Setup(n => n.NotifyOrderCreatedAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .Callback<Guid, string, string>((id, _, __) =>
                    {
                        notificationCallCount++;
                        capturedKitchenOrderIds.Add(id);
                    })
                    .Returns(Task.CompletedTask);

                _mockAuditRepository.Setup(a => a.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                var handler = new PrintToKitchenCommandHandler(
                    _mockTicketRepository.Object,
                    _mockPrintService.Object,
                    _mockRoutingService.Object,
                    _mockAuditRepository.Object,
                    _mockNotificationService.Object,
                    _mockUserService.Object,
                    _mockLogger.Object);

                var command = new PrintToKitchenCommand { TicketId = ticket.Id };

                // Act
                var task = handler.HandleAsync(command);
                task.Wait();

                // Assert properties
                var correctNumberOfCalls = notificationCallCount == kitchenOrderIds.Count;
                var allKitchenOrderIdsNotified = kitchenOrderIds.All(id => capturedKitchenOrderIds.Contains(id));

                return correctNumberOfCalls && allKitchenOrderIdsNotified;
            });
    }

    /// <summary>
    /// Property 2: Notification Failure Preserves Order Persistence
    /// For any valid ticket, if NotifyOrderCreatedAsync throws an exception, then the order 
    /// must still persist to the database and HandleAsync must return success.
    /// Validates: Requirements US-002.1, US-002.4, REQ-002.4
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property2_NotificationFailurePreservesOrderPersistence_ForAnyTicket_OrderStillPersistsAndReturnsSuccess()
    {
        return Prop.ForAll(
            ValidTicketWithKitchenItemsGenerator(),
            KitchenOrderIdsGenerator(),
            ExceptionGenerator(),
            (ticket, kitchenOrderIds, exception) =>
            {
                // Arrange
                var routingCalled = false;
                var printingCalled = false;

                _mockTicketRepository.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ticket);

                _mockRoutingService.Setup(r => r.RouteToKitchenAsync(It.IsAny<TicketDto>(), It.IsAny<List<Guid>?>()))
                    .Callback(() => routingCalled = true)
                    .ReturnsAsync(kitchenOrderIds);

                _mockPrintService.Setup(p => p.PrintTicketAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
                    .Callback(() => printingCalled = true)
                    .ReturnsAsync(new KitchenPrintResult(
                        Success: true,
                        Message: "Success",
                        PrintedCount: 1,
                        Errors: null));

                _mockNotificationService.Setup(n => n.NotifyOrderCreatedAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<string>(),
                        It.IsAny<string>()))
                    .ThrowsAsync(exception);

                _mockAuditRepository.Setup(a => a.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                var handler = new PrintToKitchenCommandHandler(
                    _mockTicketRepository.Object,
                    _mockPrintService.Object,
                    _mockRoutingService.Object,
                    _mockAuditRepository.Object,
                    _mockNotificationService.Object,
                    _mockUserService.Object,
                    _mockLogger.Object);

                var command = new PrintToKitchenCommand { TicketId = ticket.Id };

                // Act
                var task = handler.HandleAsync(command);
                task.Wait();
                var result = task.Result;

                // Assert properties
                var orderWasRouted = routingCalled;
                var orderWasPrinted = printingCalled;
                var handlerReturnedSuccess = result.Success;
                var errorWasLogged = result.Errors.Any(e => e.Contains("KDS Notification Failed"));

                return orderWasRouted && orderWasPrinted && handlerReturnedSuccess && errorWasLogged;
            });
    }

    #region Test Data Generators

    /// <summary>
    /// Generator for valid tickets with kitchen items.
    /// </summary>
    public static Arbitrary<Ticket> ValidTicketWithKitchenItemsGenerator() =>
        Arb.From(Gen.Fresh(() =>
        {
            var terminalId = Guid.NewGuid();
            var shiftId = Guid.NewGuid();
            var orderTypeId = Guid.NewGuid();
            var userId = new UserId(Guid.NewGuid());
            var ticketNumber = new System.Random().Next(1, 1000);

            var ticket = Ticket.Create(
                ticketNumber,
                userId,
                terminalId,
                shiftId,
                orderTypeId);

            // Add a kitchen item
            var menuItemId = Guid.NewGuid();
            var printerGroupId = Guid.NewGuid();
            var orderLine = OrderLine.Create(
                ticket.Id,
                menuItemId,
                "Test Item",
                1,
                new Money(10.00m, "USD"),
                0m);
            
            // Set kitchen properties via reflection or use a method if available
            typeof(OrderLine).GetProperty("ShouldPrintToKitchen")!
                .SetValue(orderLine, true);
            typeof(OrderLine).GetProperty("PrinterGroupId")!
                .SetValue(orderLine, printerGroupId);
            
            ticket.AddOrderLine(orderLine);

            return ticket;
        }));

    /// <summary>
    /// Generator for kitchen order IDs (1-5 IDs).
    /// </summary>
    public static Arbitrary<List<Guid>> KitchenOrderIdsGenerator() =>
        Arb.From(
            from count in Gen.Choose(1, 5)
            select Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList());

    /// <summary>
    /// Generator for various exception types.
    /// </summary>
    public static Arbitrary<Exception> ExceptionGenerator() =>
        Arb.From(Gen.OneOf<Exception>(
            Gen.Constant<Exception>(new Exception("SignalR connection failed")),
            Gen.Constant<Exception>(new InvalidOperationException("Hub not available")),
            Gen.Constant<Exception>(new TimeoutException("Notification timeout")),
            Gen.Constant<Exception>(new Exception("Network error"))
        ));

    #endregion

    /// <summary>
    /// Unit test: Multiple kitchen orders trigger multiple notifications
    /// Validates: Requirements REQ-002, TEST-001
    /// </summary>
    [Fact]
    public async Task MultipleKitchenOrders_TriggersMultipleNotifications()
    {
        // Arrange
        var notificationCallCount = 0;
        var capturedKitchenOrderIds = new List<Guid>();
        var kitchenOrderIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var terminalId = Guid.NewGuid();
        var shiftId = Guid.NewGuid();
        var orderTypeId = Guid.NewGuid();
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = 100;

        var ticket = Ticket.Create(
            ticketNumber,
            userId,
            terminalId,
            shiftId,
            orderTypeId);

        var menuItemId = Guid.NewGuid();
        var printerGroupId = Guid.NewGuid();
        var orderLine = OrderLine.Create(
            ticket.Id,
            menuItemId,
            "Test Item",
            1,
            new Money(10.00m, "USD"),
            0m);
        
        typeof(OrderLine).GetProperty("ShouldPrintToKitchen")!
            .SetValue(orderLine, true);
        typeof(OrderLine).GetProperty("PrinterGroupId")!
            .SetValue(orderLine, printerGroupId);
        
        ticket.AddOrderLine(orderLine);

        _mockTicketRepository.Setup(r => r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mockRoutingService.Setup(r => r.RouteToKitchenAsync(It.IsAny<TicketDto>(), It.IsAny<List<Guid>?>()))
            .ReturnsAsync(kitchenOrderIds);

        _mockPrintService.Setup(p => p.PrintTicketAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KitchenPrintResult(
                Success: true,
                Message: "Success",
                PrintedCount: 1,
                Errors: null));

        _mockNotificationService.Setup(n => n.NotifyOrderCreatedAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Callback<Guid, string, string>((id, _, __) =>
            {
                notificationCallCount++;
                capturedKitchenOrderIds.Add(id);
            })
            .Returns(Task.CompletedTask);

        _mockAuditRepository.Setup(a => a.AddAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new PrintToKitchenCommandHandler(
            _mockTicketRepository.Object,
            _mockPrintService.Object,
            _mockRoutingService.Object,
            _mockAuditRepository.Object,
            _mockNotificationService.Object,
            _mockUserService.Object,
            _mockLogger.Object);

        var command = new PrintToKitchenCommand { TicketId = ticket.Id };

        // Act
        await handler.HandleAsync(command);

        // Assert
        notificationCallCount.Should().Be(3, "notification should be called once per kitchen order ID");
        capturedKitchenOrderIds.Should().BeEquivalentTo(kitchenOrderIds, "all kitchen order IDs should be notified");
    }
}
