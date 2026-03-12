using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for customer profile management operations.
/// </summary>
public sealed class CustomerPage : BasePage
{
    // Customer profile controls
    private const string CustomerNameTextBoxId = "CustomerNameTextBox";
    private const string CustomerPhoneTextBoxId = "CustomerPhoneTextBox";
    private const string CustomerEmailTextBoxId = "CustomerEmailTextBox";
    private const string CreateCustomerButtonId = "CreateCustomerButton";
    
    // Search controls
    private const string SearchTextBoxId = "SearchTextBox";
    private const string SearchButtonId = "SearchButton";
    
    // Ticket association
    private const string CustomerIdTextBoxId = "CustomerIdTextBox";
    private const string TicketIdTextBoxId = "TicketIdTextBox";
    private const string AssociateTicketButtonId = "AssociateTicketButton";
    
    // Membership controls
    private const string MembershipTierTextBoxId = "MembershipTierTextBox";
    private const string AssignTierButtonId = "AssignTierButton";
    
    // Loyalty controls
    private const string LoyaltyPointsTextBlockId = "LoyaltyPointsTextBlock";
    private const string PointsToRedeemTextBoxId = "PointsToRedeemTextBox";
    private const string RedeemPointsButtonId = "RedeemPointsButton";
    
    // Purchase history
    private const string PurchaseHistoryListId = "PurchaseHistoryList";

    public CustomerPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Creates a new customer profile.
    /// </summary>
    /// <param name="name">Customer name.</param>
    /// <param name="phone">Customer phone number.</param>
    /// <param name="email">Customer email address.</param>
    public void CreateCustomer(string name, string phone, string email)
    {
        EnterText(CustomerNameTextBoxId, name);
        EnterText(CustomerPhoneTextBoxId, phone);
        EnterText(CustomerEmailTextBoxId, email);
        ClickButton(CreateCustomerButtonId);
    }

    /// <summary>
    /// Searches for a customer by search term.
    /// </summary>
    /// <param name="searchTerm">The search term (name, phone, or email).</param>
    public void SearchCustomer(string searchTerm)
    {
        EnterText(SearchTextBoxId, searchTerm);
        ClickButton(SearchButtonId);
    }

    /// <summary>
    /// Associates a customer with a ticket.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="ticketId">The ticket ID.</param>
    public void AssociateCustomerWithTicket(string customerId, string ticketId)
    {
        EnterText(CustomerIdTextBoxId, customerId);
        EnterText(TicketIdTextBoxId, ticketId);
        ClickButton(AssociateTicketButtonId);
    }

    /// <summary>
    /// Assigns a membership tier to a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="tierName">The membership tier name.</param>
    public void AssignMembershipTier(string customerId, string tierName)
    {
        EnterText(CustomerIdTextBoxId, customerId);
        EnterText(MembershipTierTextBoxId, tierName);
        ClickButton(AssignTierButtonId);
    }

    /// <summary>
    /// Gets the purchase history for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>Enumerable of transaction descriptions.</returns>
    public IEnumerable<string> GetPurchaseHistory(string customerId)
    {
        EnterText(CustomerIdTextBoxId, customerId);
        var historyList = FindElement(PurchaseHistoryListId);
        var items = historyList.FindAllChildren();
        
        return items.Select(item => item.Name).ToList();
    }

    /// <summary>
    /// Gets the loyalty points balance for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>The loyalty points balance.</returns>
    public int GetLoyaltyPoints(string customerId)
    {
        EnterText(CustomerIdTextBoxId, customerId);
        var pointsText = GetText(LoyaltyPointsTextBlockId);
        return int.Parse(pointsText);
    }

    /// <summary>
    /// Redeems loyalty points for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="points">The number of points to redeem.</param>
    public void RedeemPoints(string customerId, int points)
    {
        EnterText(CustomerIdTextBoxId, customerId);
        EnterText(PointsToRedeemTextBoxId, points.ToString());
        ClickButton(RedeemPointsButtonId);
    }

    /// <summary>
    /// Gets the customer name from the currently displayed customer profile.
    /// </summary>
    /// <returns>The customer name.</returns>
    public string GetCustomerName()
    {
        return GetText(CustomerNameTextBoxId);
    }

    /// <summary>
    /// Gets the customer phone from the currently displayed customer profile.
    /// </summary>
    /// <returns>The customer phone number.</returns>
    public string GetCustomerPhone()
    {
        return GetText(CustomerPhoneTextBoxId);
    }

    /// <summary>
    /// Gets the customer email from the currently displayed customer profile.
    /// </summary>
    /// <returns>The customer email address.</returns>
    public string GetCustomerEmail()
    {
        return GetText(CustomerEmailTextBoxId);
    }

    /// <summary>
    /// Updates an existing customer profile.
    /// </summary>
    /// <param name="name">Customer name.</param>
    /// <param name="phone">Customer phone number.</param>
    /// <param name="email">Customer email address.</param>
    public void UpdateCustomer(string name, string phone, string email)
    {
        EnterText(CustomerNameTextBoxId, name);
        EnterText(CustomerPhoneTextBoxId, phone);
        EnterText(CustomerEmailTextBoxId, email);
        ClickButton(CreateCustomerButtonId); // Reuse create button for update
    }
}
