using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class AddOrderLineFractionalModifierTests
{
    private readonly InMemoryTicketRepository _tickets;
    private readonly InMemoryMenuRepository _menu;
    private readonly InMemoryAuditEventRepository _audits;
    private readonly PriceCalculator _priceCalculator;
    private readonly AddOrderLineModifierCommandHandler _handler;

    public AddOrderLineFractionalModifierTests()
    {
        _tickets = new InMemoryTicketRepository();
        _menu = new InMemoryMenuRepository();
        _audits = new InMemoryAuditEventRepository();
        _priceCalculator = new PriceCalculator();
        _handler = new AddOrderLineModifierCommandHandler(_tickets, _menu, _audits, _priceCalculator);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateSumOfHalves_Correctly()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await _tickets.AddAsync(ticket);

        var menuItem = MenuItem.Create("Pizza", new Money(10.00m, "USD"), 1.0m);
        await _menu.AddAsync(menuItem);

        var orderLine = OrderLine.Create(ticket.Id, menuItem.Id, menuItem.Name, 1, menuItem.Price);
        ticket.AddOrderLine(orderLine);
        await _tickets.UpdateAsync(ticket);

        // Defined Modifiers
        var leftTopping = new FractionalModifier("Left Pepp", new Money(2.00m, "USD"), 1, ModifierPortion.Quarter1, PriceStrategy.SumOfHalves);
        leftTopping.SetSectionWisePrice(true);
        var rightTopping = new FractionalModifier("Right Mush", new Money(3.00m, "USD"), 2, ModifierPortion.Quarter2, PriceStrategy.SumOfHalves);
        rightTopping.SetSectionWisePrice(true);
        
        // Note: InMemoryMenuRepository stores MenuModifier. FractionalModifier inherits it.
        _menu.AddModifier(leftTopping);
        _menu.AddModifier(rightTopping);

        // Act 1: Add Left Half
        var cmd1 = new AddOrderLineModifierCommand
        {
            TicketId = ticket.Id,
            OrderLineId = orderLine.Id,
            ModifierId = leftTopping.Id,
            Quantity = 0.5m,
            SectionName = "Left",
        };
        
        await _handler.HandleAsync(cmd1);
        
        // Assert 1
        var line = ticket.OrderLines.First();
        var mod1 = line.Modifiers.Last();
        mod1.UnitPrice.Amount.Should().Be(1.00m); // $2 * 0.5
        mod1.BasePrice.Amount.Should().Be(2.00m);
        mod1.PortionValue.Should().Be(0.5m);
        mod1.PriceStrategy.Should().Be(PriceStrategy.SumOfHalves);
        
        // Act 2: Add Right Half (Sum Strategy)
        var cmd2 = new AddOrderLineModifierCommand
        {
            TicketId = ticket.Id,
            OrderLineId = orderLine.Id,
            ModifierId = rightTopping.Id,
            Quantity = 0.5m,
            SectionName = "Right"
        };
        
        await _handler.HandleAsync(cmd2);
        
        // Assert 2
        var mod2 = line.Modifiers.Last();
        mod2.UnitPrice.Amount.Should().Be(1.50m); // $3 * 0.5
        
        // Total Line Check
        // Base Item ($10) + Mod1 ($1) + Mod2 ($1.5) = $12.5.
        // Assuming CalculatePrice sums them all.
        line.Modifiers.Sum(m => m.TotalAmount.Amount).Should().Be(2.50m);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateHighestHalf_Correctly()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await _tickets.AddAsync(ticket);

        var menuItem = MenuItem.Create("Pizza", new Money(10.00m, "USD"), 1.0m);
        await _menu.AddAsync(menuItem);

        var orderLine = OrderLine.Create(ticket.Id, menuItem.Id, menuItem.Name, 1, menuItem.Price);
        ticket.AddOrderLine(orderLine);
        await _tickets.UpdateAsync(ticket);

        var lobster = new FractionalModifier("Lobster", new Money(10.00m, "USD"), 1, ModifierPortion.Quarter1, PriceStrategy.HighestHalf);
        lobster.SetSectionWisePrice(true);
        var cheese = new FractionalModifier("Cheese", new Money(2.00m, "USD"), 2, ModifierPortion.Quarter2, PriceStrategy.HighestHalf);
        cheese.SetSectionWisePrice(true);
        
        _menu.AddModifier(lobster);
        _menu.AddModifier(cheese);
        
        // Act 1: Add Lobster Half
        await _handler.HandleAsync(new AddOrderLineModifierCommand 
        { 
            TicketId = ticket.Id, 
            OrderLineId = orderLine.Id, 
            ModifierId = lobster.Id, 
            Quantity = 0.5m, 
            SectionName = "Left" 
        });
        
        // Assert 1
        var mod1 = ticket.OrderLines.First().Modifiers.Last();
        mod1.UnitPrice.Amount.Should().Be(5.00m); // $10 * 0.5
        
        // Act 2: Add Cheese Half
        await _handler.HandleAsync(new AddOrderLineModifierCommand
        { 
             TicketId = ticket.Id, 
             OrderLineId = orderLine.Id, 
             ModifierId = cheese.Id, 
             Quantity = 0.5m, 
             SectionName = "Right" 
        });
        
        // Assert 2
        var mod2 = ticket.OrderLines.First().Modifiers.Last();
        
        // Highest Half Logic: MaxBase = $10.00 (Lobster).
        // Lobster Portion (0.5) -> $5.00.
        // Cheese Portion (0.5) -> Should be MaxBase($10) * 0.5 = $5.00.
        
        mod2.UnitPrice.Amount.Should().Be(5.00m);
        ticket.OrderLines.First().Modifiers.First().UnitPrice.Amount.Should().Be(5.00m); // Lobster remains same
        
        // If we did "Cheese" first ($1) then "Lobster" ($5), Cheese should bump to $5.
    }
}
