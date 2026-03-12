using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Example-based tests for inventory non-negative invariant.
/// Complements property-based tests with specific scenarios.
/// </summary>
[Trait("Priority", "P1")]
[Trait("Category", "OperationalIntegrity")]
public class InventoryInvariantExamples : BaseE2ETest
{
    public InventoryInvariantExamples(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Validates that inventory quantity is non-negative after item sale.
    /// </summary>
    [Fact]
    public void SellItem_MaintainsNonNegativeInventory()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var inventoryPage = new InventoryPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var passwordEntry = new PasswordEntryPage(MainWindow!);
        var backOffice = new BackOfficePage(MainWindow!);

        // Act - Login and check initial inventory
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        backOffice.WaitForPageLoaded();
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        var initialStock = inventoryPage.GetStockLevel("Coffee");

        // Act - Sell item
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);
        orderEntry.NavigateToSettlement();
        Thread.Sleep(500);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(5.00m);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Check final inventory
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        var finalStock = inventoryPage.GetStockLevel("Coffee");

        // Assert - Inventory is non-negative
        Assert.True(finalStock >= 0, 
            $"Inventory should remain non-negative after sale. Final stock: {finalStock}");
    }

    /// <summary>
    /// Validates that inventory adjustment maintains non-negative quantity.
    /// </summary>
    [Fact]
    public void AdjustInventory_MaintainsNonNegativeQuantity()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var inventoryPage = new InventoryPage(MainWindow!);
        var passwordEntry = new PasswordEntryPage(MainWindow!);
        var backOffice = new BackOfficePage(MainWindow!);

        // Act - Login and navigate to inventory
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        backOffice.WaitForPageLoaded();
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        var initialStock = inventoryPage.GetStockLevel("Coffee");

        // Act - Adjust inventory (negative adjustment)
        var adjustmentAmount = -5;
        inventoryPage.AdjustInventory("Coffee", adjustmentAmount, "Test adjustment");
        Thread.Sleep(1000);

        var finalStock = inventoryPage.GetStockLevel("Coffee");

        // Assert - Inventory is non-negative
        Assert.True(finalStock >= 0,
            $"Inventory should remain non-negative after adjustment. " +
            $"Initial: {initialStock}, Adjustment: {adjustmentAmount}, Final: {finalStock}");
    }

    /// <summary>
    /// Validates that receiving inventory maintains non-negative quantity.
    /// </summary>
    [Fact]
    public void ReceiveInventory_MaintainsNonNegativeQuantity()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var inventoryPage = new InventoryPage(MainWindow!);
        var passwordEntry = new PasswordEntryPage(MainWindow!);
        var backOffice = new BackOfficePage(MainWindow!);

        // Act - Login and navigate to inventory
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        backOffice.WaitForPageLoaded();
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        var initialStock = inventoryPage.GetStockLevel("Coffee");

        // Act - Receive inventory
        inventoryPage.CreatePurchaseOrder("Test Supplier", ("Coffee", 50));
        Thread.Sleep(500);
        inventoryPage.ReceivePurchaseOrder("PO-TEST");
        Thread.Sleep(1000);

        var finalStock = inventoryPage.GetStockLevel("Coffee");

        // Assert - Inventory is non-negative and increased
        Assert.True(finalStock >= 0,
            $"Inventory should remain non-negative after receipt. Final: {finalStock}");
        Assert.True(finalStock > initialStock,
            $"Inventory should increase after receipt. Initial: {initialStock}, Final: {finalStock}");
    }

    /// <summary>
    /// Validates that multiple operations maintain non-negative inventory.
    /// </summary>
    [Fact]
    public void MultipleOperations_MaintainNonNegativeInventory()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var inventoryPage = new InventoryPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var passwordEntry = new PasswordEntryPage(MainWindow!);
        var backOffice = new BackOfficePage(MainWindow!);

        // Act - Login and navigate to inventory
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);

        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);

        backOffice.WaitForPageLoaded();
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        // Operation 1: Receive inventory
        inventoryPage.CreatePurchaseOrder("Test Supplier", ("Coffee", 20));
        Thread.Sleep(500);
        inventoryPage.ReceivePurchaseOrder("PO-001");
        Thread.Sleep(1000);

        var stockAfterReceipt = inventoryPage.GetStockLevel("Coffee");
        Assert.True(stockAfterReceipt >= 0, "Stock should be non-negative after receipt");

        // Operation 2: Sell item
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);
        orderEntry.NavigateToSettlement();
        Thread.Sleep(500);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(5.00m);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        switchboard.NavigateToBackOffice();
        Thread.Sleep(1000);
        passwordEntry.WaitForDialogVisible();
        passwordEntry.EnterPinAndConfirm("1234");
        Thread.Sleep(1000);
        backOffice.NavigateToInventory();
        Thread.Sleep(1500);

        var stockAfterSale = inventoryPage.GetStockLevel("Coffee");
        Assert.True(stockAfterSale >= 0, "Stock should be non-negative after sale");

        // Operation 3: Adjust inventory
        inventoryPage.AdjustInventory("Coffee", -3, "Test adjustment");
        Thread.Sleep(1000);

        var stockAfterAdjustment = inventoryPage.GetStockLevel("Coffee");
        Assert.True(stockAfterAdjustment >= 0, "Stock should be non-negative after adjustment");
    }
}
