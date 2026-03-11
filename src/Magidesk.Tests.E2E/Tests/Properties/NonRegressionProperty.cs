using System.Xml.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Xunit;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for non-regression of existing functionality after adding AutomationIds.
/// Validates that adding AutomationProperties.AutomationId attributes does not break existing UI behavior or data bindings.
/// </summary>
public class NonRegressionProperty : IDisposable
{
    // Feature: ui-automation-ids, Property 3: Non-regression of existing functionality

    private const string PresentationProjectPath = "../../../../../src/Magidesk.Presentation";
    private ApplicationLauncher? _launcher;
    private bool _disposed;

    /// <summary>
    /// Feature: ui-automation-ids, Property 3: Non-regression of existing functionality
    /// Validates: Requirements 6.5, 6.6
    /// 
    /// For any XAML page modified to include AutomationId attributes, the existing UI behavior
    /// and data binding behavior must remain unchanged (adding AutomationId attributes must not
    /// break existing functionality).
    /// </summary>
    [Property(MaxTest = 100, Skip = "Requires application launch - run manually or in CI")]
    public Property AddingAutomationIdsDoesNotBreakExistingFunctionality()
    {
        return Prop.ForAll(
            GeneratePageWithAutomationIds(),
            xamlFilePath =>
            {
                var pageName = Path.GetFileNameWithoutExtension(xamlFilePath);

                try
                {
                    // Arrange - Launch application if not already launched
                    EnsureApplicationLaunched();

                    if (_launcher?.Window == null)
                    {
                        throw new InvalidOperationException("Application window is not available");
                    }

                    // Navigate to the page
                    NavigateToPage(pageName, _launcher.Window);

                    // Act & Assert - Verify existing functionality still works
                    VerifyPageFunctionality(pageName, _launcher.Window);

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
    /// Unit test to verify LoginPage functionality is not broken by AutomationIds.
    /// </summary>
    [Fact(Skip = "Requires application launch - run manually or in CI")]
    public void LoginPage_FunctionalityNotBrokenByAutomationIds()
    {
        try
        {
            // Arrange
            EnsureApplicationLaunched();

            if (_launcher?.Window == null)
            {
                throw new InvalidOperationException("Application window is not available");
            }

            // Act - Verify LoginPage functionality
            VerifyLoginPageFunctionality(_launcher.Window);

            // Assert - If we reach here, functionality is intact
            Assert.True(true);
        }
        finally
        {
            CleanupApplication();
        }
    }

    /// <summary>
    /// Unit test to verify SwitchboardPage functionality is not broken by AutomationIds.
    /// </summary>
    [Fact(Skip = "Requires application launch and authentication - run manually or in CI")]
    public void SwitchboardPage_FunctionalityNotBrokenByAutomationIds()
    {
        try
        {
            // Arrange
            EnsureApplicationLaunched();

            if (_launcher?.Window == null)
            {
                throw new InvalidOperationException("Application window is not available");
            }

            // Authenticate to reach SwitchboardPage
            AuthenticateAsTestUser(_launcher.Window);
            Thread.Sleep(2000);

            // Act - Verify SwitchboardPage functionality
            VerifySwitchboardPageFunctionality(_launcher.Window);

            // Assert - If we reach here, functionality is intact
            Assert.True(true);
        }
        finally
        {
            CleanupApplication();
        }
    }

    /// <summary>
    /// Unit test to verify OrderPageView functionality is not broken by AutomationIds.
    /// </summary>
    [Fact(Skip = "Requires application launch and authentication - run manually or in CI")]
    public void OrderPageView_FunctionalityNotBrokenByAutomationIds()
    {
        try
        {
            // Arrange
            EnsureApplicationLaunched();

            if (_launcher?.Window == null)
            {
                throw new InvalidOperationException("Application window is not available");
            }

            // Authenticate and navigate to Order Entry
            AuthenticateAsTestUser(_launcher.Window);
            Thread.Sleep(2000);
            NavigateToOrderEntry(_launcher.Window);
            Thread.Sleep(1000);

            // Act - Verify OrderPageView functionality
            VerifyOrderPageViewFunctionality(_launcher.Window);

            // Assert - If we reach here, functionality is intact
            Assert.True(true);
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
    /// Verifies that the page functionality is intact (no broken bindings or commands).
    /// </summary>
    private void VerifyPageFunctionality(string pageName, Window window)
    {
        switch (pageName)
        {
            case "LoginPage":
                VerifyLoginPageFunctionality(window);
                break;

            case "SwitchboardPage":
                VerifySwitchboardPageFunctionality(window);
                break;

            case "OrderPageView":
            case "OrderEntryPage":
                VerifyOrderPageViewFunctionality(window);
                break;

            case "SettlePageView":
            case "SettlementPage":
                VerifySettlePageViewFunctionality(window);
                break;

            case "CashSessionPage":
                VerifyCashSessionPageFunctionality(window);
                break;

            default:
                // For other pages, perform basic verification
                VerifyBasicPageFunctionality(window);
                break;
        }
    }

    /// <summary>
    /// Verifies LoginPage functionality: PIN entry, login button, error display.
    /// </summary>
    private void VerifyLoginPageFunctionality(Window window)
    {
        // Verify PIN entry buttons are clickable
        var digit1Button = window.FindFirstDescendant(cf => cf.ByAutomationId("Digit1Button"));
        if (digit1Button == null)
        {
            throw new Exception("LoginPage: Digit1Button not found - data binding may be broken");
        }

        if (!digit1Button.IsEnabled)
        {
            throw new Exception("LoginPage: Digit1Button is not enabled - UI behavior may be broken");
        }

        // Verify login button exists and is accessible
        var loginButton = window.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"));
        if (loginButton == null)
        {
            throw new Exception("LoginPage: LoginButton not found - data binding may be broken");
        }

        // Verify error message display exists (even if not visible)
        var errorMessage = window.FindFirstDescendant(cf => cf.ByAutomationId("ErrorMessageTextBlock"));
        if (errorMessage == null)
        {
            throw new Exception("LoginPage: ErrorMessageTextBlock not found - data binding may be broken");
        }

        // Test basic interaction: click a digit button
        digit1Button.AsButton().Invoke();
        Thread.Sleep(100);

        // Verify PIN display is updated (basic data binding check)
        var pinDisplay = window.FindFirstDescendant(cf => cf.ByAutomationId("PinDisplayTextBlock"));
        if (pinDisplay != null)
        {
            var pinText = pinDisplay.AsLabel().Text;
            if (string.IsNullOrEmpty(pinText))
            {
                throw new Exception("LoginPage: PIN display not updated after digit click - data binding may be broken");
            }
        }
    }

    /// <summary>
    /// Verifies SwitchboardPage functionality: navigation buttons, user display, logout.
    /// </summary>
    private void VerifySwitchboardPageFunctionality(Window window)
    {
        // Verify current user display exists and has content
        var currentUserDisplay = window.FindFirstDescendant(cf => cf.ByAutomationId("CurrentUserDisplay"));
        if (currentUserDisplay == null)
        {
            throw new Exception("SwitchboardPage: CurrentUserDisplay not found - data binding may be broken");
        }

        var userDisplayText = currentUserDisplay.AsLabel().Text;
        if (string.IsNullOrWhiteSpace(userDisplayText))
        {
            throw new Exception("SwitchboardPage: CurrentUserDisplay has no text - data binding may be broken");
        }

        // Verify logout button exists and is enabled
        var logoutButton = window.FindFirstDescendant(cf => cf.ByAutomationId("LogoutButton"));
        if (logoutButton == null)
        {
            throw new Exception("SwitchboardPage: LogoutButton not found - data binding may be broken");
        }

        if (!logoutButton.IsEnabled)
        {
            throw new Exception("SwitchboardPage: LogoutButton is not enabled - UI behavior may be broken");
        }

        // Verify navigation buttons exist (dynamic content with AutomationProperties.Name)
        var orderEntryButton = window.FindFirstDescendant(cf => cf.ByName("Order Entry"));
        if (orderEntryButton == null)
        {
            throw new Exception("SwitchboardPage: Order Entry button not found - data binding may be broken");
        }

        if (!orderEntryButton.IsEnabled)
        {
            throw new Exception("SwitchboardPage: Order Entry button is not enabled - UI behavior may be broken");
        }
    }

    /// <summary>
    /// Verifies OrderPageView functionality: menu items, buttons, displays.
    /// </summary>
    private void VerifyOrderPageViewFunctionality(Window window)
    {
        // Verify menu items list exists
        var menuItemsList = window.FindFirstDescendant(cf => cf.ByAutomationId("MenuItemsList"));
        if (menuItemsList == null)
        {
            throw new Exception("OrderPageView: MenuItemsList not found - data binding may be broken");
        }

        // Verify action buttons exist and are accessible
        var holdButton = window.FindFirstDescendant(cf => cf.ByAutomationId("HoldButton"));
        if (holdButton == null)
        {
            throw new Exception("OrderPageView: HoldButton not found - data binding may be broken");
        }

        var settlementButton = window.FindFirstDescendant(cf => cf.ByAutomationId("SettlementButton"));
        if (settlementButton == null)
        {
            throw new Exception("OrderPageView: SettlementButton not found - data binding may be broken");
        }

        // Verify display elements exist
        var ticketTotalDisplay = window.FindFirstDescendant(cf => cf.ByAutomationId("TicketTotalDisplay"));
        if (ticketTotalDisplay == null)
        {
            throw new Exception("OrderPageView: TicketTotalDisplay not found - data binding may be broken");
        }
    }

    /// <summary>
    /// Verifies SettlePageView functionality: payment controls, buttons, displays.
    /// </summary>
    private void VerifySettlePageViewFunctionality(Window window)
    {
        // Verify payment method combo box exists
        var paymentMethodComboBox = window.FindFirstDescendant(cf => cf.ByAutomationId("PaymentMethodComboBox"));
        if (paymentMethodComboBox == null)
        {
            throw new Exception("SettlePageView: PaymentMethodComboBox not found - data binding may be broken");
        }

        // Verify process payment button exists
        var processPaymentButton = window.FindFirstDescendant(cf => cf.ByAutomationId("ProcessPaymentButton"));
        if (processPaymentButton == null)
        {
            throw new Exception("SettlePageView: ProcessPaymentButton not found - data binding may be broken");
        }

        // Verify display elements exist
        var amountDueDisplay = window.FindFirstDescendant(cf => cf.ByAutomationId("AmountDueDisplay"));
        if (amountDueDisplay == null)
        {
            throw new Exception("SettlePageView: AmountDueDisplay not found - data binding may be broken");
        }
    }

    /// <summary>
    /// Verifies CashSessionPage functionality: session controls, displays.
    /// </summary>
    private void VerifyCashSessionPageFunctionality(Window window)
    {
        // Verify open session button exists
        var openSessionButton = window.FindFirstDescendant(cf => cf.ByAutomationId("OpenSessionButton"));
        if (openSessionButton == null)
        {
            throw new Exception("CashSessionPage: OpenSessionButton not found - data binding may be broken");
        }

        // Verify starting cash text box exists
        var startingCashTextBox = window.FindFirstDescendant(cf => cf.ByAutomationId("StartingCashTextBox"));
        if (startingCashTextBox == null)
        {
            throw new Exception("CashSessionPage: StartingCashTextBox not found - data binding may be broken");
        }

        // Verify session status display exists
        var sessionStatusDisplay = window.FindFirstDescendant(cf => cf.ByAutomationId("SessionStatusDisplay"));
        if (sessionStatusDisplay == null)
        {
            throw new Exception("CashSessionPage: SessionStatusDisplay not found - data binding may be broken");
        }
    }

    /// <summary>
    /// Performs basic verification for pages without specific test logic.
    /// </summary>
    private void VerifyBasicPageFunctionality(Window window)
    {
        // Verify the window is still responsive
        if (!window.IsAvailable)
        {
            throw new Exception("Window is not available - application may have crashed");
        }

        // Verify we can find at least one interactive element
        var buttons = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));
        if (!buttons.Any())
        {
            throw new Exception("No buttons found on page - UI may be broken");
        }
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

    // ===== Property Generators =====

    /// <summary>
    /// Generates a random XAML page path that has AutomationIds.
    /// </summary>
    private static Arbitrary<string> GeneratePageWithAutomationIds()
    {
        var xamlFiles = GetXamlFilesWithAutomationIds();
        
        if (!xamlFiles.Any())
        {
            throw new InvalidOperationException(
                "No XAML files with AutomationIds found in Presentation layer. " +
                "Ensure AutomationIds have been added to XAML files before running this test.");
        }

        return Arb.From(Gen.Elements(xamlFiles.ToArray()));
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
