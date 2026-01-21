using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Unit tests for ServerAssignmentService to verify basic functionality.
/// </summary>
public class ServerAssignmentServiceTests
{
    private readonly ServerAssignmentService _service;

    public ServerAssignmentServiceTests()
    {
        _service = new ServerAssignmentService();
    }

    [Fact]
    public async Task AssignServerToSession_WithValidInputs_ShouldSucceed()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        var allocationPercentage = 100m;

        // Act
        var result = await _service.AssignServerToSessionAsync(sessionId, serverId, true, allocationPercentage);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.SessionId.Should().Be(sessionId);
        result.Data.ServerId.Should().Be(serverId);
        result.Data.AllocationPercentage.Should().Be(allocationPercentage);
        result.Data.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task AssignServerToSession_WithInvalidAllocation_ShouldFail()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        var invalidAllocation = 150m;

        // Act
        var result = await _service.AssignServerToSessionAsync(sessionId, serverId, true, invalidAllocation);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Allocation percentage must be between 0 and 100");
    }

    [Fact]
    public async Task CalculateTipAllocation_WithValidInputs_ShouldSucceed()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var tipAmount = new Money(100m);

        // Act
        var result = await _service.CalculateTipAllocationAsync(sessionId, tipAmount);

        // Assert
        result.IsValid.Should().BeTrue();
        result.SessionId.Should().Be(sessionId);
        result.TotalTipAmount.Should().Be(tipAmount);
        result.Allocations.Should().HaveCount(1);
        result.Allocations.First().AllocatedAmount.Should().Be(tipAmount);
    }

    [Fact]
    public async Task GetServerPerformanceMetrics_WithValidInputs_ShouldReturnMetrics()
    {
        // Arrange
        var serverId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow.AddDays(-30);
        var toDate = DateTime.UtcNow;

        // Act
        var result = await _service.GetServerPerformanceMetricsAsync(serverId, fromDate, toDate);

        // Assert
        result.Should().NotBeNull();
        result.ServerId.Should().Be(serverId);
        result.FromDate.Should().Be(fromDate);
        result.ToDate.Should().Be(toDate);
        result.ServerName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ReassignServer_WithValidInputs_ShouldSucceed()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var newServerId = Guid.NewGuid();
        var reason = "Server change requested";

        // Act
        var result = await _service.ReassignServerAsync(sessionId, newServerId, reason);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.SessionId.Should().Be(sessionId);
        result.Data.ServerId.Should().Be(newServerId);
        result.Data.IsPrimary.Should().BeTrue();
        result.Data.AllocationPercentage.Should().Be(100m);
    }
}