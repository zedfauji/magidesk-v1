using System;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Tests.Services;

/// <summary>
/// Property-based tests for session state transitions.
/// Feature: table-game-management, Property 5: Session State Transition Validity
/// </summary>
public class SessionStateTransitionPropertyTests
{
    /// <summary>
    /// Property 5: Session State Transition Validity
    /// For any session state change, transitions must follow valid sequences (Active↔Paused, Active→Ended), 
    /// and invalid transitions should be rejected with appropriate error messages.
    /// Validates: Requirements 2.1, 2.2, 3.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property5_SessionStateTransitionValidity_ForAnySession_ValidTransitionsSucceed()
    {
        return Prop.ForAll<decimal, int>(
            (hourlyRate, guestCount) =>
            {
                // Constrain inputs to valid ranges
                var validHourlyRate = Math.Max(1.0m, Math.Min(100.0m, Math.Abs(hourlyRate)));
                var validGuestCount = Math.Max(1, Math.Min(20, Math.Abs(guestCount)));
                
                // Arrange - Create a new active session
                var tableId = Guid.NewGuid();
                var tableTypeId = Guid.NewGuid();
                var session = TableSession.Start(tableId, tableTypeId, validHourlyRate, validGuestCount);
                
                // Test valid transitions
                var initialStatus = session.Status;
                
                // Active → Paused (should succeed)
                session.Pause();
                var pausedStatus = session.Status;
                
                // Paused → Active (should succeed)
                session.Resume();
                var resumedStatus = session.Status;
                
                // Active → Ended (should succeed)
                session.End(new Money(10.0m));
                var endedStatus = session.Status;
                
                return initialStatus == TableSessionStatus.Active &&
                       pausedStatus == TableSessionStatus.Paused &&
                       resumedStatus == TableSessionStatus.Active &&
                       endedStatus == TableSessionStatus.Ended;
            });
    }

    /// <summary>
    /// Property 5: Invalid State Transitions
    /// For any session in an invalid state for a transition, the operation should throw an exception.
    /// Validates: Requirements 2.1, 2.2, 3.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property5_InvalidStateTransitions_ForAnySession_ShouldThrowExceptions()
    {
        return Prop.ForAll<decimal, int>(
            (hourlyRate, guestCount) =>
            {
                // Constrain inputs to valid ranges
                var validHourlyRate = Math.Max(1.0m, Math.Min(100.0m, Math.Abs(hourlyRate)));
                var validGuestCount = Math.Max(1, Math.Min(20, Math.Abs(guestCount)));
                
                // Test 1: Cannot pause an ended session
                var session1 = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                session1.End(new Money(10.0m));
                
                var pauseEndedThrows = false;
                try
                {
                    session1.Pause();
                }
                catch (InvalidOperationException)
                {
                    pauseEndedThrows = true;
                }
                
                // Test 2: Cannot resume an active session
                var session2 = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                
                var resumeActiveThrows = false;
                try
                {
                    session2.Resume();
                }
                catch (InvalidOperationException)
                {
                    resumeActiveThrows = true;
                }
                
                // Test 3: Cannot end a paused session directly
                var session3 = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                session3.Pause();
                
                var endPausedThrows = false;
                try
                {
                    session3.End(new Money(10.0m));
                }
                catch (InvalidOperationException)
                {
                    endPausedThrows = true;
                }
                
                return pauseEndedThrows && resumeActiveThrows && endPausedThrows;
            });
    }

    /// <summary>
    /// Property 5: Pause Already Paused Session
    /// For any session that is already paused, attempting to pause again should throw an exception.
    /// Validates: Requirements 2.1, 2.2
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property5_PauseAlreadyPausedSession_ForAnySession_ShouldThrowException()
    {
        return Prop.ForAll<decimal, int>(
            (hourlyRate, guestCount) =>
            {
                // Constrain inputs to valid ranges
                var validHourlyRate = Math.Max(1.0m, Math.Min(100.0m, Math.Abs(hourlyRate)));
                var validGuestCount = Math.Max(1, Math.Min(20, Math.Abs(guestCount)));
                
                // Arrange - Create and pause a session
                var session = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                session.Pause();
                
                // Act & Assert - Attempting to pause again should throw
                var throwsException = false;
                try
                {
                    session.Pause();
                }
                catch (InvalidOperationException ex)
                {
                    throwsException = ex.Message.Contains("already paused");
                }
                
                return throwsException;
            });
    }

    /// <summary>
    /// Property 5: Resume Non-Paused Session
    /// For any session that is not paused, attempting to resume should throw an exception.
    /// Validates: Requirements 2.2, 2.3
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property5_ResumeNonPausedSession_ForAnySession_ShouldThrowException()
    {
        return Prop.ForAll<decimal, int>(
            (hourlyRate, guestCount) =>
            {
                // Constrain inputs to valid ranges
                var validHourlyRate = Math.Max(1.0m, Math.Min(100.0m, Math.Abs(hourlyRate)));
                var validGuestCount = Math.Max(1, Math.Min(20, Math.Abs(guestCount)));
                
                // Test 1: Resume active session (should throw)
                var activeSession = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                
                var resumeActiveThrows = false;
                try
                {
                    activeSession.Resume();
                }
                catch (InvalidOperationException ex)
                {
                    resumeActiveThrows = ex.Message.Contains("Can only resume a paused session");
                }
                
                // Test 2: Resume ended session (should throw)
                var endedSession = TableSession.Start(Guid.NewGuid(), Guid.NewGuid(), validHourlyRate, validGuestCount);
                endedSession.End(new Money(10.0m));
                
                var resumeEndedThrows = false;
                try
                {
                    endedSession.Resume();
                }
                catch (InvalidOperationException ex)
                {
                    resumeEndedThrows = ex.Message.Contains("Can only resume a paused session");
                }
                
                return resumeActiveThrows && resumeEndedThrows;
            });
    }
}