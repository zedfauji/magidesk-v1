using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Services;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Interfaces;
using Magidesk.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Services;

public class KitchenRoutingServiceTests
{
    private readonly Mock<IKitchenOrderRepository> _kitchenOrderRepositoryCheck;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<ILogger<KitchenRoutingService>> _loggerMock;
    private readonly KitchenRoutingService _service;

    public KitchenRoutingServiceTests()
    {
        _kitchenOrderRepositoryCheck = new Mock<IKitchenOrderRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _loggerMock = new Mock<ILogger<KitchenRoutingService>>();
        _service = new KitchenRoutingService(_kitchenOrderRepositoryCheck.Object, _ticketRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RouteToKitchenAsync_ShouldCreateOneKitchenOrder_WhenAllItemsHaveSamePrinterGroup()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var printerGroupId = Guid.NewGuid();
        
        var line1 = new OrderLineDto 
        { 
            Id = Guid.NewGuid(), 
            MenuItemName = "Burger", 
            Quantity = 1, 
            ShouldPrintToKitchen = true, 
            PrinterGroupId = printerGroupId,
            Modifiers = new List<OrderLineModifierDto>()
        };

        var line2 = new OrderLineDto 
        { 
            Id = Guid.NewGuid(), 
            MenuItemName = "Fries", 
            Quantity = 1, 
            ShouldPrintToKitchen = true, 
            PrinterGroupId = printerGroupId,
            Modifiers = new List<OrderLineModifierDto>()
        };

        var ticketDto = new TicketDto
        {
            Id = ticketId,
            TicketNumber = 101,
            OrderLines = new List<OrderLineDto> { line1, line2 }
        };

        var itemIdsToRoute = new List<Guid> { line1.Id, line2.Id };

        // Act
        await _service.RouteToKitchenAsync(ticketDto, itemIdsToRoute);

        // Assert
        _kitchenOrderRepositoryCheck.Verify(r => r.AddAsync(It.Is<KitchenOrder>(k => 
            k.TicketId == ticketDto.Id && 
            k.PrinterGroupId == printerGroupId &&
            k.Items.Count == 2
        )), Times.Once);
    }

    [Fact]
    public async Task RouteToKitchenAsync_ShouldCreateMultipleKitchenOrders_WhenItemsHaveDifferentPrinterGroups()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var kitchenGroupId = Guid.NewGuid();
        var barGroupId = Guid.NewGuid();
        
        var line1 = new OrderLineDto 
        { 
            Id = Guid.NewGuid(), 
            MenuItemName = "Burger", 
            Quantity = 1, 
            ShouldPrintToKitchen = true, 
            PrinterGroupId = kitchenGroupId,
            Modifiers = new List<OrderLineModifierDto>()
        };

        var line2 = new OrderLineDto 
        { 
            Id = Guid.NewGuid(), 
            MenuItemName = "Beer", 
            Quantity = 1, 
            ShouldPrintToKitchen = true, 
            PrinterGroupId = barGroupId,
            Modifiers = new List<OrderLineModifierDto>()
        };

        var ticketDto = new TicketDto
        {
            Id = ticketId,
            TicketNumber = 102,
            OrderLines = new List<OrderLineDto> { line1, line2 }
        };

        var itemIdsToRoute = new List<Guid> { line1.Id, line2.Id };

        // Act
        await _service.RouteToKitchenAsync(ticketDto, itemIdsToRoute);

        // Assert
        // Verify Kitchen Order
        _kitchenOrderRepositoryCheck.Verify(r => r.AddAsync(It.Is<KitchenOrder>(k => 
            k.TicketId == ticketDto.Id && 
            k.PrinterGroupId == kitchenGroupId &&
            k.Items.Count == 1 &&
            k.Items.First().ItemName == "Burger"
        )), Times.Once);

        // Verify Bar Order
        _kitchenOrderRepositoryCheck.Verify(r => r.AddAsync(It.Is<KitchenOrder>(k => 
            k.TicketId == ticketDto.Id && 
            k.PrinterGroupId == barGroupId &&
            k.Items.Count == 1 &&
            k.Items.First().ItemName == "Beer"
        )), Times.Once);

        // Verify total calls
        _kitchenOrderRepositoryCheck.Verify(r => r.AddAsync(It.IsAny<KitchenOrder>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RouteToKitchenAsync_ShouldHandleNullPrinterGroupId_Functionally()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        
        var line1 = new OrderLineDto 
        { 
            Id = Guid.NewGuid(), 
            MenuItemName = "Legacy Item", 
            Quantity = 1, 
            ShouldPrintToKitchen = true, 
            PrinterGroupId = null,
            Modifiers = new List<OrderLineModifierDto>()
        };

        var ticketDto = new TicketDto
        {
            Id = ticketId,
            TicketNumber = 103,
            OrderLines = new List<OrderLineDto> { line1 }
        };

        var itemIdsToRoute = new List<Guid> { line1.Id };

        // Act
        await _service.RouteToKitchenAsync(ticketDto, itemIdsToRoute);

        // Assert
        _kitchenOrderRepositoryCheck.Verify(r => r.AddAsync(It.Is<KitchenOrder>(k => 
            k.TicketId == ticketDto.Id && 
            k.PrinterGroupId == null &&
            k.Items.Count == 1
        )), Times.Once);
    }
}
