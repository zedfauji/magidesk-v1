using System.Text.RegularExpressions;
using System.Xml.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for AutomationId naming convention compliance.
/// Validates that all AutomationProperties.AutomationId values follow the pattern {ElementPurpose}{ControlType}.
/// </summary>
public class NamingConventionProperty
{
    // Feature: ui-automation-ids, Property 4: Naming convention compliance

    private const string PresentationProjectPath = "../../../../../src/Magidesk.Presentation";

    // Regex pattern to validate naming convention: {ElementPurpose}{ControlType}
    // ElementPurpose: PascalCase descriptive name (e.g., Username, Login, TicketTotal)
    // ControlType: Known UI control type suffix (e.g., TextBox, Button, ComboBox, Display, List)
    private static readonly Regex NamingConventionPattern = new(
        @"^[A-Z][a-zA-Z0-9]+(Button|TextBox|TextBlock|ComboBox|ListView|GridView|Display|List|Panel|Grid|StackPanel|ScrollViewer|Border|Image|Icon|Label|CheckBox|RadioButton|ToggleButton|Slider|ProgressBar|DatePicker|TimePicker|Calendar|WebView|MediaElement|Canvas|ViewBox|Expander|TreeView|DataGrid|Menu|MenuItem|ToolBar|StatusBar|TabControl|TabItem|Separator|Popup|ToolTip|ContentControl|ItemsControl|Selector|RangeBase|Control)$",
        RegexOptions.Compiled);

    /// <summary>
    /// Feature: ui-automation-ids, Property 4: Naming convention compliance
    /// Validates: Requirements 7.1
    /// 
    /// For any AutomationId assigned in the Presentation layer, the identifier must follow
    /// the naming convention pattern {ElementPurpose}{ControlType} where ElementPurpose is
    /// a descriptive name and ControlType is the UI control type.
    /// </summary>
    [Property(MaxTest = 100)]
    public Property AllAutomationIdsFollowNamingConvention()
    {
        return Prop.ForAll(
            GenerateAutomationIdWithSource(),
            automationIdWithSource =>
            {
                var (automationId, xamlFilePath) = automationIdWithSource;
                var fileName = Path.GetFileName(xamlFilePath);

                // Act - Validate naming convention
                var isValid = NamingConventionPattern.IsMatch(automationId);

                // Assert - AutomationId must follow naming convention
                if (!isValid)
                {
                    throw new Exception(
                        $"AutomationId '{automationId}' in {fileName} does not follow naming convention. " +
                        $"Expected pattern: {{ElementPurpose}}{{ControlType}} (e.g., UsernameTextBox, LoginButton, TicketTotalDisplay)");
                }

                return true;
            });
    }

    /// <summary>
    /// Unit test to verify that all AutomationIds in LoginPage follow naming convention.
    /// This is a specific example test that complements the property test.
    /// </summary>
    [Fact]
    public void LoginPage_AutomationIds_FollowNamingConvention()
    {
        // Arrange
        var loginPagePath = Path.Combine(PresentationProjectPath, "Views", "LoginPage.xaml");
        var automationIds = ExtractAutomationIds(loginPagePath);

        // Act & Assert
        foreach (var automationId in automationIds)
        {
            var isValid = NamingConventionPattern.IsMatch(automationId);
            Assert.True(isValid,
                $"AutomationId '{automationId}' in LoginPage.xaml does not follow naming convention. " +
                $"Expected pattern: {{ElementPurpose}}{{ControlType}}");
        }
    }

    /// <summary>
    /// Unit test to verify that all AutomationIds in SwitchboardPage follow naming convention.
    /// </summary>
    [Fact]
    public void SwitchboardPage_AutomationIds_FollowNamingConvention()
    {
        // Arrange
        var switchboardPagePath = Path.Combine(PresentationProjectPath, "Views", "SwitchboardPage.xaml");
        var automationIds = ExtractAutomationIds(switchboardPagePath);

        // Act & Assert
        foreach (var automationId in automationIds)
        {
            var isValid = NamingConventionPattern.IsMatch(automationId);
            Assert.True(isValid,
                $"AutomationId '{automationId}' in SwitchboardPage.xaml does not follow naming convention. " +
                $"Expected pattern: {{ElementPurpose}}{{ControlType}}");
        }
    }

