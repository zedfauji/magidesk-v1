using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using DomainInvalidOperationException = Magidesk.Domain.Exceptions.InvalidOperationException;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class TableTests
{
    [Fact]
    public void Table_Create_WithValidData_ShouldCreateTable()
    {
        // Arrange
        var tableNumber = 1;
        var capacity = 4;

        // Act
        var table = Table.Create(tableNumber, capacity);

        // Assert
        table.Should().NotBeNull();
        table.Id.Should().NotBeEmpty();
        table.TableNumber.Should().Be(tableNumber);
        table.Capacity.Should().Be(capacity);
        table.X.Should().Be(0);
        table.Y.Should().Be(0);
        table.FloorId.Should().BeNull();
        table.Status.Should().Be(TableStatus.Available);
        table.CurrentTicketId.Should().BeNull();
        table.IsActive.Should().BeTrue();
        table.Version.Should().Be(1);
    }

    [Fact]
    public void Table_Create_WithAllOptions_ShouldCreateTable()
    {
        // Arrange
        var tableNumber = 5;
        var capacity = 6;
        var x = 10;
        var y = 20;
        var floorId = Guid.NewGuid();
        var isActive = false;

        // Act
        var table = Table.Create(tableNumber, capacity, x, y, floorId, isActive: isActive);

        // Assert
        table.TableNumber.Should().Be(tableNumber);
        table.Capacity.Should().Be(capacity);
        table.X.Should().Be(x);
        table.Y.Should().Be(y);
        table.FloorId.Should().Be(floorId);
        table.IsActive.Should().Be(isActive);
    }

    [Fact]
    public void Table_Create_WithZeroTableNumber_ShouldThrowException()
    {
        // Act
        var act = () => Table.Create(0, 4);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*table number must be greater than zero*");
    }

    [Fact]
    public void Table_Create_WithNegativeTableNumber_ShouldThrowException()
    {
        // Act
        var act = () => Table.Create(-1, 4);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*table number must be greater than zero*");
    }

    [Fact]
    public void Table_Create_WithZeroCapacity_ShouldThrowException()
    {
        // Act
        var act = () => Table.Create(1, 0);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*capacity must be greater than zero*");
    }

    [Fact]
    public void Table_Create_WithNegativeCapacity_ShouldThrowException()
    {
        // Act
        var act = () => Table.Create(1, -1);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*capacity must be greater than zero*");
    }

    [Fact]
    public void Table_UpdateTableNumber_WithValidNumber_ShouldUpdateTableNumber()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var newTableNumber = 10;

        // Act
        table.UpdateTableNumber(newTableNumber);

        // Assert
        table.TableNumber.Should().Be(newTableNumber);
    }

    [Fact]
    public void Table_UpdateTableNumber_WithZero_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var act = () => table.UpdateTableNumber(0);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*table number must be greater than zero*");
    }

    [Fact]
    public void Table_UpdateCapacity_WithValidCapacity_ShouldUpdateCapacity()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var newCapacity = 8;

        // Act
        table.UpdateCapacity(newCapacity);

        // Assert
        table.Capacity.Should().Be(newCapacity);
    }

    [Fact]
    public void Table_UpdateCapacity_WithZero_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var act = () => table.UpdateCapacity(0);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*capacity must be greater than zero*");
    }

    [Fact]
    public void Table_UpdatePosition_ShouldUpdatePosition()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var newX = 50;
        var newY = 100;

        // Act
        table.UpdatePosition(newX, newY);

        // Assert
        table.X.Should().Be(newX);
        table.Y.Should().Be(newY);
    }

    [Fact]
    public void Table_UpdateFloor_ShouldUpdateFloor()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var newFloorId = Guid.NewGuid();

        // Act
        table.UpdateFloor(newFloorId);

        // Assert
        table.FloorId.Should().Be(newFloorId);
    }

    [Fact]
    public void Table_UpdateFloor_WithNull_ShouldClearFloor()
    {
        // Arrange
        var table = Table.Create(1, 4, floorId: Guid.NewGuid());

        // Act
        table.UpdateFloor(null);

        // Assert
        table.FloorId.Should().BeNull();
    }

    [Fact]
    public void Table_AssignTicket_WithAvailableTable_ShouldAssignTicket()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var ticketId = Guid.NewGuid();

        // Act
        table.AssignTicket(ticketId);

        // Assert
        table.CurrentTicketId.Should().Be(ticketId);
        table.Status.Should().Be(TableStatus.Seat);
    }

    [Fact]
    public void Table_AssignTicket_WithBookedTable_ShouldAssignTicket()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.Book();
        var ticketId = Guid.NewGuid();

        // Act
        table.AssignTicket(ticketId);

        // Assert
        table.CurrentTicketId.Should().Be(ticketId);
        table.Status.Should().Be(TableStatus.Seat);
    }

    [Fact]
    public void Table_AssignTicket_WithSeatTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var firstTicketId = Guid.NewGuid();
        table.AssignTicket(firstTicketId);
        var secondTicketId = Guid.NewGuid();

        // Act
        var act = () => table.AssignTicket(secondTicketId);

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*status*");
    }

    [Fact]
    public void Table_AssignTicket_WithSameTicket_ShouldSucceed()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var ticketId = Guid.NewGuid();
        table.AssignTicket(ticketId);

        // Act
        table.AssignTicket(ticketId); // Same ticket

        // Assert
        table.CurrentTicketId.Should().Be(ticketId);
    }

    [Fact]
    public void Table_ReleaseTicket_WithAssignedTicket_ShouldReleaseTicket()
    {
        // Arrange
        var table = Table.Create(1, 4);
        var ticketId = Guid.NewGuid();
        table.AssignTicket(ticketId);

        // Act
        table.ReleaseTicket();

        // Assert
        table.CurrentTicketId.Should().BeNull();
        table.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public void Table_ReleaseTicket_WithNoTicket_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var act = () => table.ReleaseTicket();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*does not have an assigned ticket*");
    }

    [Fact]
    public void Table_Book_WithAvailableTable_ShouldBookTable()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        table.Book();

        // Assert
        table.Status.Should().Be(TableStatus.Booked);
    }

    [Fact]
    public void Table_Book_WithSeatTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.AssignTicket(Guid.NewGuid());

        // Act
        var act = () => table.Book();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*status*");
    }

    [Fact]
    public void Table_MarkDirty_WithAvailableTable_ShouldMarkDirty()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        table.MarkDirty();

        // Assert
        table.Status.Should().Be(TableStatus.Dirty);
    }

    [Fact]
    public void Table_MarkDirty_WithSeatTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.AssignTicket(Guid.NewGuid());

        // Act
        var act = () => table.MarkDirty();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*status*");
    }

    [Fact]
    public void Table_MarkClean_WithDirtyTable_ShouldMarkClean()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.MarkDirty();

        // Act
        table.MarkClean();

        // Assert
        table.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public void Table_MarkClean_WithAvailableTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var act = () => table.MarkClean();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*status*");
    }

    [Fact]
    public void Table_Disable_WithAvailableTable_ShouldDisableTable()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        table.Disable();

        // Assert
        table.Status.Should().Be(TableStatus.Disable);
    }

    [Fact]
    public void Table_Disable_WithSeatTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.AssignTicket(Guid.NewGuid());

        // Act
        var act = () => table.Disable();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*active ticket*");
    }

    [Fact]
    public void Table_Enable_WithDisabledTable_ShouldEnableTable()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.Disable();

        // Act
        table.Enable();

        // Assert
        table.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public void Table_Enable_WithAvailableTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var act = () => table.Enable();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*status*");
    }

    [Fact]
    public void Table_Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var table = Table.Create(1, 4, isActive: false);

        // Act
        table.Activate();

        // Assert
        table.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Table_Deactivate_WithAvailableTable_ShouldDeactivateTable()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        table.Deactivate();

        // Assert
        table.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Table_Deactivate_WithSeatTable_ShouldThrowException()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.AssignTicket(Guid.NewGuid());

        // Act
        var act = () => table.Deactivate();

        // Assert
        act.Should().Throw<DomainInvalidOperationException>()
            .WithMessage("*active ticket*");
    }

    [Fact]
    public void Table_IsAvailable_WithActiveAvailableTable_ShouldReturnTrue()
    {
        // Arrange
        var table = Table.Create(1, 4);

        // Act
        var result = table.IsAvailable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Table_IsAvailable_WithInactiveTable_ShouldReturnFalse()
    {
        // Arrange
        var table = Table.Create(1, 4, isActive: false);

        // Act
        var result = table.IsAvailable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Table_IsAvailable_WithSeatTable_ShouldReturnFalse()
    {
        // Arrange
        var table = Table.Create(1, 4);
        table.AssignTicket(Guid.NewGuid());

        // Act
        var result = table.IsAvailable();

        // Assert
        result.Should().BeFalse();
    }
}

