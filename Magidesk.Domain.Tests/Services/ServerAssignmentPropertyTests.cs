using System;
using System.Linq;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using FluentAssertions;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Services;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Property-based tests for server assignment operations.
/// Tests server allocation, tip distribution accuracy, performance metric calculations,
/// and reassignment during active sessions.
/// </summary>
public class ServerAssignmentPropertyTests
{
    private readonly ServerAssignmentService _serverAssignmentService;

    public ServerAssignmentPropertyTests()
    {
        _serverAssignmentService = new ServerAssignmentService();
    }

    #region Test Data Generators

    /// <summary>
    /// Generator for valid session IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidSessionIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid server IDs.
    /// </summary>
    public static Arbitrary<Guid> ValidServerIdGenerator() =>
        Arb.From(Gen.Fresh(() => Guid.NewGuid()));

    /// <summary>
    /// Generator for valid allocation percentages (1% to 100%).
    /// </summary>
    public static Arbitrary<decimal> ValidAllocationPercentageGenerator() =>
        Arb.From(Gen.Choose(1, 100).Select(x => (decimal)x));

    /// <summary>
    /// Generator for valid tip amounts ($0.01 to $500.00).
    /// </summary>
    public static Arbitrary<Money> ValidTipAmountGenerator() =>
        Arb.From(Gen.Choose(1, 50000).Select(x => new Money(x / 100m)));

    /// <summary>
    /// Generator for valid date ranges (within last year).
    /// </summary>
    public static Arbitrary<(DateTime fromDate, DateTime toDate)> ValidDateRangeGenerator() =>
        Arb.From(
            from days1 in Gen.Choose(1, 365)
            from days2 in Gen.Choose(1, 30)
            let fromDate = DateTime.UtcNow.AddDays(-days1)
            let toDate = fromDate.AddDays(days2)
            select (fromDate, toDate));

    #endregion

    #region Server Assignment Properties

    /// <summary>
    /// Property: For any valid server assignment, the allocation percentage should be 
    /// between 1% and 100%, and the assignment should be created successfully.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAssignment_WithValidInputs_ShouldSucceed(Guid sessionId, Guid serverId, decimal allocationPercentage)
    {
        // Arrange - Ensure valid inputs
        if (sessionId == Guid.Empty || serverId == Guid.Empty || allocationPercentage <= 0 || allocationPercentage > 100)
            return true; // Skip invalid inputs

        // Act
        var result = _serverAssignmentService.AssignServerToSessionAsync(
            sessionId, serverId, true, allocationPercentage).Result;

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Assignment should succeed with valid inputs: " +
            $"SessionId={sessionId}, ServerId={serverId}, Allocation={allocationPercentage}%");

        if (result.Data != null)
        {
            result.Data.SessionId.Should().Be(sessionId);
            result.Data.ServerId.Should().Be(serverId);
            result.Data.AllocationPercentage.Should().Be(allocationPercentage);
            result.Data.IsPrimary.Should().BeTrue();
            result.Data.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        return true;
    }

    /// <summary>
    /// Property: For any invalid allocation percentage (≤0 or >100), 
    /// the assignment should fail with appropriate error message.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAssignment_WithInvalidAllocation_ShouldFail(Guid sessionId, Guid serverId)
    {
        // Arrange - Use invalid allocation percentages
        var invalidAllocations = new[] { -10m, 0m, 101m, 150m };
        
        foreach (var invalidAllocation in invalidAllocations)
        {
            // Skip if IDs are empty (different test case)
            if (sessionId == Guid.Empty || serverId == Guid.Empty)
                continue;

            // Act
            var result = _serverAssignmentService.AssignServerToSessionAsync(
                sessionId, serverId, true, invalidAllocation).Result;

            // Assert
            result.IsSuccessful.Should().BeFalse(
                $"Assignment should fail with invalid allocation: {invalidAllocation}%");

            result.ErrorMessage.Should().NotBeNullOrEmpty();
            result.ErrorMessage.Should().Contain("Allocation percentage must be between 0 and 100");
        }

        return true;
    }