    /// <summary>
    /// Unit test to verify that all AutomationIds in OrderPageView follow naming convention.
    /// </summary>
    [Fact]
    public void OrderPageView_AutomationIds_FollowNamingConvention()
    {
        // Arrange
        var orderPagePath = Path.Combine(PresentationProjectPath, "Views", "OrderPageView.xaml");
        var automationIds = ExtractAutomationIds(orderPagePath);

        // Act & Assert
        foreach (var automationId in automationIds)
        {
            var isValid = NamingConventionPattern.IsMatch(automationId);
            Assert.True(isValid,
                $"AutomationId '{automationId}' in OrderPageView.xaml does not follow naming convention. " +
                $"Expected pattern: {{ElementPurpose}}{{ControlType}}");
        }
    }

    /// <summary>
    /// Unit test to verify that all AutomationIds in SettlePageView follow naming convention.
    /// </summary>
    [Fact]
    public void SettlePageView_AutomationIds_FollowNamingConvention()
    {
        // Arrange
        var settlePagePath = Path.Combine(PresentationProjectPath, "Views", "SettlePageView.xaml");
        var automationIds = ExtractAutomationIds(settlePagePath);

        // Act & Assert
        foreach (var automationId in automationIds)
        {
            var isValid = NamingConventionPattern.IsMatch(automationId);
            Assert.True(isValid,
                $"AutomationId '{automationId}' in SettlePageView.xaml does not follow naming convention. " +
                $"Expected pattern: {{ElementPurpose}}{{ControlType}}");
        }
    }

    /// <summary>
    /// Unit test to verify that all AutomationIds in CashSessionPage follow naming convention.
    /// </summary>
    [Fact]
    public void CashSessionPage_AutomationIds_FollowNamingConvention()
    {
        // Arrange
        var cashSessionPagePath = Path.Combine(PresentationProjectPath, "Views", "CashSessionPage.xaml");
        var automationIds = ExtractAutomationIds(cashSessionPagePath);

        // Act & Assert
        foreach (var automationId in automationIds)
        {
            var isValid = NamingConventionPattern.IsMatch(automationId);
            Assert.True(isValid,
                $"AutomationId '{automationId}' in CashSessionPage.xaml does not follow naming convention. " +
                $"Expected pattern: {{ElementPurpose}}{{ControlType}}");
        }
    }

    // ===== Helper Methods =====

    /// <summary>
    /// Extracts all AutomationProperties.AutomationId values from a XAML file.
    /// </summary>
    private static List<string> ExtractAutomationIds(string xamlFilePath)
    {
        if (!File.Exists(xamlFilePath))
        {
            throw new FileNotFoundException($"XAML file not found: {xamlFilePath}");
        }

        var doc = XDocument.Load(xamlFilePath);
        var automationIds = new List<string>();

        // Search for AutomationProperties.AutomationId attributes
        // In XAML, this appears as an attribute with LocalName "AutomationProperties.AutomationId"
        var automationIdAttributes = doc.Descendants()
            .SelectMany(e => e.Attributes())
            .Where(a => a.Name.LocalName.Contains("AutomationId") && 
                       (a.Name.LocalName == "AutomationProperties.AutomationId" ||
                        a.Name.LocalName == "AutomationId"))
            .Select(a => a.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        automationIds.AddRange(automationIdAttributes);

        return automationIds;
    }

    /// <summary>
    /// Gets all XAML files from the Presentation layer.
    /// </summary>
    private static List<string> GetAllXamlFiles()
    {
        var viewsPath = Path.Combine(PresentationProjectPath, "Views");
        
        if (!Directory.Exists(viewsPath))
        {
            throw new DirectoryNotFoundException($"Views directory not found: {viewsPath}");
        }

        return Directory.GetFiles(viewsPath, "*.xaml", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".xaml.cs")) // Exclude code-behind files
            .ToList();
    }

    /// <summary>
    /// Gets all (AutomationId, XamlFilePath) pairs from all XAML files.
    /// </summary>
    private static List<(string AutomationId, string XamlFilePath)> GetAllAutomationIdsWithSource()
    {
        var pairs = new List<(string, string)>();
        var xamlFiles = GetAllXamlFiles();

        foreach (var xamlFile in xamlFiles)
        {
            var automationIds = ExtractAutomationIds(xamlFile);
            foreach (var automationId in automationIds)
            {
                pairs.Add((automationId, xamlFile));
            }
        }

        return pairs;
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates a random (AutomationId, XamlFilePath) pair from the Presentation layer.
    /// </summary>
    private static Arbitrary<(string AutomationId, string XamlFilePath)> GenerateAutomationIdWithSource()
    {
        var pairs = GetAllAutomationIdsWithSource();
        
        if (pairs.Count == 0)
        {
            throw new InvalidOperationException(
                "No AutomationIds found in Presentation layer XAML files. " +
                "Ensure AutomationIds have been added to XAML files before running this test.");
        }

        return Arb.From(Gen.Elements(pairs.ToArray()));
    }
}
