using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the order entry page.
/// </summary>
public sealed class OrderEntryPage : BasePage
{
    private const string MenuItemsListViewId = "MenuItemsListView";
    private const string ModifiersListViewId = "ModifiersListView";
    private const string QuantityTextBoxId = "QuantityTextBox";
    private const string DiscountComboBoxId = "DiscountComboBox";
    private const string HoldTicketButtonId = "HoldTicketButton";
    private const string RecallTicketButtonId = "RecallTicketButton";
    private const string SettlementButtonId = "SettlementButton";
    private const string TicketTotalTextBlockId = "TicketTotalTextBlock";
    private const string ItemCountTextBlockId = "ItemCountTextBlock";
    private const string SendToKitchenButtonId = "SendToKitchenButton";
    private const string VoidTicketButtonId = "VoidTicketButton";
    private const string PendingKitchenItemsIndicatorId = "PendingKitchenItemsIndicator";

    public OrderEntryPage(Window window) : base(window)
    {
    }

    public void SelectMenuItem(string itemName)
    {
        var listView = FindElement(MenuItemsListViewId).AsListBox();
        var item = listView.FindFirstDescendant(cf => cf.ByName(itemName));
        if (item == null)
        {
            throw new InvalidOperationException($"Menu item '{itemName}' not found");
        }
        item.Click();
    }

    public void AddModifier(string modifierName)
    {
        var listView = FindElement(ModifiersListViewId).AsListBox();
        var modifier = listView.FindFirstDescendant(cf => cf.ByName(modifierName));
        if (modifier == null)
        {
            throw new InvalidOperationException($"Modifier '{modifierName}' not found");
        }
        modifier.Click();
    }

    public void SetQuantity(int quantity)
    {
        EnterText(QuantityTextBoxId, quantity.ToString());
    }

    public void ApplyDiscount(string discountName)
    {
        var comboBox = FindElement(DiscountComboBoxId).AsComboBox();
        comboBox.Select(discountName);
    }

    public void HoldTicket()
    {
        ClickButton(HoldTicketButtonId);
    }

    public void RecallTicket(string ticketNumber)
    {
        ClickButton(RecallTicketButtonId);
        // Additional logic to select ticket would go here
    }

    public void NavigateToSettlement()
    {
        ClickButton(SettlementButtonId);
    }

    public decimal GetTicketTotal()
    {
        var totalText = GetText(TicketTotalTextBlockId);
        return decimal.Parse(totalText.Replace("$", "").Trim());
    }

    public int GetItemCount()
    {
        var countText = GetText(ItemCountTextBlockId);
        return int.Parse(countText);
    }

    public void SendToKitchen()
    {
        try
        {
            ClickButton(SendToKitchenButtonId);
        }
        catch (InvalidOperationException)
        {
            // Button may not exist if items are automatically sent to kitchen
            // or if there are no kitchen items on the ticket
        }
    }

    public void VoidTicket()
    {
        ClickButton(VoidTicketButtonId);
    }

    public bool HasPendingKitchenItems()
    {
        try
        {
            var indicator = FindElement(PendingKitchenItemsIndicatorId);
            return indicator != null && indicator.IsEnabled;
        }
        catch (InvalidOperationException)
        {
            // Indicator not found means no pending items
            return false;
        }
    }
}
