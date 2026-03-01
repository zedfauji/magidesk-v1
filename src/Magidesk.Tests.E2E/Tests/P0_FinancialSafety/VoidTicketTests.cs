using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class VoidTicketTests : BaseE2ETest
{
    public VoidTicketTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void VoidTicket_ShouldPreventPaymentProcessing()
    {
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");

        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();

        var orderEntry = new OrderEntryPage(MainWindow!);
        orderEntry.SelectMenuItem("Soup");
        
        orderEntry.NavigateToSettlement();

        var settlement = new SettlementPage(MainWindow!);
        settlement.VoidTicket();
        
        Assert.False(settlement.IsProcessPaymentEnabled());
    }
}
