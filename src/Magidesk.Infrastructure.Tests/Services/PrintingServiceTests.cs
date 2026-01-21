using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Printing;

namespace Magidesk.Infrastructure.Tests.Services;

public class PrintingServiceTests
{
    private class StubAuditEventRepository : IAuditEventRepository
    {
        public List<AuditEvent> Events { get; } = new();

        public Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
        {
            Events.Add(auditEvent);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<AuditEvent>> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuditEvent>> GetByEntityIdAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task PrintTicketReceiptAsync_ShouldLogAuditEvent()
    {
        // Arrange
        var stubRepo = new StubAuditEventRepository();
        var service = new MockReceiptPrintService(stubRepo, null);
        var ticket = Ticket.Create(1, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var userId = Guid.NewGuid();

        // Act
        await service.PrintTicketReceiptAsync(ticket, userId);

        // Assert
        Assert.Single(stubRepo.Events);
        var evt = stubRepo.Events[0];
        Assert.Equal(AuditEventType.Printed, evt.EventType);
        Assert.Equal("Ticket", evt.EntityType);
        Assert.Equal(ticket.Id, evt.EntityId);
        Assert.Equal(userId, evt.UserId);
    }

    [Fact]
    public async Task PrintPaymentReceiptAsync_ShouldLogAuditEvent()
    {
        // Arrange
        var stubRepo = new StubAuditEventRepository();
        var service = new MockReceiptPrintService(stubRepo, null);
        var ticket = Ticket.Create(1, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var payment = CashPayment.Create(ticket.Id, new Money(10m), new UserId(Guid.NewGuid()), Guid.NewGuid());
        // Note: CashPayment.Create signature: (Guid ticketId, Money amount, UserId createdBy, Guid? correlationId = null)
        // I used 'new CashPayment(...)' in previous attempt? 
        // Let's use generic Payment creation or specific one.
        // Wait, CashPayment constructor might be internal or protected?
        // Step 4788 showed `payment is CashPayment cashPayment`.
        // Let's use `CashPayment.Create`.
        // Or if Entity has public constructor.
        
        // I'll check Payment creation. Ticket.Create returns logic.
        // I'll stick to 'new CashPayment' if I can see it. 
        // I'll check CashPayment.cs if needed. 
        // Current code block uses CashPayment.Create.
        
        var userId = Guid.NewGuid();

        // Act
        await service.PrintPaymentReceiptAsync(payment, ticket, userId);

        // Assert
        Assert.Single(stubRepo.Events);
        var evt = stubRepo.Events[0];
        Assert.Equal(AuditEventType.Printed, evt.EventType);
        Assert.Equal("Payment", evt.EntityType);
        Assert.Equal(payment.Id, evt.EntityId);
    }
}
