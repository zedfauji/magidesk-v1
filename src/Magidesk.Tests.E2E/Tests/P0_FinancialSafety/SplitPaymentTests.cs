using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class SplitPaymentTests : BaseE2ETest
{
    public SplitPaymentTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact(Skip = "Requires application UI to be fully implemented")]
    public void SplitPayment_CashAndCredit_ShouldEqualTicketTotal()
    {
        var loginPage = new LoginPage(MainWindow!);
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();

        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();

        var orderEntry = new OrderEntryPage(MainWindow!);
        orderEntry.SelectMenuItem("Steak");
        var ticketTotal = orderEntry.GetTicketTotal();

        orderEntry.NavigateToSettlement();

        var settlement = new SettlementPage(MainWindow!);
        
        // First payment - Cash
        settlement.SelectPaymentMethod("Cash");
        settlement.EnterPaymentAmount(ticketTotal / 2);
        settlement.ProcessPayment();

        // Second payment - Credit
        settlement.SelectPaymentMethod("Credit Card");
        settlement.EnterPaymentAmount(ticketTotal / 2);
        settlement.ProcessPayment();

        Assert.Equal(0, settlement.GetAmountDue());
        Assert.Equal(ticketTotal, settlement.GetAmountPaid());
    }
}
