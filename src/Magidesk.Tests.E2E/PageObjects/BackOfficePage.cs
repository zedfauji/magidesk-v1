using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the Back Office page (BackOfficePage.xaml).
/// Handles navigating to sub-sections via the left-side NavigationView.
/// </summary>
public sealed class BackOfficePage : BasePage
{
    public BackOfficePage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Waits for the Back Office page to finish loading by waiting for
    /// the NavigationView to become available.
    /// </summary>
    public void WaitForPageLoaded()
    {
        // NavigationView is always present on Back Office; we wait for it to render
        Thread.Sleep(1500); // Allow navigation animation to complete
    }

    /// <summary>
    /// Clicks the "Inventory" item in the Back Office NavigationView left panel.
    /// </summary>
    public void NavigateToInventory()
    {
        // The NavigationView items are rendered as list items with text = LocalizedTitle.
        // In en-US, "BO_Nav_Inventory" resolves to "Inventory".
        ClickNavigationItem("Inventory");
        Thread.Sleep(1000); // Allow page transition and initial data load
    }

    /// <summary>
    /// Clicks any navigation item in the Back Office pane by its display text.
    /// </summary>
    public void ClickNavigationItem(string itemName)
    {
        var navItem = Window.FindFirstDescendant(
            cf => cf.ByName(itemName).And(cf.ByControlType(FlaUI.Core.Definitions.ControlType.ListItem)));

        if (navItem == null)
        {
            // Some WinUI versions render NavigationViewItem as TreeViewItem or custom control type
            navItem = Window.FindFirstDescendant(cf => cf.ByName(itemName));
        }

        if (navItem == null)
            throw new InvalidOperationException($"Navigation item '{itemName}' not found in the Back Office navigation pane.");

        navItem.Click();
        Thread.Sleep(500);
    }
}
