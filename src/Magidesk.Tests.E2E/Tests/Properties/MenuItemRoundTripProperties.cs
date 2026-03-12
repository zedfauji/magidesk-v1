using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Magidesk.Tests.Workflows.Infrastructure;
using Xunit;
using Xunit.Abstracts;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for menu item serialization round-trip.
/// Validates that menu item data can be serialized and deserialized without data loss.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 13: Menu item serialization round-trip
/// Validates: Requirements 13.1, 13.2, 21.3
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class MenuItemRoundTripProperties : BaseE2ETest
{
    public MenuItemRoundTripProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 13: Menu item serialization round-trip
    /// Validates: Requirements 13.1, 13.2, 21.3
    /// 
    /// For any menu item, deserialize(serialize(item)) must equal the original item.
    /// This property verifies that menu item data can be saved and retrieved without data loss,
    /// ensuring data integrity for menu configuration operations.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property MenuItem_RoundTripPreservesAllData()
    {
        return Prop.ForAll(
            TestDataGenerators.MenuItemGenerator(),
            menuItemData =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);
                    var menuConfigPage = new MenuConfigPage(MainWindow!);

                    // Act - Login and navigate to menu configuration
                    loginPage.LoginWithPin("1234");
                    Thread.Sleep(1000);
                    switchboard.NavigateToBackOffice();
                    Thread.Sleep(1000);

                    var passwordEntry = new PasswordEntryPage(MainWindow!);
                    passwordEntry.WaitForDialogVisible();
                    passwordEntry.EnterPinAndConfirm("1234");
                    Thread.Sleep(1000);

                    var backOffice = new BackOfficePage(MainWindow!);
                    backOffice.WaitForPageLoaded();
                    backOffice.ClickNavigationItem("Menu Configuration");
                    Thread.Sleep(1500);

                    // Act - Create menu item (serialize operation)
                    const string category = "Test Category";
                    menuConfigPage.CreateMenuItem(menuItemData.Name, menuItemData.Price, category);
                    Thread.Sleep(1000);

                    // Act - Search for the menu item by name (deserialize operation)
                    menuConfigPage.SearchMenuItem(menuItemData.Name);
                    Thread.Sleep(1000);

                    // Act - Retrieve menu item data
                    var retrievedName = menuConfigPage.GetMenuItemName();
                    var retrievedPrice = menuConfigPage.GetMenuItemPrice();
                    var retrievedCategory = menuConfigPage.GetMenuItemCategory();

                    // Assert - Verify round-trip preserves all data
                    var nameMatches = retrievedName == menuItemData.Name;
                    var priceMatches = Math.Abs(retrievedPrice - menuItemData.Price) < 0.01m;
                    var categoryMatches = retrievedCategory == category;

                    if (!nameMatches)
                    {
                        return false.ToProperty()
                            .Label($"Menu item name should be preserved in round-trip. " +
                                   $"Original: '{menuItemData.Name}', Retrieved: '{retrievedName}'");
                    }

                    if (!priceMatches)
                    {
                        return false.ToProperty()
                            .Label($"Menu item price should be preserved in round-trip. " +
                                   $"Original: {menuItemData.Price:C}, Retrieved: {retrievedPrice:C}");
                    }

                    if (!categoryMatches)
                    {
                        return false.ToProperty()
                            .Label($"Menu item category should be preserved in round-trip. " +
                                   $"Original: '{category}', Retrieved: '{retrievedCategory}'");
                    }

                    return (nameMatches && priceMatches && categoryMatches)
                        .ToProperty()
                        .Label("Menu item round-trip preserves all data");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Menu item round-trip check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that menu item creation persists data correctly.
    /// </summary>
    [Fact]
    public void MenuItem_CreationPersistsData()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        const string itemName = "Test Coffee";
        const decimal itemPrice = 3.50m;
        const string itemCategory = "Beverages";

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create menu item
        menuConfigPage.CreateMenuItem(itemName, itemPrice, itemCategory);
        Thread.Sleep(1000);

        // Act - Search for the menu item
        menuConfigPage.SearchMenuItem(itemName);
        Thread.Sleep(1000);

        // Assert - Verify menu item data is persisted
        var retrievedName = menuConfigPage.GetMenuItemName();
        var retrievedPrice = menuConfigPage.GetMenuItemPrice();
        var retrievedCategory = menuConfigPage.GetMenuItemCategory();

        Assert.Equal(itemName, retrievedName);
        Assert.Equal(itemPrice, retrievedPrice);
        Assert.Equal(itemCategory, retrievedCategory);
    }

    /// <summary>
    /// Validates that menu item search returns correct results.
    /// </summary>
    [Fact]
    public void MenuItem_SearchReturnsCorrectResults()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        const string item1Name = "Espresso";
        const decimal item1Price = 2.50m;
        const string item1Category = "Beverages";

        const string item2Name = "Cappuccino";
        const decimal item2Price = 3.50m;
        const string item2Category = "Beverages";

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create first menu item
        menuConfigPage.CreateMenuItem(item1Name, item1Price, item1Category);
        Thread.Sleep(1000);

        // Act - Create second menu item
        menuConfigPage.CreateMenuItem(item2Name, item2Price, item2Category);
        Thread.Sleep(1000);

        // Act - Search for first menu item by name
        menuConfigPage.SearchMenuItem(item1Name);
        Thread.Sleep(1000);

        // Assert - Verify correct menu item is returned
        var retrievedName = menuConfigPage.GetMenuItemName();
        Assert.Equal(item1Name, retrievedName);

        // Act - Search for second menu item
        menuConfigPage.SearchMenuItem(item2Name);
        Thread.Sleep(1000);

        // Assert - Verify correct menu item is returned
        retrievedName = menuConfigPage.GetMenuItemName();
        Assert.Equal(item2Name, retrievedName);
    }

    /// <summary>
    /// Validates that menu item data is not corrupted by special characters.
    /// </summary>
    [Fact]
    public void MenuItem_HandlesSpecialCharactersCorrectly()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        const string itemName = "Café au Lait";
        const decimal itemPrice = 4.25m;
        const string itemCategory = "Beverages & Drinks";

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create menu item with special characters
        menuConfigPage.CreateMenuItem(itemName, itemPrice, itemCategory);
        Thread.Sleep(1000);

        // Act - Search for the menu item
        menuConfigPage.SearchMenuItem(itemName);
        Thread.Sleep(1000);

        // Assert - Verify special characters are preserved
        var retrievedName = menuConfigPage.GetMenuItemName();
        var retrievedPrice = menuConfigPage.GetMenuItemPrice();
        var retrievedCategory = menuConfigPage.GetMenuItemCategory();

        Assert.Equal(itemName, retrievedName);
        Assert.Equal(itemPrice, retrievedPrice);
        Assert.Equal(itemCategory, retrievedCategory);
    }

    /// <summary>
    /// Validates that menu item fields are not truncated.
    /// </summary>
    [Fact]
    public void MenuItem_DoesNotTruncateFields()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        const string itemName = "Very Long Menu Item Name With Many Words";
        const decimal itemPrice = 99.99m;
        const string itemCategory = "Very Long Category Name";

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create menu item with long fields
        menuConfigPage.CreateMenuItem(itemName, itemPrice, itemCategory);
        Thread.Sleep(1000);

        // Act - Search for the menu item
        menuConfigPage.SearchMenuItem(itemName);
        Thread.Sleep(1000);

        // Assert - Verify fields are not truncated
        var retrievedName = menuConfigPage.GetMenuItemName();
        var retrievedPrice = menuConfigPage.GetMenuItemPrice();
        var retrievedCategory = menuConfigPage.GetMenuItemCategory();

        Assert.Equal(itemName, retrievedName);
        Assert.Equal(itemPrice, retrievedPrice);
        Assert.Equal(itemCategory, retrievedCategory);
    }

    /// <summary>
    /// Validates that multiple menu items can be created and retrieved independently.
    /// </summary>
    [Fact]
    public void MenuItem_MultipleItemsIndependent()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        var menuItems = new[]
        {
            ("Coffee", 2.50m, "Beverages"),
            ("Tea", 2.00m, "Beverages"),
            ("Burger", 8.50m, "Food")
        };

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create multiple menu items
        foreach (var (name, price, category) in menuItems)
        {
            menuConfigPage.CreateMenuItem(name, price, category);
            Thread.Sleep(500);
        }

        // Assert - Verify each menu item can be retrieved independently
        foreach (var (name, price, category) in menuItems)
        {
            menuConfigPage.SearchMenuItem(name);
            Thread.Sleep(500);

            var retrievedName = menuConfigPage.GetMenuItemName();
            var retrievedPrice = menuConfigPage.GetMenuItemPrice();
            var retrievedCategory = menuConfigPage.GetMenuItemCategory();

            Assert.Equal(name, retrievedName);
            Assert.Equal(price, retrievedPrice);
            Assert.Equal(category, retrievedCategory);
        }
    }

    /// <summary>
    /// Validates that menu item update preserves data integrity.
    /// </summary>
    [Fact]
    public void MenuItem_UpdatePreservesDataIntegrity()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var menuConfigPage = new MenuConfigPage(MainWindow!);

        const string originalName = "Original Item";
        const decimal originalPrice = 5.00m;
        const string originalCategory = "Original Category";

        const string itemId = "ITEM-001";
        const string updatedName = "Updated Item";
        const decimal updatedPrice = 6.00m;

        // Act - Login and navigate to menu configuration
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        var passwordEntry = new PasswordEntryPage(MainWindow!);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        var backOffice = new BackOfficePage(MainWindow!);
        backOffice.WaitForPageLoaded();
        backOffice.ClickNavigationItem("Menu Configuration");
        Thread.Sleep(1500);

        // Act - Create menu item
        menuConfigPage.CreateMenuItem(originalName, originalPrice, originalCategory);
        Thread.Sleep(1000);

        // Act - Update menu item
        menuConfigPage.UpdateMenuItem(itemId, updatedName, updatedPrice);
        Thread.Sleep(1000);

        // Act - Retrieve updated menu item
        menuConfigPage.SearchMenuItem(updatedName);
        Thread.Sleep(1000);

        // Assert - Verify updated data is preserved
        var retrievedName = menuConfigPage.GetMenuItemName();
        var retrievedPrice = menuConfigPage.GetMenuItemPrice();

        Assert.Equal(updatedName, retrievedName);
        Assert.Equal(updatedPrice, retrievedPrice);
    }
}
