using System.Xml.Linq;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for AutomationId uniqueness across XAML pages.
/// Validates that all AutomationProperties.AutomationId values are unique within each page.
/// </summary>
public class AutomationIdUniquenessProperty
{
    // Feature: ui-automation-ids, Property 1: AutomationId uniqueness within pages

    private const string PresentationProjectPath = "../../../../../src/Magidesk.Presentation";
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private static readonly XNamespace XamlXNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Feature: ui-automation-ids, Property 1: AutomationId uniqueness within pages
    /// Validates: Requirements 7.2
    /// 
    /// For any XAML page in the Presentation layer, all AutomationProperties.AutomationId values
    /// assigned to elements within that page must be unique (no duplicate AutomationIds within a single page).
    /// </summary>
    [Property(MaxTest = 100)]
    public Property AllAutomationIdsAreUniqueWithinEachPage()
    {
        return Prop.ForAll(
            GenerateXamlFilePath(),
            xamlFilePath =>
            {
                // Arrange - Parse XAML file
                var automationIds = ExtractAutomationIds(xamlFilePath);

                // Act - Check for duplicates
                var duplicates = automationIds
                    .GroupBy(id => id)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                // Assert - No duplicates should exist
                if (duplicates.Any())
                {
                    var fileName = Path.GetFileName(xamlFilePath);
                    throw new Exception(
                        $"Duplicate AutomationIds found in {fileName}: {string.Join(", ", duplicates)}");
                }

                return true;
            });
    }

    /// <summary>
    /// Unit test to verify that LoginPage has unique AutomationIds.
    /// This is a specific example test that complements the property test.
    /// </summary>
    [Fact]
    public void LoginPage_HasUniqueAutomationIds()
    {
        // Arrange
        var loginPagePath = Path.Combine(PresentationProjectPath, "Views", "LoginPage.xaml");

        // Act
        var automationIds = ExtractAutomationIds(loginPagePath);
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Unit test to verify that SwitchboardPage has unique AutomationIds.
    /// </summary>
    [Fact]
    public void SwitchboardPage_HasUniqueAutomationIds()
    {
        // Arrange
        var switchboardPagePath = Path.Combine(PresentationProjectPath, "Views", "SwitchboardPage.xaml");

        // Act
        var automationIds = ExtractAutomationIds(switchboardPagePath);
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Unit test to verify that OrderPageView has unique AutomationIds.
    /// </summary>
    [Fact]
    public void OrderPageView_HasUniqueAutomationIds()
    {
        // Arrange
        var orderPagePath = Path.Combine(PresentationProjectPath, "Views", "OrderPageView.xaml");

        // Act
        var automationIds = ExtractAutomationIds(orderPagePath);
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Unit test to verify that SettlePageView has unique AutomationIds.
    /// </summary>
    [Fact]
    public void SettlePageView_HasUniqueAutomationIds()
    {
        // Arrange
        var settlePagePath = Path.Combine(PresentationProjectPath, "Views", "SettlePageView.xaml");

        // Act
        var automationIds = ExtractAutomationIds(settlePagePath);
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
    }

    /// <summary>
    /// Unit test to verify that CashSessionPage has unique AutomationIds.
    /// </summary>
    [Fact]
    public void CashSessionPage_HasUniqueAutomationIds()
    {
        // Arrange
        var cashSessionPagePath = Path.Combine(PresentationProjectPath, "Views", "CashSessionPage.xaml");

        // Act
        var automationIds = ExtractAutomationIds(cashSessionPagePath);
        var duplicates = automationIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        Assert.Empty(duplicates);
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
        // This can appear in multiple namespace formats
        var automationIdAttributes = doc.Descendants()
            .SelectMany(e => e.Attributes())
            .Where(a => a.Name.LocalName == "AutomationId" && 
                       (a.Name.Namespace.NamespaceName.Contains("AutomationProperties") ||
                        a.Name.Namespace == XNamespace.None))
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

    // ===== Property Generators =====

    /// <summary>
    /// Generates a random XAML file path from the Presentation layer.
    /// </summary>
    private static Arbitrary<string> GenerateXamlFilePath()
    {
        var xamlFiles = GetAllXamlFiles();
        
        if (!xamlFiles.Any())
        {
            throw new InvalidOperationException("No XAML files found in Presentation layer");
        }

        return Arb.From(Gen.Elements(xamlFiles.ToArray()));
    }
}
