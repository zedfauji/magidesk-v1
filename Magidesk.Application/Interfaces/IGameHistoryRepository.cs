using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for game history data access.
/// </summary>
public interface IGameHistoryRepository : IRepository<GameHistory>
{
    /// <summary>
    /// Gets game history for a specific table within a date range.
    /// </summary>
    /// <param name="tableId">ID of the table</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>List of game history records</returns>
    Task<IEnumerable<GameHistory>> GetGameHistoryByTableIdAsync(Guid tableId, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets game history for a specific session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <returns>Game history record for the session</returns>
    Task<GameHistory?> GetGameHistoryBySessionIdAsync(Guid sessionId);

    /// <summary>
    /// Gets game history by game type within a date range.
    /// </summary>
    /// <param name="gameType">Type of game</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>List of game history records</returns>
    Task<IEnumerable<GameHistory>> GetGameHistoryByTypeAsync(GameType gameType, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets the most popular game types based on frequency.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <param name="limit">Maximum number of results</param>
    /// <returns>List of popular game types with counts</returns>
    Task<IEnumerable<GameTypePopularityData>> GetPopularGameTypesAsync(DateTime fromDate, DateTime toDate, int limit = 10);

    /// <summary>
    /// Gets table utilization analytics.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Table utilization data</returns>
    Task<IEnumerable<TableUtilizationData>> GetTableUtilizationAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets revenue analytics by table.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Revenue data by table</returns>
    Task<IEnumerable<TableRevenueData>> GetTableRevenueAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets peak time analysis data.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Peak time analysis data</returns>
    Task<IEnumerable<PeakTimeData>> GetPeakTimeAnalysisAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets customer preference data for frequent players.
    /// </summary>
    /// <param name="minimumSessions">Minimum number of sessions to be considered frequent</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Customer preference data</returns>
    Task<IEnumerable<CustomerPreferenceData>> GetCustomerPreferencesAsync(int minimumSessions, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets average session duration by game type.
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Average duration data by game type</returns>
    Task<IEnumerable<GameTypeDurationData>> GetAverageSessionDurationByGameTypeAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// Game type popularity data for analytics.
/// </summary>
public record GameTypePopularityData(
    GameType GameType,
    int SessionCount,
    TimeSpan TotalDuration,
    Money TotalRevenue,
    decimal AveragePlayerCount
);

/// <summary>
/// Table utilization data for analytics.
/// </summary>
public record TableUtilizationData(
    Guid TableId,
    string TableName,
    int SessionCount,
    TimeSpan TotalUsageTime,
    decimal UtilizationPercentage,
    Money TotalRevenue
);

/// <summary>
/// Table revenue data for analytics.
/// </summary>
public record TableRevenueData(
    Guid TableId,
    string TableName,
    Money TotalRevenue,
    Money AverageRevenuePerSession,
    Money RevenuePerHour,
    int SessionCount
);

/// <summary>
/// Peak time analysis data.
/// </summary>
public record PeakTimeData(
    int HourOfDay,
    DayOfWeek DayOfWeek,
    int SessionCount,
    decimal AverageUtilization,
    Money AverageRevenue
);

/// <summary>
/// Customer preference data for frequent players.
/// </summary>
public record CustomerPreferenceData(
    Guid CustomerId,
    string CustomerName,
    GameType PreferredGameType,
    int TotalSessions,
    TimeSpan AverageSessionDuration,
    Money TotalSpent,
    DateTime LastVisit
);

/// <summary>
/// Game type duration data for analytics.
/// </summary>
public record GameTypeDurationData(
    GameType GameType,
    TimeSpan AverageDuration,
    TimeSpan MinDuration,
    TimeSpan MaxDuration,
    int SessionCount
);