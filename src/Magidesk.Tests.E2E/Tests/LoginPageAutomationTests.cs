using System.Xml.Linq;
using Xunit;

namespace Magidesk.Tests.E2E.Tests;

/// <summary>
/// Unit tests to verify LoginPage XAML contains required AutomationId attributes.
/// These tests parse the XAML file statically without launching the application.
/// </summary>
/// <remarks>
/// Requirements validated: 1.1, 1.2, 1.3, 1.4
/// Note: LoginPage uses PIN-based authentication, not username/password.
/// The requirements specify "UsernameTextBox" and "PasswordTextBox" but the actual
/// implementation uses UserSelectionGridView and PIN entry, so we verify the
/// AutomationIds that actually exist in the implementation.
/// </remarks>
public class LoginPageAutomationTests
{
    private readonly XDocument _xamlDocument;

    public LoginPageAutomationTests()
    {
        // Resolve path to LoginPage.xaml by walking up from test assembly directory
        var xamlPath = ResolveLoginPageXamlPath();
        _xamlDocument = XDocument.Load(xamlPath);
    }

    /// <summary>
    /// Resolves the path to LoginPage.xaml by walking up from the test assembly directory.
    /// </summary>
    private static string ResolveLoginPageXamlPath()
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

        var xamlPath = Path.Combine(currentDir.FullName, "Magidesk.Presentation", "Views", "LoginPage.xaml");
        
        if (!File.Exists(xamlPath))
        {
            throw new FileNotFoundException(
                $"LoginPage.xaml not found at expected path: {xamlPath}. " +
                "Ensure the Presentation project contains the LoginPage.xaml file.");
        }

        return xamlPath;
    }

    [Fact]
    public void LoginPage_Contains_UserSelectionGridView_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("UserSelectionGridView");

        // Assert
        Assert.True(hasAutomationId,
            "LoginPage.xaml must contain an element with AutomationProperties.AutomationId='UserSelectionGridView' " +
            "to enable E2E tests to locate the user selection control.");
    }

    [Fact]
    public void LoginPage_Contains_PinDisplayTextBlock_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("PinDisplayTextBlock");

        // Assert
        Assert.True(hasAutomationId,
            "LoginPage.xaml must contain an element with AutomationProperties.AutomationId='PinDisplayTextBlock' " +
            "to enable E2E tests to verify PIN entry display.");
    }

    [Fact]
    public void LoginPage_Contains_LoginButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("LoginButton");

        // Assert
        Assert.True(hasAutomationId,
            "LoginPage.xaml must contain an element with AutomationProperties.AutomationId='LoginButton' " +
            "to enable E2E tests to trigger login action. (Requirement 1.3)");
    }

    [Fact]
    public void LoginPage_Contains_ErrorMessageTextBlock_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("ErrorMessageTextBlock");

        // Assert
        Assert.True(hasAutomationId,
            "LoginPage.xaml must contain an element with AutomationProperties.AutomationId='ErrorMessageTextBlock' " +
            "to enable E2E tests to verify error messages. (Requirement 1.4)");
    }

    [Fact]
    public void LoginPage_Contains_NumericKeypad_AutomationIds()
    {
        // Arrange
        var expectedDigitButtons = new[]
        {
            "Digit0Button", "Digit1Button", "Digit2Button", "Digit3Button", "Digit4Button",
            "Digit5Button", "Digit6Button", "Digit7Button", "Digit8Button", "Digit9Button"
        };

        // Act & Assert
        foreach (var buttonId in expectedDigitButtons)
        {
            var hasAutomationId = HasAutomationId(buttonId);
            Assert.True(hasAutomationId,
                $"LoginPage.xaml must contain an element with AutomationProperties.AutomationId='{buttonId}' " +
                "to enable E2E tests to enter PIN digits.");
        }
    }

    [Fact]
    public void LoginPage_Contains_BackspaceButton_AutomationId()
    {
        // Act
        var hasAutomationId = HasAutomationId("BackspaceButton");

        // Assert
        Assert.True(hasAutomationId,
            "LoginPage.xaml must contain an element with AutomationProperties.AutomationId='BackspaceButton' " +
            "to enable E2E tests to correct PIN entry mistakes.");
    }

    [Fact]
    public void LoginPage_Contains_QuickAction_AutomationIds()
    {
        // Arrange
        var expectedQuickActions = new[]
        {
            "ClockInOutButton",
            "ChangeLanguageButton",
            "ShutdownButton"
        };

        // Act & Assert
        foreach (var actionId in expectedQuickActions)
        {
            var hasAutomationId = HasAutomationId(actionId);
            Assert.True(hasAutomationId,
                $"LoginPage.xaml must contain an element with AutomationProperties.AutomationId='{actionId}' " +
                "to enable E2E tests to access quick actions.");
        }
    }

    [Fact]
    public void LoginPage_AutomationIds_Are_Unique()
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
