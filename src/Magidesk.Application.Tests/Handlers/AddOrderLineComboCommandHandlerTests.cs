using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;
using Xunit;

namespace Magidesk.Application.Tests.Handlers;

public class AddOrderLineComboCommandHandlerTests
{
    private readonly InMemoryTicketRepository _tickets;
    private readonly InMemoryMenuRepository _menu;
    private readonly InMemoryAuditEventRepository _audits;
    private readonly PriceCalculator _priceCalculator;
    private readonly AddOrderLineComboCommandHandler _handler;

    public AddOrderLineComboCommandHandlerTests()
    {
        _tickets = new InMemoryTicketRepository();
        _menu = new InMemoryMenuRepository();
        _audits = new InMemoryAuditEventRepository();
        _priceCalculator = new PriceCalculator();
        _handler = new AddOrderLineComboCommandHandler(_tickets, _menu, _audits, _priceCalculator);
    }

    [Fact]
    public async Task Handle_ShouldAddComboItem_WithCorrectUpcharge()
    {
        // Arrange
        // 1. Create Combo Definition & Items
        var combo = new ComboDefinition("Burger Meal", new Money(10.00m, "USD"));
        var group = new ComboGroup(combo.Id, "Sides", 1);
        
        var fries = MenuItem.Create("Fries", new Money(2.00m, "USD")); // Ala carte price
        var rings = MenuItem.Create("Onion Rings", new Money(4.00m, "USD")); // Ala carte price

        // Combo Logic: Fries are included ($0 upcharge), Rings are +$1.
        var groupItemFries = new ComboGroupItem(group.Id, fries.Id, Money.Zero());
        var groupItemRings = new ComboGroupItem(group.Id, rings.Id, new Money(1.00m, "USD"));

        // Helper to add items to group (since we can't easily modify private list in test unless we use reflection or if we added a method)
        // Actually ComboDefinition and ComboGroup have private lists.
        // We need to rely on the Repo returning the structure, OR use reflection in test setup, OR add methods to Entity.
        // For 'ComboDefinition', we have 'AddGroup'.
        combo.AddGroup(group);
        
        // For 'ComboGroup', we don't have 'AddItem'. 
        // We should add 'AddItem' to ComboGroup or use reflection.
        // Let's assume for now we can add it via a hack or we update the entity.
        // Update: ComboGroup.cs has private _items. We need a public method.
        // BUT, since I can't modify entity easily right this second without a new tool call, 
        // I will use reflection to set it for the test.
        SetPrivateField(group, "_items", new List<ComboGroupItem> { groupItemFries, groupItemRings });

        // 2. Setup Menu
        var mealItem = MenuItem.Create("Burger Meal", new Money(10.00m, "USD"));
        mealItem.SetComboDefinition(combo.Id);

        await _menu.AddAsync(fries);
        await _menu.AddAsync(rings);
        await _menu.AddAsync(mealItem);
        _menu.AddComboDefinition(combo);

        // 3. Create Ticket & OrderLine
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        await _tickets.AddAsync(ticket);

        var orderLine = OrderLine.Create(ticket.Id, mealItem.Id, mealItem.Name, 1, mealItem.Price);
        ticket.AddOrderLine(orderLine);
        await _tickets.UpdateAsync(ticket);

        // Act: Select Onion Rings (+$1)
        var cmd = new AddOrderLineComboCommand
        {
            TicketId = ticket.Id,
            OrderLineId = orderLine.Id,
            Selections = new List<AddOrderLineComboCommand.ComboSelection>
            {
                new() 
                { 
                    ComboGroupId = group.Id, 
                    ComboGroupItemId = groupItemRings.Id,
                    Quantity = 1
                }
            }
        };

        await _handler.HandleAsync(cmd, default);

        // Assert
        var updatedLine = ticket.OrderLines.First();
        updatedLine.Modifiers.Should().HaveCount(1);
        var mod = updatedLine.Modifiers.First();
        
        mod.Name.Should().Be("Onion Rings");
        mod.UnitPrice.Amount.Should().Be(1.00m); // Upcharge
        mod.TotalAmount.Amount.Should().Be(1.00m);
        
        // Ticket Total
        // Line Base ($10) + Mod ($1) = $11.
        updatedLine.CalculatePrice();
        updatedLine.TotalAmount.Amount.Should().Be(11.00m);
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(target, value);
    }
}
