using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Tests.TestDoubles;

public class InMemoryTableSessionRepository : ITableSessionRepository
{
    private readonly List<TableSession> _sessions = new();

    public Task<TableSession?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));
    }

    public Task<TableSession?> GetActiveSessionByTableIdAsync(Guid tableId)
    {
        return Task.FromResult(_sessions.FirstOrDefault(s => s.TableId == tableId && s.Status == TableSessionStatus.Active));
    }

    public Task<TableSession?> GetActiveSessionByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_sessions.FirstOrDefault(s => s.TicketId == ticketId && s.Status == TableSessionStatus.Active));
    }

    public Task<IEnumerable<TableSession>> GetActiveSessionsAsync()
    {
       return Task.FromResult(_sessions.Where(s => s.Status == TableSessionStatus.Active).AsEnumerable());
    }

    public Task<IEnumerable<TableSession>> GetSessionsByTableIdAsync(Guid tableId)
    {
        return Task.FromResult(_sessions.Where(s => s.TableId == tableId));
    }

    public Task<IEnumerable<TableSession>> GetSessionsByCustomerIdAsync(Guid customerId)
    {
        return Task.FromResult(_sessions.Where(s => s.CustomerId == customerId));
    }

    public Task<IEnumerable<TableSession>> GetSessionsByStatusAsync(TableSessionStatus status)
    {
        return Task.FromResult(_sessions.Where(s => s.Status == status));
    }

    public Task<TableSession> AddAsync(TableSession session)
    {
        _sessions.Add(session);
        return Task.FromResult(session);
    }

    public Task UpdateAsync(TableSession session)
    {
        // Object is reference type, so in-memory update is implicit
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        if (session != null)
        {
            _sessions.Remove(session);
        }
        return Task.CompletedTask;
    }
}
