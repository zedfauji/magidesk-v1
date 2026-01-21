using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for TicketRepository.
/// </summary>
[Collection("Database Tests")]
public class TicketRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TicketRepository _repository;

    public TicketRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres;")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new TicketRepository(_context);

        // Ensure a clean schema for each test run (model changes frequently)
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task AddAsync_ShouldCreateTicket()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await _repository.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(
            ticketNumber,
            userId,
            Guid.NewGuid(), // terminalId
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId

        // Act
        await _repository.AddAsync(ticket);

        // Assert
        var retrieved = await _repository.GetByIdAsync(ticket.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(ticket.Id, retrieved.Id);
        Assert.Equal(ticket.TicketNumber, retrieved.TicketNumber);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTicketWithOrderLines()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await _repository.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(
            ticketNumber,
            userId,
            Guid.NewGuid(), // terminalId
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId

        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(10.00m),
            taxRate: 0.10m);

        ticket.AddOrderLine(orderLine);
        await _repository.AddAsync(ticket);

        // Act
        var retrieved = await _repository.GetByIdAsync(ticket.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Single(retrieved.OrderLines);
        Assert.Equal(orderLine.Id, retrieved.OrderLines.First().Id);
    }

    [Fact]
    public async Task GetByTicketNumberAsync_ShouldReturnTicket()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await _repository.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(
            ticketNumber,
            userId,
            Guid.NewGuid(), // terminalId
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId

        await _repository.AddAsync(ticket);

        // Act
        var retrieved = await _repository.GetByTicketNumberAsync(ticketNumber);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(ticket.Id, retrieved.Id);
        Assert.Equal(ticketNumber, retrieved.TicketNumber);
    }

    [Fact]
    public async Task GetOpenTicketsAsync_ShouldReturnOnlyOpenTickets()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var terminalId = Guid.NewGuid();
        
        // Create an open ticket
        var openTicketNumber = await _repository.GetNextTicketNumberAsync();
        var openTicket = Ticket.Create(
            openTicketNumber,
            userId,
            terminalId,
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId
        await _repository.AddAsync(openTicket);

        // Create a closed ticket - build it step by step to avoid EF Core tracking issues
        var closedTicketNumber = await _repository.GetNextTicketNumberAsync();
        var closedTicket = Ticket.Create(
            closedTicketNumber,
            userId,
            terminalId,
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId
        
        // Add order line first
        var orderLine = OrderLine.Create(
            closedTicket.Id,
            Guid.NewGuid(),
            "Test Item",
            1m,
            new Money(10.00m));
        closedTicket.AddOrderLine(orderLine);
        
        // Save the ticket with order line
        await _repository.AddAsync(closedTicket);
        
        // Reload to get fresh state
        closedTicket = await _repository.GetByIdAsync(closedTicket.Id);
        Assert.NotNull(closedTicket);
        
        // Add payment equal to total to make it paid
        var payment = CashPayment.Create(
            closedTicket.Id,
            closedTicket.TotalAmount,
            userId,
            terminalId);
        closedTicket.AddPayment(payment);
        
        // Update to save payment
        await _repository.UpdateAsync(closedTicket);
        
        // Reload to get Paid status
        closedTicket = await _repository.GetByIdAsync(closedTicket.Id);
        Assert.NotNull(closedTicket);
        Assert.Equal(TicketStatus.Paid, closedTicket.Status);
        
        // Now close
        closedTicket.Close(userId);
        await _repository.UpdateAsync(closedTicket);

        // Act
        var openTickets = await _repository.GetOpenTicketsAsync();

        // Assert
        Assert.Contains(openTickets, t => t.Id == openTicket.Id);
        Assert.DoesNotContain(openTickets, t => t.Id == closedTicket.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTicket()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber = await _repository.GetNextTicketNumberAsync();
        var ticket = Ticket.Create(
            ticketNumber,
            userId,
            Guid.NewGuid(), // terminalId
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId

        await _repository.AddAsync(ticket);

        // Act - Add order line directly without reloading
        // The ticket from AddAsync already has the correct version
        var orderLine = OrderLine.Create(
            ticket.Id,
            Guid.NewGuid(),
            "New Item",
            1m,
            new Money(15.00m));
        ticket.AddOrderLine(orderLine);
        
        // Save the update
        await _repository.UpdateAsync(ticket);

        // Assert
        var retrieved = await _repository.GetByIdAsync(ticket.Id);
        Assert.NotNull(retrieved);
        Assert.Single(retrieved.OrderLines);
    }

    [Fact]
    public async Task GetNextTicketNumberAsync_ShouldReturnIncrementedNumber()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var ticketNumber1 = await _repository.GetNextTicketNumberAsync();
        var ticket1 = Ticket.Create(
            ticketNumber1,
            userId,
            Guid.NewGuid(), // terminalId
            Guid.NewGuid(), // shiftId
            Guid.NewGuid()); // orderTypeId
        await _repository.AddAsync(ticket1);

        // Act
        var nextNumber = await _repository.GetNextTicketNumberAsync();

        // Assert
        Assert.True(nextNumber > ticket1.TicketNumber);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

