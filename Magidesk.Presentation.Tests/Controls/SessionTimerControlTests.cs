using FsCheck;
using FsCheck.Xunit;
using Magidesk.Presentation.Controls;
using System;
using Xunit;

namespace Magidesk.Presentation.Tests.Controls;

/// <summary>
/// Property-based tests for SessionTimerControl.
/// Feature: ui-polish-optimization
/// </summary>
public class SessionTimerControlTests
{
    /// <summary>
    /// Property 2: Session Timer Accuracy
    /// Validates: Requirements 2.1, 2.2
    /// 
    /// For any active table session, the session timer should display elapsed time
    /// that matches the actual time difference between the current time and session
    /// start time (within 1 second tolerance).
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property2_SessionTimerAccuracy()
    {
        return Prop.ForAll(
            // Generate random session start times within the last 48 hours
            Arb.From(Gen.Choose(0, 48 * 60).Select(minutesAgo => 
                DateTime.Now.AddMinutes(-minutesAgo))),
            (sessionStartTime) =>
            {
                // Arrange
                var control = new SessionTimerControl
                {
                    SessionStartTime = sessionStartTime,
                    IsPaused = false
                };

                // Act - Simulate the timer update by calling the internal logic
                // The control calculates: ElapsedTime = DateTime.Now - SessionStartTime
                var expectedElapsedTime = DateTime.Now - sessionStartTime;
                var actualElapsedTime = control.ElapsedTime;

                // Assert - Elapsed time should match within 1 second tolerance
                var timeDifference = Math.Abs((expectedElapsedTime - actualElapsedTime).TotalSeconds);
                
                return timeDifference <= 1.0;
            }
        );
    }

    /// <summary>
    /// Unit test: Verify timer displays HH:MM:SS format for sessions under 24 hours.
    /// </summary>
    [Fact]
    public void FormattedTime_DisplaysHHMMSSFormat_ForSessionsUnder24Hours()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddHours(-5).AddMinutes(-30).AddSeconds(-45);
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = false
        };

        // Act
        var formattedTime = control.FormattedTime;

        // Assert
        // Should be in format "05:30:XX" (seconds may vary slightly)
        Assert.Matches(@"^\d{2}:\d{2}:\d{2}$", formattedTime);
        Assert.StartsWith("05:3", formattedTime); // Hours and first digit of minutes
    }

    /// <summary>
    /// Unit test: Verify timer displays days format for sessions exceeding 24 hours.
    /// </summary>
    [Fact]
    public void FormattedTime_DisplaysDaysFormat_ForSessionsOver24Hours()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddDays(-2).AddHours(-3).AddMinutes(-15);
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = false
        };

        // Act
        var formattedTime = control.FormattedTime;

        // Assert
        // Should be in format "2d HH:MM:SS"
        Assert.Matches(@"^\d+d \d{2}:\d{2}:\d{2}$", formattedTime);
        Assert.StartsWith("2d", formattedTime);
    }

    /// <summary>
    /// Unit test: Verify timer displays "PAUSED" when session is paused.
    /// </summary>
    [Fact]
    public void FormattedTime_DisplaysPaused_WhenSessionIsPaused()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddHours(-1);
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = true
        };

        // Act
        var formattedTime = control.FormattedTime;

        // Assert
        Assert.Equal("PAUSED", formattedTime);
    }

    /// <summary>
    /// Unit test: Verify background color changes at 50-minute threshold (yellow).
    /// </summary>
    [Fact]
    public void BackgroundBrush_ChangesToYellow_At50MinuteThreshold()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddMinutes(-52); // 52 minutes ago
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = false
        };

        // Act
        var brush = control.BackgroundBrush;

        // Assert
        // Should be yellow (between 50-55 minutes)
        Assert.NotNull(brush);
        var solidBrush = Assert.IsType<Microsoft.UI.Xaml.Media.SolidColorBrush>(brush);
        // Yellow color: RGB(202, 160, 0)
        Assert.Equal((byte)202, solidBrush.Color.R);
        Assert.Equal((byte)160, solidBrush.Color.G);
        Assert.Equal((byte)0, solidBrush.Color.B);
    }

    /// <summary>
    /// Unit test: Verify background color changes at 55-minute threshold (red).
    /// </summary>
    [Fact]
    public void BackgroundBrush_ChangesToRed_At55MinuteThreshold()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddMinutes(-57); // 57 minutes ago
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = false
        };

        // Act
        var brush = control.BackgroundBrush;

        // Assert
        // Should be red (>= 55 minutes)
        Assert.NotNull(brush);
        var solidBrush = Assert.IsType<Microsoft.UI.Xaml.Media.SolidColorBrush>(brush);
        // Red color: RGB(196, 43, 28)
        Assert.Equal((byte)196, solidBrush.Color.R);
        Assert.Equal((byte)43, solidBrush.Color.G);
        Assert.Equal((byte)28, solidBrush.Color.B);
    }

    /// <summary>
    /// Unit test: Verify background color is green for sessions under 50 minutes.
    /// </summary>
    [Fact]
    public void BackgroundBrush_IsGreen_ForSessionsUnder50Minutes()
    {
        // Arrange
        var sessionStartTime = DateTime.Now.AddMinutes(-30); // 30 minutes ago
        var control = new SessionTimerControl
        {
            SessionStartTime = sessionStartTime,
            IsPaused = false
        };

        // Act
        var brush = control.BackgroundBrush;

        // Assert
        // Should be green (< 50 minutes)
        Assert.NotNull(brush);
        var solidBrush = Assert.IsType<Microsoft.UI.Xaml.Media.SolidColorBrush>(brush);
        // Green color: RGB(16, 124, 16)
        Assert.Equal((byte)16, solidBrush.Color.R);
        Assert.Equal((byte)124, solidBrush.Color.G);
        Assert.Equal((byte)16, solidBrush.Color.B);
    }

    /// <summary>
    /// Unit test: Verify elapsed time is zero when no session start time is set.
    /// </summary>
    [Fact]
    public void ElapsedTime_IsZero_WhenNoSessionStartTime()
    {
        // Arrange
        var control = new SessionTimerControl
        {
            SessionStartTime = null,
            IsPaused = false
        };

        // Act
        var elapsedTime = control.ElapsedTime;

        // Assert
        Assert.Equal(TimeSpan.Zero, elapsedTime);
    }
}
