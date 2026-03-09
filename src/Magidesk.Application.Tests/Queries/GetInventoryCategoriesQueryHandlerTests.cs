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

public class GetInventoryCategoriesQueryHandlerTests
{
    private readonly Mock<IInventoryCategoryRepository> _mockRepository;
    private readonly GetInventoryCategoriesQueryHandler _handler;

    public GetInventoryCategoriesQueryHandlerTests()
    {
        _mockRepository = new Mock<IInventoryCategoryRepository>();
        _handler = new GetInventoryCategoriesQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllActiveCategories()
    {
        // Arrange
        var categories = new List<InventoryCategory>
        {
            InventoryCategory.Create("Beverages", 1),
            InventoryCategory.Create("Snacks", 2),
            InventoryCategory.Create("Dairy", 3)
        };

        _mockRepository
            .Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetInventoryCategoriesQuery();

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeAssignableTo<IReadOnlyList<InventoryCategoryDto>>();
    }

    [Fact]
    public async Task Handle_MapsCategoryToDtoCorrectly()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var category = InventoryCategory.Create("Coffee Products", 2, parentId);

        // Use reflection to set the ID for testing
        var idProperty = typeof(InventoryCategory).GetProperty("Id");
        idProperty?.SetValue(category, categoryId);

        var categories = new List<InventoryCategory> { category };

        _mockRepository
            .Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetInventoryCategoriesQuery();

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var dto = result.First();
        dto.Id.Should().Be(categoryId);
        dto.Name.Should().Be("Coffee Products");
        dto.SortOrder.Should().Be(2);
        dto.ParentCategoryId.Should().Be(parentId);
    }
}
