using System;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents the assignment of a server to a table session.
/// Tracks server allocation and tip distribution percentages.
/// </summary>
public class ServerAssignment
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public Guid ServerId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? UnassignedAt { get; private set; }
    public bool IsPrimary { get; private set; }
    public decimal AllocationPercentage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private ServerAssignment()
    {
    }

    /// <summary>
    /// Creates a new server assignment.
    /// </summary>
    /// <param name="sessionId">ID of the table session</param>
    /// <param name="serverId">ID of the server</param>
    /// <param name="isPrimary">Whether this is the primary server</param>
    /// <param name="allocationPercentage">Percentage of tips allocated to this server</param>
    /// <returns>New ServerAssignment instance</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    /// <exception cref="BusinessRuleViolationException">Thrown when allocation percentage is invalid</exception>
    public static ServerAssignment Create(
        Guid sessionId, 
        Guid serverId, 
        bool isPrimary = true, 
        decimal allocationPercentage = 100m)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
        }

        if (serverId == Guid.Empty)
        {
            throw new ArgumentException("Server ID cannot be empty.", nameof(serverId));
        }

        if (allocationPercentage <= 0 || allocationPercentage > 100)
        {
            throw new BusinessRuleViolationException("Allocation percentage must be between 0 and 100");
        }

        var now = DateTime.UtcNow;

        return new ServerAssignment
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            ServerId = serverId,
            AssignedAt = now,
            IsPrimary = isPrimary,
            AllocationPercentage = allocationPercentage,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Unassigns the server from the session.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Thrown when server is already unassigned</exception>
    public void Unassign()
    {
        if (UnassignedAt.HasValue)
        {
            throw new System.InvalidOperationException("Server is already unassigned from this session.");
        }

        UnassignedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the allocation percentage for this server.
    /// </summary>
    /// <param name="allocationPercentage">New allocation percentage</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when percentage is invalid</exception>
    /// <exception cref="System.InvalidOperationException">Thrown when server is unassigned</exception>
    public void UpdateAllocationPercentage(decimal allocationPercentage)
    {
        if (UnassignedAt.HasValue)
        {
            throw new System.InvalidOperationException("Cannot update allocation for an unassigned server.");
        }

        if (allocationPercentage <= 0 || allocationPercentage > 100)
        {
            throw new BusinessRuleViolationException("Allocation percentage must be between 0 and 100");
        }

        AllocationPercentage = allocationPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets whether this server is the primary server for the session.
    /// </summary>
    /// <param name="isPrimary">Whether this is the primary server</param>
    /// <exception cref="System.InvalidOperationException">Thrown when server is unassigned</exception>
    public void SetPrimary(bool isPrimary)
    {
        if (UnassignedAt.HasValue)
        {
            throw new System.InvalidOperationException("Cannot change primary status for an unassigned server.");
        }

        IsPrimary = isPrimary;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the server is currently assigned to the session.
    /// </summary>
    /// <returns>True if assigned, false if unassigned</returns>
    public bool IsCurrentlyAssigned()
    {
        return !UnassignedAt.HasValue;
    }

    /// <summary>
    /// Calculates the duration this server was assigned to the session.
    /// </summary>
    /// <returns>Assignment duration</returns>
    public TimeSpan GetAssignmentDuration()
    {
        var endTime = UnassignedAt ?? DateTime.UtcNow;
        return endTime - AssignedAt;
    }
}