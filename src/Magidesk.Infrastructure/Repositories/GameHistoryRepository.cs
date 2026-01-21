using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;
using Magidesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Magidesk.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for GameHistory entity.
/// </summary>
public class GameHistoryRepository : EfRepository<GameHistory>, IGameHistoryRepository
{
    public GameHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<GameHistory>> GetGameHistoryByTableIdAsync(Guid tableId, DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.Set<GameHistory>()
            .Where(g => g.TableId == tableId && 
                       g.StartTime >= fromDate && 
                       g.StartTime <= toDate)
            .OrderByDescending(g => g.StartTime)
            .ToListAsync();
    }

    public async Task<GameHistory?> GetGameHistoryBySessionIdAsync(Guid sessionId)
    {
        return await _dbContext.Set<GameHistory>()
            .FirstOrDefaultAsync(g => g.SessionId == sessionId);
    }

    public async Task<IEnumerable<GameHistory>> GetGameHistoryByTypeAsync(GameType gameType, DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.Set<GameHistory>()
            .Where(g => g.GameType == gameType && 
                       g.StartTime >= fromDate && 
                       g.StartTime <= toDate)
            .OrderByDescending(g => g.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<GameTypePopularityData>> GetPopularGameTypesAsync(DateTime fromDate, DateTime toDate, int limit = 10)
    {
        var popularityData = await _dbContext.Set<GameHistory>()
            .Where(g => g.StartTime >= fromDate && g.StartTime <= toDate)
            .GroupBy(g => g.GameType)
            .Select(group => new GameTypePopularityData(
                group.Key,
                group.Count(),
                TimeSpan.FromTicks(group.Sum(g => g.Duration.Ticks)),
                new Money(group.Sum(g => g.TotalCharge.Amount), "USD"),
                (decimal)group.Average(g => g.PlayerCount)
            ))
            .OrderByDescending(data => data.SessionCount)
            .Take(limit)
            .ToListAsync();

        return popularityData;
    }

    public async Task<IEnumerable<TableUtilizationData>> GetTableUtilizationAsync(DateTime fromDate, DateTime toDate)
    {
        var utilizationData = await _dbContext.Set<GameHistory>()
            .Where(g => g.StartTime >= fromDate && g.StartTime <= toDate)
            .Join(_dbContext.Tables,
                  g => g.TableId,
                  t => t.Id,
                  (g, t) => new { GameHistory = g, Table = t })
            .GroupBy(joined => new { joined.Table.Id, joined.Table.TableNumber })
            .Select(group => new TableUtilizationData(
                group.Key.Id,
                group.Key.TableNumber.ToString(),
                group.Count(),
                TimeSpan.FromTicks(group.Sum(x => x.GameHistory.Duration.Ticks)),
                0.0m, // Would be calculated based on total available time
                new Money(group.Sum(x => x.GameHistory.TotalCharge.Amount), "USD")
            ))
            .OrderByDescending(data => data.SessionCount)
            .ToListAsync();

        return utilizationData;
    }

    public async Task<IEnumerable<TableRevenueData>> GetTableRevenueAsync(DateTime fromDate, DateTime toDate)
    {
        var revenueData = await _dbContext.Set<GameHistory>()
            .Where(g => g.StartTime >= fromDate && g.StartTime <= toDate)
            .Join(_dbContext.Tables,
                  g => g.TableId,
                  t => t.Id,
                  (g, t) => new { GameHistory = g, Table = t })
            .GroupBy(joined => new { joined.Table.Id, joined.Table.TableNumber })
            .Select(group => new
            {
                TableId = group.Key.Id,
                TableName = group.Key.TableNumber.ToString(),
                TotalRevenue = group.Sum(x => x.GameHistory.TotalCharge.Amount),
                SessionCount = group.Count(),
                TotalHours = group.Sum(x => x.GameHistory.Duration.TotalHours)
            })
            .ToListAsync();

        return revenueData.Select(data => new TableRevenueData(
            data.TableId,
            data.TableName,
            new Money(data.TotalRevenue, "USD"),
            new Money(data.SessionCount > 0 ? data.TotalRevenue / data.SessionCount : 0, "USD"),
            new Money(data.TotalHours > 0 ? data.TotalRevenue / (decimal)data.TotalHours : 0, "USD"),
            data.SessionCount
        ));
    }

    public async Task<IEnumerable<PeakTimeData>> GetPeakTimeAnalysisAsync(DateTime fromDate, DateTime toDate)
    {
        var peakTimeData = await _dbContext.Set<GameHistory>()
            .Where(g => g.StartTime >= fromDate && g.StartTime <= toDate)
            .GroupBy(g => new { 
                HourOfDay = g.StartTime.Hour, 
                DayOfWeek = g.StartTime.DayOfWeek 
            })
            .Select(group => new PeakTimeData(
                group.Key.HourOfDay,
                group.Key.DayOfWeek,
                group.Count(),
                0.0m, // Would be calculated based on capacity
                new Money(group.Average(g => g.TotalCharge.Amount), "USD")
            ))
            .OrderByDescending(data => data.SessionCount)
            .ToListAsync();

        return peakTimeData;
    }

    public async Task<IEnumerable<CustomerPreferenceData>> GetCustomerPreferencesAsync(int minimumSessions, DateTime fromDate, DateTime toDate)
    {
        // This would require joining with Customer data and TableSession data
        // For now, returning empty collection as customer tracking isn't fully implemented
        return new List<CustomerPreferenceData>();
    }

    public async Task<IEnumerable<GameTypeDurationData>> GetAverageSessionDurationByGameTypeAsync(DateTime fromDate, DateTime toDate)
    {
        var durationData = await _dbContext.Set<GameHistory>()
            .Where(g => g.StartTime >= fromDate && g.StartTime <= toDate)
            .GroupBy(g => g.GameType)
            .Select(group => new
            {
                GameType = group.Key,
                AverageTicks = group.Average(g => g.Duration.Ticks),
                MinTicks = group.Min(g => g.Duration.Ticks),
                MaxTicks = group.Max(g => g.Duration.Ticks),
                SessionCount = group.Count()
            })
            .ToListAsync();

        return durationData.Select(data => new GameTypeDurationData(
            data.GameType,
            TimeSpan.FromTicks((long)data.AverageTicks),
            TimeSpan.FromTicks(data.MinTicks),
            TimeSpan.FromTicks(data.MaxTicks),
            data.SessionCount
        ));
    }

    public override async Task<GameHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<GameHistory>()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<GameHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<GameHistory>()
            .OrderByDescending(g => g.StartTime)
            .ToListAsync(cancellationToken);
    }
}