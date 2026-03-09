using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands.Inventory;
using Magidesk.Application.Commands.Inventory.Handlers;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Moq;
using Xunit;

namespace Magidesk.Application.Tests.Commands.Inventory.Handlers;

/// <summary>
/// Unit tests for DeleteInventoryItemCommandHandler.
/// Tests soft deletion, validation, and business rule enforcement.
/// </summary>
public class DeleteInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryItemRepository> _mockItemRepository;
    private readonly Mock<ITicketRepository> _mockTicketRepository;
    private readonly DeleteInventoryItemCommandHandler _handler;

    public DeleteInventoryItemCommandHandlerTests()
    {
        _mockItemRepository = new Mock<IInventoryItemRepository>();
        _mockTicketRepository = new Mock<ITicketRepository>();
        
        _handler = new DeleteInventoryItemCommandHandler(
            _mockItemRepository.Object,
            _mockTicketRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidItemDeletion_DeactivatesItemSuccessfully()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse("Item should be deactivated (soft deleted)");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Item should be updated to persist the soft delete");

        _mockTicketRepository.Verify(
            x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Active order references should be checked before deletion");
    }

    [Fact]
    public async Task Handle_ItemNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentItemId = Guid.NewGuid();
        var command = new DeleteInventoryItemCommand(nonExistentItemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(nonExistentItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Item not found");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when it does not exist");

        _mockTicketRepository.Verify(
            x => x.HasActiveOrdersWithItemAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Active order check should not be performed when item does not exist");
    }

    [Fact]
    public async Task Handle_ItemHasActiveOrders_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete item with active order references");

        existingItem.IsActive.Should().BeTrue("Item should remain active when deletion is rejected");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), 
            Times.Never,
            "Item should not be updated when it has active order references");

        _mockTicketRepository.Verify(
            x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Active order references should be checked");
    }

    [Fact]
    public async Task Handle_ValidDeletion_SetsIsActiveToFalse()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);

        // Verify item starts as active
        existingItem.IsActive.Should().BeTrue("Item should be active before deletion");

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse("IsActive should be set to false for soft delete");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(
                It.Is<InventoryItem>(i => i.IsActive == false), 
                It.IsAny<CancellationToken>()), 
            Times.Once,
            "Item with IsActive=false should be persisted");
    }

    [Fact]
    public async Task Handle_AlreadyInactiveItem_CanBeDeletedAgain()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            null);
        
        // Item is already inactive
        existingItem.Deactivate();
        existingItem.IsActive.Should().BeFalse("Item should be inactive before test");

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse("Item should remain inactive");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once,
            "Update should still be called even if item is already inactive");
    }

    [Fact]
    public async Task Handle_ItemWithCategory_CanBeDeleted()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            100m,
            20m,
            "TEST-001",
            categoryId);

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse("Item with category should be deletable");
        existingItem.CategoryId.Should().Be(categoryId, "Category association should be preserved");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ItemWithZeroStock_CanBeDeleted()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = InventoryItem.Create(
            "Test Item",
            "kg",
            0m,
            20m,
            "TEST-001",
            null);

        var command = new DeleteInventoryItemCommand(itemId);

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        _mockTicketRepository
            .Setup(x => x.HasActiveOrdersWithItemAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingItem.IsActive.Should().BeFalse("Item with zero stock should be deletable");
        existingItem.StockQuantity.Should().Be(0m, "Stock quantity should be preserved");

        _mockItemRepository.Verify(
            x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellationToken()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new DeleteInventoryItemCommand(itemId);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockItemRepository
            .Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
