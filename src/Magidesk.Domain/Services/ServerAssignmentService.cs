using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Services;

/// <summary>
/// Domain service for managing server assignments to table sessions.
/// </summary>
public class ServerAssignmentService : IServerAssignmentService
{
    public async Task<ServerAssignmentResult> AssignServerToSessionAsync(
        Guid sessionId, 
        Guid serverId, 
        bool isPrimary = true, 
        decimal allocationPercentage = 100m)
    {
        try
        {
            // Validate inputs
            if (sessionId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Session ID cannot be empty");
            
            if (serverId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Server ID cannot be empty");

            if (allocationPercentage <= 0 || allocationPercentage > 100)
                return ServerAssignmentResult.ValidationError("Allocation percentage must be between 0 and 100");

            // Create the server assignment
            var assignment = ServerAssignment.Create(sessionId, serverId, isPrimary, allocationPercentage);

            var data = new ServerAssignmentData(
                assignment.Id,
                assignment.SessionId,
                assignment.ServerId,
                assignment.IsPrimary,
                assignment.AllocationPercentage,
                assignment.AssignedAt
            );

            return ServerAssignmentResult.Success(data);
        }
        catch (BusinessRuleViolationException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
    }

    public async Task<ServerAssignmentResult> ReassignServerAsync(
        Guid sessionId, 
        Guid newServerId, 
        string reason)
    {
        try
        {
            // Validate inputs
            if (sessionId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Session ID cannot be empty");
            
            if (newServerId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Server ID cannot be empty");

            if (string.IsNullOrWhiteSpace(reason))
                return ServerAssignmentResult.ValidationError("Reason for reassignment is required");

            // Note: In a full implementation, this would:
            // 1. Get existing assignments for the session
            // 2. Unassign current primary server
            // 3. Create new assignment for new server
            // 4. Log the reassignment with reason
            
            // For now, create a new assignment (repository layer would handle the logic)
            var assignment = ServerAssignment.Create(sessionId, newServerId, true, 100m);

            var data = new ServerAssignmentData(
                assignment.Id,
                assignment.SessionId,
                assignment.ServerId,
                assignment.IsPrimary,
                assignment.AllocationPercentage,
                assignment.AssignedAt
            );

            return ServerAssignmentResult.Success(data);
        }
        catch (BusinessRuleViolationException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
    }

    public async Task<ServerAssignmentResult> AddSecondaryServerAsync(
        Guid sessionId, 
        Guid serverId, 
        decimal allocationPercentage)
    {
        try
        {
            // Validate inputs
            if (sessionId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Session ID cannot be empty");
            
            if (serverId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Server ID cannot be empty");

            if (allocationPercentage <= 0 || allocationPercentage > 100)
                return ServerAssignmentResult.ValidationError("Allocation percentage must be between 0 and 100");

            // Create secondary server assignment
            var assignment = ServerAssignment.Create(sessionId, serverId, false, allocationPercentage);

            var data = new ServerAssignmentData(
                assignment.Id,
                assignment.SessionId,
                assignment.ServerId,
                assignment.IsPrimary,
                assignment.AllocationPercentage,
                assignment.AssignedAt
            );

            return ServerAssignmentResult.Success(data);
        }
        catch (BusinessRuleViolationException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ServerAssignmentResult.ValidationError(ex.Message);
        }
    }

    public async Task<ServerAssignmentResult> RemoveServerAssignmentAsync(
        Guid sessionId, 
        Guid serverId)
    {
        try
        {
            // Validate inputs
            if (sessionId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Session ID cannot be empty");
            
            if (serverId == Guid.Empty)
                return ServerAssignmentResult.ValidationError("Server ID cannot be empty");

            // Note: In a full implementation, this would:
            // 1. Find the assignment by sessionId and serverId
            // 2. Call Unassign() on the assignment
            // 3. Update the repository
            
            return ServerAssignmentResult.Success();
        }
        catch (System.InvalidOperationException ex)
        {
            return ServerAssignmentResult.InvalidOperation(ex.Message);
        }
    }

    public async Task<TipAllocationResult> CalculateTipAllocationAsync(
        Guid sessionId, 
        Money totalTipAmount)
    {
        try
        {
            // Validate inputs
            if (sessionId == Guid.Empty)
                return TipAllocationResult.ValidationError(sessionId, totalTipAmount, "Session ID cannot be empty");

            if (totalTipAmount.Amount < 0)
                return TipAllocationResult.ValidationError(sessionId, totalTipAmount, "Tip amount cannot be negative");

            // Note: In a full implementation, this would:
            // 1. Get all active server assignments for the session
            // 2. Validate that allocation percentages total 100%
            // 3. Calculate individual allocations
            // 4. Return the breakdown

            // For now, return a sample allocation (would be replaced with repository calls)
            var allocations = new List<ServerTipAllocation>
            {
                new ServerTipAllocation(
                    Guid.NewGuid(),
                    "Sample Server",
                    100m,
                    totalTipAmount,
                    true
                )
            };

            return TipAllocationResult.Success(sessionId, totalTipAmount, allocations);
        }
        catch (Exception ex)
        {
            return TipAllocationResult.ValidationError(sessionId, totalTipAmount, ex.Message);
        }
    }

    public async Task<ServerPerformanceMetrics> GetServerPerformanceMetricsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        // Validate inputs
        if (serverId == Guid.Empty)
            throw new ArgumentException("Server ID cannot be empty", nameof(serverId));

        if (fromDate > toDate)
            throw new ArgumentException("From date cannot be after to date", nameof(fromDate));

        // Note: In a full implementation, this would query the repository
        // and calculate metrics from actual session data
        
        return new ServerPerformanceMetrics(
            ServerId: serverId,
            ServerName: "Sample Server",
            FromDate: fromDate,
            ToDate: toDate,
            TotalSessionsServed: 0,
            TotalServiceTime: TimeSpan.Zero,
            TotalSalesGenerated: Money.Zero(),
            TotalTipsEarned: Money.Zero(),
            AverageSessionDuration: 0,
            CustomerSatisfactionScore: 0,
            PrimarySessionCount: 0,
            SecondarySessionCount: 0,
            AverageTipPerSession: Money.Zero(),
            SalesPerHour: 0
        );
    }

    public async Task<IEnumerable<ServerAssignment>> GetActiveServerAssignmentsAsync(Guid sessionId)
    {
        // Validate input
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

        // Note: In a full implementation, this would query the repository
        // for all active assignments for the session
        
        return new List<ServerAssignment>();
    }

    public async Task<bool> ValidateAllocationPercentagesAsync(Guid sessionId)
    {
        // Validate input
        if (sessionId == Guid.Empty)
            return false;

        // Note: In a full implementation, this would:
        // 1. Get all active server assignments for the session
        // 2. Sum up allocation percentages
        // 3. Return true if total equals 100%
        
        return true;
    }

    public async Task<ServerAnalytics> GetServerAnalyticsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        // Validate inputs
        if (serverId == Guid.Empty)
            throw new ArgumentException("Server ID cannot be empty", nameof(serverId));

        if (fromDate > toDate)
            throw new ArgumentException("From date cannot be after to date", nameof(fromDate));

        // Note: In a full implementation, this would calculate comprehensive analytics
        
        var performanceMetrics = await GetServerPerformanceMetricsAsync(serverId, fromDate, toDate);
        
        return new ServerAnalytics(
            ServerId: serverId,
            ServerName: "Sample Server",
            FromDate: fromDate,
            ToDate: toDate,
            PerformanceMetrics: performanceMetrics,
            DailyBreakdown: new List<DailyServerMetrics>(),
            CommissionData: new CommissionCalculation(
                BaseSalary: Money.Zero(),
                CommissionEarned: Money.Zero(),
                CommissionRate: 0,
                TotalCompensation: Money.Zero(),
                BonusEligible: Money.Zero()
            ),
            Ranking: new ServerRanking(
                SalesRank: 1,
                TipsRank: 1,
                SessionCountRank: 1,
                CustomerSatisfactionRank: 1,
                OverallRank: 1,
                TotalServers: 1
            )
        );
    }
}