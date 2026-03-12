using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Magidesk.Tests.Workflows.Infrastructure;
using Xunit;
using Xunit.Abstracts;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for inventory quantity invariants.
/// Validates that inventory quantity never becomes negative after any operation.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 10: Inventory quantity never becomes negative
/// Validates: Requirements 10.1, 22.4
/// </summary>
[Trait("Priority", "P1")]
[Trait("Category", "OperationalIntegrity")]
public class InventoryInvariantProperties : BaseE2ETest
{
    public InventoryInvariantProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 10: Inventory quantity never becomes negative
    /// Validates: Requirements 10.1, 22.4
    /// 
    /// For any sequence of inventory operations (sales, adjustments, receipts),
    /// the inventory quantity must always remain >= 0. This property verifies that
    /// the system prevents negative inventory and maintains data integrity.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property InventoryQuantity_NeverBecomesNegative()
    {
        return Prop.ForAll(
            InventoryOperationGenerators.GenerateInventoryOperations(),
            operations =>
            {
                try
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

                    // Track inventory quantities after each operation
                    var itemQuantities = new Dictionary<string, int>();

                    // Act - Perform inventory operations
                    foreach (var operation in operations)
                    {
                        switch (operation.Type)
                        {
                            case InventoryOperationType.Sale:
                                // Navigate to order entry and sell item
                                switchboard.NavigateToOrderEntry();
                                Thread.Sleep(1000);
                                orderEntry.SelectMenuItem(operation.ItemName);
                                Thread.Sleep(500);
                                orderEntry.NavigateToSettlement();
                                Thread.Sleep(500);
                                settlement.SelectPaymentMethod("Cash");
                                Thread.Sleep(300);
                                settlement.EnterPaymentAmount(10.00m);
                                Thread.Sleep(300);
                                settlement.ProcessPayment();
                                Thread.Sleep(1000);

                                // Navigate back to inventory
                                switchboard.NavigateToBackOffice();
                                Thread.Sleep(1000);
                                passwordEntry.WaitForDialogVisible();
                                passwordEntry.EnterPinAndConfirm("1234");
                                Thread.Sleep(1000);
                                backOffice.NavigateToInventory();
                                Thread.Sleep(1500);
                                break;

                            case InventoryOperationType.Adjustment:
                                // Adjust inventory (positive or negative)
                                inventoryPage.AdjustInventory(
                                    operation.ItemName, 
                                    operation.Quantity, 
                                    "Property test adjustment");
                                Thread.Sleep(1000);
                                break;

                            case InventoryOperationType.Receipt:
                                // Receive inventory (always positive)
                                inventoryPage.CreatePurchaseOrder(
                                    "Test Supplier", 
                                    (operation.ItemName, operation.Quantity));
                                Thread.Sleep(500);
                                inventoryPage.ReceivePurchaseOrder("PO-TEST");
                                Thread.Sleep(1000);
                                break;
                        }

                        // Check inventory quantity after operation
                        var currentQuantity = inventoryPage.GetStockLevel(operation.ItemName);
                        itemQuantities[operation.ItemName] = currentQuantity;

                        // Assert - Verify quantity is non-negative
                        if (currentQuantity < 0)
                        {
                            return false.ToProperty()
                                .Label($"Inventory quantity became negative for {operation.ItemName}. " +
                                       $"Quantity: {currentQuantity}, Operation: {operation.Type}");
                        }
                    }

                    // Assert - All tracked items have non-negative quantities
                    var allNonNegative = itemQuantities.All(kvp => kvp.Value >= 0);

                    if (!allNonNegative)
                    {
                        var negativeItems = itemQuantities.Where(kvp => kvp.Value < 0)
                            .Select(kvp => $"{kvp.Key}: {kvp.Value}");
                        return false.ToProperty()
                            .Label($"Inventory quantities became negative: {string.Join(", ", negativeItems)}");
                    }

                    return allNonNegative
                        .ToProperty()
                        .Label("Inventory quantity never becomes negative");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Inventory non-negative invariant check failed: {ex.Message}");
                }
            });
    }
}
