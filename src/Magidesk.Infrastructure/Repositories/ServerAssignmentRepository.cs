using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ServerAssignment entity.
/// </summary>
public class ServerAssignmentRepository : EfRepository<ServerAssignment>, IServerAssignmentRepository
{
    public ServerAssignmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<ServerAssignment>> GetActiveAssignmentsBySessionIdAsync(Guid sessionId)
    {
        return await _dbContext.Set<ServerAssignment>()
            .Where(s => s.SessionId == sessionId && !s.UnassignedAt.HasValue)
            .OrderByDescending(s => s.IsPrimary)
            .ThenBy(s => s.AssignedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServerAssignment>> GetAssignmentsByServerIdAsync(Guid serverId, DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.Set<ServerAssignment>()
            .Where(s => s.ServerId == serverId && 
                       s.AssignedAt >= fromDate && 
                       s.AssignedAt <= toDate)
            .OrderByDescending(s => s.AssignedAt)
            .ToListAsync();
    }

    public async Task<ServerAssignment?> GetPrimaryAssignmentBySessionIdAsync(Guid sessionId)
    {
        return await _dbContext.Set<ServerAssignment>()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId && 
                                     s.IsPrimary && 
                                     !s.UnassignedAt.HasValue);
    }

    public async Task<IEnumerable<ServerAssignment>> GetActiveAssignmentsByServerIdAsync(Guid serverId)
    {
        return await _dbContext.Set<ServerAssignment>()
            .Where(s => s.ServerId == serverId && !s.UnassignedAt.HasValue)
            .OrderByDescending(s => s.AssignedAt)
            .ToListAsync();
    }

    public async Task<bool> IsServerAssignedToSessionAsync(Guid sessionId, Guid serverId)
    {
        return await _dbContext.Set<ServerAssignment>()
            .AnyAsync(s => s.SessionId == sessionId && 
                          s.ServerId == serverId && 
                          !s.UnassignedAt.HasValue);
    }

    public async Task<ServerAssignmentPerformanceData> GetServerPerformanceDataAsync(Guid serverId, DateTime fromDate, DateTime toDate)
    {
        var assignments = await _dbContext.Set<ServerAssignment>()
            .Where(s => s.ServerId == serverId && 
                       s.AssignedAt >= fromDate && 
                       s.AssignedAt <= toDate)
            .ToListAsync();

        var totalSessions = assignments.Count;
        var totalServiceTime = assignments.Sum(a => a.GetAssignmentDuration().Ticks);
        var primarySessions = assignments.Count(a => a.IsPrimary);
        var secondarySessions = totalSessions - primarySessions;

        // In a real implementation, this would join with TableSession and Payment data
        // to calculate actual sales and tips
        var totalSales = 0m;
        var totalTips = 0m;

        // Group by date for daily data
        var dailyData = assignments
            .GroupBy(a => a.AssignedAt.Date)
            .Select(group => new DailyAssignmentPerformanceData(
                group.Key,
                group.Count(),
                TimeSpan.FromTicks(group.Sum(a => a.GetAssignmentDuration().Ticks)),
                0m, // Would be calculated from actual sales data
                0m  // Would be calculated from actual tips data
            ))
            .OrderBy(d => d.Date);

        return new ServerAssignmentPerformanceData(
            serverId,
            totalSessions,
            TimeSpan.FromTicks(totalServiceTime),
            totalSales,
            totalTips,
            primarySessions,
            secondarySessions,
            dailyData
        );
    }

    public override async Task<ServerAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ServerAssignment>()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<ServerAssignment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ServerAssignment>()
            .OrderByDescending(s => s.AssignedAt)
            .ToListAsync(cancellationToken);
    }
}