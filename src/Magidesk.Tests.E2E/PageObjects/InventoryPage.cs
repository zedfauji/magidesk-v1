using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for inventory management operations.
/// </summary>
public sealed class InventoryPage : BasePage
{
    // Inventory control AutomationIds
    private const string ItemNameTextBoxId = "ItemNameTextBox";
    private const string StockLevelTextBlockId = "StockLevelTextBlock";
    private const string QuantityTextBoxId = "QuantityTextBox";
    private const string ReasonTextBoxId = "ReasonTextBox";
    private const string AdjustInventoryButtonId = "AdjustInventoryButton";
    
    // Purchase order controls
    private const string VendorTextBoxId = "VendorTextBox";
    private const string CreatePurchaseOrderButtonId = "CreatePurchaseOrderButton";
    private const string PurchaseOrderNumberTextBoxId = "PurchaseOrderNumberTextBox";
    private const string ReceivePurchaseOrderButtonId = "ReceivePurchaseOrderButton";
    
    // Search and alerts
    private const string SearchTextBoxId = "SearchTextBox";
    private const string SearchButtonId = "SearchButton";
    private const string LowStockAlertsListId = "LowStockAlertsList";

    public InventoryPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Gets the current stock level for an inventory item.
    /// </summary>
    /// <param name="itemName">The name of the inventory item.</param>
    /// <returns>The current stock quantity.</returns>
    public int GetStockLevel(string itemName)
    {
        EnterText(ItemNameTextBoxId, itemName);
        var stockText = GetText(StockLevelTextBlockId);
        return int.Parse(stockText);
    }

    /// <summary>
    /// Adjusts inventory quantity with a reason.
    /// </summary>
    /// <param name="itemName">The name of the inventory item.</param>
    /// <param name="quantity">The quantity adjustment (positive or negative).</param>
    /// <param name="reason">The reason for the adjustment.</param>
    public void AdjustInventory(string itemName, int quantity, string reason)
    {
        EnterText(ItemNameTextBoxId, itemName);
        EnterText(QuantityTextBoxId, quantity.ToString());
        EnterText(ReasonTextBoxId, reason);
        ClickButton(AdjustInventoryButtonId);
    }

    /// <summary>
    /// Creates a purchase order with vendor and items.
    /// </summary>
    /// <param name="vendor">The vendor name.</param>
    /// <param name="items">Array of tuples containing item name and quantity.</param>
    public void CreatePurchaseOrder(string vendor, params (string item, int quantity)[] items)
    {
        EnterText(VendorTextBoxId, vendor);
        
        foreach (var (item, quantity) in items)
        {
            EnterText(ItemNameTextBoxId, item);
            EnterText(QuantityTextBoxId, quantity.ToString());
        }
        
        ClickButton(CreatePurchaseOrderButtonId);
    }

    /// <summary>
    /// Receives a purchase order and updates inventory.
    /// </summary>
    /// <param name="poNumber">The purchase order number.</param>
    public void ReceivePurchaseOrder(string poNumber)
    {
        EnterText(PurchaseOrderNumberTextBoxId, poNumber);
        ClickButton(ReceivePurchaseOrderButtonId);
    }

    /// <summary>
    /// Gets the list of low stock alerts.
    /// </summary>
    /// <returns>Enumerable of item names with low stock.</returns>
    public IEnumerable<string> GetLowStockAlerts()
    {
        var alertsList = FindElement(LowStockAlertsListId);
        var items = alertsList.FindAllChildren();
        
        return items.Select(item => item.Name).ToList();
    }

    /// <summary>
    /// Searches for an inventory item.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    public void SearchInventoryItem(string searchTerm)
    {
        EnterText(SearchTextBoxId, searchTerm);
        ClickButton(SearchButtonId);
    }
}
