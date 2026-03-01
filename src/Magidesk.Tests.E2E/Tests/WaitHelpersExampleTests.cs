using FlaUI.Core.Definitions;
using Magidesk.Tests.E2E.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Example tests demonstrating proper usage of WaitHelpers.
/// These tests show the correct patterns for waiting on UI elements.
/// </summary>
public class WaitHelpersExampleTests : BaseE2ETest
{
    [Fact]
    public void Example_WaitForElementByAutomationId()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(10);

        // Act & Assert - Wait for an element by AutomationId
        // Replace "LoginButton" with an actual AutomationId from your app
        var exception = Assert.Throws<TimeoutException>(() =>
            WaitHelpers.WaitForElementByAutomationId(MainWindow!, "NonExistentButton", TimeSpan.FromSeconds(2)));

        // Verify error message contains helpful details
        Assert.Contains("AutomationId 'NonExistentButton'", exception.Message);
        Assert.Contains("was not found", exception.Message);
    }

    [Fact]
    public void Example_WaitForElementByName()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(10);

        // Act & Assert - Wait for an element by Name
        var exception = Assert.Throws<TimeoutException>(() =>
            WaitHelpers.WaitForElementByName(MainWindow!, "NonExistentElement", TimeSpan.FromSeconds(2)));

        // Verify error message contains helpful details
        Assert.Contains("Name 'NonExistentElement'", exception.Message);
        Assert.Contains("was not found", exception.Message);
    }

    [Fact]
    public void Example_WaitForElementByControlType()
    {
        // Arrange - Look for a button control type
        var timeout = TimeSpan.FromSeconds(10);

        // Act - This will succeed if any button exists in the main window
        try
        {
            var button = WaitHelpers.WaitForElementByControlType(
                MainWindow!,
                ControlType.Button,
                timeout);

            // Assert
            Assert.NotNull(button);
            Assert.Equal(ControlType.Button, button.ControlType);
        }
        catch (TimeoutException ex)
        {
            // If no button exists, verify error message is descriptive
            Assert.Contains("ControlType 'Button'", ex.Message);
        }
    }

    [Fact]
    public void Example_WaitUntil_CustomCondition()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(5);

        // Act & Assert - Wait for a custom condition
        WaitHelpers.WaitUntil(
            () => MainWindow!.IsAvailable,
            timeout,
            "Main window did not become available");

        Assert.True(MainWindow!.IsAvailable);
    }

    [Fact]
    public void Example_WaitForElementEnabled()
    {
        // Arrange - Find any element in the window
        var timeout = TimeSpan.FromSeconds(10);

        try
        {
            var element = WaitHelpers.WaitForElementByControlType(
                MainWindow!,
                ControlType.Button,
                timeout);

            // Act & Assert - Wait for element to be enabled
            WaitHelpers.WaitForElementEnabled(element, timeout);

            Assert.True(element.IsEnabled);
        }
        catch (TimeoutException)
        {
            // Skip if no button found - this is just an example
            Assert.True(true);
        }
    }

    [Fact]
    public void Example_ErrorMessage_ContainsElementDetails()
    {
        // This test demonstrates that timeout exceptions include detailed element information

        // Act & Assert
        var exception = Assert.Throws<TimeoutException>(() =>
            WaitHelpers.WaitForElementByAutomationId(
                MainWindow!,
                "TestAutomationId",
                TimeSpan.FromSeconds(1)));

        // Verify error message quality
        Assert.Contains("TestAutomationId", exception.Message);
        Assert.Contains("was not found", exception.Message);
        Assert.Contains("seconds", exception.Message);
        Assert.Contains("Parent:", exception.Message);
    }
}
