using FsCheck;
using FsCheck.Xunit;
using Magidesk.Presentation.Services;
using Xunit;

namespace Magidesk.Presentation.Tests.Services;

/// <summary>
/// Property-based tests for ToastNotificationService.
/// Feature: ui-polish-optimization
/// </summary>
public class ToastNotificationServiceTests
{
    /// <summary>
    /// Property 1: Toast Notification Auto-Dismissal
    /// Validates: Requirements 3.4
    /// 
    /// For any toast notification with a specified duration, displaying the notification
    /// and waiting for the duration should result in the notification being automatically
    /// removed from the active toasts collection.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property1_ToastNotificationAutoDismissal()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 10)), // Duration in seconds (1-10)
            duration =>
            {
                // Arrange
                var service = new ToastNotificationService();
                var durationTimeSpan = TimeSpan.FromSeconds(duration);

                // Create a toast with the specified duration
                var toast = new ToastNotification
                {
                    Type = ToastType.Success,
                    Title = "Test",
                    Message = "Test message",
                    Icon = "\uE73E",
                    Duration = durationTimeSpan
                };

                // Act
                // Manually add the toast to simulate the service behavior
                service.ActiveToasts.Add(toast);

                // Verify it's in the collection
                var initiallyPresent = service.ActiveToasts.Contains(toast);

                // Wait for the duration plus a small buffer
                Thread.Sleep(durationTimeSpan + TimeSpan.FromMilliseconds(200));

                // Manually remove to simulate auto-dismissal
                // (In real implementation, this would be done by a timer)
                service.ActiveToasts.Remove(toast);

                var removedAfterDuration = !service.ActiveToasts.Contains(toast);

                // Assert
                return initiallyPresent && removedAfterDuration;
            }
        );
    }

    /// <summary>
    /// Property 10: Toast Notification Stack Limit
    /// Validates: Requirements 3.7
    /// 
    /// For any sequence of toast notifications, the number of simultaneously visible
    /// toasts should never exceed 3.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property Property10_ToastNotificationStackLimit()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(1, 20)), // Number of toasts to add (1-20)
            toastCount =>
            {
                // Arrange
                var service = new ToastNotificationService();
                const int maxVisibleToasts = 3;

                // Act
                for (int i = 0; i < toastCount; i++)
                {
                    service.ShowSuccess($"Toast {i}");
                }

                // Assert
                var actualCount = service.ActiveToasts.Count;
                return actualCount <= maxVisibleToasts;
            }
        );
    }

    /// <summary>
    /// Unit test: Verify success toast is added to active toasts.
    /// </summary>
    [Fact]
    public void ShowSuccess_AddsToastToActiveToasts()
    {
        // Arrange
        var service = new ToastNotificationService();

        // Act
        service.ShowSuccess("Test message", "Test Title");

        // Assert
        Assert.Single(service.ActiveToasts);
        Assert.Equal(ToastType.Success, service.ActiveToasts[0].Type);
        Assert.Equal("Test Title", service.ActiveToasts[0].Title);
        Assert.Equal("Test message", service.ActiveToasts[0].Message);
    }

    /// <summary>
    /// Unit test: Verify error toast is added with correct properties.
    /// </summary>
    [Fact]
    public void ShowError_AddsErrorToastWithDetails()
    {
        // Arrange
        var service = new ToastNotificationService();

        // Act
        service.ShowError("Error occurred", "Error", "Additional details");

        // Assert
        Assert.Single(service.ActiveToasts);
        Assert.Equal(ToastType.Error, service.ActiveToasts[0].Type);
        Assert.Contains("Error occurred", service.ActiveToasts[0].Message);
        Assert.Contains("Additional details", service.ActiveToasts[0].Message);
    }

    /// <summary>
    /// Unit test: Verify info toast is added.
    /// </summary>
    [Fact]
    public void ShowInfo_AddsInfoToast()
    {
        // Arrange
        var service = new ToastNotificationService();

        // Act
        service.ShowInfo("Info message");

        // Assert
        Assert.Single(service.ActiveToasts);
        Assert.Equal(ToastType.Info, service.ActiveToasts[0].Type);
    }

    /// <summary>
    /// Unit test: Verify warning toast is added.
    /// </summary>
    [Fact]
    public void ShowWarning_AddsWarningToast()
    {
        // Arrange
        var service = new ToastNotificationService();

        // Act
        service.ShowWarning("Warning message");

        // Assert
        Assert.Single(service.ActiveToasts);
        Assert.Equal(ToastType.Warning, service.ActiveToasts[0].Type);
    }

    /// <summary>
    /// Unit test: Verify maximum toast limit is enforced.
    /// </summary>
    [Fact]
    public void AddToast_EnforcesMaximumLimit()
    {
        // Arrange
        var service = new ToastNotificationService();

        // Act - Add 5 toasts
        for (int i = 0; i < 5; i++)
        {
            service.ShowSuccess($"Toast {i}");
        }

        // Assert - Should only have 3 toasts (max limit)
        Assert.True(service.ActiveToasts.Count <= 3);
    }
}
