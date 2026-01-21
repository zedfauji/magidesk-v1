using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Infrastructure.Data;
using Magidesk.Infrastructure.Repositories;

namespace Magidesk.Infrastructure.Tests.Repositories;

/// <summary>
/// Integration tests for ServerAssignmentRepository.
/// </summary>
[Collection("Database Tests")]
public class ServerAssignmentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ServerAssignmentRepository _repository;

    public ServerAssignmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ServerAssignmentRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateServerAssignment()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        var assignment = ServerAssignment.Create(sessionId, serverId, true, 100m);

        // Act
        await _repository.AddAsync(assignment);

        // Assert
        var retrieved = await _repository.GetByIdAsync(assignment.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(assignment.Id, retrieved.Id);
        Assert.Equal(sessionId, retrieved.SessionId);
        Assert.Equal(serverId, retrieved.ServerId);
        Assert.True(retrieved.IsPrimary);
        Assert.Equal(100m, retrieved.AllocationPercentage);
    }

    [Fact]
    public async Task GetActiveAssignmentsBySessionIdAsync_ShouldReturnOnlyActiveAssignments()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var server1Id = Guid.NewGuid();
        var server2Id = Guid.NewGuid();
        var server3Id = Guid.NewGuid();

        var assignment1 = ServerAssignment.Create(sessionId, server1Id, true, 60m);
        var assignment2 = ServerAssignment.Create(sessionId, server2Id, false, 40m);
        var assignment3 = ServerAssignment.Create(sessionId, server3Id, false, 30m);

        await _repository.AddAsync(assignment1);
        await _repository.AddAsync(assignment2);
        await _repository.AddAsync(assignment3);

        // Unassign one server
        assignment3.Unassign();
        await _repository.UpdateAsync(assignment3);

        // Act
        var activeAssignments = await _repository.GetActiveAssignmentsBySessionIdAsync(sessionId);

        // Assert
        Assert.Equal(2, activeAssignments.Count());
        Assert.Contains(activeAssignments, a => a.Id == assignment1.Id);
        Assert.Contains(activeAssignments, a => a.Id == assignment2.Id);
        Assert.DoesNotContain(activeAssignments, a => a.Id == assignment3.Id);
    }

    [Fact]
    public async Task GetPrimaryAssignmentBySessionIdAsync_ShouldReturnPrimaryServer()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var primaryServerId = Guid.NewGuid();
        var secondaryServerId = Guid.NewGuid();

        var primaryAssignment = ServerAssignment.Create(sessionId, primaryServerId, true, 70m);
        var secondaryAssignment = ServerAssignment.Create(sessionId, secondaryServerId, false, 30m);

        await _repository.AddAsync(primaryAssignment);
        await _repository.AddAsync(secondaryAssignment);

        // Act
        var primaryServer = await _repository.GetPrimaryAssignmentBySessionIdAsync(sessionId);

        // Assert
        Assert.NotNull(primaryServer);
        Assert.Equal(primaryAssignment.Id, primaryServer.Id);
        Assert.True(primaryServer.IsPrimary);
    }

    [Fact]
    public async Task GetAssignmentsByServerIdAsync_ShouldReturnAssignmentsInDateRange()
    {
        // Arrange
        var serverId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var assignment1 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);
        var assignment2 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);
        var assignment3 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);

        await _repository.AddAsync(assignment1);
        await _repository.AddAsync(assignment2);
        await _repository.AddAsync(assignment3);

        // Act
        var assignments = await _repository.GetAssignmentsByServerIdAsync(serverId, fromDate, toDate);

        // Assert
        Assert.Equal(3, assignments.Count());
        Assert.All(assignments, a => Assert.Equal(serverId, a.ServerId));
    }

    [Fact]
    public async Task GetActiveAssignmentsByServerIdAsync_ShouldReturnOnlyActiveAssignments()
    {
        // Arrange
        var serverId = Guid.NewGuid();
        
        var assignment1 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);
        var assignment2 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);
        var assignment3 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);

        await _repository.AddAsync(assignment1);
        await _repository.AddAsync(assignment2);
        await _repository.AddAsync(assignment3);

        // Unassign one
        assignment2.Unassign();
        await _repository.UpdateAsync(assignment2);

        // Act
        var activeAssignments = await _repository.GetActiveAssignmentsByServerIdAsync(serverId);

        // Assert
        Assert.Equal(2, activeAssignments.Count());
        Assert.Contains(activeAssignments, a => a.Id == assignment1.Id);
        Assert.Contains(activeAssignments, a => a.Id == assignment3.Id);
        Assert.DoesNotContain(activeAssignments, a => a.Id == assignment2.Id);
    }

    [Fact]
    public async Task IsServerAssignedToSessionAsync_ShouldReturnCorrectStatus()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var assignedServerId = Guid.NewGuid();
        var unassignedServerId = Guid.NewGuid();
        var notAssignedServerId = Guid.NewGuid();

        var activeAssignment = ServerAssignment.Create(sessionId, assignedServerId, true, 100m);
        var inactiveAssignment = ServerAssignment.Create(sessionId, unassignedServerId, false, 50m);

        await _repository.AddAsync(activeAssignment);
        await _repository.AddAsync(inactiveAssignment);

        // Unassign one server
        inactiveAssignment.Unassign();
        await _repository.UpdateAsync(inactiveAssignment);

        // Act & Assert
        Assert.True(await _repository.IsServerAssignedToSessionAsync(sessionId, assignedServerId));
        Assert.False(await _repository.IsServerAssignedToSessionAsync(sessionId, unassignedServerId));
        Assert.False(await _repository.IsServerAssignedToSessionAsync(sessionId, notAssignedServerId));
    }

    [Fact]
    public async Task GetServerPerformanceDataAsync_ShouldCalculateCorrectMetrics()
    {
        // Arrange
        var serverId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var assignment1 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 70m);
        var assignment2 = ServerAssignment.Create(Guid.NewGuid(), serverId, false, 30m);
        var assignment3 = ServerAssignment.Create(Guid.NewGuid(), serverId, true, 100m);

        await _repository.AddAsync(assignment1);
        await _repository.AddAsync(assignment2);
        await _repository.AddAsync(assignment3);

        // Unassign one to simulate completed session
        assignment1.Unassign();
        await _repository.UpdateAsync(assignment1);

        // Act
        var performanceData = await _repository.GetServerPerformanceDataAsync(serverId, fromDate, toDate);

        // Assert
        Assert.Equal(serverId, performanceData.ServerId);
        Assert.Equal(3, performanceData.TotalSessions);
        Assert.Equal(2, performanceData.PrimarySessions); // assignment1 and assignment3
        Assert.Equal(1, performanceData.SecondarySessions); // assignment2
        Assert.True(performanceData.TotalServiceTime >= TimeSpan.Zero);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var assignment = ServerAssignment.Create(Guid.NewGuid(), Guid.NewGuid(), false, 50m);
        await _repository.AddAsync(assignment);

        // Act
        assignment.SetPrimary(true);
        assignment.UpdateAllocationPercentage(75m);
        await _repository.UpdateAsync(assignment);

        // Assert
        var retrieved = await _repository.GetByIdAsync(assignment.Id);
        Assert.NotNull(retrieved);
        Assert.True(retrieved.IsPrimary);
        Assert.Equal(75m, retrieved.AllocationPercentage);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}