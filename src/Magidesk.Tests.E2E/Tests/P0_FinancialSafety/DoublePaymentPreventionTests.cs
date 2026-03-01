using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class DoublePaymentPreventionTests : BaseE2ETest
{
    public DoublePaymentPreventionTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact(Skip = "Requires application UI to be fully implemented")]
    public void ProcessPaymentButton_AfterSuccessfulPayment_ShouldBeDisabled()
    {
        var loginPage = new LoginPage(MainWindow!);
        loginPage.EnterUsername("admin");
        loginPage.EnterPassword("admin123");
        loginPage.ClickLogin();

        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();

        var orderEntry = new OrderEntryPage(MainWindow!);
        orderEntry.SelectMenuItem("Salad");
        var ticketTotal = orderEntry.GetTicketTotal();

        orderEntry.NavigateToSettlement();

        var settlement = new SettlementPage(MainWindow!);
        settlement.SelectPaymentMethod("Cash");
        settlement.EnterPaymentAmount(ticketTotal);
        
        Assert.True(settlement.IsProcessPaymentEnabled());
        
        settlement.ProcessPayment();
        
        Assert.False(settlement.IsProcessPaymentEnabled());
    }
}
