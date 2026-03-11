using System.Xml.Linq;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests to verify OrderPageView XAML contains required AutomationId attributes.
/// These tests parse the XAML file statically without launching the application.
/// </summary>
/// <remarks>
/// Requirements validated: 3.1, 3.3, 3.5, 3.7, 3.8
/// Note: The actual implementation uses OrderPageView.xaml (not OrderEntryPage.xaml).
/// This test verifies the AutomationIds that exist in the order entry implementation.
/// </remarks>
public class OrderEntryPageAutomationTests
{
    private readonly XDocument _xamlDocument;

    public OrderEntryPageAutomationTests()
    {
        // Resolve path to OrderPageView.xaml by walking up from test assembly directory
        var xamlPath = ResolveOrderPageViewXamlPath();
        _xamlDocument = XDocument.Load(xamlPath);
    }

    /// <summary>
    /// Resolves the path to OrderPageView.xaml by walking up from the test assembly directory.
    /// </summary>
    private static string ResolveOrderPageViewXamlPath()
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

        var xamlPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "Views", "OrderPageView.xaml");
        
        if (!File.Exists(xamlPath))
        {
            throw new FileNotFoundException(
                $"OrderPageView.xaml not found at expected path: {xamlPath}. " +
                "Ensure the Presentation project contains the OrderPageView.xaml file.");
        }

        return xamlPath;
    }

    [Fact]
    public void OrderPageView_Contains_MenuItemsList_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("MenuItemsList");

        // Assert
        Assert.True(hasAutomationId,
            "OrderPageView.xaml must contain an element with AutomationProperties.AutomationId='MenuItemsList' " +
            "to enable E2E tests to locate the menu items list control. (Requirement 3.1)");
    }

    [Fact]
    public void OrderPageView_Contains_TicketTotalDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("TicketTotalDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "OrderPageView.xaml must contain an element with AutomationProperties.AutomationId='TicketTotalDisplay' " +
            "to enable E2E tests to verify the ticket total display. (Requirement 3.8)");
    }

    [Fact]
    public void OrderPageView_Contains_SettlementButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("SettlementButton");

        // Assert
        Assert.True(hasAutomationId,
            "OrderPageView.xaml must contain an element with AutomationProperties.AutomationId='SettlementButton' " +
            "to enable E2E tests to navigate to settlement. (Requirement 3.7)");
    }

    [Fact]
    public void OrderPageView_Contains_OrderItemsList_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("OrderItemsList");

        // Assert
        Assert.True(hasAutomationId,
            "OrderPageView.xaml must contain an element with AutomationProperties.AutomationId='OrderItemsList' " +
            "to enable E2E tests to interact with order items.");
    }

    [Fact]
    public void OrderPageView_Contains_ItemCountDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ItemCountDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "OrderPageView.xaml must contain an element with AutomationProperties.AutomationId='ItemCountDisplay' " +
            "to enable E2E tests to verify the item count display.");
    }

    [Fact]
    public void OrderPageView_AutomationIds_Are_Unique()
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
