using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Services;

/// <summary>
/// Service for managing server assignments to table sessions.
/// Handles server allocation, tip distribution, and performance tracking.
/// </summary>
public interface IServerAssignmentService
{
    /// <summary>
    /// Assigns a server to a table session during session start.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="serverId">ID of the server to assign</param>
    /// <param name="isPrimary">Whether this is the primary server</param>
    /// <param name="allocationPercentage">Percentage of tips allocated to this server</param>
    /// <returns>Result of the assignment operation</returns>
    Task<ServerAssignmentResult> AssignServerToSessionAsync(
        Guid sessionId, 
        Guid serverId, 
        bool isPrimary = true, 
        decimal allocationPercentage = 100m);

    /// <summary>
    /// Reassigns servers during an active session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="newServerId">ID of the new server</param>
    /// <param name="reason">Reason for reassignment</param>
    /// <returns>Result of the reassignment operation</returns>
    Task<ServerAssignmentResult> ReassignServerAsync(
        Guid sessionId, 
        Guid newServerId, 
        string reason);

    /// <summary>
    /// Adds a secondary server to an existing session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="serverId">ID of the secondary server</param>
    /// <param name="allocationPercentage">Percentage of tips allocated to this server</param>
    /// <returns>Result of the assignment operation</returns>
    Task<ServerAssignmentResult> AddSecondaryServerAsync(
        Guid sessionId, 
        Guid serverId, 
        decimal allocationPercentage);

    /// <summary>
    /// Removes a server assignment from a session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="serverId">ID of the server to remove</param>
    /// <returns>Result of the removal operation</returns>
    Task<ServerAssignmentResult> RemoveServerAssignmentAsync(
        Guid sessionId, 
        Guid serverId);

    /// <summary>
    /// Calculates tip allocation for all servers assigned to a session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="totalTipAmount">Total tip amount to distribute</param>
    /// <returns>Tip allocation breakdown by server</returns>
    Task<TipAllocationResult> CalculateTipAllocationAsync(
        Guid sessionId, 
        Money totalTipAmount);

    /// <summary>
    /// Gets performance metrics for a server over a specified period.
    /// </summary>
    /// <param name="serverId">ID of the server</param>
    /// <param name="fromDate">Start date for metrics calculation</param>
    /// <param name="toDate">End date for metrics calculation</param>
    /// <returns>Server performance metrics</returns>
    Task<ServerPerformanceMetrics> GetServerPerformanceMetricsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate);

    /// <summary>
    /// Gets all active server assignments for a session.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <returns>List of active server assignments</returns>
    Task<IEnumerable<ServerAssignment>> GetActiveServerAssignmentsAsync(Guid sessionId);

    /// <summary>
    /// Validates that server allocation percentages for a session total 100%.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <returns>Validation result</returns>
    Task<bool> ValidateAllocationPercentagesAsync(Guid sessionId);

    /// <summary>
    /// Gets server-specific analytics and commission calculations.
    /// </summary>
    /// <param name="serverId">ID of the server</param>
    /// <param name="fromDate">Start date for analytics</param>
    /// <param name="toDate">End date for analytics</param>
    /// <returns>Server analytics data</returns>
    Task<ServerAnalytics> GetServerAnalyticsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate);
}