using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P1_OperationalIntegrity;

/// <summary>
/// P1 UI flow tests for Inventory Management.
/// Validates page load, search, filtering, category selection, and pagination.
///
/// Prerequisites:
/// - The Magidesk.Presentation project must be built in x86 Debug configuration before running.
/// - Use: dotnet test Magidesk.Tests.E2E --filter "Category=Inventory" -p:Platform=x86
/// </summary>
[Trait("Priority", "P1")]
[Trait("Category", "Inventory")]
public class InventoryManagementTests : BaseE2ETest
{
    private const string DefaultPin = "1234";

    public InventoryManagementTests(ITestOutputHelper output) : base(output)
    {
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Full navigation flow:
    ///   1. Enter PIN on the Login page
    ///   2. Click "Back Office" on the Switchboard
    ///   3. Handle Manager PIN dialog if it appears (optional based on permissions)
    ///   4. Click "Inventory" in the Back Office NavigationView
    ///   5. Wait for the Inventory page to load
    /// </summary>
    private InventoryPage NavigateToInventory()
    {
        // Step 1: Login
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin(DefaultPin);

        // Step 2: Click Back Office on Switchboard
        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToBackOffice();

        // Step 3: Handle the Password Entry dialog
        // This dialog appears when accessing Back Office
        Thread.Sleep(1500); // Allow navigation and dialog to appear
        
        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm(DefaultPin);

        // Step 4: Navigate to Inventory in the Back Office left panel
        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.NavigateToInventory();

        // Step 5: Wait for the Inventory page to finish loading
        var inventoryPage = new InventoryPage(MainWindow!);
        inventoryPage.WaitForPageLoaded();

        return inventoryPage;
    }

    // ─── Tests ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the Inventory page loads correctly after login and displays a status message.
    /// </summary>
    [Fact]
    public void InventoryPage_Loads_ShowsStatusMessage()
    {
        // Arrange + Act
        var inventory = NavigateToInventory();

        // Assert
        var statusMessage = inventory.GetStatusMessage();
        Assert.False(
            string.IsNullOrWhiteSpace(statusMessage),
            $"Expected a non-empty status message but got: '{statusMessage}'");
    }

    /// <summary>
    /// Verifies that typing a term in the search box triggers a filtered result.
    /// After a keyword search the total item count should drop (or stay the same if
    /// no matching items exist — the status message should change to reflect this).
    /// </summary>
    [Fact]
    public void SearchBox_TypeKeyword_UpdatesStatusMessage()
    {
        // Arrange
        var inventory = NavigateToInventory();
        var initialStatus = inventory.GetStatusMessage();

        // Act — type a search term and wait for the debounce + reload
        inventory.SearchFor("coffee");
        var afterSearchStatus = inventory.GetStatusMessage();

        // Assert — status message must have updated (reload occurred)
        // We compare strings; even "Loaded 0 items" is a valid, changed status.
        Assert.NotEqual(initialStatus, afterSearchStatus);
    }

    /// <summary>
    /// Verifies that clearing the search box after a search resets the results.
    /// </summary>
    [Fact]
    public void SearchBox_ClearSearch_RestoresFullList()
    {
        // Arrange
        var inventory = NavigateToInventory();
        inventory.SearchFor("coffee");
        var filteredStatus = inventory.GetStatusMessage();

        // Act
        inventory.ClearSearch();
        var restoredStatus = inventory.GetStatusMessage();

        // Assert — status after clearing must differ from the filtered status
        Assert.NotEqual(filteredStatus, restoredStatus);
    }

    /// <summary>
    /// Verifies that clicking the "Low Stock" filter radio button activates that filter.
    /// The radio button IsChecked property should become true, and "All" should become false.
    /// </summary>
    [Fact]
    public void FilterChip_LowStock_BecomesActive()
    {
        // Arrange
        var inventory = NavigateToInventory();

        // Pre-condition: "All" should be selected by default
        Assert.True(
            inventory.IsFilterAllSelected(),
            "Expected 'All' filter to be active on page load.");

        // Act
        inventory.SelectFilterLowStock();
        Thread.Sleep(300); // Allow binding propagation

        // Assert
        Assert.True(
            inventory.IsFilterLowStockSelected(),
            "Expected 'Low Stock' filter to be selected after clicking the Low Stock radio button.");

        Assert.False(
            inventory.IsFilterAllSelected(),
            "Expected 'All' filter to be deselected after switching to Low Stock.");
    }

    /// <summary>
    /// Verifies that switching from a non-default filter back to "All" restores the
    /// unfiltered filter state.
    /// </summary>
    [Fact]
    public void FilterChip_SwitchBackToAll_DeactivatesOtherFilter()
    {
        // Arrange
        var inventory = NavigateToInventory();
        inventory.SelectFilterLowStock();
        Thread.Sleep(300);

        // Act — switch back to "All"
        inventory.SelectFilterAll();
        Thread.Sleep(300);

        // Assert
        Assert.True(
            inventory.IsFilterAllSelected(),
            "Expected 'All' filter to be active after clicking All.");

        Assert.False(
            inventory.IsFilterLowStockSelected(),
            "Expected 'Low Stock' filter to be deselected after switching to All.");
    }

    /// <summary>
    /// Verifies that the "Previous" pagination button is disabled on the first page,
    /// as there is nothing to go back to.
    /// </summary>
    [Fact]
    public void Pagination_PreviousButton_DisabledOnFirstPage()
    {
        // Arrange + Act
        var inventory = NavigateToInventory();

        // Assert
        Assert.False(
            inventory.IsPreviousPageEnabled(),
            "Expected 'Previous' button to be disabled when on the first page.");
    }
}
