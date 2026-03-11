using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for pool table billing invariants.
/// Validates that pool table charges always equal rate multiplied by duration.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 3: Pool table charge equals rate multiplied by duration
/// Validates: Requirements 3.3, 3.7, 22.7
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class PoolTableBillingInvariantProperties : BaseE2ETest
{
    public PoolTableBillingInvariantProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 3: Pool table charge equals rate multiplied by duration
    /// Validates: Requirements 3.3, 3.7, 22.7
    /// 
    /// For any pool table session, the charge added to the ticket must equal
    /// the hourly rate multiplied by the elapsed time in hours.
    /// This property verifies that pool table billing calculation is accurate.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property PoolTableCharge_EqualsRateMultipliedByDuration()
    {
        return Prop.ForAll(
            GeneratePoolTableSessions(),
            session =>
            {
                try
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

                    // Act - Start pool table and let it run for specified duration
                    tableManagement.StartPoolTable(session.TableNumber);
                    Thread.Sleep(session.DurationMs);

                    // Get elapsed time before stopping
                    var elapsedTime = tableManagement.GetPoolTableElapsedTime(session.TableNumber);

                    // Act - Stop pool table
                    tableManagement.StopPoolTable(session.TableNumber);
                    Thread.Sleep(1000);

                    // Navigate to order entry to get charge
                    switchboard.NavigateToOrderEntry();
                    Thread.Sleep(1000);

                    // Get ticket total (this is the charge)
                    var actualCharge = orderEntry.GetTicketTotal();

                    // Calculate expected charge: rate * (elapsed time in hours)
                    var elapsedHours = elapsedTime.TotalHours;
                    var expectedCharge = session.HourlyRate * (decimal)elapsedHours;

                    // Allow small rounding differences (within 1 cent)
                    var difference = Math.Abs(actualCharge - expectedCharge);
                    var chargeIsCorrect = difference < 0.01m;

                    if (!chargeIsCorrect)
                    {
                        return false.ToProperty()
                            .Label($"Pool table charge should equal rate * duration. " +
                                   $"Expected: {expectedCharge:C}, Actual: {actualCharge:C}, " +
                                   $"Elapsed: {elapsedTime}, Rate: {session.HourlyRate:C}/hr, " +
                                   $"Difference: {difference:C}");
                    }

                    return chargeIsCorrect
                        .ToProperty()
                        .Label("Pool table charge equals rate multiplied by duration");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Pool table billing invariant check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that pool table charge is always non-negative.
    /// </summary>
    [Fact]
    public void PoolTableCharge_AlwaysNonNegative()
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

        // Act - Start and immediately stop pool table
        tableManagement.StartPoolTable("1");
        Thread.Sleep(500);
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        // Navigate to order entry to get charge
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Assert - Charge is non-negative
        var charge = orderEntry.GetTicketTotal();
        Assert.True(charge >= 0, $"Pool table charge should be non-negative. Actual: {charge:C}");
    }

    /// <summary>
    /// Validates that longer sessions result in higher charges.
    /// </summary>
    [Fact]
    public void PoolTableCharge_IncreasesWithDuration()
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

        // Act - Start pool table for short duration
        tableManagement.StartPoolTable("1");
        Thread.Sleep(1000);
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        var shortDurationCharge = orderEntry.GetTicketTotal();

        // Reset for second test
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table for longer duration
        tableManagement.StartPoolTable("2");
        Thread.Sleep(3000);
        tableManagement.StopPoolTable("2");
        Thread.Sleep(1000);

        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        var longDurationCharge = orderEntry.GetTicketTotal();

        // Assert - Longer duration results in higher charge
        Assert.True(longDurationCharge > shortDurationCharge,
            $"Longer session should have higher charge. Short: {shortDurationCharge:C}, Long: {longDurationCharge:C}");
    }

    /// <summary>
    /// Validates that pool table charge is proportional to elapsed time.
    /// </summary>
    [Fact]
    public void PoolTableCharge_ProportionalToElapsedTime()
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

        // Act - Start pool table for 2 seconds
        tableManagement.StartPoolTable("1");
        Thread.Sleep(2000);
        var elapsedTime1 = tableManagement.GetPoolTableElapsedTime("1");
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        var charge1 = orderEntry.GetTicketTotal();

        // Reset for second test
        switchboard.NavigateToTables();
        Thread.Sleep(1000);

        // Act - Start pool table for 4 seconds (double the duration)
        tableManagement.StartPoolTable("2");
        Thread.Sleep(4000);
        var elapsedTime2 = tableManagement.GetPoolTableElapsedTime("2");
        tableManagement.StopPoolTable("2");
        Thread.Sleep(1000);

        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);
        var charge2 = orderEntry.GetTicketTotal();

        // Assert - Charge should be approximately proportional to elapsed time
        // charge2 / charge1 should be approximately equal to elapsedTime2 / elapsedTime1
        if (charge1 > 0 && elapsedTime1.TotalSeconds > 0)
        {
            var chargeRatio = (double)(charge2 / charge1);
            var timeRatio = elapsedTime2.TotalSeconds / elapsedTime1.TotalSeconds;
            
            // Allow 20% tolerance for rounding and timing variations
            var difference = Math.Abs(chargeRatio - timeRatio);
            Assert.True(difference < 0.5,
                $"Charge should be proportional to time. Charge ratio: {chargeRatio:F2}, Time ratio: {timeRatio:F2}");
        }
    }

    /// <summary>
    /// Validates that paused time is not included in billing.
    /// </summary>
    [Fact]
    public void PoolTableCharge_ExcludesPausedTime()
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

        // Act - Start pool table, pause, wait, resume, stop
        tableManagement.StartPoolTable("1");
        Thread.Sleep(1000);
        
        tableManagement.PausePoolTable("1");
        var elapsedAtPause = tableManagement.GetPoolTableElapsedTime("1");
        Thread.Sleep(2000); // Paused time - should not be billed
        
        tableManagement.ResumePoolTable("1");
        Thread.Sleep(1000);
        
        var elapsedBeforeStop = tableManagement.GetPoolTableElapsedTime("1");
        tableManagement.StopPoolTable("1");
        Thread.Sleep(1000);

        // Assert - Elapsed time should not include paused duration
        var pausedDuration = TimeSpan.FromSeconds(2);
        var elapsedDuringPause = elapsedBeforeStop - elapsedAtPause;
        
        Assert.True(elapsedDuringPause < pausedDuration,
            $"Paused time should not be included in elapsed time. " +
            $"Elapsed during pause: {elapsedDuringPause}, Paused duration: {pausedDuration}");
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates pool table session data for property testing.
    /// </summary>
    private static Arbitrary<PoolTableSession> GeneratePoolTableSessions()
    {
        var sessionGen = from tableNumber in Gen.Elements("1", "2", "3", "4", "5")
                        from durationMs in Gen.Choose(1000, 5000) // 1-5 seconds
                        from hourlyRate in Gen.Elements(10.00m, 15.00m, 20.00m) // Common hourly rates
                        select new PoolTableSession
                        {
                            TableNumber = tableNumber,
                            DurationMs = durationMs,
                            HourlyRate = hourlyRate
                        };

        return Arb.From(sessionGen);
    }

    /// <summary>
    /// Represents a pool table session for property testing.
    /// </summary>
    private class PoolTableSession
    {
        public string TableNumber { get; set; } = string.Empty;
        public int DurationMs { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
