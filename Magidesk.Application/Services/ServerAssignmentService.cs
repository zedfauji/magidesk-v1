using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Application service for server assignment operations.
/// Orchestrates domain services and repository operations.
/// </summary>
public class ServerAssignmentService
{
    private readonly IServerAssignmentService _domainService;
    private readonly IServerAssignmentRepository _assignmentRepository;
    private readonly ITableSessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    public ServerAssignmentService(
        IServerAssignmentService domainService,
        IServerAssignmentRepository assignmentRepository,
        ITableSessionRepository sessionRepository,
        IUserRepository userRepository)
    {
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Assigns a server to a table session during session start.
    /// </summary>
    public async Task<ServerAssignmentResult> AssignServerToSessionAsync(
        Guid sessionId, 
        Guid serverId, 
        bool isPrimary = true, 
        decimal allocationPercentage = 100m)
    {
        // Verify session exists
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return ServerAssignmentResult.NotFound("Session");
        }

        // Verify server exists
        var server = await _userRepository.GetByIdAsync(serverId);
        if (server == null)
        {
            return ServerAssignmentResult.NotFound("Server");
        }

        // Check if server is already assigned to this session
        var isAlreadyAssigned = await _assignmentRepository.IsServerAssignedToSessionAsync(sessionId, serverId);
        if (isAlreadyAssigned)
        {
            return ServerAssignmentResult.InvalidOperation("Server is already assigned to this session");
        }

        // If this is a primary assignment, check if there's already a primary server
        if (isPrimary)
        {
            var existingPrimary = await _assignmentRepository.GetPrimaryAssignmentBySessionIdAsync(sessionId);
            if (existingPrimary != null)
            {
                return ServerAssignmentResult.InvalidOperation("Session already has a primary server assigned");
            }
        }

        // Use domain service to create assignment
        var result = await _domainService.AssignServerToSessionAsync(sessionId, serverId, isPrimary, allocationPercentage);
        
        if (result.IsSuccessful && result.Data != null)
        {
            // Create and save the assignment entity
            var assignment = ServerAssignment.Create(sessionId, serverId, isPrimary, allocationPercentage);
            await _assignmentRepository.AddAsync(assignment);
        }

        return result;
    }

    /// <summary>
    /// Reassigns servers during an active session.
    /// </summary>
    public async Task<ServerAssignmentResult> ReassignServerAsync(
        Guid sessionId, 
        Guid newServerId, 
        string reason)
    {
        // Verify session exists and is active
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return ServerAssignmentResult.NotFound("Session");
        }

        // Verify new server exists
        var newServer = await _userRepository.GetByIdAsync(newServerId);
        if (newServer == null)
        {
            return ServerAssignmentResult.NotFound("Server");
        }

        // Get current primary assignment
        var currentPrimary = await _assignmentRepository.GetPrimaryAssignmentBySessionIdAsync(sessionId);
        if (currentPrimary == null)
        {
            return ServerAssignmentResult.InvalidOperation("No primary server currently assigned to session");
        }

        // Check if new server is already assigned
        var isAlreadyAssigned = await _assignmentRepository.IsServerAssignedToSessionAsync(sessionId, newServerId);
        if (isAlreadyAssigned)
        {
            return ServerAssignmentResult.InvalidOperation("New server is already assigned to this session");
        }

        // Unassign current primary server
        currentPrimary.Unassign();
        await _assignmentRepository.UpdateAsync(currentPrimary);

        // Use domain service to create new assignment
        var result = await _domainService.AssignServerToSessionAsync(sessionId, newServerId, true, 100m);
        
        if (result.IsSuccessful && result.Data != null)
        {
            // Create and save the new assignment
            var newAssignment = ServerAssignment.Create(sessionId, newServerId, true, 100m);
            await _assignmentRepository.AddAsync(newAssignment);
        }

