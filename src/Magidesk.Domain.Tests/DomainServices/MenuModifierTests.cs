using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class MenuModifierTests
{
    [Fact]
    public void MenuModifier_Create_WithValidData_ShouldCreateModifier()
    {
        // Arrange
        var name = "Extra Cheese";
        var modifierType = ModifierType.Extra;
        var basePrice = new Money(2.50m);

        // Act
        var modifier = MenuModifier.Create(name, modifierType, basePrice);

        // Assert
        modifier.Should().NotBeNull();
        modifier.Id.Should().NotBeEmpty();
        modifier.Name.Should().Be(name);
        modifier.ModifierType.Should().Be(modifierType);
        modifier.BasePrice.Should().Be(basePrice);
        modifier.TaxRate.Should().Be(0m);
        modifier.ShouldPrintToKitchen.Should().BeTrue();
        modifier.IsSectionWisePrice.Should().BeFalse();
        modifier.IsActive.Should().BeTrue();
        modifier.Version.Should().Be(1);
    }

    [Fact]
    public void MenuModifier_Create_WithAllOptions_ShouldCreateModifier()
    {
        // Arrange
        var name = "Large Size";
        var modifierType = ModifierType.Normal;
        var basePrice = new Money(3.00m);
        var modifierGroupId = Guid.NewGuid();
        var description = "Large size option";
        var taxRate = 0.10m;
        var shouldPrintToKitchen = false;
        var isSectionWisePrice = true;
        var sectionName = "Full";
        var multiplierName = "1x";
        var displayOrder = 5;
        var isActive = false;

        // Act
        var modifier = MenuModifier.Create(
            name,
            modifierType,
            basePrice,
            modifierGroupId,
            description,
            taxRate,
            shouldPrintToKitchen,
            isSectionWisePrice,
            sectionName,
            multiplierName,
            displayOrder,
            isActive);

        // Assert
        modifier.Name.Should().Be(name);
        modifier.ModifierType.Should().Be(modifierType);
        modifier.BasePrice.Should().Be(basePrice);
        modifier.ModifierGroupId.Should().Be(modifierGroupId);
        modifier.Description.Should().Be(description);
        modifier.TaxRate.Should().Be(taxRate);
        modifier.ShouldPrintToKitchen.Should().Be(shouldPrintToKitchen);
        modifier.IsSectionWisePrice.Should().Be(isSectionWisePrice);
        modifier.SectionName.Should().Be(sectionName);
        modifier.MultiplierName.Should().Be(multiplierName);
        modifier.DisplayOrder.Should().Be(displayOrder);
        modifier.IsActive.Should().Be(isActive);
    }

    [Fact]
    public void MenuModifier_Create_WithEmptyName_ShouldThrowException()
    {
        // Act
        var act = () => MenuModifier.Create("", ModifierType.Normal, new Money(1m));

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void MenuModifier_Create_WithNegativeBasePrice_ShouldThrowException()
    {
        // Act
        var act = () => MenuModifier.Create("Test", ModifierType.Normal, new Money(-1m));

        // Assert
        // Money value object itself prevents negative amounts, so ArgumentException is thrown
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void MenuModifier_Create_WithInvalidTaxRate_ShouldThrowException()
    {
        // Act
        var act = () => MenuModifier.Create("Test", ModifierType.Normal, new Money(1m), taxRate: 1.5m);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*tax rate must be between 0 and 1*");
    }

    [Fact]
    public void MenuModifier_UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var modifier = MenuModifier.Create("Old Name", ModifierType.Normal, new Money(1m));
        var newName = "New Name";

        // Act
        modifier.UpdateName(newName);

        // Assert
        modifier.Name.Should().Be(newName);
    }

    [Fact]
    public void MenuModifier_UpdateName_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));

        // Act
        var act = () => modifier.UpdateName("");

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void MenuModifier_UpdateBasePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));
        var newPrice = new Money(2.50m);

        // Act
        modifier.UpdateBasePrice(newPrice);

        // Assert
        modifier.BasePrice.Should().Be(newPrice);
    }

    [Fact]
    public void MenuModifier_UpdateBasePrice_WithNegativePrice_ShouldThrowException()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));

        // Act
        var act = () => modifier.UpdateBasePrice(new Money(-1m));

        // Assert
        // Money value object itself prevents negative amounts, so ArgumentException is thrown
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void MenuModifier_UpdateTaxRate_WithValidRate_ShouldUpdateTaxRate()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));
        var newTaxRate = 0.15m;

        // Act
        modifier.UpdateTaxRate(newTaxRate);

        // Assert
        modifier.TaxRate.Should().Be(newTaxRate);
    }

    [Fact]
    public void MenuModifier_UpdateTaxRate_WithInvalidRate_ShouldThrowException()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));

        // Act
        var act = () => modifier.UpdateTaxRate(1.5m);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*tax rate must be between 0 and 1*");
    }

    [Fact]
    public void MenuModifier_SetSectionWisePrice_ShouldUpdateSectionConfiguration()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m));
        var sectionName = "Half";

        // Act
        modifier.SetSectionWisePrice(true, sectionName);

        // Assert
        modifier.IsSectionWisePrice.Should().BeTrue();
        modifier.SectionName.Should().Be(sectionName);
    }

    [Fact]
    public void MenuModifier_Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m), isActive: false);

        // Act
        modifier.Activate();

        // Assert
        modifier.IsActive.Should().BeTrue();
    }

    [Fact]
    public void MenuModifier_Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var modifier = MenuModifier.Create("Test", ModifierType.Normal, new Money(1m), isActive: true);

        // Act
        modifier.Deactivate();

        // Assert
        modifier.IsActive.Should().BeFalse();
    }
}

