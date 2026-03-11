using FlaUI.Core.AutomationElements;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// FlaUI integration tests for CashSessionPage.
/// Validates that FlaUI can locate and interact with CashSessionPage elements using AutomationIds.
/// Requirements: 7.7
/// </summary>
[Collection("E2E Tests")]
public class CashSessionPageFlaUITests : BaseE2ETest
{
    private readonly ITestOutputHelper _output;

    public CashSessionPageFlaUITests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    private void AuthenticateAndNavigateToCashSession()
    {
        // Authenticate with default test user PIN
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");
        
        // Wait for switchboard to load
        Thread.Sleep(1000);
        
        // Navigate to Manager Functions (which contains Cash Session)
        var switchboardPage = new SwitchboardPage(MainWindow!);
        switchboardPage.NavigateToManagerFunctions();
        
        // Wait for manager functions page to load
        Thread.Sleep(1000);
        
        // Note: Actual navigation to Cash Session page may require additional steps
        // depending on the Manager Functions menu structure
    }

    [Fact]
    public void CanFindOpenSessionButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the OpenSessionButton by AutomationId
        var openSessionButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("OpenSessionButton"));

        // Assert
        Assert.NotNull(openSessionButton);
        Assert.True(openSessionButton.IsAvailable);
        
        // Verify it's a button
        var button = openSessionButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located OpenSessionButton, IsEnabled: {openSessionButton.IsEnabled}");
    }

    [Fact]
    public void CanFindStartingCashTextBox()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the StartingCashTextBox by AutomationId
        var startingCashTextBox = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StartingCashTextBox"));

        // Assert
        Assert.NotNull(startingCashTextBox);
        Assert.True(startingCashTextBox.IsAvailable);
        
        // Verify it's a text box
        var textBox = startingCashTextBox.AsTextBox();
        Assert.NotNull(textBox);
        
        _output.WriteLine($"Successfully located StartingCashTextBox, IsEnabled: {startingCashTextBox.IsEnabled}");
    }

    [Fact]
    public void CanFindExpectedCashDisplay()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the ExpectedCashTextBlock by AutomationId
        var expectedCashDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ExpectedCashTextBlock"));

        // Assert
        Assert.NotNull(expectedCashDisplay);
        Assert.True(expectedCashDisplay.IsAvailable);
        
        // Verify we can read the expected cash value
        var cashText = expectedCashDisplay.Name ?? string.Empty;
        Assert.NotNull(cashText);
        
        _output.WriteLine($"Successfully located ExpectedCashTextBlock with value: {cashText}");
    }

    [Fact]
    public void CanFindSessionStatusDisplay()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the SessionStatusTextBlock by AutomationId
        var sessionStatusDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SessionStatusTextBlock"));

        // Assert
        Assert.NotNull(sessionStatusDisplay);
        Assert.True(sessionStatusDisplay.IsAvailable);
        
        // Verify we can read the session status
        var statusText = sessionStatusDisplay.Name ?? string.Empty;
        Assert.NotNull(statusText);
        
        _output.WriteLine($"Successfully located SessionStatusTextBlock with value: {statusText}");
    }

    [Fact]
    public void CanFindCashDropButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the CashDropButton by AutomationId
        var cashDropButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CashDropButton"));

        // Assert
        Assert.NotNull(cashDropButton);
        Assert.True(cashDropButton.IsAvailable);
        
        // Verify it's a button
        var button = cashDropButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located CashDropButton, IsEnabled: {cashDropButton.IsEnabled}");
    }

    [Fact]
    public void CanFindCloseSessionButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Find the CloseSessionButton by AutomationId
        var closeSessionButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CloseSessionButton"));

        // Assert
        Assert.NotNull(closeSessionButton);
        Assert.True(closeSessionButton.IsAvailable);
        
        // Verify it's a button
        var button = closeSessionButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located CloseSessionButton, IsEnabled: {closeSessionButton.IsEnabled}");
    }

    [Fact]
    public void CashSessionPageElementsAreDiscoverableByFlaUI()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToCashSession();

        // Act - Verify all critical CashSessionPage elements are discoverable
        var elements = new Dictionary<string, AutomationElement?>
        {
            ["OpenSessionButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("OpenSessionButton")),
            ["CloseSessionButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CloseSessionButton")),
            ["CashDropButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CashDropButton")),
            ["StartingCashTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StartingCashTextBox")),
            ["ExpectedCashTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ExpectedCashTextBlock")),
            ["SessionStatusTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SessionStatusTextBlock"))
        };

        // Assert - All elements should be found
        foreach (var kvp in elements)
        {
            Assert.NotNull(kvp.Value);
            Assert.True(kvp.Value.IsAvailable, $"{kvp.Key} should be available");
            _output.WriteLine($"✓ {kvp.Key} is discoverable and available");
        }
        
        _output.WriteLine($"\nAll {elements.Count} CashSessionPage elements are successfully discoverable by FlaUI");
    }
}