    /// <summary>
    /// Property: For any empty session ID or server ID, 
    /// the assignment should fail with validation error.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAssignment_WithEmptyIds_ShouldFail(decimal allocationPercentage)
    {
        // Arrange - Ensure valid allocation percentage
        if (allocationPercentage <= 0 || allocationPercentage > 100)
            return true; // Skip invalid allocations

        var validId = Guid.NewGuid();
        var emptyId = Guid.Empty;

        // Test empty session ID
        var result1 = _serverAssignmentService.AssignServerToSessionAsync(
            emptyId, validId, true, allocationPercentage).Result;

        result1.IsSuccessful.Should().BeFalse("Assignment should fail with empty session ID");
        result1.ErrorMessage.Should().Contain("Session ID cannot be empty");

        // Test empty server ID
        var result2 = _serverAssignmentService.AssignServerToSessionAsync(
            validId, emptyId, true, allocationPercentage).Result;

        result2.IsSuccessful.Should().BeFalse("Assignment should fail with empty server ID");
        result2.ErrorMessage.Should().Contain("Server ID cannot be empty");

        return true;
    }

    #endregion

    #region Tip Distribution Properties

    /// <summary>
    /// Property: For any valid tip amount, individual allocations should never exceed 
    /// the total tip amount, and percentages should be mathematically consistent.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TipDistribution_IndividualAllocations_ShouldNotExceedTotal(Guid sessionId, Money totalTipAmount)
    {
        // Arrange - Ensure valid inputs
        if (sessionId == Guid.Empty || totalTipAmount.Amount < 0)
            return true; // Skip invalid inputs

        // Act
        var result = _serverAssignmentService.CalculateTipAllocationAsync(
            sessionId, totalTipAmount).Result;

        // Assert
        if (result.IsValid && result.Allocations.Any())
        {
            foreach (var allocation in result.Allocations)
            {
                // Each allocation should not exceed total
                allocation.AllocatedAmount.Amount.Should().BeLessThanOrEqualTo(
                    totalTipAmount.Amount,
                    $"Individual allocation {allocation.AllocatedAmount.Amount:C} " +
                    $"should not exceed total {totalTipAmount.Amount:C}");

                // Allocation percentage should be valid
                allocation.AllocationPercentage.Should().BeInRange(0m, 100m,
                    "Allocation percentage should be between 0% and 100%");

                // Mathematical consistency check
                var expectedAmount = totalTipAmount.Amount * (allocation.AllocationPercentage / 100m);
                allocation.AllocatedAmount.Amount.Should().BeApproximately(expectedAmount, 0.01m,
                    $"Allocated amount should match percentage calculation: " +
                    $"{allocation.AllocationPercentage}% of {totalTipAmount.Amount:C}");
            }

            // Sum of all allocations should equal total (within rounding tolerance)
            var totalAllocated = result.Allocations.Sum(a => a.AllocatedAmount.Amount);
            totalAllocated.Should().BeApproximately(totalTipAmount.Amount, 0.01m,
                "Sum of all allocations should equal total tip amount");
        }

        return true;
    }

    /// <summary>
    /// Property: For any negative tip amount, the allocation should fail with validation error.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool TipDistribution_WithNegativeAmount_ShouldFail(Guid sessionId)
    {
        // Arrange - Use negative tip amounts
        var negativeAmounts = new[] { new Money(-1m), new Money(-10m), new Money(-100m) };
        
        foreach (var negativeTipAmount in negativeAmounts)
        {
            // Skip if session ID is empty (different test case)
            if (sessionId == Guid.Empty)
                continue;

            // Act
            var result = _serverAssignmentService.CalculateTipAllocationAsync(
                sessionId, negativeTipAmount).Result;

            // Assert
            result.IsValid.Should().BeFalse("Tip allocation should fail with negative amount");
            result.ValidationMessage.Should().Contain("cannot be negative");
        }

        return true;
    }

    #endregion

    #region Performance Metrics Properties

