using FluentAssertions;
using Magidesk.Application.Commands;
using Magidesk.Application.Services;
using Magidesk.Application.Tests.TestDoubles;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Tests.Handlers;

public class AddOrderLineInstructionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldAddInstruction_AndWriteAuditEvent()
    {
        // Arrange
        var tickets = new InMemoryTicketRepository();
        var audits = new InMemoryAuditEventRepository();
        var handler = new AddOrderLineInstructionCommandHandler(tickets, audits);

        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await tickets.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(ticketNumber, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        // Add an order line to attach instruction to
        var orderLine = OrderLine.Create(ticket.Id, Guid.NewGuid(), "Burger", 1m, new Money(10m));
        ticket.AddOrderLine(orderLine);
        
        await tickets.AddAsync(ticket);

        var cmd = new AddOrderLineInstructionCommand
        {
            TicketId = ticket.Id,
            OrderLineId = orderLine.Id,
            Instruction = "NO ONIONS"
        };

        // Act
        // Act
        await handler.HandleAsync(cmd);

        // Assert
        var updatedTicket = await tickets.GetByIdAsync(ticket.Id);
        updatedTicket.Should().NotBeNull();
        
        var updatedLine = updatedTicket!.OrderLines.First(ol => ol.Id == orderLine.Id);
        updatedLine.Modifiers.Should().Contain(m => m.Name == "NO ONIONS");
        
        var instruction = updatedLine.Modifiers.First(m => m.Name == "NO ONIONS");
        instruction.ModifierType.Should().Be(Magidesk.Domain.Enumerations.ModifierType.InfoOnly);
        instruction.ShouldPrintToKitchen.Should().BeTrue();
        
        audits.Events.Should().HaveCount(1);
        audits.Events.First().EventType.Should().Be(Magidesk.Domain.Enumerations.AuditEventType.Modified);
        audits.Events.First().EntityType.Should().Be("Ticket");
        audits.Events.First().Description.Should().Contain("Added instruction");
    }
}
