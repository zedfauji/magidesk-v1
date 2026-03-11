using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Magidesk.Tests.Workflows.Infrastructure;
using Xunit;
using Xunit.Abstracts;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for cash session balance invariants.
/// Validates that ending balance equals starting balance plus receipts minus disbursements.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 8: Ending balance equals starting balance plus receipts minus disbursements
/// Validates: Requirements 8.2, 8.3, 8.4, 8.5, 8.6, 22.2
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class CashSessionInvariantProperties : BaseE2ETest
{
    public CashSessionInvariantProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 8: Ending balance equals starting balance plus receipts minus disbursements
    /// Validates: Requirements 8.2, 8.3, 8.4, 8.5, 8.6, 22.2
    /// 
    /// For any cash session, the ending balance must equal the starting balance plus all receipts
    /// (cash payments) minus all disbursements (cash drops, payouts, drawer pulls).
    /// This property verifies that cash session balance calculations are accurate and prevent
    /// financial discrepancies.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property CashSessionEndingBalance_EqualsStartingPlusReceiptsMinusDisbursements()
    {
        return Prop.ForAll(
            TestDataGenerators.CashSessionGenerator(),
            sessionData =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);
                    var cashSession = new CashSessionPage(MainWindow!);
                    var orderEntry = new OrderEntryPage(MainWindow!);
                    var settlement = new SettlementPage(MainWindow!);

                    // Act - Login and navigate to cash session
                    loginPage.LoginWithPin("1234");
                    Thread.Sleep(1000);
                    switchboard.NavigateToCashSession();
                    Thread.Sleep(1000);

                    // Act - Open cash session with starting balance
                    cashSession.OpenSession(sessionData.StartingBalance);
                    Thread.Sleep(1000);

                    // Act - Process receipts (cash payments)
                    foreach (var receipt in sessionData.Receipts)
                    {
                        // Navigate to order entry
                        switchboard.NavigateToOrderEntry();
                        Thread.Sleep(500);

                        // Create a simple order with the receipt amount
                        // Note: In a real scenario, we would select menu items that total to the receipt amount
                        // For this property test, we simulate by creating an order and processing payment
                        orderEntry.SelectMenuItem("Coffee");
                        Thread.Sleep(300);

                        // Navigate to settlement
                        orderEntry.NavigateToSettlement();
                        Thread.Sleep(500);

                        // Process cash payment
                        settlement.SelectPaymentMethod("Cash");
                        Thread.Sleep(300);
                        settlement.EnterPaymentAmount(receipt);
                        Thread.Sleep(300);
                        settlement.ProcessPayment();
                        Thread.Sleep(500);

                        // Navigate back to cash session
                        switchboard.NavigateToCashSession();
                        Thread.Sleep(500);
                    }

                    // Act - Process disbursements (cash drops, payouts)
                    foreach (var disbursement in sessionData.Disbursements)
                    {
                        // Record cash drop (represents any disbursement: drop, payout, or drawer pull)
                        cashSession.RecordCashDrop(disbursement);
                        Thread.Sleep(300);
                    }

                    // Act - Get expected cash (this is the calculated ending balance)
                    var actualEndingBalance = cashSession.GetExpectedCash();

                    // Calculate expected ending balance using the invariant formula
                    var expectedEndingBalance = sessionData.EndingBalance;

                    // Assert - Verify ending balance equals starting balance + receipts - disbursements
                    // Allow small rounding differences (within 1 cent)
                    var difference = Math.Abs(actualEndingBalance - expectedEndingBalance);
                    var balanceIsCorrect = difference < 0.01m;

                    if (!balanceIsCorrect)
                    {
                        return false.ToProperty()
                            .Label($"Cash session ending balance should equal starting balance + receipts - disbursements. " +
                                   $"Expected: {expectedEndingBalance:C}, Actual: {actualEndingBalance:C}, " +
                                   $"Difference: {difference:C}, Starting: {sessionData.StartingBalance:C}, " +
                                   $"Receipts: {sessionData.Receipts.Sum():C}, Disbursements: {sessionData.Disbursements.Sum():C}");
                    }

                    return balanceIsCorrect
                        .ToProperty()
                        .Label("Ending balance equals starting balance plus receipts minus disbursements");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Cash session balance invariant check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that cash session ending balance is always non-negative.
    /// </summary>
    [Fact]
    public void CashSessionEndingBalance_AlwaysNonNegative()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);

        decimal startingBalance = 100.00m;

        // Act - Login and navigate to cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);

        // Act - Open cash session
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        // Assert - Expected cash is non-negative
        var expectedCash = cashSession.GetExpectedCash();
        Assert.True(expectedCash >= 0, $"Cash session ending balance should be non-negative. Actual: {expectedCash:C}");
    }

    /// <summary>
    /// Validates that receipts increase drawer balance.
    /// </summary>
    [Fact]
    public void CashReceipts_IncreaseDrawerBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        decimal startingBalance = 100.00m;

        // Act - Login and open cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        var initialBalance = cashSession.GetExpectedCash();

        // Act - Process cash payment (receipt)
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);
        var ticketTotal = orderEntry.GetTicketTotal();

        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Navigate back to cash session
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);

        // Assert - Balance increased by receipt amount
        var finalBalance = cashSession.GetExpectedCash();
        Assert.True(finalBalance > initialBalance, 
            $"Cash receipt should increase drawer balance. Initial: {initialBalance:C}, Final: {finalBalance:C}");
        Assert.Equal(initialBalance + ticketTotal, finalBalance);
    }

    /// <summary>
    /// Validates that disbursements decrease drawer balance.
    /// </summary>
    [Fact]
    public void CashDisbursements_DecreaseDrawerBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);

        decimal startingBalance = 200.00m;
        decimal disbursementAmount = 50.00m;

        // Act - Login and open cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        var initialBalance = cashSession.GetExpectedCash();

        // Act - Record cash drop (disbursement)
        cashSession.RecordCashDrop(disbursementAmount);
        Thread.Sleep(1000);

        // Assert - Balance decreased by disbursement amount
        var finalBalance = cashSession.GetExpectedCash();
        Assert.True(finalBalance < initialBalance,
            $"Cash disbursement should decrease drawer balance. Initial: {initialBalance:C}, Final: {finalBalance:C}");
        Assert.Equal(initialBalance - disbursementAmount, finalBalance);
    }

    /// <summary>
    /// Validates that multiple receipts and disbursements maintain balance invariant.
    /// </summary>
    [Fact]
    public void MultipleTransactions_MaintainBalanceInvariant()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        decimal startingBalance = 200.00m;

        // Act - Login and open cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        // Track expected balance manually
        var expectedBalance = startingBalance;

        // Act - Process first receipt
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);
        var receipt1 = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(receipt1);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);
        expectedBalance += receipt1;

        // Act - Process first disbursement
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        var disbursement1 = 30.00m;
        cashSession.RecordCashDrop(disbursement1);
        Thread.Sleep(1000);
        expectedBalance -= disbursement1;

        // Act - Process second receipt
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Burger");
        Thread.Sleep(500);
        var receipt2 = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(receipt2);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);
        expectedBalance += receipt2;

        // Act - Process second disbursement
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        var disbursement2 = 20.00m;
        cashSession.RecordCashDrop(disbursement2);
        Thread.Sleep(1000);
        expectedBalance -= disbursement2;

        // Assert - Final balance matches expected
        var actualBalance = cashSession.GetExpectedCash();
        Assert.Equal(expectedBalance, actualBalance);
    }

    /// <summary>
    /// Validates that closing cash session preserves balance invariant.
    /// </summary>
    [Fact]
    public void CloseCashSession_PreservesBalanceInvariant()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        decimal startingBalance = 150.00m;

        // Act - Login and open cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        // Act - Process some transactions
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);
        var receipt = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(receipt);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        var disbursement = 25.00m;
        cashSession.RecordCashDrop(disbursement);
        Thread.Sleep(1000);

        // Calculate expected ending balance
        var expectedEndingBalance = startingBalance + receipt - disbursement;

        // Get expected cash before closing
        var expectedCashBeforeClose = cashSession.GetExpectedCash();

        // Act - Close cash session
        cashSession.CloseSession();
        Thread.Sleep(1000);

        // Assert - Ending balance matches expected
        var actualCash = cashSession.GetActualCash();
        Assert.Equal(expectedEndingBalance, expectedCashBeforeClose);
        Assert.Equal(expectedCashBeforeClose, actualCash);
    }

    /// <summary>
    /// Validates that zero receipts and disbursements maintain starting balance.
    /// </summary>
    [Fact]
    public void NoTransactions_MaintainStartingBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var cashSession = new CashSessionPage(MainWindow!);

        decimal startingBalance = 100.00m;

        // Act - Login and open cash session
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToCashSession();
        Thread.Sleep(1000);
        cashSession.OpenSession(startingBalance);
        Thread.Sleep(1000);

        // Assert - Expected cash equals starting balance (no transactions)
        var expectedCash = cashSession.GetExpectedCash();
        Assert.Equal(startingBalance, expectedCash);
    }
}