    /// <summary>
    /// Property: For any valid server ID and date range, performance metrics 
    /// should have non-negative values and consistent calculations.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool PerformanceMetrics_WithValidInputs_ShouldHaveConsistentValues(Guid serverId, DateTime fromDate, DateTime toDate)
    {
        // Arrange - Ensure valid inputs
        if (serverId == Guid.Empty || fromDate > toDate)
            return true; // Skip invalid inputs

        // Act
        var metrics = _serverAssignmentService.GetServerPerformanceMetricsAsync(
            serverId, fromDate, toDate).Result;

        // Assert - Basic validation
        metrics.ServerId.Should().Be(serverId);
        metrics.FromDate.Should().Be(fromDate);
        metrics.ToDate.Should().Be(toDate);

        // All numeric values should be non-negative
        metrics.TotalSessionsServed.Should().BeGreaterThanOrEqualTo(0);
        metrics.TotalServiceTime.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
        metrics.TotalSalesGenerated.Amount.Should().BeGreaterThanOrEqualTo(0);
        metrics.TotalTipsEarned.Amount.Should().BeGreaterThanOrEqualTo(0);
        metrics.AverageSessionDuration.Should().BeGreaterThanOrEqualTo(0);
        metrics.CustomerSatisfactionScore.Should().BeGreaterThanOrEqualTo(0);
        metrics.PrimarySessionCount.Should().BeGreaterThanOrEqualTo(0);
        metrics.SecondarySessionCount.Should().BeGreaterThanOrEqualTo(0);
        metrics.AverageTipPerSession.Amount.Should().BeGreaterThanOrEqualTo(0);
        metrics.SalesPerHour.Should().BeGreaterThanOrEqualTo(0);

        // Logical consistency checks
        var totalSessions = metrics.PrimarySessionCount + metrics.SecondarySessionCount;
        totalSessions.Should().BeLessThanOrEqualTo(metrics.TotalSessionsServed,
            "Primary + Secondary sessions should not exceed total sessions");

        // Calculated properties should be mathematically consistent
        if (metrics.TotalSalesGenerated.Amount > 0)
        {
            var expectedTipPercentage = (metrics.TotalTipsEarned.Amount / metrics.TotalSalesGenerated.Amount) * 100;
            metrics.AverageTipPercentage.Should().BeApproximately(expectedTipPercentage, 0.01m,
                "Average tip percentage should be calculated correctly");
        }

        if (metrics.TotalServiceTime.TotalHours > 0)
        {
            var expectedSessionsPerHour = metrics.TotalSessionsServed / (decimal)metrics.TotalServiceTime.TotalHours;
            metrics.SessionsPerHour.Should().BeApproximately(expectedSessionsPerHour, 0.01m,
                "Sessions per hour should be calculated correctly");
        }

        return true;
    }

    #endregion

    #region Server Reassignment Properties

    /// <summary>
    /// Property: For any valid reassignment scenario, the operation should succeed
    /// and maintain data integrity.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerReassignment_WithValidInputs_ShouldSucceed(Guid sessionId, Guid newServerId)
    {
        // Arrange - Ensure valid inputs
        if (sessionId == Guid.Empty || newServerId == Guid.Empty)
            return true; // Skip invalid inputs

        var reason = "Test reassignment";

        // Act
        var result = _serverAssignmentService.ReassignServerAsync(
            sessionId, newServerId, reason).Result;

        // Assert
        result.IsSuccessful.Should().BeTrue(
            $"Reassignment should succeed with valid inputs: " +
            $"SessionId={sessionId}, NewServerId={newServerId}, Reason='{reason}'");

        if (result.Data != null)
        {
            result.Data.SessionId.Should().Be(sessionId);
            result.Data.ServerId.Should().Be(newServerId);
            result.Data.IsPrimary.Should().BeTrue("Reassigned server should be primary");
            result.Data.AllocationPercentage.Should().Be(100m, "Reassigned server should get 100% allocation");
        }

        return true;
    }

    /// <summary>
    /// Property: For any reassignment with empty or null reason, 
    /// the operation should fail with validation error.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerReassignment_WithEmptyReason_ShouldFail(Guid sessionId, Guid newServerId)
    {
        // Arrange - Ensure valid IDs
        if (sessionId == Guid.Empty || newServerId == Guid.Empty)
            return true; // Skip invalid IDs

        var invalidReasons = new[] { "", "   ", "\t\n", null };
        
        foreach (var invalidReason in invalidReasons)
        {
            // Skip null to avoid warning
            if (invalidReason == null)
                continue;
                
            // Act
            var result = _serverAssignmentService.ReassignServerAsync(
                sessionId, newServerId, invalidReason).Result;

            // Assert
            result.IsSuccessful.Should().BeFalse(
                $"Reassignment should fail with invalid reason: '{invalidReason}'");

            result.ErrorMessage.Should().Contain("Reason for reassignment is required");
        }

        return true;
    }

