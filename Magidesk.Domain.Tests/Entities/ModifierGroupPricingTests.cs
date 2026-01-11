using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Tests.Entities;

/// <summary>
/// Unit tests for ModifierGroup pricing functionality (Feature G.5 - BE-G.5-01)
/// </summary>
public class ModifierGroupPricingTests
{
    [Fact]
    public void Create_WithFreeModifiers_SetsPricingTier()
    {
        // Arrange & Act
        var group = ModifierGroup.Create(
            "Toppings",
            freeModifiers: 2,
            extraModifierPrice: 0.50m);

        // Assert
        Assert.Equal(2, group.FreeModifiers);
        Assert.Equal(0.50m, group.ExtraModifierPrice);
    }

    [Fact]
    public void Create_WithNegativeFreeModifiers_ThrowsException()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() =>
            ModifierGroup.Create("Toppings", freeModifiers: -1));

        Assert.Contains("Free modifiers count cannot be negative", exception.Message);
    }

    [Fact]
    public void Create_WithNegativeExtraPrice_ThrowsException()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() =>
            ModifierGroup.Create("Toppings", extraModifierPrice: -0.50m));

        Assert.Contains("Extra modifier price cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateModifierCost_WithinFreeCount_ReturnsZero()
    {
        // Arrange
        var group = ModifierGroup.Create(
            "Toppings",
            freeModifiers: 3,
            extraModifierPrice: 0.75m);

        // Act
        var cost = group.CalculateModifierCost(2); // 2 < 3 free

        // Assert
        Assert.Equal(0m, cost);
    }

    [Fact]
    public void CalculateModifierCost_ExactlyFreeCount_ReturnsZero()
    {
        // Arrange
        var group = ModifierGroup.Create(
            "Toppings",
            freeModifiers: 3,
            extraModifierPrice: 0.75m);

        // Act
        var cost = group.CalculateModifierCost(3); // 3 == 3 free

        // Assert
        Assert.Equal(0m, cost);
    }

    [Fact]
    public void CalculateModifierCost_BeyondFreeCount_CalculatesCorrectly()
    {
        // Arrange
        var group = ModifierGroup.Create(
            "Toppings",
            freeModifiers: 2,
            extraModifierPrice: 0.50m);

        // Act
        var cost = group.CalculateModifierCost(5); // 5 - 2 = 3 extra @ $0.50 = $1.50

        // Assert
        Assert.Equal(1.50m, cost);
    }

    [Fact]
    public void CalculateModifierCost_NoFreeModifiers_ChargesAll()
    {
        // Arrange
        var group = ModifierGroup.Create(
            "Premium Toppings",
            freeModifiers: 0,
            extraModifierPrice: 1.00m);

        // Act
        var cost = group.CalculateModifierCost(3); // 3 - 0 = 3 @ $1.00 = $3.00

        // Assert
        Assert.Equal(3.00m, cost);
    }

    [Fact]
    public void UpdatePricingTier_ValidValues_UpdatesProperties()
    {
        // Arrange
        var group = ModifierGroup.Create("Toppings");

        // Act
        group.UpdatePricingTier(freeModifiers: 2, extraModifierPrice: 0.75m);

        // Assert
        Assert.Equal(2, group.FreeModifiers);
        Assert.Equal(0.75m, group.ExtraModifierPrice);
    }

    [Fact]
    public void UpdatePricingTier_NegativeFreeModifiers_ThrowsException()
    {
        // Arrange
        var group = ModifierGroup.Create("Toppings");

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() =>
            group.UpdatePricingTier(-1, 0.50m));

        Assert.Contains("Free modifiers count cannot be negative", exception.Message);
    }

    [Fact]
    public void UpdatePricingTier_NegativePrice_ThrowsException()
    {
        // Arrange
        var group = ModifierGroup.Create("Toppings");

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleViolationException>(() =>
            group.UpdatePricingTier(2, -0.50m));

        Assert.Contains("Extra modifier price cannot be negative", exception.Message);
    }
}
