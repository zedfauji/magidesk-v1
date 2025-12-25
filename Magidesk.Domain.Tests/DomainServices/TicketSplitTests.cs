using FluentAssertions;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Magidesk.Domain.Tests.DomainServices;

public class TicketSplitTests
{
    private readonly TaxDomainService _taxDomainService;
    private readonly TicketDomainService _ticketDomainService;

    public TicketSplitTests()
    {
        _taxDomainService = new TaxDomainService();
        _ticketDomainService = new TicketDomainService(_taxDomainService);
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property?.SetValue(obj, value);
    }

    [Fact]
    public void Ticket_CanSplit_WithOpenTicket_ShouldReturnTrue()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(100m));
        ticket.AddOrderLine(orderLine);

        // Act
        var canSplit = ticket.CanSplit();

        // Assert
        canSplit.Should().BeTrue();
    }

    [Fact]
    public void Ticket_CanSplit_WithClosedTicket_ShouldReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetPrivateProperty(ticket, "Status", TicketStatus.Closed);

        // Act
        var canSplit = ticket.CanSplit();

        // Assert
        canSplit.Should().BeFalse();
    }

    [Fact]
    public void TicketDomainService_CanSplitTicket_WithOpenTicket_ShouldReturnTrue()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(100m));
        ticket.AddOrderLine(orderLine);

        // Act
        var canSplit = _ticketDomainService.CanSplitTicket(ticket);

        // Assert
        canSplit.Should().BeTrue();
    }

    [Fact]
    public void TicketDomainService_CanSplitTicket_WithPaidTicket_ShouldReturnFalse()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticket = Ticket.Create(1, userId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        SetPrivateProperty(ticket, "Status", TicketStatus.Paid);

        // Act
        var canSplit = _ticketDomainService.CanSplitTicket(ticket);

        // Assert
        canSplit.Should().BeFalse();
    }
}

