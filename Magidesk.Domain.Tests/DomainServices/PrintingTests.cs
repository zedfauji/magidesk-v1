using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class PrintingTests
{
    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(obj, value);
    }

    [Fact]
    public void OrderLine_MarkPrintedToKitchen_WithShouldPrint_ShouldMarkAsPrinted()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var orderLine = OrderLine.Create(
            ticketId,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(10m));
        
        SetPrivateProperty(orderLine, "ShouldPrintToKitchen", true);
        SetPrivateProperty(orderLine, "PrintedToKitchen", false);

        // Act
        orderLine.MarkPrintedToKitchen();

        // Assert
        orderLine.PrintedToKitchen.Should().BeTrue();
    }

    [Fact]
    public void OrderLine_MarkPrintedToKitchen_WithoutShouldPrint_ShouldThrowException()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var orderLine = OrderLine.Create(
            ticketId,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(10m));
        
        SetPrivateProperty(orderLine, "ShouldPrintToKitchen", false);

        // Act
        var act = () => orderLine.MarkPrintedToKitchen();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not configured to print*");
    }

    [Fact]
    public void OrderLine_MarkPrintedToKitchen_WithModifiers_ShouldMarkModifiersAsPrinted()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var orderLineId = Guid.NewGuid();
        var orderLine = OrderLine.Create(
            ticketId,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(10m));
        
        SetPrivateProperty(orderLine, "Id", orderLineId);
        SetPrivateProperty(orderLine, "ShouldPrintToKitchen", true);
        SetPrivateProperty(orderLine, "PrintedToKitchen", false);

        // Add a modifier that should print
        var modifier = OrderLineModifier.Create(
            orderLineId,
            Guid.NewGuid(),
            "Extra Cheese",
            ModifierType.Extra,
            1,
            new Money(2m),
            shouldPrintToKitchen: true);
        
        SetPrivateProperty(modifier, "PrintedToKitchen", false);
        
        // Use reflection to add modifier to order line's private collection
        var modifiersField = typeof(OrderLine).GetField("_modifiers", BindingFlags.Instance | BindingFlags.NonPublic);
        var modifiersList = modifiersField?.GetValue(orderLine) as System.Collections.IList;
        modifiersList?.Add(modifier);

        // Act
        orderLine.MarkPrintedToKitchen();

        // Assert
        orderLine.PrintedToKitchen.Should().BeTrue();
        modifier.PrintedToKitchen.Should().BeTrue();
    }

    [Fact]
    public void OrderLineModifier_MarkPrintedToKitchen_WithShouldPrint_ShouldMarkAsPrinted()
    {
        // Arrange
        var modifier = OrderLineModifier.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Extra Cheese",
            ModifierType.Extra,
            1,
            new Money(2m),
            shouldPrintToKitchen: true);
        
        SetPrivateProperty(modifier, "PrintedToKitchen", false);

        // Act
        modifier.MarkPrintedToKitchen();

        // Assert
        modifier.PrintedToKitchen.Should().BeTrue();
    }

    [Fact]
    public void OrderLineModifier_MarkPrintedToKitchen_WithoutShouldPrint_ShouldThrowException()
    {
        // Arrange
        var modifier = OrderLineModifier.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Extra Cheese",
            ModifierType.Extra,
            1,
            new Money(2m),
            shouldPrintToKitchen: false);

        // Act
        var act = () => modifier.MarkPrintedToKitchen();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not configured to print*");
    }
}

