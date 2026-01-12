using System;
using Xunit;
using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Entities;

public class MenuItemTests
{
    [Fact]
    public void GetPriceForLevel_ShouldReturnSpecificPrice_WhenLevelMatches()
    {
        // Arrange
        var item = MenuItem.Create("Test Item", new Money(10m));
        var level1 = PriceLevel.Create("Happy Hour");
        var price1 = new Money(5m);
        item.SetPriceForLevel(level1, price1);

        // Act
        var result = item.GetPriceForLevel(level1.Id);

        // Assert
        result.Should().Be(price1);
    }

    [Fact]
    public void GetPriceForLevel_ShouldReturnDefaultLevelPrice_WhenSpecificLevelMissing_AndDefaultExists()
    {
        // Arrange
        var item = MenuItem.Create("Test Item", new Money(10m));
        var defaultLevel = PriceLevel.Create("Standard", isDefault: true);
        var defaultPrice = new Money(12m);
        item.SetPriceForLevel(defaultLevel, defaultPrice);
        
        var requestedLevelId = Guid.NewGuid(); // Non-existent specific level

        // Act
        // We pass the defaultLevel.Id as the fallback to check
        var result = item.GetPriceForLevel(requestedLevelId, defaultLevel.Id);

        // Assert
        result.Should().Be(defaultPrice);
    }

    [Fact]
    public void GetPriceForLevel_ShouldReturnBasePrice_WhenNoSpecificOrDefaultMatch()
    {
        // Arrange
        var basePrice = new Money(10m);
        var item = MenuItem.Create("Test Item", basePrice);
        var requestedLevelId = Guid.NewGuid();

        // Act
        var result = item.GetPriceForLevel(requestedLevelId);

        // Assert
        result.Should().Be(basePrice);
    }
}
