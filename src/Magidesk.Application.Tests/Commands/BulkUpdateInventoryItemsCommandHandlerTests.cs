using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Commands;

public class BulkUpdateInventoryItemsCommandHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly Mock<IInventoryAdjustmentRepository> _mockAdjustmentRepository;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly BulkUpdateInventoryItemsCommandHandler _handler;

    public BulkUpdateInventoryItemsCommandHandlerTests()
    {
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _mockAdjustmentRepository = new Mock<IInventoryAdjustmentRepository>();
        _mockUserContextService = new Mock<IUserContextService>();
        _handler = new BulkUpdateInventoryItemsCommandHandler(
            _mockItemRepository.Object,
            _mockAdjustmentRepository.Object,
            _mockUserContextService.Object);
    }

    [Fact]
    public async Task Handle_ValidItems_UpdatesEachItemAndCreatesAdjustment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var item1 = InventoryItem.Create("Item 1", "unit", 10m, 5m);
        var item2 = InventoryItem.Create("Item 2", "unit", 20m, 10m);

        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(item1.Id, 15m, 6m),  // Delta = 5
            new(item2.Id, 18m, 12m)  // Delta = -2
        };

        var command = new BulkUpdateInventoryItemsCommand(entries, "Inventory audit");

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(item1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item1);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(item2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item2);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _mockItemRepository.Verify(x => x.UpdateAsync(item1, It.IsAny<CancellationToken>()), Times.Once);
        _mockItemRepository.Verify(x => x.UpdateAsync(item2, It.IsAny<CancellationToken>()), Times.Once);
        _mockAdjustmentRepository.Verify(x => x.AddAsync(It.IsAny<InventoryAdjustment>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ZeroDelta_DoesNotCreateAdjustmentRecord()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var item = InventoryItem.Create("Item", "unit", 10m, 5m);

        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(item.Id, 10m, 6m)  // Delta = 0
        };

        var command = new BulkUpdateInventoryItemsCommand(entries, "Adjustment");

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _mockAdjustmentRepository.Verify(x => x.AddAsync(It.IsAny<InventoryAdjustment>()), Times.Never);
        _mockItemRepository.Verify(x => x.UpdateAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(itemId, 10m, 5m)
        };

        var command = new BulkUpdateInventoryItemsCommand(entries, "Adjustment");

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        Func<Task> act = () => _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_UsesNonEmptyUserIdentity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        userId.Should().NotBe(Guid.Empty);

        var item = InventoryItem.Create("Item", "unit", 10m, 5m);

        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(item.Id, 15m, 5m)
        };

        var command = new BulkUpdateInventoryItemsCommand(entries, "Adjustment");

        _mockUserContextService
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        InventoryAdjustment? capturedAdjustment = null;
        _mockAdjustmentRepository
            .Setup(x => x.AddAsync(It.IsAny<InventoryAdjustment>()))
            .Callback<InventoryAdjustment>(adj => capturedAdjustment = adj)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        capturedAdjustment.Should().NotBeNull();
        capturedAdjustment!.UserId.Should().Be(userId);
        capturedAdjustment.UserId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Validator_EmptyItemList_ReturnsValidationFailure()
    {
        // Arrange
        var validator = new BulkUpdateInventoryItemsCommandValidator();
        var command = new BulkUpdateInventoryItemsCommand(new List<BulkUpdateInventoryItemEntryDto>(), "Reason");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage.Contains("Items list must not be empty"));
    }

    [Fact]
    public void Validator_NegativeStockQuantity_ReturnsValidationFailure()
    {
        // Arrange
        var validator = new BulkUpdateInventoryItemsCommandValidator();
        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(Guid.NewGuid(), -5m, 10m)
        };
        var command = new BulkUpdateInventoryItemsCommand(entries, "Reason");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.ErrorMessage.Contains("Stock quantity must be greater than or equal to 0"));
    }

    [Fact]
    public void Validator_ValidItems_PassesValidation()
    {
        // Arrange
        var validator = new BulkUpdateInventoryItemsCommandValidator();
        var entries = new List<BulkUpdateInventoryItemEntryDto>
        {
            new(Guid.NewGuid(), 10m, 5m),
            new(Guid.NewGuid(), 20m, 10m)
        };
        var command = new BulkUpdateInventoryItemsCommand(entries, "Inventory audit");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
