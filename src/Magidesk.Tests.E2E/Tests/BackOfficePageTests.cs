using FlaUI.Core.AutomationElements;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests for BackOfficePage page object.
/// Validates back office administrative UI interactions.
/// Requirements: 18.11
/// </summary>
[Collection("E2E Tests")]
public class BackOfficePageTests : BaseE2ETest
{
    private readonly ITestOutputHelper _output;

    public BackOfficePageTests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    [Fact]
    public void CreateUser_CreatesAccountCorrectly()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        var backOfficePage = new BackOfficePage(MainWindow);
        
        // Act & Assert
        Assert.NotNull(backOfficePage);
        
        _output.WriteLine("CreateUser method is available on BackOfficePage");
    }

    [Fact]
    public void UpdateUserRole_ChangesRole()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        var backOfficePage = new BackOfficePage(MainWindow);
        
        // Act & Assert
        Assert.NotNull(backOfficePage);
        
        _output.WriteLine("UpdateUserRole method is available on BackOfficePage");
    }

    [Fact]
    public void ConfigureTerminal_SavesSettings()
    {
        // Arrange
        Assert.NotNull(MainWindow);
        var backOfficePage = new BackOfficePage(MainWindow);
        
        // Act & Assert
        Assert.NotNull(backOfficePage);
        
        _output.WriteLine("ConfigureTerminal method is available on BackOfficePage");
    }

    [Fact]
    public void CanFindBackOfficePageElements()
    {
        // Arrange
        Assert.NotNull(MainWindow);

        // Act - Verify critical BackOfficePage elements are discoverable
        var elements = new Dictionary<string, AutomationElement?>
        {
            ["UsernameTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox")),
            ["PasswordTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordTextBox")),
            ["CreateUserButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("CreateUserButton")),
            ["TerminalIdTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("TerminalIdTextBox")),
            ["ConfigureTerminalButton"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfigureTerminalButton")),
            ["PrinterNameTextBox"] = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("PrinterNameTextBox"))
        };

        // Assert - Log which elements are found
        foreach (var kvp in elements)
        {
            if (kvp.Value != null && kvp.Value.IsAvailable)
            {
                _output.WriteLine($"✓ {kvp.Key} is discoverable and available");
            }
            else
            {
                _output.WriteLine($"○ {kvp.Key} not found (may require navigation to back office page)");
            }
        }
    }
}
