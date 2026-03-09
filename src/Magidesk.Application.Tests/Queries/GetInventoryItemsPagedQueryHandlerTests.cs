using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Queries;

public class GetInventoryItemsPagedQueryHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockRepository;
    private readonly GetInventoryItemsPagedQueryHandler _handler;

    public GetInventoryItemsPagedQueryHandlerTests()
    {
        _mockRepository = new Mock<IInventoryItemRepository>();
        _handler = new GetInventoryItemsPagedQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsPagedResult()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            InventoryItem.Create("Item 1", "unit", 10m, 5m),
            InventoryItem.Create("Item 2", "unit", 20m, 10m),
            InventoryItem.Create("Item 3", "unit", 30m, 15m),
            InventoryItem.Create("Item 4", "unit", 40m, 20m),
            InventoryItem.Create("Item 5", "unit", 50m, 25m)
        };
        var totalCount = 100;

        _mockRepository
            .Setup(x => x.GetPagedAsync(null, InventoryFilterType.None, null, 0, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, totalCount));

        var query = new GetInventoryItemsPagedQuery(null, InventoryFilterType.None, null, 0, 5);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(100);
        result.Items.Count.Should().Be(5);
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_PassesSearchTermToRepository()
    {
        // Arrange
        var searchTerm = "coffee";
        var items = new List<InventoryItem>
        {
            InventoryItem.Create("Coffee Beans", "kg", 10m, 5m)
        };

        _mockRepository
            .Setup(x => x.GetPagedAsync(searchTerm, InventoryFilterType.None, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 1));

        var query = new GetInventoryItemsPagedQuery(searchTerm, InventoryFilterType.None, null, 0, 10);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            x => x.GetPagedAsync(searchTerm, InventoryFilterType.None, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithLowStockFilter_PassesLowStockFilterToRepository()
    {
        // Arrange
        var items = new List<InventoryItem>();
        var filter = InventoryFilterType.LowStock;

        _mockRepository
            .Setup(x => x.GetPagedAsync(null, filter, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 0));

        var query = new GetInventoryItemsPagedQuery(null, filter, null, 0, 10);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            x => x.GetPagedAsync(null, filter, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MapsEntityToDtoCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);

        // Create items using reflection to set private properties for testing
        var item = InventoryItem.Create("Test Item", "kg", 25m, 10m, "SKU-TEST", categoryId);
        var itemsProp = typeof(InventoryItem).GetProperty("CreatedAt");
        itemsProp?.SetValue(item, createdAt);

        var items = new List<InventoryItem> { item };

        _mockRepository
            .Setup(x => x.GetPagedAsync(null, InventoryFilterType.None, null, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 1));

        var query = new GetInventoryItemsPagedQuery(null, InventoryFilterType.None, null, 0, 10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        var dto = result.Items.First();
        dto.Id.Should().Be(item.Id);
        dto.Name.Should().Be("Test Item");
        dto.Unit.Should().Be("kg");
        dto.SkuCode.Should().Be("SKU-TEST");
        dto.StockQuantity.Should().Be(25m);
        dto.ReorderPoint.Should().Be(10m);
        dto.CategoryId.Should().Be(categoryId);
        dto.IsActive.Should().BeTrue();
    }
}
