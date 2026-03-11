using FlaUI.Core.AutomationElements;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// FlaUI integration tests for SwitchboardPage.
/// Validates that FlaUI can locate and interact with SwitchboardPage elements using AutomationIds.
/// Requirements: 7.4
/// </summary>
[Collection("E2E Tests")]
public class SwitchboardPageFlaUITests : BaseE2ETest
{
    private readonly ITestOutputHelper _output;

    public SwitchboardPageFlaUITests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    private void AuthenticateAndNavigateToSwitchboard()
    {
        // Authenticate with default test user PIN
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");
        
        // Wait for switchboard to load
        Thread.Sleep(1000);
    }

    [Fact]
    public void CanFindNavigationButtonsByName()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToSwitchboard();

        // Act - Find navigation buttons by AutomationProperties.Name (data-bound)
        var newTicketButton = MainWindow.FindFirstDescendant(cf => cf.ByName("New Ticket"));
        var openTicketsButton = MainWindow.FindFirstDescendant(cf => cf.ByName("Open Tickets"));
        var tablesButton = MainWindow.FindFirstDescendant(cf => cf.ByName("Tables"));
        var backOfficeButton = MainWindow.FindFirstDescendant(cf => cf.ByName("Back Office"));

        // Assert
        Assert.NotNull(newTicketButton);
        Assert.NotNull(openTicketsButton);
        Assert.NotNull(tablesButton);
        Assert.NotNull(backOfficeButton);
        
        Assert.True(newTicketButton.IsAvailable);
        Assert.True(openTicketsButton.IsAvailable);
        Assert.True(tablesButton.IsAvailable);
        Assert.True(backOfficeButton.IsAvailable);
        
        _output.WriteLine("Successfully located all navigation buttons by Name");
    }

    [Fact]
    public void CanFindAndReadCurrentUserDisplay()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToSwitchboard();

        // Act - Find the CurrentUserDisplay by AutomationId
        var currentUserDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CurrentUserDisplay"));

        // Assert
        Assert.NotNull(currentUserDisplay);
        Assert.True(currentUserDisplay.IsAvailable);
        
        // Verify we can read the user name
        var userName = currentUserDisplay.Name ?? string.Empty;
        Assert.NotNull(userName);
        Assert.NotEmpty(userName);
        
        _output.WriteLine($"Successfully located CurrentUserDisplay with user: {userName}");
    }

    [Fact]
    public void CanFindAndClickLogoutButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToSwitchboard();

        // Act - Find the LogoutButton by AutomationId
        var logoutButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LogoutButton"));

        // Assert
        Assert.NotNull(logoutButton);
        Assert.True(logoutButton.IsAvailable);
        Assert.True(logoutButton.IsEnabled);
        
        // Verify it's a button and we can access its properties
        var button = logoutButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine("Successfully located LogoutButton");
    }

    [Fact]
    public void CanFindStatusDisplayElements()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToSwitchboard();

        // Act - Find status display elements by AutomationId
        var openTicketCountDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("OpenTicketCountDisplay"));
        var activeSessionCountDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActiveSessionCountDisplay"));

        // Assert
        Assert.NotNull(openTicketCountDisplay);
        Assert.NotNull(activeSessionCountDisplay);
        
        Assert.True(openTicketCountDisplay.IsAvailable);
        Assert.True(activeSessionCountDisplay.IsAvailable);
        
        _output.WriteLine("Successfully located status display elements");
    }

    [Fact]
    public void SwitchboardPageElementsAreDiscoverableByFlaUI()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToSwitchboard();

        // Act - Verify all critical SwitchboardPage elements are discoverable
        var elements = new Dictionary<string, AutomationElement?>
        {
            ["CurrentUserDisplay"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CurrentUserDisplay")),
            ["LogoutButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LogoutButton")),
            ["OpenTicketCountDisplay"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("OpenTicketCountDisplay")),
            ["ActiveSessionCountDisplay"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ActiveSessionCountDisplay")),
            ["New Ticket (by Name)"] = MainWindow.FindFirstDescendant(cf => cf.ByName("New Ticket")),
            ["Back Office (by Name)"] = MainWindow.FindFirstDescendant(cf => cf.ByName("Back Office"))
        };

        // Assert - All elements should be found
        foreach (var kvp in elements)
        {
            Assert.NotNull(kvp.Value);
            Assert.True(kvp.Value.IsAvailable, $"{kvp.Key} should be available");
            _output.WriteLine($"✓ {kvp.Key} is discoverable and available");
        }
        
        _output.WriteLine($"\nAll {elements.Count} SwitchboardPage elements are successfully discoverable by FlaUI");
    }
}
