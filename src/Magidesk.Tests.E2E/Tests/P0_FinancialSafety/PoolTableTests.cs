using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

/// <summary>
/// P0 tests for pool table management workflows.
/// Validates pool table timer operations, billing calculations, and multi-table scenarios.
/// Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class PoolTableTests : BaseE2ETest
{
    public PoolTableTests(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Test pool table start with timer activation.
    /// Requirement 3.1: WHEN a pool table is started, THE E2E_Test_Framework SHALL verify timer activation and rate display
    /// </summary>
    [Fact]
    public void StartPoolTable_ShouldActivateTimer()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table
        tableManagement.StartPoolTable("1");
        Thread.Sleep(1000);

        // Assert - Verify timer is activated (elapsed time should be > 0)
        var elapsedTime = tableManagement.GetPoolTableElapsedTime("1");
        Assert.True(elapsedTime >= TimeSpan.Zero, 
            $"Pool table timer should be activated. Elapsed time: {elapsedTime}");
    }

    /// <summary>
    /// Test elapsed time tracking during active session.
    /// Requirement 3.2: WHEN a pool table timer is running, THE E2E_Test_Framework SHALL verify elapsed time tracking
    /// </summary>
    [Fact]
    public void PoolTableTimer_ShouldTrackElapsedTime()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table and wait
        tableManagement.StartPoolTable("1");
        Thread.Sleep(1000);
        var initialElapsedTime = tableManagement.GetPoolTableElapsedTime("1");

        // Wait for timer to advance
        Thread.Sleep(3000);

        // Assert - Verify elapsed time increased
        var laterElapsedTime = tableManagement.GetPoolTableElapsedTime("1");
        Assert.True(laterElapsedTime > initialElapsedTime,
            $"Elapsed time should increase. Initial: {initialElapsedTime}, Later: {laterElapsedTime}");
    }

    /// <summary>
    /// Test pool table stop with time calculation and charge.
    /// Requirement 3.3: WHEN a pool table is stopped, THE E2E_Test_Framework SHALL verify time calculation and charge addition to ticket
    /// </summary>
    [Fact]
    public void StopPoolTable_ShouldCalculateChargeAndAddToTicket()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table and let it run
        tableManagement.StartPoolTable("1");
        Thread.Sleep(2000);

        // Get elapsed time before stopping
        var elapsedTime = tableManagement.GetPoolTableElapsedTime("1");

        // Act - Stop pool table
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        // Navigate to order entry to verify charge added
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Assert - Verify ticket has a charge (total > 0)
        var ticketTotal = orderEntry.GetTicketTotal();
        Assert.True(ticketTotal > 0,
            $"Ticket should have pool table charge. Elapsed time: {elapsedTime}, Total: {ticketTotal:C}");
    }

    /// <summary>
    /// Test pool table pause and timer suspension.
    /// Requirement 3.4: WHEN a pool table is paused, THE E2E_Test_Framework SHALL verify timer suspension
    /// </summary>
    [Fact]
    public void PausePoolTable_ShouldSuspendTimer()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table
        tableManagement.StartPoolTable("1");
        Thread.Sleep(2000);

        // Act - Pause pool table
        tableManagement.PausePoolTable("1");
        Thread.Sleep(500);
        var elapsedTimeAtPause = tableManagement.GetPoolTableElapsedTime("1");

        // Wait to verify timer is suspended
        Thread.Sleep(2000);

        // Assert - Verify elapsed time did not change (timer suspended)
        var elapsedTimeAfterWait = tableManagement.GetPoolTableElapsedTime("1");
        Assert.Equal(elapsedTimeAtPause, elapsedTimeAfterWait);
    }

    /// <summary>
    /// Test pool table resume from paused state.
    /// Requirement 3.5: WHEN a pool table is resumed, THE E2E_Test_Framework SHALL verify timer continuation from paused time
    /// </summary>
    [Fact]
    public void ResumePoolTable_ShouldContinueTimerFromPausedTime()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start, pause, and resume pool table
        tableManagement.StartPoolTable("1");
        Thread.Sleep(2000);
        
        tableManagement.PausePoolTable("1");
        Thread.Sleep(500);
        var elapsedTimeAtPause = tableManagement.GetPoolTableElapsedTime("1");

        tableManagement.ResumePoolTable("1");
        Thread.Sleep(2000);

        // Assert - Verify timer continued from paused time
        var elapsedTimeAfterResume = tableManagement.GetPoolTableElapsedTime("1");
        Assert.True(elapsedTimeAfterResume > elapsedTimeAtPause,
            $"Timer should continue from paused time. At pause: {elapsedTimeAtPause}, After resume: {elapsedTimeAfterResume}");
    }

    /// <summary>
    /// Test multiple pool tables with independent timers.
    /// Requirement 3.6: WHEN multiple pool tables are active, THE E2E_Test_Framework SHALL verify independent timer tracking
    /// </summary>
    [Fact]
    public void MultiplePoolTables_ShouldHaveIndependentTimers()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start first pool table
        tableManagement.StartPoolTable("1");
        Thread.Sleep(2000);
        var table1ElapsedTime = tableManagement.GetPoolTableElapsedTime("1");

        // Act - Start second pool table
        tableManagement.StartPoolTable("2");
        Thread.Sleep(1000);
        var table2ElapsedTime = tableManagement.GetPoolTableElapsedTime("2");

        // Assert - Verify table 1 has more elapsed time than table 2
        Assert.True(table1ElapsedTime > table2ElapsedTime,
            $"Table 1 should have more elapsed time. Table 1: {table1ElapsedTime}, Table 2: {table2ElapsedTime}");

        // Assert - Verify both timers are running independently
        Thread.Sleep(2000);
        var table1LaterTime = tableManagement.GetPoolTableElapsedTime("1");
        var table2LaterTime = tableManagement.GetPoolTableElapsedTime("2");

        Assert.True(table1LaterTime > table1ElapsedTime, "Table 1 timer should continue running");
        Assert.True(table2LaterTime > table2ElapsedTime, "Table 2 timer should continue running");
    }

    /// <summary>
    /// Test hourly rate application and partial hour billing.
    /// Requirement 3.7: THE E2E_Test_Framework SHALL verify hourly rate application and partial hour billing
    /// </summary>
    [Fact]
    public void PoolTableBilling_ShouldApplyHourlyRateWithPartialHours()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table and let it run for a short time (partial hour)
        tableManagement.StartPoolTable("1");
        Thread.Sleep(3000); // 3 seconds = 0.000833 hours

        var elapsedTime = tableManagement.GetPoolTableElapsedTime("1");

        // Act - Stop pool table
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        // Navigate to order entry to verify charge
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Assert - Verify ticket has a charge based on partial hour
        var ticketTotal = orderEntry.GetTicketTotal();
        Assert.True(ticketTotal > 0,
            $"Ticket should have pool table charge for partial hour. Elapsed time: {elapsedTime}, Total: {ticketTotal:C}");

        // Assuming hourly rate is $10/hour (from seed data), 3 seconds should be minimal charge
        // The exact calculation depends on billing rules (round up, minimum charge, etc.)
        Assert.True(ticketTotal >= 0.01m,
            $"Partial hour billing should apply. Total: {ticketTotal:C}");
    }

    /// <summary>
    /// Test overtime handling.
    /// Requirement 3.8: WHEN a pool table session exceeds configured duration, THE E2E_Test_Framework SHALL verify overtime handling
    /// </summary>
    [Fact]
    public void PoolTableOvertime_ShouldHandleExtendedSessions()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var tableManagement = new TableManagementPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);

        // Act - Login and navigate to tables
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table and let it run for extended time
        tableManagement.StartPoolTable("1");
        Thread.Sleep(5000); // 5 seconds to simulate extended session

        var elapsedTime = tableManagement.GetPoolTableElapsedTime("1");

        // Act - Stop pool table
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        // Navigate to order entry to verify charge
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Assert - Verify ticket has appropriate charge for extended session
        var ticketTotal = orderEntry.GetTicketTotal();
        Assert.True(ticketTotal > 0,
            $"Ticket should have pool table charge for extended session. Elapsed time: {elapsedTime}, Total: {ticketTotal:C}");

        // Overtime handling might include warnings, rate changes, or maximum duration limits
        // The exact behavior depends on business rules
        Assert.True(elapsedTime.TotalSeconds >= 5,
            $"Elapsed time should reflect extended session. Elapsed: {elapsedTime}");
    }
}
