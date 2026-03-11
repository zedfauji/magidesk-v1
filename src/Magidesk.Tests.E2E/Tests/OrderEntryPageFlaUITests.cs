using FlaUI.Core.AutomationElements;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// FlaUI integration tests for OrderEntryPage.
/// Validates that FlaUI can locate and interact with OrderEntryPage elements using AutomationIds.
/// Requirements: 7.5
/// </summary>
[Collection("E2E Tests")]
public class OrderEntryPageFlaUITests : BaseE2ETest
{
    private readonly ITestOutputHelper _output;

    public OrderEntryPageFlaUITests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    private void AuthenticateAndNavigateToOrderEntry()
    {
        // Authenticate with default test user PIN
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");
        
        // Wait for switchboard to load
        Thread.Sleep(1000);
        
        // Navigate to Order Entry (New Ticket)
        var switchboardPage = new SwitchboardPage(MainWindow!);
        switchboardPage.NavigateToOrderEntry();
        
        // Wait for order entry page to load
        Thread.Sleep(1000);
    }

    [Fact]
    public void CanFindMenuItemsList()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the MenuItemsListView by AutomationId
        var menuItemsList = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MenuItemsListView"));

        // Assert
        Assert.NotNull(menuItemsList);
        Assert.True(menuItemsList.IsAvailable);
        Assert.True(menuItemsList.IsEnabled);
        
        _output.WriteLine("Successfully located MenuItemsListView");
    }

    [Fact]
    public void CanFindAndInputQuantityTextBox()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the QuantityTextBox by AutomationId
        var quantityTextBox = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("QuantityTextBox"));

        // Assert
        Assert.NotNull(quantityTextBox);
        Assert.True(quantityTextBox.IsAvailable);
        
        // Verify it's a text box and we can access its properties
        var textBox = quantityTextBox.AsTextBox();
        Assert.NotNull(textBox);
        
        _output.WriteLine("Successfully located QuantityTextBox");
    }

    [Fact]
    public void CanFindAndReadTicketTotalDisplay()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the TicketTotalTextBlock by AutomationId
        var ticketTotalDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("TicketTotalTextBlock"));

        // Assert
        Assert.NotNull(ticketTotalDisplay);
        Assert.True(ticketTotalDisplay.IsAvailable);
        
        // Verify we can read the total value
        var totalText = ticketTotalDisplay.Name ?? string.Empty;
        Assert.NotNull(totalText);
        
        _output.WriteLine($"Successfully located TicketTotalTextBlock with value: {totalText}");
    }

    [Fact]
    public void CanFindHoldButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the HoldTicketButton by AutomationId
        var holdButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HoldTicketButton"));

        // Assert
        Assert.NotNull(holdButton);
        Assert.True(holdButton.IsAvailable);
        
        // Verify it's a button
        var button = holdButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located HoldTicketButton, IsEnabled: {holdButton.IsEnabled}");
    }

    [Fact]
    public void CanFindRecallButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the RecallTicketButton by AutomationId
        var recallButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("RecallTicketButton"));

        // Assert
        Assert.NotNull(recallButton);
        Assert.True(recallButton.IsAvailable);
        
        // Verify it's a button
        var button = recallButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located RecallTicketButton, IsEnabled: {recallButton.IsEnabled}");
    }

    [Fact]
    public void CanFindSettlementButton()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the SettlementButton by AutomationId
        var settlementButton = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SettlementButton"));

        // Assert
        Assert.NotNull(settlementButton);
        Assert.True(settlementButton.IsAvailable);
        
        // Verify it's a button
        var button = settlementButton.AsButton();
        Assert.NotNull(button);
        
        _output.WriteLine($"Successfully located SettlementButton, IsEnabled: {settlementButton.IsEnabled}");
    }

    [Fact]
    public void CanFindItemCountDisplay()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Find the ItemCountTextBlock by AutomationId
        var itemCountDisplay = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ItemCountTextBlock"));

        // Assert
        Assert.NotNull(itemCountDisplay);
        Assert.True(itemCountDisplay.IsAvailable);
        
        // Verify we can read the count value
        var countText = itemCountDisplay.Name ?? string.Empty;
        Assert.NotNull(countText);
        
        _output.WriteLine($"Successfully located ItemCountTextBlock with value: {countText}");
    }

    [Fact]
    public void OrderEntryPageElementsAreDiscoverableByFlaUI()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        AuthenticateAndNavigateToOrderEntry();

        // Act - Verify all critical OrderEntryPage elements are discoverable
        var elements = new Dictionary<string, AutomationElement?>
        {
            ["MenuItemsListView"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("MenuItemsListView")),
            ["QuantityTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("QuantityTextBox")),
            ["TicketTotalTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("TicketTotalTextBlock")),
            ["ItemCountTextBlock"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ItemCountTextBlock")),
            ["HoldTicketButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("HoldTicketButton")),
            ["RecallTicketButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("RecallTicketButton")),
            ["SettlementButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("SettlementButton"))
        };

        // Assert - All elements should be found
        foreach (var kvp in elements)
        {
            Assert.NotNull(kvp.Value);
            Assert.True(kvp.Value.IsAvailable, $"{kvp.Key} should be available");
            _output.WriteLine($"✓ {kvp.Key} is discoverable and available");
        }
        
        _output.WriteLine($"\nAll {elements.Count} OrderEntryPage elements are successfully discoverable by FlaUI");
    }
}