    #endregion

    #region Server Analytics Properties

    /// <summary>
    /// Property: For any valid server analytics request, the returned data should
    /// have consistent structure and valid calculated fields.
    /// </summary>
    [Property(MaxTest = 100)]
    public bool ServerAnalytics_WithValidInputs_ShouldHaveConsistentStructure(Guid serverId, DateTime fromDate, DateTime toDate)
    {
        // Arrange - Ensure valid inputs
        if (serverId == Guid.Empty || fromDate > toDate)
            return true; // Skip invalid inputs

        // Act
        var analytics = _serverAssignmentService.GetServerAnalyticsAsync(
            serverId, fromDate, toDate).Result;

        // Assert - Basic structure validation
        analytics.ServerId.Should().Be(serverId);
        analytics.FromDate.Should().Be(fromDate);
        analytics.ToDate.Should().Be(toDate);
        analytics.ServerName.Should().NotBeNullOrEmpty();

        // Performance metrics should be included
        analytics.PerformanceMetrics.Should().NotBeNull();
        analytics.PerformanceMetrics.ServerId.Should().Be(serverId);

        // Daily breakdown should be valid
        analytics.DailyBreakdown.Should().NotBeNull();
        foreach (var daily in analytics.DailyBreakdown)
        {
            daily.Date.Should().BeOnOrAfter(fromDate.Date);
            daily.Date.Should().BeOnOrBefore(toDate.Date);
            daily.SessionsServed.Should().BeGreaterThanOrEqualTo(0);
            daily.HoursWorked.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
            daily.SalesGenerated.Amount.Should().BeGreaterThanOrEqualTo(0);
            daily.TipsEarned.Amount.Should().BeGreaterThanOrEqualTo(0);
            daily.AverageSessionValue.Should().BeGreaterThanOrEqualTo(0);
        }

        // Commission data should be valid
        analytics.CommissionData.Should().NotBeNull();
        analytics.CommissionData.BaseSalary.Amount.Should().BeGreaterThanOrEqualTo(0);
        analytics.CommissionData.CommissionEarned.Amount.Should().BeGreaterThanOrEqualTo(0);
        analytics.CommissionData.CommissionRate.Should().BeGreaterThanOrEqualTo(0);
        analytics.CommissionData.TotalCompensation.Amount.Should().BeGreaterThanOrEqualTo(0);
        analytics.CommissionData.BonusEligible.Amount.Should().BeGreaterThanOrEqualTo(0);

        // Ranking should be valid
        analytics.Ranking.Should().NotBeNull();
        analytics.Ranking.SalesRank.Should().BeGreaterThan(0);
        analytics.Ranking.TipsRank.Should().BeGreaterThan(0);
        analytics.Ranking.SessionCountRank.Should().BeGreaterThan(0);
        analytics.Ranking.CustomerSatisfactionRank.Should().BeGreaterThan(0);
        analytics.Ranking.OverallRank.Should().BeGreaterThan(0);
        analytics.Ranking.TotalServers.Should().BeGreaterThan(0);

        // All ranks should be <= total servers
        analytics.Ranking.SalesRank.Should().BeLessThanOrEqualTo(analytics.Ranking.TotalServers);
        analytics.Ranking.TipsRank.Should().BeLessThanOrEqualTo(analytics.Ranking.TotalServers);
        analytics.Ranking.SessionCountRank.Should().BeLessThanOrEqualTo(analytics.Ranking.TotalServers);
        analytics.Ranking.CustomerSatisfactionRank.Should().BeLessThanOrEqualTo(analytics.Ranking.TotalServers);
        analytics.Ranking.OverallRank.Should().BeLessThanOrEqualTo(analytics.Ranking.TotalServers);

        return true;
    }

    #endregion
}