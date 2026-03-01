using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

/// <summary>
/// P0 tests for single cash payment workflow.
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class SingleCashPaymentTests : BaseE2ETest
{
    public SingleCashPaymentTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact(Skip = "Requires application UI to be fully implemented with correct AutomationIds")]
    public void CompleteCashPayment_ShouldUpdateCashDrawerBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Act - Login
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();

        // Act - Navigate to order entry
        switchboard.NavigateToOrderEntry();

        // Act - Add item to ticket
        orderEntry.SelectMenuItem("Coffee");
        var ticketTotal = orderEntry.GetTicketTotal();

        // Act - Navigate to settlement
        orderEntry.NavigateToSettlement();

        // Act - Process cash payment
        settlement.SelectPaymentMethod("Cash");
        settlement.EnterPaymentAmount(ticketTotal);
        settlement.ProcessPayment();

        // Assert - Payment processed successfully
        Assert.Equal(0, settlement.GetAmountDue());
        Assert.Equal(ticketTotal, settlement.GetAmountPaid());
    }

    [Fact(Skip = "Requires application UI to be fully implemented with correct AutomationIds")]
    public void CashPayment_PaymentTotalEqualsTicketTotal()
    {
        // This test validates the business invariant:
        // Payment total must equal ticket total for successful payment
        
        var loginPage = new LoginPage(MainWindow!);
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();

        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();

        var orderEntry = new OrderEntryPage(MainWindow!);
        orderEntry.SelectMenuItem("Burger");
        var ticketTotal = orderEntry.GetTicketTotal();

        orderEntry.NavigateToSettlement();

        var settlement = new SettlementPage(MainWindow!);
        settlement.SelectPaymentMethod("Cash");
        settlement.EnterPaymentAmount(ticketTotal);
        
        var amountDueBefore = settlement.GetAmountDue();
        Assert.Equal(ticketTotal, amountDueBefore);

        settlement.ProcessPayment();

        var amountDueAfter = settlement.GetAmountDue();
        var amountPaid = settlement.GetAmountPaid();
        
        Assert.Equal(0, amountDueAfter);
        Assert.Equal(ticketTotal, amountPaid);
    }
}
