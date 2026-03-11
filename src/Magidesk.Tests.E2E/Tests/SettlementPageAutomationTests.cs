using System.Xml.Linq;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests to verify SettlePageView XAML contains required AutomationId attributes.
/// These tests parse the XAML file statically without launching the application.
/// </summary>
/// <remarks>
/// Requirements validated: 4.1, 4.3, 4.6, 4.7
/// Note: The actual implementation uses SettlePageView.xaml (not SettlementPage.xaml).
/// This test verifies the AutomationIds that exist in the settlement implementation.
/// </remarks>
public class SettlementPageAutomationTests
{
    private readonly XDocument _xamlDocument;

    public SettlementPageAutomationTests()
    {
        // Resolve path to SettlePageView.xaml by walking up from test assembly directory
        var xamlPath = ResolveSettlePageViewXamlPath();
        _xamlDocument = XDocument.Load(xamlPath);
    }

    /// <summary>
    /// Resolves the path to SettlePageView.xaml by walking up from the test assembly directory.
    /// </summary>
    private static string ResolveSettlePageViewXamlPath()
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

        var xamlPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "Views", "SettlePageView.xaml");
        
        if (!File.Exists(xamlPath))
        {
            throw new FileNotFoundException(
                $"SettlePageView.xaml not found at expected path: {xamlPath}. " +
                "Ensure the Presentation project contains the SettlePageView.xaml file.");
        }

        return xamlPath;
    }

    [Fact]
    public void SettlePageView_Contains_PaymentMethodComboBox_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("PaymentMethodComboBox");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='PaymentMethodComboBox' " +
            "to enable E2E tests to locate the payment method selection combo box. (Requirement 4.1)");
    }

    [Fact]
    public void SettlePageView_Contains_ProcessPaymentButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ProcessPaymentButton");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='ProcessPaymentButton' " +
            "to enable E2E tests to trigger payment processing. (Requirement 4.3)");
    }

    [Fact]
    public void SettlePageView_Contains_TicketTotalDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("TicketTotalDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='TicketTotalDisplay' " +
            "to enable E2E tests to verify the ticket total display. (Requirement 4.6)");
    }

    [Fact]
    public void SettlePageView_Contains_AmountDueDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("AmountDueDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='AmountDueDisplay' " +
            "to enable E2E tests to verify the amount due display. (Requirement 4.7)");
    }

    [Fact]
    public void SettlePageView_Contains_AmountPaidDisplay_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("AmountPaidDisplay");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='AmountPaidDisplay' " +
            "to enable E2E tests to verify the amount paid display.");
    }

    [Fact]
    public void SettlePageView_Contains_PaymentAmountTextBox_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("PaymentAmountTextBox");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='PaymentAmountTextBox' " +
            "to enable E2E tests to verify the payment amount input.");
    }

    [Fact]
    public void SettlePageView_Contains_SplitPaymentButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("SplitPaymentButton");

        // Assert
        Assert.True(hasAutomationId,
            "SettlePageView.xaml must contain an element with AutomationProperties.AutomationId='SplitPaymentButton' " +
            "to enable E2E tests to trigger split payment functionality.");
    }

    [Fact]
    public void SettlePageView_AutomationIds_Are_Unique()
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
