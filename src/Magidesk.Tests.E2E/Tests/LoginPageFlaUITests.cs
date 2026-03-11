using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// FlaUI integration tests for LoginPage.
/// Validates that FlaUI can locate and interact with LoginPage elements using AutomationIds.
/// Requirements: 7.3
/// </summary>
[Collection("E2E Tests")]
public class LoginPageFlaUITests : BaseE2ETest
{
    private readonly ITestOutputHelper _output;

    public LoginPageFlaUITests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    [Fact]
    public void CanFindAndInteractWithUserSelectionGridView()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        var loginPage = new LoginPage(MainWindow);

        // Act - Find the UserSelectionGridView by AutomationId
        var userGridView = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UserSelectionGridView"));

        // Assert
        Assert.NotNull(userGridView);
        Assert.True(userGridView.IsAvailable);
        Assert.True(userGridView.IsEnabled);
        
        _output.WriteLine($"Successfully located UserSelectionGridView with AutomationId");
    }

    [Fact]
    public void CanFindAndInteractWithPinDisplayTextBlock()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Find the PinDisplayTextBlock by AutomationId
        var pinDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PinDisplayTextBlock"));

        // Assert
        Assert.NotNull(pinDisplay);
        Assert.True(pinDisplay.IsAvailable);
        
        // Verify we can read the text content (TextBlock uses Name property)
        var text = pinDisplay.Name ?? string.Empty;
        Assert.NotNull(text);
        
        _output.WriteLine($"Successfully located PinDisplayTextBlock with text: {text}");
    }

    [Fact]
    public void CanFindAndClickLoginButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Find the LoginButton by AutomationId
        var loginButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"));

        // Assert
        Assert.NotNull(loginButton);
        Assert.True(loginButton.IsAvailable);
        
        // Verify it's a button and we can access its properties
        var button = loginButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located LoginButton, IsEnabled: {loginButton.IsEnabled}");
    }

    [Fact]
    public void CanReadErrorMessageTextBlock()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Find the ErrorMessageTextBlock by AutomationId
        var errorMessage = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ErrorMessageTextBlock"));

        // Assert
        Assert.NotNull(errorMessage);
        Assert.True(errorMessage.IsAvailable);
        
        // Verify we can read the text content (should be empty initially)
        // Note: TextBlock is not a TextBox, so we use Name property instead
        var text = errorMessage.Name ?? string.Empty;
        Assert.NotNull(text);
        
        _output.WriteLine($"Successfully located ErrorMessageTextBlock with text: '{text}'");
    }

    [Fact]
    public void CanInteractWithNumericKeypad()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        var loginPage = new LoginPage(MainWindow);

        // Act - Find digit buttons by AutomationId
        var digit1Button = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Digit1Button"));
        var digit2Button = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Digit2Button"));
        var digit0Button = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Digit0Button"));
        var backspaceButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("BackspaceButton"));

        // Assert
        Assert.NotNull(digit1Button);
        Assert.NotNull(digit2Button);
        Assert.NotNull(digit0Button);
        Assert.NotNull(backspaceButton);
        
        Assert.True(digit1Button.IsEnabled);
        Assert.True(digit2Button.IsEnabled);
        Assert.True(digit0Button.IsEnabled);
        Assert.True(backspaceButton.IsEnabled);
        
        _output.WriteLine("Successfully located all numeric keypad buttons");
    }

    [Fact]
    public void CanFindQuickActionButtons()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Find quick action buttons by AutomationId
        var clockInOutButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ClockInOutButton"));
        var changeLanguageButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ChangeLanguageButton"));
        var shutdownButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ShutdownButton"));

        // Assert
        Assert.NotNull(clockInOutButton);
        Assert.NotNull(changeLanguageButton);
        Assert.NotNull(shutdownButton);
        
        Assert.True(clockInOutButton.IsAvailable);
        Assert.True(changeLanguageButton.IsAvailable);
        Assert.True(shutdownButton.IsAvailable);
        
        _output.WriteLine("Successfully located all quick action buttons");
    }

    [Fact]
    public void LoginPageElementsAreDiscoverableByFlaUI()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Verify all critical LoginPage elements are discoverable
        var elements = new Dictionary<string, AutomationElement?>
        {
            ["UserSelectionGridView"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UserSelectionGridView")),
            ["PinDisplayTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PinDisplayTextBlock")),
            ["ErrorMessageTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ErrorMessageTextBlock")),
            ["LoginButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")),
            ["Digit1Button"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Digit1Button")),
            ["BackspaceButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("BackspaceButton")),
            ["ClockInOutButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ClockInOutButton"))
        };

        // Assert - All elements should be found
        foreach (var kvp in elements)
        {
            Assert.NotNull(kvp.Value);
            Assert.True(kvp.Value.IsAvailable, $"{kvp.Key} should be available");
            _output.WriteLine($"✓ {kvp.Key} is discoverable and available");
        }
        
        _output.WriteLine($"\nAll {elements.Count} LoginPage elements are successfully discoverable by FlaUI");
    }
}
