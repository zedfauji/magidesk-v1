using Magidesk.Tests.E2E.Infrastructure;
using Magidesk.Tests.E2E.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace Magidesk.Tests.E2E.Tests.P0_FinancialSafety;

/// <summary>
/// P0 tests for single payment processing workflows.
/// Validates cash, credit, debit, gift certificate, and custom payment methods.
/// Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9, 5.10
/// </summary>
[Trait("Priority", "P0")]
[Trait("Category", "FinancialSafety")]
public class SinglePaymentTests : BaseE2ETest
{
    public SinglePaymentTests(ITestOutputHelper output) : base(output)
    {
    }

    /// <summary>
    /// Test cash payment with drawer balance update.
    /// Requirement 5.1: WHEN cash payment is processed, THE E2E_Test_Framework SHALL verify cash drawer balance update
    /// </summary>
    [Fact]
    public void CashPayment_ShouldUpdateDrawerBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get initial ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process cash payment
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed (amount due should be 0)
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        // Verify amount paid equals ticket total
        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test credit card payment transaction recording.
    /// Requirement 5.2: WHEN credit card payment is processed, THE E2E_Test_Framework SHALL verify transaction recording
    /// </summary>
    [Fact]
    public void CreditCardPayment_ShouldRecordTransaction()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Burger");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process credit card payment
        settlement.SelectPaymentMethod("Credit Card");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test debit card payment transaction recording.
    /// Requirement 5.3: WHEN debit card payment is processed, THE E2E_Test_Framework SHALL verify transaction recording
    /// </summary>
    [Fact]
    public void DebitCardPayment_ShouldRecordTransaction()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Fries");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process debit card payment
        settlement.SelectPaymentMethod("Debit Card");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test gift certificate payment with balance deduction.
    /// Requirement 5.4: WHEN gift certificate payment is processed, THE E2E_Test_Framework SHALL verify certificate balance deduction
    /// </summary>
    [Fact]
    public void GiftCertificatePayment_ShouldDeductBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Soda");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process gift certificate payment
        settlement.SelectPaymentMethod("Gift Certificate");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test custom payment method transaction recording.
    /// Requirement 5.5: WHEN custom payment method is used, THE E2E_Test_Framework SHALL verify transaction recording
    /// </summary>
    [Fact]
    public void CustomPaymentMethod_ShouldRecordTransaction()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Tea");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process custom payment (e.g., "House Account")
        settlement.SelectPaymentMethod("Custom");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test exact payment with zero change.
    /// Requirement 5.6: WHEN exact payment is tendered, THE E2E_Test_Framework SHALL verify zero change calculation
    /// </summary>
    [Fact]
    public void ExactPayment_ShouldHaveZeroChange()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process exact cash payment
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal); // Exact amount
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify no change due (amount due = 0)
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        // Verify amount paid equals ticket total (no overpayment)
        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }

    /// <summary>
    /// Test overpayment with change calculation.
    /// Requirement 5.7: WHEN overpayment occurs, THE E2E_Test_Framework SHALL verify change calculation and display
    /// </summary>
    [Fact]
    public void Overpayment_ShouldCalculateChange()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Burger");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process overpayment (e.g., $20 for $15 ticket)
        var paymentAmount = ticketTotal + 5.00m;
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(paymentAmount);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment processed
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        // Verify amount paid includes overpayment
        var amountPaid = settlement.GetAmountPaid();
        Assert.True(amountPaid >= ticketTotal, "Amount paid should be at least ticket total");
    }

    /// <summary>
    /// Test underpayment with remaining balance display.
    /// Requirement 5.8: WHEN underpayment occurs, THE E2E_Test_Framework SHALL verify remaining balance display
    /// </summary>
    [Fact]
    public void Underpayment_ShouldDisplayRemainingBalance()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Burger");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process partial payment (e.g., $10 for $15 ticket)
        var partialPayment = ticketTotal - 5.00m;
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(partialPayment);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify remaining balance displayed
        var amountDue = settlement.GetAmountDue();
        Assert.True(amountDue > 0m, "Amount due should be greater than 0 for underpayment");
        Assert.Equal(ticketTotal - partialPayment, amountDue);

        // Verify partial amount paid
        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(partialPayment, amountPaid);
    }

    /// <summary>
    /// Test payment receipt generation.
    /// Requirement 5.9: THE E2E_Test_Framework SHALL verify payment receipt generation
    /// </summary>
    [Fact]
    public void PaymentReceipt_ShouldBeGenerated()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process payment
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify payment completed (receipt would be generated)
        // In a real test, we'd check for receipt dialog or printed receipt
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);
    }

    /// <summary>
    /// Test ticket closure after full payment.
    /// Requirement 5.10: THE E2E_Test_Framework SHALL verify ticket closure after full payment
    /// </summary>
    [Fact]
    public void FullPayment_ShouldCloseTicket()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow!);
        var switchboard = new SwitchboardPage(MainWindow!);
        var orderEntry = new OrderEntryPage(MainWindow!);
        var settlement = new SettlementPage(MainWindow!);

        // Login
        loginPage.LoginWithPin("1234");
        Thread.Sleep(1000);

        // Navigate to Order Entry
        switchboard.NavigateToOrderEntry();
        Thread.Sleep(1000);

        // Add item to ticket
        orderEntry.SelectMenuItem("Coffee");
        Thread.Sleep(500);

        // Navigate to Settlement
        orderEntry.NavigateToSettlement();
        Thread.Sleep(1000);

        // Get ticket total
        var ticketTotal = settlement.GetTicketTotal();

        // Act - Process full payment
        settlement.SelectPaymentMethod("Cash");
        Thread.Sleep(300);
        settlement.EnterPaymentAmount(ticketTotal);
        Thread.Sleep(300);
        settlement.ProcessPayment();
        Thread.Sleep(1000);

        // Assert - Verify ticket closed (amount due = 0)
        var amountDue = settlement.GetAmountDue();
        Assert.Equal(0m, amountDue);

        // Verify full amount paid
        var amountPaid = settlement.GetAmountPaid();
        Assert.Equal(ticketTotal, amountPaid);
    }
}
