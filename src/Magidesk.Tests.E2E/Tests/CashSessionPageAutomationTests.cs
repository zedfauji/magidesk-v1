using System.Xml.Linq;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests to verify CashSessionPage XAML contains required AutomationId attributes.
/// These tests parse the XAML file statically without launching the application.
/// </summary>
/// <remarks>
/// Requirements validated: 5.1, 5.5, 5.6, 5.8
/// </remarks>
public class CashSessionPageAutomationTests
{
    private readonly XDocument _xamlDocument;

    public CashSessionPageAutomationTests()
    {
        var xamlPath = ResolveCashSessionPageXamlPath();
        _xamlDocument = XDocument.Load(xamlPath);
    }

    /// <summary>
    /// Resolves the path to CashSessionPage.xaml by walking up from the test assembly directory.
    /// </summary>
    private static string ResolveCashSessionPageXamlPath()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && currentDir.Name != "src")
            currentDir = currentDir.Parent;

        if (currentDir == null)
        {
            throw new FileNotFoundException(
                "Could not locate the 'src' directory by walking up from the test assembly location. " +
                "Ensure the test project is in the correct location relative to the Presentation project.");
        }

        var xamlPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "Views", "CashSessionPage.xaml");
        
        if (!File.Exists(xamlPath))
        {
            throw new FileNotFoundException(
                $"CashSessionPage.xaml not found at expected path: {xamlPath}. " +
                "Ensure the Presentation project contains the CashSessionPage.xaml file.");
        }

        return xamlPath;
    }

    [Fact]
    public void CashSessionPage_Contains_OpenSessionButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("OpenSessionButton");

        // Assert
        Assert.True(hasAutomationId,
            "CashSessionPage.xaml must contain an element with AutomationProperties.AutomationId='OpenSessionButton' " +
            "to enable E2E tests to open cash sessions. (Requirement 5.1)");
    }

    [Fact]
    public void CashSessionPage_Contains_StartingCashTextBox_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("StartingCashTextBox");

        // Assert
        Assert.True(hasAutomationId,
            "CashSessionPage.xaml must contain an element with AutomationProperties.AutomationId='StartingCashTextBox' " +
            "to enable E2E tests to input starting cash amounts. (Requirement 5.5)");
    }

    [Fact]
    public void CashSessionPage_Contains_ExpectedCashDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ExpectedCashDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "CashSessionPage.xaml must contain an element with AutomationProperties.AutomationId='ExpectedCashDisplay' " +
            "to enable E2E tests to verify expected cash amounts. (Requirement 5.6)");
    }

    [Fact]
    public void CashSessionPage_Contains_SessionStatusDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("SessionStatusDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "CashSessionPage.xaml must contain an element with AutomationProperties.AutomationId='SessionStatusDisplay' " +
            "to enable E2E tests to verify session status. (Requirement 5.8)");
    }

    [Fact]
    public void CashSessionPage_AutomationIds_Are_Unique()
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
        return _xamlDocument.Descendants()
            .Any(element =>
            {
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
