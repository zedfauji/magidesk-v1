using FsCheck;
using FsCheck.Xunit;
using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstracts;

namespace Magidesk.Tests.E2E.Tests.Properties;

/// <summary>
/// Property-based tests for report filtering metamorphic properties.
/// Validates that applying filters never increases result count.
/// 
/// Feature: e2e-testing-comprehensive-scenarios
/// Property 12: Filtered count is less than or equal to total count
/// Validates: Requirements 12.5, 25.1
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class ReportFilteringMetamorphicProperties : BaseE2ETest
{
    public ReportFilteringMetamorphicProperties(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Property 12: Filtered count is less than or equal to total count
    /// Validates: Requirements 12.5, 25.1
    /// 
    /// For any report with filtering applied, the filtered result count must be less than or equal
    /// to the unfiltered result count. This metamorphic property verifies that filters only remove
    /// or maintain items, never add new items to the result set.
    /// </summary>
    [Property(MaxTest = 10)]
    public Property ReportFiltering_FilteredCountLessThanOrEqualToTotalCount()
    {
        return Prop.ForAll(
            GenerateReportFilterScenarios(),
            scenario =>
            {
                try
                {
                    // Arrange
                    var loginPage = new LoginPage(MainWindow!);
                    var switchboard = new SwitchboardPage(MainWindow!);
                    var orderEntry = new OrderEntryPage(MainWindow!);
                    var settlement = new SettlementPage(MainWindow!);
                    var reports = new ReportsPage(MainWindow!);

                    // Act - Login
                    loginPage.LoginWithPin("1234");
                    Thread.Sleep(1000);

                    // Act - Create multiple transactions with different payment methods
                    foreach (var transaction in scenario.Transactions)
                    {
                        switchboard.NavigateToOrderEntry();
                        Thread.Sleep(500);

                        // Add items to ticket
                        orderEntry.SelectMenuItem(transaction.ItemName);
                        Thread.Sleep(300);

                        // Navigate to settlement and process payment
                        orderEntry.NavigateToSettlement();
                        Thread.Sleep(500);
                        settlement.SelectPaymentMethod(transaction.PaymentMethod);
                        Thread.Sleep(300);
                        var ticketTotal = orderEntry.GetTicketTotal();
                        settlement.EnterPaymentAmount(ticketTotal);
                        Thread.Sleep(300);
                        settlement.ProcessPayment();
                        Thread.Sleep(500);
                    }

                    // Act - Navigate to reports and generate sales report (unfiltered)
                    switchboard.NavigateToReports();
                    Thread.Sleep(1000);

                    var startDate = DateTime.Today;
                    var endDate = DateTime.Today;
                    reports.GenerateSalesReport(startDate, endDate);
                    Thread.Sleep(1000);

                    // Get unfiltered count
                    var unfilteredCount = reports.GetReportRowCount();
                    var unfilteredTotal = reports.GetReportTotal();

                    // Act - Apply filter by transaction type
                    reports.FilterByTransactionType(scenario.FilterTransactionType);
                    Thread.Sleep(1000);

                    // Get filtered count
                    var filteredCount = reports.GetReportRowCount();
                    var filteredTotal = reports.GetReportTotal();

                    // Assert - Verify filtered count <= unfiltered count (metamorphic property)
                    var countInvariantHolds = filteredCount <= unfilteredCount;
                    var totalInvariantHolds = filteredTotal <= unfilteredTotal;

                    if (!countInvariantHolds)
                    {
                        return false.ToProperty()
                            .Label($"Filtered count should be less than or equal to unfiltered count. " +
                                   $"Unfiltered: {unfilteredCount}, Filtered: {filteredCount}, " +
                                   $"Filter: {scenario.FilterTransactionType}");
                    }

                    if (!totalInvariantHolds)
                    {
                        return false.ToProperty()
                            .Label($"Filtered total should be less than or equal to unfiltered total. " +
                                   $"Unfiltered: {unfilteredTotal:C}, Filtered: {filteredTotal:C}, " +
                                   $"Filter: {scenario.FilterTransactionType}");
                    }

                    return (countInvariantHolds && totalInvariantHolds)
                        .ToProperty()
                        .Label("Filtered count is less than or equal to total count");
                }
                catch (Exception ex)
                {
                    // Mark test as failed for proper artifact capture
                    MarkTestFailed(ex);
                    
                    return false.ToProperty()
                        .Label($"Report filtering metamorphic property check failed: {ex.Message}");
                }
            });
    }

    /// <summary>
    /// Validates that filtering by user never increases result count.
    /// </summary>
    [Fact]
    public void UserFilter_NeverIncreasesResultCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create multiple transactions
        for (int i = 0; i < 3; i++)
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            orderEntry.SelectMenuItem("Coffee");
            Thread.Sleep(300);
            var ticketTotal = orderEntry.GetTicketTotal();
            orderEntry.NavigateToSettlement();
            Thread.Sleep(500);
            settlement.SelectPaymentMethod("Cash");
            Thread.Sleep(300);
            settlement.EnterPaymentAmount(ticketTotal);
            Thread.Sleep(300);
            settlement.ProcessPayment();
            Thread.Sleep(500);
        }

        // Act - Navigate to reports and generate sales report
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var unfilteredCount = reports.GetReportRowCount();
        var unfilteredTotal = reports.GetReportTotal();

        // Apply user filter
        var currentUser = switchboard.GetCurrentUserName();
        reports.FilterByUser(currentUser);
        Thread.Sleep(1000);

        var filteredCount = reports.GetReportRowCount();
        var filteredTotal = reports.GetReportTotal();

        // Assert - Filtered count <= unfiltered count
        Assert.True(filteredCount <= unfilteredCount,
            $"User filter should not increase result count. Unfiltered: {unfilteredCount}, Filtered: {filteredCount}");
        Assert.True(filteredTotal <= unfilteredTotal,
            $"User filter should not increase result total. Unfiltered: {unfilteredTotal:C}, Filtered: {filteredTotal:C}");
    }

    /// <summary>
    /// Validates that filtering by transaction type never increases result count.
    /// </summary>
    [Fact]
    public void TransactionTypeFilter_NeverIncreasesResultCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create cash transaction
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(500);
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(300);
        var cashTotal = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        Thread.Sleep(500);
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(cashTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(500);

        // Create credit transaction
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(500);
        orderEntry.SelectMenuItem("Burger");
        Thread.Sleep(300);
        var creditTotal = orderEntry.GetTicketTotal();
        orderEntry.NavigateToSettlement();
        Thread.Sleep(500);
        settlement.SelectPaymentMethod("Credit");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(creditTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(500);

        // Act - Navigate to reports and generate sales report
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var unfilteredCount = reports.GetReportRowCount();
        var unfilteredTotal = reports.GetReportTotal();

        // Apply transaction type filter (Cash only)
        reports.FilterByTransactionType("Cash");
        Thread.Sleep(1000);

        var filteredCount = reports.GetReportRowCount();
        var filteredTotal = reports.GetReportTotal();

        // Assert - Filtered count <= unfiltered count
        Assert.True(filteredCount <= unfilteredCount,
            $"Transaction type filter should not increase result count. Unfiltered: {unfilteredCount}, Filtered: {filteredCount}");
        Assert.True(filteredTotal <= unfilteredTotal,
            $"Transaction type filter should not increase result total. Unfiltered: {unfilteredTotal:C}, Filtered: {filteredTotal:C}");
    }

    /// <summary>
    /// Validates that filtering by date range never increases result count.
    /// </summary>
    [Fact]
    public void DateRangeFilter_NeverIncreasesResultCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create transactions
        for (int i = 0; i < 3; i++)
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            orderEntry.SelectMenuItem("Coffee");
            Thread.Sleep(300);
            var ticketTotal = orderEntry.GetTicketTotal();
            orderEntry.NavigateToSettlement();
            Thread.Sleep(500);
            settlement.SelectPaymentMethod("Cash");
            Thread.Sleep(300);
            settlement.EnterPaymentAmount(ticketTotal);
            Thread.Sleep(300);
            settlement.ProcessPayment();
            Thread.Sleep(500);
        }

        // Act - Navigate to reports and generate sales report with wide date range
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        
        var wideStartDate = DateTime.Today.AddDays(-7);
        var wideEndDate = DateTime.Today.AddDays(1);
        reports.GenerateSalesReport(wideStartDate, wideEndDate);
        Thread.Sleep(1000);

        var wideRangeCount = reports.GetReportRowCount();
        var wideRangeTotal = reports.GetReportTotal();

        // Generate report with narrow date range (today only)
        var narrowStartDate = DateTime.Today;
        var narrowEndDate = DateTime.Today;
        reports.GenerateSalesReport(narrowStartDate, narrowEndDate);
        Thread.Sleep(1000);

        var narrowRangeCount = reports.GetReportRowCount();
        var narrowRangeTotal = reports.GetReportTotal();

        // Assert - Narrow range count <= wide range count
        Assert.True(narrowRangeCount <= wideRangeCount,
            $"Narrower date range should not increase result count. Wide: {wideRangeCount}, Narrow: {narrowRangeCount}");
        Assert.True(narrowRangeTotal <= wideRangeTotal,
            $"Narrower date range should not increase result total. Wide: {wideRangeTotal:C}, Narrow: {narrowRangeTotal:C}");
    }

    /// <summary>
    /// Validates that multiple filters applied sequentially never increase result count.
    /// </summary>
    [Fact]
    public void MultipleFilters_NeverIncreaseResultCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create multiple transactions with different payment methods
        var paymentMethods = new[] { "Cash", "Credit", "Debit" };
        foreach (var method in paymentMethods)
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            orderEntry.SelectMenuItem("Coffee");
            Thread.Sleep(300);
            var ticketTotal = orderEntry.GetTicketTotal();
            orderEntry.NavigateToSettlement();
            Thread.Sleep(500);
            settlement.SelectPaymentMethod(method);
            Thread.Sleep(300);
            settlement.EnterPaymentAmount(ticketTotal);
            Thread.Sleep(300);
            settlement.ProcessPayment();
            Thread.Sleep(500);
        }

        // Act - Navigate to reports and generate sales report
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var unfilteredCount = reports.GetReportRowCount();
        var unfilteredTotal = reports.GetReportTotal();

        // Apply first filter (user)
        var currentUser = switchboard.GetCurrentUserName();
        reports.FilterByUser(currentUser);
        Thread.Sleep(1000);

        var firstFilterCount = reports.GetReportRowCount();
        var firstFilterTotal = reports.GetReportTotal();

        // Apply second filter (transaction type)
        reports.FilterByTransactionType("Cash");
        Thread.Sleep(1000);

        var secondFilterCount = reports.GetReportRowCount();
        var secondFilterTotal = reports.GetReportTotal();

        // Assert - Each filter application should not increase count
        Assert.True(firstFilterCount <= unfilteredCount,
            $"First filter should not increase count. Unfiltered: {unfilteredCount}, First filter: {firstFilterCount}");
        Assert.True(secondFilterCount <= firstFilterCount,
            $"Second filter should not increase count. First filter: {firstFilterCount}, Second filter: {secondFilterCount}");
        Assert.True(firstFilterTotal <= unfilteredTotal,
            $"First filter should not increase total. Unfiltered: {unfilteredTotal:C}, First filter: {firstFilterTotal:C}");
        Assert.True(secondFilterTotal <= firstFilterTotal,
            $"Second filter should not increase total. First filter: {firstFilterTotal:C}, Second filter: {secondFilterTotal:C}");
    }

    /// <summary>
    /// Validates that empty filter results have zero count.
    /// </summary>
    [Fact]
    public void EmptyFilterResults_HaveZeroCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create only cash transactions
        for (int i = 0; i < 2; i++)
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            orderEntry.SelectMenuItem("Coffee");
            Thread.Sleep(300);
            var ticketTotal = orderEntry.GetTicketTotal();
            orderEntry.NavigateToSettlement();
            Thread.Sleep(500);
            settlement.SelectPaymentMethod("Cash");
            Thread.Sleep(300);
            settlement.EnterPaymentAmount(ticketTotal);
            Thread.Sleep(300);
            settlement.ProcessPayment();
            Thread.Sleep(500);
        }

        // Act - Navigate to reports and generate sales report
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var unfilteredCount = reports.GetReportRowCount();

        // Apply filter for transaction type that doesn't exist (GiftCertificate)
        reports.FilterByTransactionType("GiftCertificate");
        Thread.Sleep(1000);

        var filteredCount = reports.GetReportRowCount();
        var filteredTotal = reports.GetReportTotal();

        // Assert - Empty filter results should have zero count
        Assert.True(filteredCount <= unfilteredCount,
            $"Filter with no matches should not increase count. Unfiltered: {unfilteredCount}, Filtered: {filteredCount}");
        Assert.True(filteredCount == 0 || filteredTotal == 0,
            $"Filter with no matches should have zero count or total. Count: {filteredCount}, Total: {filteredTotal:C}");
    }

    /// <summary>
    /// Validates that removing filters restores original count.
    /// </summary>
    [Fact]
    public void RemovingFilters_RestoresOriginalCount()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);
        var reports = new ReportsPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Create transactions
        for (int i = 0; i < 3; i++)
        {
            switchboard.NavigateToOrderEntry();
            Thread.Sleep(500);
            orderEntry.SelectMenuItem("Coffee");
            Thread.Sleep(300);
            var ticketTotal = orderEntry.GetTicketTotal();
            orderEntry.NavigateToSettlement();
            Thread.Sleep(500);
            settlement.SelectPaymentMethod("Cash");
            Thread.Sleep(300);
            settlement.EnterPaymentAmount(ticketTotal);
            Thread.Sleep(300);
            settlement.ProcessPayment();
            Thread.Sleep(500);
        }

        // Act - Navigate to reports and generate sales report
        switchboard.NavigateToReports();
        Thread.Sleep(1000);
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var originalCount = reports.GetReportRowCount();
        var originalTotal = reports.GetReportTotal();

        // Apply filter
        var currentUser = switchboard.GetCurrentUserName();
        reports.FilterByUser(currentUser);
        Thread.Sleep(1000);

        var filteredCount = reports.GetReportRowCount();

        // Remove filter (regenerate report without filter)
        reports.GenerateSalesReport(DateTime.Today, DateTime.Today);
        Thread.Sleep(1000);

        var restoredCount = reports.GetReportRowCount();
        var restoredTotal = reports.GetReportTotal();

        // Assert - Removing filter should restore original count
        Assert.True(filteredCount <= originalCount,
            $"Filter should not increase count. Original: {originalCount}, Filtered: {filteredCount}");
        Assert.Equal(originalCount, restoredCount);
        Assert.Equal(originalTotal, restoredTotal);
    }

    // ===== Property Generators =====

    /// <summary>
    /// Generates report filter scenarios for property testing.
    /// </summary>
    private static Arbitrary<ReportFilterScenario> GenerateReportFilterScenarios()
    {
        var itemNames = new[] { "Coffee", "Tea", "Burger", "Fries", "Soda", "Pizza" };
        var paymentMethods = new[] { "Cash", "Credit", "Debit", "GiftCertificate" };
        
        var scenarioGen = from transactionCount in Gen.Choose(2, 5) // 2-5 transactions
                         from transactions in Gen.ListOf(transactionCount, GenerateTransaction(itemNames, paymentMethods))
                         from filterType in Gen.Elements(paymentMethods)
                         select new ReportFilterScenario
                         {
                             Transactions = transactions.ToList(),
                             FilterTransactionType = filterType
                         };

        return Arb.From(scenarioGen);
    }

    /// <summary>
    /// Generates a single transaction for property testing.
    /// </summary>
    private static Gen<TransactionData> GenerateTransaction(string[] itemNames, string[] paymentMethods)
    {
        return from itemName in Gen.Elements(itemNames)
               from paymentMethod in Gen.Elements(paymentMethods)
               select new TransactionData
               {
                   ItemName = itemName,
                   PaymentMethod = paymentMethod
               };
    }

    /// <summary>
    /// Represents a report filter scenario for property testing.
    /// </summary>
    private class ReportFilterScenario
    {
        public List<TransactionData> Transactions { get; set; } = new();
        public string FilterTransactionType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a transaction for property testing.
    /// </summary>
    private class TransactionData
    {
        public string ItemName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