        return result;
    }

    /// <summary>
    /// Adds a secondary server to an existing session.
    /// </summary>
    public async Task<ServerAssignmentResult> AddSecondaryServerAsync(
        Guid sessionId, 
        Guid serverId, 
        decimal allocationPercentage)
    {
        // Verify session exists
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return ServerAssignmentResult.NotFound("Session");
        }

        // Verify server exists
        var server = await _userRepository.GetByIdAsync(serverId);
        if (server == null)
        {
            return ServerAssignmentResult.NotFound("Server");
        }

        // Check if server is already assigned
        var isAlreadyAssigned = await _assignmentRepository.IsServerAssignedToSessionAsync(sessionId, serverId);
        if (isAlreadyAssigned)
        {
            return ServerAssignmentResult.InvalidOperation("Server is already assigned to this session");
        }

        // Get current assignments to validate total allocation
        var currentAssignments = await _assignmentRepository.GetActiveAssignmentsBySessionIdAsync(sessionId);
        var currentTotal = currentAssignments.Sum(a => a.AllocationPercentage);
        
        if (currentTotal + allocationPercentage > 100)
        {
            return ServerAssignmentResult.ValidationError(
                $"Total allocation would exceed 100%. Current: {currentTotal}%, Requested: {allocationPercentage}%");
        }

        // Use domain service to create assignment
        var result = await _domainService.AddSecondaryServerAsync(sessionId, serverId, allocationPercentage);
        
        if (result.IsSuccessful && result.Data != null)
        {
            // Create and save the assignment
            var assignment = ServerAssignment.Create(sessionId, serverId, false, allocationPercentage);
            await _assignmentRepository.AddAsync(assignment);
        }

        return result;
    }

    /// <summary>
    /// Removes a server assignment from a session.
    /// </summary>
    public async Task<ServerAssignmentResult> RemoveServerAssignmentAsync(
        Guid sessionId, 
        Guid serverId)
    {
        // Find the assignment
        var assignments = await _assignmentRepository.GetActiveAssignmentsBySessionIdAsync(sessionId);
        var assignment = assignments.FirstOrDefault(a => a.ServerId == serverId);
        
        if (assignment == null)
        {
            return ServerAssignmentResult.NotFound("Server assignment");
        }

        // Don't allow removing the last server
        if (assignments.Count() == 1)
        {
            return ServerAssignmentResult.InvalidOperation("Cannot remove the last server from a session");
        }

        // Unassign the server
        assignment.Unassign();
        await _assignmentRepository.UpdateAsync(assignment);

        return ServerAssignmentResult.Success();
    }

    /// <summary>
    /// Calculates tip allocation for all servers assigned to a session.
    /// </summary>
    public async Task<TipAllocationResult> CalculateTipAllocationAsync(
        Guid sessionId, 
        Money totalTipAmount)
    {
        // Get all active assignments for the session
        var assignments = await _assignmentRepository.GetActiveAssignmentsBySessionIdAsync(sessionId);
        
        if (!assignments.Any())
        {
            return TipAllocationResult.ValidationError(sessionId, totalTipAmount, "No servers assigned to session");
        }

        // Validate allocation percentages total 100%
        var totalAllocation = assignments.Sum(a => a.AllocationPercentage);
        if (Math.Abs(totalAllocation - 100m) > 0.01m)
        {
            return TipAllocationResult.ValidationError(
                sessionId, 
                totalTipAmount, 
                $"Server allocation percentages total {totalAllocation}%, must equal 100%");
        }

        // Calculate individual allocations
        var allocations = new List<ServerTipAllocation>();
        
        foreach (var assignment in assignments)
        {
            var server = await _userRepository.GetByIdAsync(assignment.ServerId);
            var serverName = server != null ? $"{server.FirstName} {server.LastName}" : "Unknown Server";
            
            var allocatedAmount = new Money(totalTipAmount.Amount * (assignment.AllocationPercentage / 100m));
            
            allocations.Add(new ServerTipAllocation(
                assignment.ServerId,
                serverName,
                assignment.AllocationPercentage,
                allocatedAmount,
                assignment.IsPrimary
            ));
        }

        return TipAllocationResult.Success(sessionId, totalTipAmount, allocations);
    }

    /// <summary>
    /// Gets performance metrics for a server over a specified period.
    /// </summary>
    public async Task<ServerPerformanceMetrics> GetServerPerformanceMetricsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        // Get server information
        var server = await _userRepository.GetByIdAsync(serverId);
        var serverName = server != null ? $"{server.FirstName} {server.LastName}" : "Unknown Server";

        // Get performance data from repository
        var performanceData = await _assignmentRepository.GetServerPerformanceDataAsync(serverId, fromDate, toDate);

        // Calculate metrics
        var averageSessionDuration = performanceData.TotalSessions > 0 
            ? (decimal)performanceData.TotalServiceTime.TotalMinutes / performanceData.TotalSessions 
            : 0;

        var salesPerHour = performanceData.TotalServiceTime.TotalHours > 0 
            ? (decimal)(performanceData.TotalSales / (decimal)performanceData.TotalServiceTime.TotalHours) 
            : 0;

        var averageTipPerSession = performanceData.TotalSessions > 0 
            ? new Money(performanceData.TotalTips / performanceData.TotalSessions) 
            : Money.Zero();

        return new ServerPerformanceMetrics(
            ServerId: serverId,
            ServerName: serverName,
            FromDate: fromDate,
            ToDate: toDate,
            TotalSessionsServed: performanceData.TotalSessions,
            TotalServiceTime: performanceData.TotalServiceTime,
            TotalSalesGenerated: new Money(performanceData.TotalSales),
            TotalTipsEarned: new Money(performanceData.TotalTips),
            AverageSessionDuration: averageSessionDuration,
            CustomerSatisfactionScore: 0, // Would be calculated from customer feedback data
            PrimarySessionCount: performanceData.PrimarySessions,
            SecondarySessionCount: performanceData.SecondarySessions,
            AverageTipPerSession: averageTipPerSession,
            SalesPerHour: salesPerHour
        );
    }

    /// <summary>
    /// Gets all active server assignments for a session.
    /// </summary>
    public async Task<IEnumerable<ServerAssignment>> GetActiveServerAssignmentsAsync(Guid sessionId)
    {
        return await _assignmentRepository.GetActiveAssignmentsBySessionIdAsync(sessionId);
    }

    /// <summary>
    /// Gets server-specific analytics and commission calculations.
    /// </summary>
    public async Task<ServerAnalytics> GetServerAnalyticsAsync(
        Guid serverId, 
        DateTime fromDate, 
        DateTime toDate)
    {
        // Get server information
        var server = await _userRepository.GetByIdAsync(serverId);
        var serverName = server != null ? $"{server.FirstName} {server.LastName}" : "Unknown Server";

        // Get performance metrics
        var performanceMetrics = await GetServerPerformanceMetricsAsync(serverId, fromDate, toDate);

        // Get performance data for daily breakdown
        var performanceData = await _assignmentRepository.GetServerPerformanceDataAsync(serverId, fromDate, toDate);
        
        var dailyBreakdown = performanceData.DailyData.Select(d => new DailyServerMetrics(
            Date: d.Date,
            SessionsServed: d.Sessions,
            HoursWorked: d.HoursWorked,
            SalesGenerated: new Money(d.Sales),
            TipsEarned: new Money(d.Tips),
            AverageSessionValue: d.Sessions > 0 ? d.Sales / d.Sessions : 0
        )).ToList();

        // Calculate commission (simplified calculation)
        var baseSalary = server?.HourlyRate ?? Money.Zero();
        var hoursWorked = (decimal)performanceMetrics.TotalServiceTime.TotalHours;
        var totalBasePay = new Money(baseSalary.Amount * hoursWorked);
        
        var commissionRate = 0.05m; // 5% commission on sales
        var commissionEarned = new Money(performanceMetrics.TotalSalesGenerated.Amount * commissionRate);
        var totalCompensation = new Money(totalBasePay.Amount + commissionEarned.Amount);

        var commissionData = new CommissionCalculation(
            BaseSalary: totalBasePay,
            CommissionEarned: commissionEarned,
            CommissionRate: commissionRate,
            TotalCompensation: totalCompensation,
            BonusEligible: Money.Zero() // Would be calculated based on performance thresholds
        );

        // Simplified ranking (would require comparison with other servers)
        var ranking = new ServerRanking(
            SalesRank: 1,
            TipsRank: 1,
            SessionCountRank: 1,
            CustomerSatisfactionRank: 1,
            OverallRank: 1,
            TotalServers: 1
        );

        return new ServerAnalytics(
            ServerId: serverId,
            ServerName: serverName,
            FromDate: fromDate,
            ToDate: toDate,
            PerformanceMetrics: performanceMetrics,
            DailyBreakdown: dailyBreakdown,
            CommissionData: commissionData,
            Ranking: ranking
        );
    }
}