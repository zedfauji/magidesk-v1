using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Repository interface for server assignment data access.
/// </summary>
public interface IServerAssignmentRepository : IRepository<ServerAssignment>
{
    /// <summary>
    /// Gets all active server assignments for a specific session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <returns>List of active server assignments</returns>
    Task<IEnumerable<ServerAssignment>> GetActiveAssignmentsBySessionIdAsync(Guid sessionId);

    /// <summary>
    /// Gets all server assignments for a specific server within a date range.
    /// </summary>
    /// <param name="serverId">ID of the server</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>List of server assignments</returns>
    Task<IEnumerable<ServerAssignment>> GetAssignmentsByServerIdAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate);

    /// <summary>
    /// Gets the primary server assignment for a session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <returns>Primary server assignment, if any</returns>
    Task<ServerAssignment?> GetPrimaryAssignmentBySessionIdAsync(Guid sessionId);

    /// <summary>
    /// Gets all assignments for a server that are currently active.
    /// </summary>
    /// <param name="serverId">ID of the server</param>
    /// <returns>List of active assignments</returns>
    Task<IEnumerable<ServerAssignment>> GetActiveAssignmentsByServerIdAsync(Guid serverId);

    /// <summary>
    /// Checks if a server is already assigned to a session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="serverId">ID of the server</param>
    /// <returns>True if server is assigned, false otherwise</returns>
    Task<bool> IsServerAssignedToSessionAsync(Guid sessionId, Guid serverId);

    /// <summary>
    /// Gets server performance data for analytics calculations.
    /// </summary>
    /// <param name="serverId">ID of the server</param>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Performance data for the server</returns>
    Task<ServerAssignmentPerformanceData> GetServerPerformanceDataAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate);
}

/// <summary>
/// Raw performance data for server analytics calculations.
/// </summary>
public record ServerAssignmentPerformanceData(
    Guid ServerId,
    int TotalSessions,
    TimeSpan TotalServiceTime,
    decimal TotalSales,
    decimal TotalTips,
    int PrimarySessions,
    int SecondarySessions,
    IEnumerable<DailyAssignmentPerformanceData> DailyData
);

/// <summary>
/// Daily performance data for a server.
/// </summary>
public record DailyAssignmentPerformanceData(
    DateTime Date,
    int Sessions,
    TimeSpan HoursWorked,
    decimal Sales,
    decimal Tips
);