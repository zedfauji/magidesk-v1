using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class OrderTypeTests
{
    [Fact]
    public void OrderType_Create_WithValidData_ShouldCreateOrderType()
    {
        // Arrange
        var name = "Dine In";

        // Act
        var orderType = OrderType.Create(name);

        // Assert
        orderType.Should().NotBeNull();
        orderType.Id.Should().NotBeEmpty();
        orderType.Name.Should().Be(name);
        orderType.CloseOnPaid.Should().BeFalse();
        orderType.AllowSeatBasedOrder.Should().BeFalse();
        orderType.AllowToAddTipsLater.Should().BeFalse();
        orderType.IsBarTab.Should().BeFalse();
        orderType.IsActive.Should().BeTrue();
        orderType.Version.Should().Be(1);
    }

    [Fact]
    public void OrderType_Create_WithAllOptions_ShouldCreateOrderType()
    {
        // Arrange
        var name = "Bar Tab";
        var closeOnPaid = true;
        var allowSeatBasedOrder = true;
        var allowToAddTipsLater = true;
        var isBarTab = true;
        var isActive = false;

        // Act
        var orderType = OrderType.Create(name, closeOnPaid, allowSeatBasedOrder, allowToAddTipsLater, isBarTab, isActive);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.CloseOnPaid.Should().Be(closeOnPaid);
        orderType.AllowSeatBasedOrder.Should().Be(allowSeatBasedOrder);
        orderType.AllowToAddTipsLater.Should().Be(allowToAddTipsLater);
        orderType.IsBarTab.Should().Be(isBarTab);
        orderType.IsActive.Should().Be(isActive);
    }

    [Fact]
    public void OrderType_Create_WithEmptyName_ShouldThrowException()
    {
        // Act
        var act = () => OrderType.Create("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void OrderType_Create_WithWhitespaceName_ShouldThrowException()
    {
        // Act
        var act = () => OrderType.Create("   ");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void OrderType_UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var orderType = OrderType.Create("Old Name");
        var newName = "New Name";

        // Act
        orderType.UpdateName(newName);

        // Assert
        orderType.Name.Should().Be(newName);
    }

    [Fact]
    public void OrderType_UpdateName_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var orderType = OrderType.Create("Test Name");

        // Act
        var act = () => orderType.UpdateName("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void OrderType_SetCloseOnPaid_ShouldUpdateCloseOnPaid()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        orderType.SetCloseOnPaid(true);

        // Assert
        orderType.CloseOnPaid.Should().BeTrue();
    }

    [Fact]
    public void OrderType_SetAllowSeatBasedOrder_ShouldUpdateAllowSeatBasedOrder()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        orderType.SetAllowSeatBasedOrder(true);

        // Assert
        orderType.AllowSeatBasedOrder.Should().BeTrue();
    }

    [Fact]
    public void OrderType_SetAllowToAddTipsLater_ShouldUpdateAllowToAddTipsLater()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        orderType.SetAllowToAddTipsLater(true);

        // Assert
        orderType.AllowToAddTipsLater.Should().BeTrue();
    }

    [Fact]
    public void OrderType_SetIsBarTab_ShouldUpdateIsBarTab()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        orderType.SetIsBarTab(true);

        // Assert
        orderType.IsBarTab.Should().BeTrue();
    }

    [Fact]
    public void OrderType_Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var orderType = OrderType.Create("Test", isActive: false);

        // Act
        orderType.Activate();

        // Assert
        orderType.IsActive.Should().BeTrue();
    }

    [Fact]
    public void OrderType_Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var orderType = OrderType.Create("Test", isActive: true);

        // Act
        orderType.Deactivate();

        // Assert
        orderType.IsActive.Should().BeFalse();
    }

    [Fact]
    public void OrderType_SetProperty_ShouldAddProperty()
    {
        // Arrange
        var orderType = OrderType.Create("Test");
        var key = "testKey";
        var value = "testValue";

        // Act
        orderType.SetProperty(key, value);

        // Assert
        orderType.GetProperty(key).Should().Be(value);
        orderType.Properties.Should().ContainKey(key);
        orderType.Properties[key].Should().Be(value);
    }

    [Fact]
    public void OrderType_SetProperty_WithEmptyKey_ShouldThrowException()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        var act = () => orderType.SetProperty("", "value");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*key cannot be empty*");
    }

    [Fact]
    public void OrderType_RemoveProperty_ShouldRemoveProperty()
    {
        // Arrange
        var orderType = OrderType.Create("Test");
        var key = "testKey";
        orderType.SetProperty(key, "value");

        // Act
        orderType.RemoveProperty(key);

        // Assert
        orderType.GetProperty(key).Should().BeNull();
        orderType.Properties.Should().NotContainKey(key);
    }

    [Fact]
    public void OrderType_GetProperty_WithNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var orderType = OrderType.Create("Test");

        // Act
        var result = orderType.GetProperty("nonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void OrderType_SetProperty_ShouldUpdateExistingProperty()
    {
        // Arrange
        var orderType = OrderType.Create("Test");
        var key = "testKey";
        orderType.SetProperty(key, "oldValue");

        // Act
        orderType.SetProperty(key, "newValue");

        // Assert
        orderType.GetProperty(key).Should().Be("newValue");
    }
}

