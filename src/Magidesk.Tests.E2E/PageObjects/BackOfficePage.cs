using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the Back Office page (BackOfficePage.xaml).
/// Handles navigating to sub-sections via the left-side NavigationView and administrative functions.
/// </summary>
public sealed class BackOfficePage : BasePage
{
    // User management controls
    private const string UsernameTextBoxId = "UsernameTextBox";
    private const string PasswordTextBoxId = "PasswordTextBox";
    private const string RoleTextBoxId = "RoleTextBoxId";
    private const string CreateUserButtonId = "CreateUserButton";
    private const string UpdateRoleButtonId = "UpdateRoleButton";
    
    // Terminal configuration controls
    private const string TerminalIdTextBoxId = "TerminalIdTextBox";
    private const string TerminalNameTextBoxId = "TerminalNameTextBox";
    private const string TerminalLocationTextBoxId = "TerminalLocationTextBox";
    private const string ConfigureTerminalButtonId = "ConfigureTerminalButton";
    
    // Payment method controls
    private const string PaymentMethodNameTextBoxId = "PaymentMethodNameTextBox";
    private const string RequiresAuthorizationCheckBoxId = "RequiresAuthorizationCheckBox";
    private const string ConfigurePaymentMethodButtonId = "ConfigurePaymentMethodButton";
    
    // Tax rate controls
    private const string TaxRateNameTextBoxId = "TaxRateNameTextBox";
    private const string TaxRateValueTextBoxId = "TaxRateValueTextBox";
    private const string ConfigureTaxRateButtonId = "ConfigureTaxRateButton";
    
    // System settings controls
    private const string SettingKeyTextBoxId = "SettingKeyTextBox";
    private const string SettingValueTextBoxId = "SettingValueTextBox";
    private const string UpdateSettingButtonId = "UpdateSettingButton";
    
    // Printer configuration controls
    private const string PrinterNameTextBoxId = "PrinterNameTextBox";
    private const string PrinterTypeTextBoxId = "PrinterTypeTextBox";
    private const string ConfigurePrinterButtonId = "ConfigurePrinterButton";

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

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="role">The user role.</param>
    public void CreateUser(string username, string password, string role)
    {
        EnterText(UsernameTextBoxId, username);
        EnterText(PasswordTextBoxId, password);
        EnterText(RoleTextBoxId, role);
        ClickButton(CreateUserButtonId);
    }

    /// <summary>
    /// Updates a user's role.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="newRole">The new role.</param>
    public void UpdateUserRole(string username, string newRole)
    {
        EnterText(UsernameTextBoxId, username);
        EnterText(RoleTextBoxId, newRole);
        ClickButton(UpdateRoleButtonId);
    }

    /// <summary>
    /// Configures a terminal.
    /// </summary>
    /// <param name="terminalId">The terminal ID.</param>
    /// <param name="name">The terminal name.</param>
    /// <param name="location">The terminal location.</param>
    public void ConfigureTerminal(string terminalId, string name, string location)
    {
        EnterText(TerminalIdTextBoxId, terminalId);
        EnterText(TerminalNameTextBoxId, name);
        EnterText(TerminalLocationTextBoxId, location);
        ClickButton(ConfigureTerminalButtonId);
    }

    /// <summary>
    /// Configures a payment method.
    /// </summary>
    /// <param name="name">The payment method name.</param>
    /// <param name="requiresAuthorization">Whether authorization is required.</param>
    public void ConfigurePaymentMethod(string name, bool requiresAuthorization)
    {
        EnterText(PaymentMethodNameTextBoxId, name);
        
        var checkbox = FindElement(RequiresAuthorizationCheckBoxId);
        var currentState = checkbox.IsChecked();
        if (currentState != requiresAuthorization)
        {
            checkbox.AsCheckBox().Toggle();
        }
        
        ClickButton(ConfigurePaymentMethodButtonId);
    }

    /// <summary>
    /// Configures a tax rate.
    /// </summary>
    /// <param name="name">The tax rate name.</param>
    /// <param name="rate">The tax rate value.</param>
    public void ConfigureTaxRate(string name, decimal rate)
    {
        EnterText(TaxRateNameTextBoxId, name);
        EnterText(TaxRateValueTextBoxId, rate.ToString("F4"));
        ClickButton(ConfigureTaxRateButtonId);
    }

    /// <summary>
    /// Updates a system setting.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="value">The setting value.</param>
    public void UpdateSystemSetting(string key, string value)
    {
        EnterText(SettingKeyTextBoxId, key);
        EnterText(SettingValueTextBoxId, value);
        ClickButton(UpdateSettingButtonId);
    }

    /// <summary>
    /// Configures a printer.
    /// </summary>
    /// <param name="printerName">The printer name.</param>
    /// <param name="printerType">The printer type.</param>
    public void ConfigurePrinter(string printerName, string printerType)
    {
        EnterText(PrinterNameTextBoxId, printerName);
        EnterText(PrinterTypeTextBoxId, printerType);
        ClickButton(ConfigurePrinterButtonId);
    }
}
