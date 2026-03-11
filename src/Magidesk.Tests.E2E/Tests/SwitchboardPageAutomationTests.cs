using System.Xml.Linq;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests to verify SwitchboardPage XAML contains required AutomationId attributes.
/// These tests parse the XAML file statically without launching the application.
/// </summary>
/// <remarks>
/// Requirements validated: 2.5, 2.6
/// Note: Dynamic navigation buttons use AutomationProperties.Name (data-bound to Label)
/// rather than AutomationId. This test verifies the static AutomationIds that exist
/// for user context display elements.
/// </remarks>
public class SwitchboardPageAutomationTests
{
    private readonly XDocument _xamlDocument;

    public SwitchboardPageAutomationTests()
    {
        // Resolve path to SwitchboardPage.xaml by walking up from test assembly directory
        var xamlPath = ResolveSwitchboardPageXamlPath();
        _xamlDocument = XDocument.Load(xamlPath);
    }

    /// <summary>
    /// Resolves the path to SwitchboardPage.xaml by walking up from the test assembly directory.
    /// </summary>
    private static string ResolveSwitchboardPageXamlPath()
    {
        // Walk up from the test assembly directory to find the 'src' folder
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && currentDir.Name != "src")
            currentDir = currentDir.Parent;

        if (currentDir == null)
        {
            throw new FileNotFoundException(
                "Could not locate the 'src' directory by walking up from the test assembly location. " +
                "Ensure the test project is in the correct location relative to the Presentation project.");
        }

        var xamlPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "Views", "SwitchboardPage.xaml");
        
        if (!File.Exists(xamlPath))
        {
            throw new FileNotFoundException(
                $"SwitchboardPage.xaml not found at expected path: {xamlPath}. " +
                "Ensure the Presentation project contains the SwitchboardPage.xaml file.");
        }

        return xamlPath;
    }

    [Fact]
    public void SwitchboardPage_Contains_CurrentUserDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("CurrentUserDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SwitchboardPage.xaml must contain an element with AutomationProperties.AutomationId='CurrentUserDisplay' " +
            "to enable E2E tests to verify the current user display. (Requirement 2.6)");
    }

    [Fact]
    public void SwitchboardPage_Contains_TerminalIdDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("TerminalIdDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SwitchboardPage.xaml must contain an element with AutomationProperties.AutomationId='TerminalIdDisplay' " +
            "to enable E2E tests to verify the terminal ID display.");
    }

    [Fact]
    public void SwitchboardPage_Contains_ShiftStatusDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ShiftStatusDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SwitchboardPage.xaml must contain an element with AutomationProperties.AutomationId='ShiftStatusDisplay' " +
            "to enable E2E tests to verify the shift status display.");
    }

    [Fact]
    public void SwitchboardPage_Contains_OpenTicketCountDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("OpenTicketCountDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SwitchboardPage.xaml must contain an element with AutomationProperties.AutomationId='OpenTicketCountDisplay' " +
            "to enable E2E tests to verify the open ticket count display.");
    }

    [Fact]
    public void SwitchboardPage_Contains_ActiveSessionCountDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ActiveSessionCountDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SwitchboardPage.xaml must contain an element with AutomationProperties.AutomationId='ActiveSessionCountDisplay' " +
            "to enable E2E tests to verify the active session count display.");
    }

    [Fact]
    public void SwitchboardPage_DynamicButtons_Use_AutomationName()
    {
        // Act
        var hasDynamicButtonsWithAutomationName = _xamlDocument.Descendants()
            .Any(element =>
            {
                var automationNameAttr = element.Attribute("AutomationProperties.Name");
                return automationNameAttr != null && 
                       automationNameAttr.Value.Contains("{x:Bind Label");
            });

        // Assert
        Assert.True(hasDynamicButtonsWithAutomationName,
            "SwitchboardPage.xaml must use AutomationProperties.Name bound to Label for dynamic navigation buttons " +
            "to enable E2E tests to locate buttons by their label text.");
    }

    [Fact]
    public void SwitchboardPage_AutomationIds_Are_Unique()
    {
        // Act
        var automationIds = GetAllAutomationIds();
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Checks if the XAML document contains an element with the specified AutomationId.
    /// </summary>
    private bool HasAutomationId(string automationId)
    {
        // AutomationProperties.AutomationId is an attached property in XAML
        // It appears as an attribute in the format: AutomationProperties.AutomationId="value"
        return _xamlDocument.Descendants()
            .Any(element =>
            {
                // Check for the attribute in various possible formats
                var attr = element.Attribute("AutomationProperties.AutomationId") ??
                          element.Attribute(XName.Get("AutomationId", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
                return attr != null && attr.Value == automationId;
            });
    }

    /// <summary>
    /// Extracts all AutomationId values from the XAML document.
    /// </summary>
    private List<string> GetAllAutomationIds()
    {
        return _xamlDocument.Descendants()
            .Select(element =>
                element.Attribute("AutomationProperties.AutomationId") ??
                element.Attribute(XName.Get("AutomationId", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")))
            .Where(attr => attr != null)
            .Select(attr => attr!.Value)
            .ToList();
    }
}
