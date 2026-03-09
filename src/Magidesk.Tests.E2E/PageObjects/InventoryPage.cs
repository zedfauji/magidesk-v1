using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the Inventory Management page.
/// Wraps interactions with search, filtering, category selection,
/// item listing, pagination, and bulk edit.
/// </summary>
public sealed class InventoryPage : BasePage
{
    // Automation IDs matching InventoryPage.xaml
    private const string SearchBoxId = "InventorySearchBox";
    private const string FilterAllId = "InventoryFilterAll";
    private const string FilterLowStockId = "InventoryFilterLowStock";
    private const string FilterOutOfStockId = "InventoryFilterOutOfStock";
    private const string FilterRecentlyAddedId = "InventoryFilterRecentlyAdded";
    private const string CategoryListId = "InventoryCategoryList";
    private const string ItemListId = "InventoryItemList";
    private const string PreviousPageButtonId = "InventoryPreviousPageButton";
    private const string NextPageButtonId = "InventoryNextPageButton";
    private const string StatusMessageId = "InventoryStatusMessage";
    private const string TotalCountId = "InventoryTotalCount";
    private const string BulkEditBarId = "InventoryBulkEditBar";
    private const string BulkEditButtonId = "InventoryBulkEditButton";

    public InventoryPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Waits for the Inventory page to finish loading by waiting for the
    /// status message element to appear. Call after navigation.
    /// </summary>
    public void WaitForPageLoaded()
    {
        // Status message is the last element populated when page loads
        Infrastructure.WaitHelpers.WaitForElementByAutomationId(Window, StatusMessageId, DefaultTimeout);
    }

    /// <summary>
    /// Gets the current status message text (e.g. "Loaded 10 items (Page 1)").
    /// </summary>
    public string GetStatusMessage() => GetText(StatusMessageId);

    /// <summary>
    /// Gets the total item count displayed in the pagination row.
    /// </summary>
    public int GetTotalCount()
    {
        var text = GetText(TotalCountId);
        return int.TryParse(text, out var count) ? count : 0;
    }

    /// <summary>
    /// Types a search term into the search box. Triggers the debounced search.
    /// </summary>
    public void SearchFor(string term)
    {
        var searchBox = FindElement(SearchBoxId);
        Infrastructure.WaitHelpers.WaitForElementEnabled(searchBox, DefaultTimeout);
        searchBox.AsTextBox().Text = term;
        // Allow debounce (300 ms defined in ViewModel) + render time
        Thread.Sleep(600);
    }

    /// <summary>
    /// Clears the search box.
    /// </summary>
    public void ClearSearch()
    {
        var searchBox = FindElement(SearchBoxId);
        Infrastructure.WaitHelpers.WaitForElementEnabled(searchBox, DefaultTimeout);
        searchBox.AsTextBox().Text = string.Empty;
        Thread.Sleep(600);
    }

    /// <summary>
    /// Clicks the "All" filter radio button.
    /// </summary>
    public void SelectFilterAll() => ClickButton(FilterAllId);

    /// <summary>
    /// Clicks the "Low Stock" filter radio button.
    /// </summary>
    public void SelectFilterLowStock() => ClickButton(FilterLowStockId);

    /// <summary>
    /// Clicks the "Out of Stock" filter radio button.
    /// </summary>
    public void SelectFilterOutOfStock() => ClickButton(FilterOutOfStockId);

    /// <summary>
    /// Clicks the "Recently Added" filter radio button.
    /// </summary>
    public void SelectFilterRecentlyAdded() => ClickButton(FilterRecentlyAddedId);

    /// <summary>
    /// Returns whether the "All" filter radio button is currently checked.
    /// </summary>
    public bool IsFilterAllSelected()
    {
        var radio = FindElement(FilterAllId).AsRadioButton();
        return radio.IsChecked;
    }

    /// <summary>
    /// Returns whether the "Low Stock" filter radio button is currently checked.
    /// </summary>
    public bool IsFilterLowStockSelected()
    {
        var radio = FindElement(FilterLowStockId).AsRadioButton();
        return radio.IsChecked;
    }

    /// <summary>
    /// Counts the number of items currently visible in the inventory ListView.
    /// Returns -1 if the list element cannot be found.
    /// </summary>
    public int GetInventoryItemCount()
    {
        try
        {
            var list = FindElement(ItemListId).AsListBox();
            return list.Items.Length;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Selects a category by its display name in the category ListBox panel.
    /// </summary>
    public void SelectCategory(string categoryName)
    {
        var list = FindElement(CategoryListId).AsListBox();
        var item = list.Items.FirstOrDefault(i => i.Text == categoryName);
        if (item is null)
            throw new InvalidOperationException($"Category '{categoryName}' was not found in the category list.");
        item.Select();
        Thread.Sleep(500); // Allow filter to apply
    }

    /// <summary>
    /// Returns whether the bulk edit action bar is visible on screen.
    /// </summary>
    public bool IsBulkEditBarVisible()
    {
        try
        {
            var bar = Window.FindFirstDescendant(cf => cf.ByAutomationId(BulkEditBarId));
            return bar != null && bar.IsAvailable && !bar.IsOffscreen;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether the "Next" pagination button is currently enabled.
    /// </summary>
    public bool IsNextPageEnabled()
    {
        try
        {
            return FindElement(NextPageButtonId).IsEnabled;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether the "Previous" pagination button is currently enabled.
    /// </summary>
    public bool IsPreviousPageEnabled()
    {
        try
        {
            return FindElement(PreviousPageButtonId).IsEnabled;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Clicks "Next" to advance to the next page of results.
    /// </summary>
    public void GoToNextPage()
    {
        ClickButton(NextPageButtonId);
        Thread.Sleep(500);
    }

    /// <summary>
    /// Clicks "Previous" to go back to the previous page of results.
    /// </summary>
    public void GoToPreviousPage()
    {
        ClickButton(PreviousPageButtonId);
        Thread.Sleep(500);
    }

    /// <summary>
    /// Clicks the "Bulk Edit" action button in the bulk action bar.
    /// Requires that at least one item is selected.
    /// </summary>
    public void ClickBulkEdit()
    {
        ClickButton(BulkEditButtonId);
    }
}
