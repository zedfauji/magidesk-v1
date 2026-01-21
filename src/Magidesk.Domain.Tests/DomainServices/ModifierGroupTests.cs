using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class ModifierGroupTests
{
    [Fact]
    public void ModifierGroup_Create_WithValidData_ShouldCreateGroup()
    {
        // Arrange
        var name = "Toppings";

        // Act
        var group = ModifierGroup.Create(name);

        // Assert
        group.Should().NotBeNull();
        group.Id.Should().NotBeEmpty();
        group.Name.Should().Be(name);
        group.IsRequired.Should().BeFalse();
        group.MinSelections.Should().Be(0);
        group.MaxSelections.Should().Be(1);
        group.AllowMultipleSelections.Should().BeFalse();
        group.IsActive.Should().BeTrue();
        group.Version.Should().Be(1);
    }

    [Fact]
    public void ModifierGroup_Create_WithAllOptions_ShouldCreateGroup()
    {
        // Arrange
        var name = "Size";
        var isRequired = true;
        var minSelections = 1;
        var maxSelections = 1;
        var allowMultipleSelections = false;
        var description = "Size options";
        var displayOrder = 1;
        var isActive = false;

        // Act
        var group = ModifierGroup.Create(
            name,
            isRequired,
            minSelections,
            maxSelections,
            allowMultipleSelections,
            description,
            displayOrder,
            isActive);

        // Assert
        group.Name.Should().Be(name);
        group.IsRequired.Should().Be(isRequired);
        group.MinSelections.Should().Be(minSelections);
        group.MaxSelections.Should().Be(maxSelections);
        group.AllowMultipleSelections.Should().Be(allowMultipleSelections);
        group.Description.Should().Be(description);
        group.DisplayOrder.Should().Be(displayOrder);
        group.IsActive.Should().Be(isActive);
    }

    [Fact]
    public void ModifierGroup_Create_WithMultipleSelections_ShouldCreateGroup()
    {
        // Arrange
        var name = "Toppings";
        var maxSelections = 5;
        var allowMultipleSelections = true;

        // Act
        var group = ModifierGroup.Create(name, maxSelections: maxSelections, allowMultipleSelections: allowMultipleSelections);

        // Assert
        group.MaxSelections.Should().Be(maxSelections);
        group.AllowMultipleSelections.Should().BeTrue();
    }

    [Fact]
    public void ModifierGroup_Create_WithEmptyName_ShouldThrowException()
    {
        // Act
        var act = () => ModifierGroup.Create("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void ModifierGroup_Create_WithNegativeMinSelections_ShouldThrowException()
    {
        // Act
        var act = () => ModifierGroup.Create("Test", minSelections: -1);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*minimum selections cannot be negative*");
    }

    [Fact]
    public void ModifierGroup_Create_WithMaxLessThanMin_ShouldThrowException()
    {
        // Act
        var act = () => ModifierGroup.Create("Test", minSelections: 2, maxSelections: 1);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*maximum selections cannot be less than minimum*");
    }

    [Fact]
    public void ModifierGroup_Create_WithMaxGreaterThanOneWithoutMultiple_ShouldThrowException()
    {
        // Act
        var act = () => ModifierGroup.Create("Test", maxSelections: 3, allowMultipleSelections: false);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*multiple selections must be allowed*");
    }

    [Fact]
    public void ModifierGroup_UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var group = ModifierGroup.Create("Old Name");
        var newName = "New Name";

        // Act
        group.UpdateName(newName);

        // Assert
        group.Name.Should().Be(newName);
    }

    [Fact]
    public void ModifierGroup_UpdateName_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var group = ModifierGroup.Create("Test");

        // Act
        var act = () => group.UpdateName("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void ModifierGroup_UpdateSelectionConstraints_WithValidConstraints_ShouldUpdate()
    {
        // Arrange
        var group = ModifierGroup.Create("Test");
        var minSelections = 1;
        var maxSelections = 3;
        var allowMultipleSelections = true;

        // Act
        group.UpdateSelectionConstraints(minSelections, maxSelections, allowMultipleSelections);

        // Assert
        group.MinSelections.Should().Be(minSelections);
        group.MaxSelections.Should().Be(maxSelections);
        group.AllowMultipleSelections.Should().Be(allowMultipleSelections);
    }

    [Fact]
    public void ModifierGroup_UpdateSelectionConstraints_WithInvalidConstraints_ShouldThrowException()
    {
        // Arrange
        var group = ModifierGroup.Create("Test");

        // Act
        var act = () => group.UpdateSelectionConstraints(2, 1, false);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*maximum selections cannot be less than minimum*");
    }

    [Fact]
    public void ModifierGroup_IsValidSelectionCount_WithValidCount_ShouldReturnTrue()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", minSelections: 1, maxSelections: 3, allowMultipleSelections: true);

        // Act
        var result = group.IsValidSelectionCount(2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ModifierGroup_IsValidSelectionCount_WithCountBelowMin_ShouldReturnFalse()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", minSelections: 2, maxSelections: 5, allowMultipleSelections: true);

        // Act
        var result = group.IsValidSelectionCount(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ModifierGroup_IsValidSelectionCount_WithCountAboveMax_ShouldReturnFalse()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", minSelections: 1, maxSelections: 3, allowMultipleSelections: true);

        // Act
        var result = group.IsValidSelectionCount(4);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ModifierGroup_IsValidSelectionCount_WithRequiredGroupAndZero_ShouldReturnFalse()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", isRequired: true, minSelections: 1, maxSelections: 1);

        // Act
        var result = group.IsValidSelectionCount(0);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ModifierGroup_Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", isActive: false);

        // Act
        group.Activate();

        // Assert
        group.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ModifierGroup_Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var group = ModifierGroup.Create("Test", isActive: true);

        // Act
        group.Deactivate();

        // Assert
        group.IsActive.Should().BeFalse();
    }
}

