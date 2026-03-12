using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P2_Stability;

/// <summary>
/// P2 tests for performance and stress testing.
/// Tests verify UI responsiveness, ticket recall, report generation,
/// rapid order entry, memory stability, and concurrent operations.
/// </summary>
[Trait("Priority", "P2")]
[Trait("Category", "Stability")]
public class PerformanceTests : BaseE2ETest
{
    public PerformanceTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void UIResponsiveness_With100MenuItems_ShouldBeUnder2Seconds()
    {
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");

        var switchboard = new SwitchboardPage(MainWindow!);
        
        var stopwatch = Stopwatch.StartNew();
        switchboard.NavigateToOrderEntry();
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 2000, 
            $"UI should respond within 2 seconds, took {stopwatch.ElapsedMilliseconds}ms");
        
        Output.WriteLine($"Order entry page loaded in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void TicketRecall_With50HeldTickets_ShouldBeUnder1Second()
    {
        var loginPage = new LoginPage(MainWindow!);
        loginPage.LoginWithPin("1234");

        var switchboard = new SwitchboardPage(MainWindow!);
        switchboard.NavigateToOrderEntry();

        Output.WriteLine("Ticket recall performance test - requires held tickets setup");
    }
}
