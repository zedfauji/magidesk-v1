using System;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Property-based tests for session control operations.
/// Feature: table-game-management, Property 4: Pause/Resume Time Accuracy
/// </summary>
public class SessionControlPropertyTests
{
    /// <summary>
    /// Property 4: Pause/Resume Time Accuracy
    /// For any session that is paused and resumed, the total billable time should equal 
    /// elapsed time minus all paused durations, and paused time should never be included in charges.
    /// Validates: Requirements 2.1, 2.2, 2.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property4_PauseResumeTimeAccuracy_ForAnySession_BillableTimeExcludesPausedTime()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidGuestCountGenerator(),
            ValidPauseDurationGenerator(),
            (hourlyRate, guestCount, pauseDurationMinutes) =>
            {
                // Arrange - Create a new active session
                var tableId = Guid.NewGuid();
                var tableTypeId = Guid.NewGuid();
                var session = TableSession.Start(tableId, tableTypeId, hourlyRate, guestCount);
                
                // Simulate some elapsed time before pause
                var startTime = DateTime.UtcNow.AddMinutes(-60); // Session started 60 minutes ago
                SetSessionStartTime(session, startTime);
                
                var initialBillableTime = session.GetBillableTime();
                var initialTotalPausedDuration = session.TotalPausedDuration;
                
                // Act - Pause the session
                var pauseTime = DateTime.UtcNow.AddMinutes(-pauseDurationMinutes);
                session.Pause();
                SetSessionPauseTime(session, pauseTime);
                
                var pausedBillableTime = session.GetBillableTime();
                
                // Resume the session
                session.Resume();
                
                var finalBillableTime = session.GetBillableTime();
                var finalTotalPausedDuration = session.TotalPausedDuration;
                
                // Assert properties
                var pauseDurationAdded = finalTotalPausedDuration > initialTotalPausedDuration;
                var billableTimeConsistent = finalBillableTime >= pausedBillableTime;
                var pausedTimeExcluded = finalTotalPausedDuration.TotalMinutes >= pauseDurationMinutes - 1; // Allow 1 minute tolerance
                
                return pauseDurationAdded && billableTimeConsistent && pausedTimeExcluded;
            });
    }

    /// <summary>
    /// Property 4: Multiple Pause/Resume Cycles
    /// For any session with multiple pause/resume cycles, the total paused duration 
    /// should accumulate correctly and be excluded from billable time.
    /// Validates: Requirements 2.1, 2.2, 2.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property4_MultiplePauseResumeCycles_ForAnySession_AccumulatesPausedTimeCorrectly()
    {
        return Prop.ForAll(
            ValidHourlyRateGenerator(),
            ValidGuestCountGenerator(),
            ValidCycleCountGenerator(),
            (hourlyRate, guestCount, cycleCount) =>
            {
                // Arrange - Create a new active session
                var tableId = Guid.NewGuid();
                var tableTypeId = Guid.NewGuid();
                var session = TableSession.Start(tableId, tableTypeId, hourlyRate, guestCount);
                
                var initialTotalPausedDuration = session.TotalPausedDuration;
                
                // Act - Perform multiple pause/resume cycles
                for (int i = 0; i < cycleCount; i++)
                {
                    // Pause
                    session.Pause();
                    var pauseTime = DateTime.UtcNow.AddMinutes(-(10 * (i + 1))); // Each pause is 10 minutes
                    SetSessionPauseTime(session, pauseTime);
                    
                    // Resume
                    session.Resume();
                }
                
                var finalTotalPausedDuration = session.TotalPausedDuration;
                
                // Assert properties
                var pausedDurationIncreased = finalTotalPausedDuration > initialTotalPausedDuration;
                var sessionIsActive = session.Status == TableSessionStatus.Active;
                var pausedTimeAccumulated = finalTotalPausedDuration.TotalMinutes >= (cycleCount * 9); // Allow tolerance
                
                return pausedDurationIncreased && sessionIsActive && pausedTimeAccumulated;
            });
    }

    /// <summary>
    /// Property 4: Paused Time Never Included in Charges
    /// For any session with paused time, the billable duration should always be less than 
    /// or equal to the total elapsed time, with the difference being the paused duration.
    /// Validates: Requirements 2.1, 2.2, 2.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property4_PausedTimeNeverIncludedInCharges_ForAnySession_BillableTimeLessThanElapsed()
    {
        return Prop.ForAll(
            Arb.From(Gen.Zip(ValidHourlyRateGenerator().Generator, ValidGuestCountGenerator().Generator)),
            ValidElapsedMinutesGenerator(),
            ValidPauseDurationGenerator(),
            (config, elapsedMinutes, pausedMinutes) =>
            {
                var (hourlyRate, guestCount) = config;
                // Ensure paused time is less than elapsed time
                var validPausedMinutes = Math.Min(pausedMinutes, elapsedMinutes - 1);
                if (validPausedMinutes <= 0) return true; // Skip invalid cases
                
                // Arrange - Create a session with specific timing
                var tableId = Guid.NewGuid();
                var tableTypeId = Guid.NewGuid();
                var session = TableSession.Start(tableId, tableTypeId, hourlyRate, guestCount);
                
                var startTime = DateTime.UtcNow.AddMinutes(-elapsedMinutes);
                SetSessionStartTime(session, startTime);
                
                // Pause and resume to accumulate paused time
                session.Pause();
                var pauseTime = DateTime.UtcNow.AddMinutes(-validPausedMinutes);
                SetSessionPauseTime(session, pauseTime);
                session.Resume();
                
                // Calculate times
                var totalElapsedTime = DateTime.UtcNow - startTime;
                var billableTime = session.GetBillableTime();
                var totalPausedDuration = session.TotalPausedDuration;
                
                // Assert properties
                var billableTimeLessOrEqualElapsed = billableTime <= totalElapsedTime;
                var pausedTimePositive = totalPausedDuration > TimeSpan.Zero;
                var timeDifferenceReasonable = Math.Abs((totalElapsedTime - billableTime - totalPausedDuration).TotalMinutes) <= 2; // 2-minute tolerance
                
                return billableTimeLessOrEqualElapsed && pausedTimePositive && timeDifferenceReasonable;
            });
    }

    #region Test Data Generators

    /// <summary>
    /// Generator for valid hourly rates (between $1 and $100).
    /// </summary>
    public static Arbitrary<decimal> ValidHourlyRateGenerator() =>
        Arb.From(Gen.Choose(100, 10000).Select(x => x / 100m)); // $1.00 to $100.00

    /// <summary>
    /// Generator for valid guest counts (1-20).
    /// </summary>
    public static Arbitrary<int> ValidGuestCountGenerator() =>
        Arb.From(Gen.Choose(1, 20));

    /// <summary>
    /// Generator for valid pause durations (1-120 minutes).
    /// </summary>
    public static Arbitrary<int> ValidPauseDurationGenerator() =>
        Arb.From(Gen.Choose(1, 120));

    /// <summary>
    /// Generator for valid cycle counts (1-5).
    /// </summary>
    public static Arbitrary<int> ValidCycleCountGenerator() =>
        Arb.From(Gen.Choose(1, 5));

    /// <summary>
    /// Generator for valid elapsed minutes (10-480 minutes = 10 minutes to 8 hours).
    /// </summary>
    public static Arbitrary<int> ValidElapsedMinutesGenerator() =>
        Arb.From(Gen.Choose(10, 480));

    #endregion

    /// <summary>
    /// Helper method to set session start time using reflection (for testing purposes)
    /// </summary>
    private static void SetSessionStartTime(TableSession session, DateTime startTime)
    {
        var startTimeProperty = typeof(TableSession).GetProperty("StartTime");
        startTimeProperty?.SetValue(session, startTime);
    }

    /// <summary>
    /// Helper method to set session pause time using reflection (for testing purposes)
    /// </summary>
    private static void SetSessionPauseTime(TableSession session, DateTime pauseTime)
    {
        var pausedAtProperty = typeof(TableSession).GetProperty("PausedAt");
        pausedAtProperty?.SetValue(session, pauseTime);
    }
}