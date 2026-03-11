using System.Xml.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for FlaUI discoverability of UI elements with AutomationIds.
/// Validates that all elements with AutomationProperties.AutomationId can be discovered by FlaUI.
/// </summary>
public class FlaUIDiscoverabilityProperty : IDisposable
{
    // Feature: ui-automation-ids, Property 2: Universal FlaUI discoverability

    private const string PresentationProjectPath = "../../../../../src/Magidesk.Presentation";
    private ApplicationLauncher? _launcher;
    private bool _disposed;

    /// <summary>
    /// Feature: ui-automation-ids, Property 2: Universal FlaUI discoverability
    /// Validates: Requirements 1.5, 1.6, 1.7, 2.7, 3.10, 4.9, 5.9, 7.8
    /// 
    /// For any interactive UI element with an assigned AutomationProperties.AutomationId attribute,
    /// FlaUI must be able to discover and locate that element programmatically using the ByAutomationId search criteria.
    /// </summary>
    [Property(MaxTest = 100, Skip = "Requires application launch - run manually or in CI")]
    public Property AllAutomationIdsAreDiscoverableByFlaUI()
    {
        return Prop.ForAll(
            GeneratePageAndAutomationId(),
            pageAndId =>
            {
                var (xamlFilePath, automationId) = pageAndId;
                var pageName = Path.GetFileNameWithoutExtension(xamlFilePath);

                try
                {
                    // Arrange - Launch application if not already launched
                    EnsureApplicationLaunched();

                    if (_launcher?.Window == null)
                    {
                        throw new InvalidOperationException("Application window is not available");
                    }

                    // Navigate to the page (if not LoginPage which is the default)
                    NavigateToPage(pageName, _launcher.Window);

                    // Act - Search for element by AutomationId using FlaUI
                    var element = _launcher.Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));

                    // Assert - Element must be found and accessible
                    if (element == null)
                    {
                        throw new Exception(
                            $"FlaUI could not discover element with AutomationId '{automationId}' " +
                            $"in page '{pageName}'. The element exists in XAML but is not accessible via FlaUI.");
                    }

                    // Verify element is accessible (has basic properties)
                    var isAccessible = element.IsAvailable;
                    if (!isAccessible)
                    {
                        throw new Exception(
                            $"Element with AutomationId '{automationId}' in page '{pageName}' " +
                            $"was found but is not accessible (IsAvailable = false).");
                    }

                    return true;
                }
                catch (Exception ex) when (ex is not InvalidOperationException)
                {
                    // Clean up on failure
                    CleanupApplication();
                    throw;
                }
            });
    }

    /// <summary>
    /// Unit test to verify LoginPage elements are discoverable by FlaUI.
    /// This is a specific example test that complements the property test.
    /// </summary>
    [Fact(Skip = "Requires application launch - run manually or in CI")]
    public void LoginPage_AutomationIds_AreDiscoverableByFlaUI()
    {
        // Arrange
        var expectedAutomationIds = new[]
        {
            "UserSelectionGridView",
            "PinDisplayTextBlock",
            "LoginButton",
            "ErrorMessageTextBlock",
            "BackspaceButton"
        };

        try
        {
            EnsureApplicationLaunched();

            if (_launcher?.Window == null)
            {
                throw new InvalidOperationException("Application window is not available");
            }

            // Act & Assert - Verify each AutomationId is discoverable
            foreach (var automationId in expectedAutomationIds)
            {
                var element = _launcher.Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                
                Assert.NotNull(element);
                Assert.True(element.IsAvailable,
                    $"Element with AutomationId '{automationId}' is not accessible");
            }
        }
        finally
        {
            CleanupApplication();
        }
    }

    /// <summary>
    /// Unit test to verify SwitchboardPage elements are discoverable by FlaUI after login.
    /// </summary>
    [Fact(Skip = "Requires application launch and authentication - run manually or in CI")]
    public void SwitchboardPage_AutomationIds_AreDiscoverableByFlaUI()
    {
        // Arrange
        var expectedAutomationIds = new[]
        {
            "CurrentUserDisplay",
            "LogoutButton"
        };

        try
        {
            EnsureApplicationLaunched();

            if (_launcher?.Window == null)
            {
                throw new InvalidOperationException("Application window is not available");
            }

            // Authenticate to reach SwitchboardPage
            AuthenticateAsTestUser(_launcher.Window);

            // Wait for navigation to complete
            Thread.Sleep(2000);

            // Act & Assert - Verify each AutomationId is discoverable
            foreach (var automationId in expectedAutomationIds)
            {
                var element = _launcher.Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                
                Assert.NotNull(element);
                Assert.True(element.IsAvailable,
                    $"Element with AutomationId '{automationId}' is not accessible");
            }
        }
        finally
        {
            CleanupApplication();
        }
    }

    // ===== Helper Methods =====

    /// <summary>
    /// Ensures the application is launched and ready for testing.
    /// </summary>
    private void EnsureApplicationLaunched()
    {
        if (_launcher != null)
        {
            return; // Already launched
        }

        var executablePath = ApplicationLauncher.ResolveExecutablePath();
        _launcher = new ApplicationLauncher(executablePath);
        _launcher.Launch();
        _launcher.GetMainWindow(TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Cleans up the application launcher.
    /// </summary>
    private void CleanupApplication()
    {
        _launcher?.Dispose();
        _launcher = null;
    }

    /// <summary>
    /// Navigates to the specified page in the application.
    /// </summary>
    private void NavigateToPage(string pageName, Window window)
    {
        // LoginPage is the default page, no navigation needed
        if (pageName == "LoginPage")
        {
            return;
        }

        // For other pages, we need to authenticate first
        if (!IsAuthenticated(window))
        {
            AuthenticateAsTestUser(window);
            Thread.Sleep(2000); // Wait for navigation to SwitchboardPage
        }

        // Navigate to the target page from SwitchboardPage
        switch (pageName)
        {
            case "SwitchboardPage":
                // Already on SwitchboardPage after authentication
                break;

            case "OrderPageView":
            case "OrderEntryPage":
                NavigateToOrderEntry(window);
                break;

            case "SettlePageView":
            case "SettlementPage":
                // Need to create an order first, then navigate to settlement
                NavigateToOrderEntry(window);
                Thread.Sleep(1000);
                NavigateToSettlement(window);
                break;

            case "CashSessionPage":
                NavigateToCashSession(window);
                break;

            default:
                // For other pages, attempt to find and click navigation button by name
                TryNavigateByButtonName(window, pageName);
                break;
        }

        Thread.Sleep(1000); // Wait for page to load
    }

    /// <summary>
    /// Checks if the user is authenticated (on SwitchboardPage or beyond).
    /// </summary>
    private bool IsAuthenticated(Window window)
    {
        // Check if we can find the LogoutButton (present on SwitchboardPage)
        var logoutButton = window.FindFirstDescendant(cf => cf.ByAutomationId("LogoutButton"));
        return logoutButton != null;
    }

    /// <summary>
    /// Authenticates as a test user using PIN.
    /// </summary>
    private void AuthenticateAsTestUser(Window window)
    {
        // Default test PIN is "1234" (configured in test database seed)
        const string testPin = "1234";

        // Enter PIN
        foreach (char digit in testPin)
        {
            var digitButton = window.FindFirstDescendant(cf => cf.ByAutomationId($"Digit{digit}Button"));
            if (digitButton != null)
            {
                digitButton.AsButton().Invoke();
                Thread.Sleep(100);
            }
        }

        // Click Login button
        var loginButton = window.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"));
        if (loginButton != null)
        {
            loginButton.AsButton().Invoke();
        }
    }

    /// <summary>
    /// Navigates to Order Entry page from SwitchboardPage.
    /// </summary>
    private void NavigateToOrderEntry(Window window)
    {
        // Try to find Order Entry button by name (dynamic navigation buttons use AutomationProperties.Name)
        var orderEntryButton = window.FindFirstDescendant(cf => cf.ByName("Order Entry"));
        if (orderEntryButton != null)
        {
            orderEntryButton.AsButton().Invoke();
        }
    }

    /// <summary>
    /// Navigates to Settlement page from Order Entry page.
    /// </summary>
    private void NavigateToSettlement(Window window)
    {
        var settlementButton = window.FindFirstDescendant(cf => cf.ByAutomationId("SettlementButton"));
        if (settlementButton != null)
        {
            settlementButton.AsButton().Invoke();
        }
    }

    /// <summary>
    /// Navigates to Cash Session page from SwitchboardPage.
    /// </summary>
    private void NavigateToCashSession(Window window)
    {
        var cashSessionButton = window.FindFirstDescendant(cf => cf.ByName("Cash Session"));
        if (cashSessionButton != null)
        {
            cashSessionButton.AsButton().Invoke();
        }
    }

    /// <summary>
    /// Attempts to navigate to a page by finding and clicking a button with the page name.
    /// </summary>
    private void TryNavigateByButtonName(Window window, string pageName)
    {
        // Try to find button by name (remove "Page" or "View" suffix)
        var buttonName = pageName
            .Replace("Page", "")
            .Replace("View", "")
            .Trim();

        var button = window.FindFirstDescendant(cf => cf.ByName(buttonName));
        if (button != null)
        {
            button.AsButton().Invoke();
        }
    }

    /// <summary>
    /// Extracts all AutomationProperties.AutomationId values from a XAML file.
    /// </summary>
    private static List<string> ExtractAutomationIds(string xamlFilePath)
    {
        if (!File.Exists(xamlFilePath))
        {
            return new List<string>();
        }

        var doc = XDocument.Load(xamlFilePath);
        var automationIds = new List<string>();

        // Search for AutomationProperties.AutomationId attributes
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
    /// Gets all XAML files from the Presentation layer that have AutomationIds.
    /// </summary>
    private static List<string> GetXamlFilesWithAutomationIds()
    {
        var viewsPath = Path.Combine(PresentationProjectPath, "Views");
        
        if (!Directory.Exists(viewsPath))
        {
            throw new DirectoryNotFoundException($"Views directory not found: {viewsPath}");
        }

        var xamlFiles = Directory.GetFiles(viewsPath, "*.xaml", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith(".xaml.cs")) // Exclude code-behind files
            .Where(f => ExtractAutomationIds(f).Any()) // Only files with AutomationIds
            .ToList();

        return xamlFiles;
    }

    /// <summary>
    /// Gets all (page, AutomationId) pairs from XAML files.
    /// </summary>
    private static List<(string XamlFilePath, string AutomationId)> GetAllPageAutomationIdPairs()
    {
        var pairs = new List<(string, string)>();
        var xamlFiles = GetXamlFilesWithAutomationIds();

        foreach (var xamlFile in xamlFiles)
        {
            var automationIds = ExtractAutomationIds(xamlFile);
            foreach (var automationId in automationIds)
            {
                pairs.Add((xamlFile, automationId));
            }
        }

        return pairs;
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates a random (page, AutomationId) pair from the Presentation layer.
    /// </summary>
    private static Arbitrary<(string XamlFilePath, string AutomationId)> GeneratePageAndAutomationId()
    {
        var pairs = GetAllPageAutomationIdPairs();
        
        if (!pairs.Any())
        {
            throw new InvalidOperationException(
                "No XAML files with AutomationIds found in Presentation layer. " +
                "Ensure AutomationIds have been added to XAML files before running this test.");
        }

        return Arb.From(Gen.Elements(pairs.ToArray()));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        CleanupApplication();
        _disposed = true;
    }
}
